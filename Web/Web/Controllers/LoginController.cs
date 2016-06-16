using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Web.Filter;
using Web.Models;
using Web.Models.Repository;
using System.Net;
using System.Text.RegularExpressions;

namespace Web.Controllers
{

    [InitializeMembership]

    public class LoginController : Controller
    {
        Repo repository;
        public LoginController()
        {
            repository = new Repo();
        }
        //
        // GET: /Login/
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {         
            //Если пользователь зарегистрирован, но вас кинуло именно сюда, значит вам на ту страницу нельзя 401
         //   if (WebSecurity.IsAuthenticated && WebSecurity.Initialized)
         //       return RedirectToAction("Error_401", "Login");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
 //       [AllowAnonymous]
 //       [ValidateJsonAntiForgeryToken]
        public ActionResult Index(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                FormsAuthentication.SetAuthCookie(model.UserName, false);
                uk_profile uk = null;
                try
                {
                    Account_model result = new Account_model();
                    result.id = WebSecurity.GetUserId(model.UserName);
                    result.Login = model.UserName;
                    result.Role = Roles.GetRolesForUser(model.UserName);
                    string requestDomain = Request.Headers["host"];
                    UserProfile user = repository.UserProfile.Where(p => p.UserId.Equals(result.id)).SingleOrDefault();
                    if (user != null)
                    {
                        uk = repository.uk_profile.Where(p => p.id.Equals(user.id_uk)).SingleOrDefault();
                        
                        if (requestDomain.Equals(uk.host))
                        {
                            //User have direct company
                            return Json(result);
                    //        return new HttpStatusCodeResult(200, "{id:"+ WebSecurity.CurrentUserId.ToString() + "}");
                        }
                        else
                        {
                            //User have no current direct company
                            TempData["message"] = string.Format("Хост: \"{0}\" ", requestDomain);
                            
                            return Json(result);
                            //WebSecurity.Logout();
                          //return new HttpStatusCodeResult(203, "login или пароль");
                        }
                    }
                    else
                    {
                        //User nobody direct company
                        
                        return Json(result);
                    }                  
                }
                catch (Exception ex)
                {
                    string requestDomain = Request.Headers["host"];
                    ModelState.AddModelError("", "Внутренняя ошибка при авторизации");
                }        
                WebSecurity.Logout();
                return Json(new string[] { "Error", "Логин следует вводить с учетом регистра" });       
            }
            ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно.");        
            return Json(new string[] { "Error", "Имя пользователя или пароль указаны неверно." }); 
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {           
            return View();
        }

        [HttpPost]
        //        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (!WebSecurity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                        //Membership.CreateUser(model.UserName, model.Password, model.Email,
                        //passwordQuestion: null, passwordAnswer: null, isApproved: true,
                        //providerUserKey: null, status: out createStatus);
                        WebSecurity.RequireRoles();
                        WebSecurity.Login(model.UserName, model.Password);
                        Account_model result = new Account_model();
                        result.id = WebSecurity.CurrentUserId;
                        result.Login = WebSecurity.CurrentUserName;
                        result.Role = Roles.GetRolesForUser();
                        return Json(result);

                        //                   return RedirectToAction("Index", "User");
                    }
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("Ошибка при регистрации: ", ErrorCodeToString(e.StatusCode));
                        return new HttpStatusCodeResult(203, "Ошибка при регистрации: " + ErrorCodeToString(e.StatusCode));
                    }
                }
                if (string.IsNullOrEmpty(model.UserName))
                {
                    return Json("Error", "Неправильный E-mail");
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    return Json("Error", "Ошибка при вводе пароля");
                }

            }
            return RedirectToAction("Index", "Home");
        } 
        [Authorize]
        public ActionResult LogoOut(string ReturnUrl)
        {
            WebSecurity.Logout();
            return new HttpStatusCodeResult(200);
//            return RedirectToLocal("/home#/login");
        }


        [Authorize]
        public string getUser()
        {
            int us_id = WebSecurity.CurrentUserId;
            string user = repository.UserProfile.Where(p => p.UserId.Equals(us_id)).SingleOrDefault().login;
            
            return user;
        }

        [Authorize]
        public string get_uk(int uk = 0)
        {
         
            string str = "";

            try
            {
                str = repository.uk_profile.Where(p => p.id.Equals(uk)).SingleOrDefault().Name;
            }
            catch
            {
                str = "-";
            }
            return str;
        }


        //изменение пароля
        [HttpGet]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.RemoveLoginSuccess ? "Внешняя учетная запись удалена."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
//        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                // У пользователя нет локального пароля, уберите все ошибки проверки, вызванные отсутствующим
                // полем OldPassword
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }
                if (ModelState.IsValid)
                {
                    // В ряде случаев при сбое ChangePassword породит исключение, а не вернет false.
                    bool changePasswordSucceeded;
                    try
                    {
                        string token = WebSecurity.GeneratePasswordResetToken(User.Identity.Name);
                        changePasswordSucceeded = WebSecurity.ResetPassword(token, model.NewPassword);
  //                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
//                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                        return Json(new string[] { "Ok", "Пароль успешно изменен" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный текущий пароль или недопустимый новый пароль.");
                        return Json(new string[] { "Error", "Неправильный текущий пароль или недопустимый новый пароль." });
                    }
                }
            }
            else
            {
                // У пользователя нет локального пароля, уберите все ошибки проверки, вызванные отсутствующим
                // полем OldPassword
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
    //                    return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                        return Json(new string[] { "Ok", "Пароль успешно изменен" });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Не удалось создать локальную учетную запись. Возможно, учетная запись \"{0}\" уже существует.", User.Identity.Name));
                        return Json(new string[] { "Error", String.Format("Не удалось создать локальную учетную запись. Возможно, учетная запись \"{0}\" уже существует.", User.Identity.Name) });
                    }
                }
            }
            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return Json(new string[] { "Error", "Пароль изменить не удалось." });
        }

        public string Error_401()
        {
            return "Доступ запрещен!!! Пшел вон";
        }
        #region Вспомогательные методы

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }



        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", new { controller = "User" });
               
            }
        }


        private string getMd5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();
 
            // Преобразуем входную строку в массив байт и вычисляем хэш
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
 
            // Создаем новый Stringbuilder (Изменяемую строку) для набора байт
            StringBuilder sBuilder = new StringBuilder();
 
            // Преобразуем каждый байт хэша в шестнадцатеричную строку
            for (int i = 0;  i<data.Length; i++)
            {
                //указывает, что нужно преобразовать элемент в шестнадцатиричную строку длиной в два символа
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Полный список кодов состояния см. по адресу http://go.microsoft.com/fwlink/?LinkID=177550
            //.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Имя пользователя уже существует. Введите другое имя пользователя.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Имя пользователя для данного адреса электронной почты уже существует. Введите другой адрес электронной почты.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Указан недопустимый пароль. Введите допустимое значение пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Указан недопустимый адрес электронной почты. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Указан недопустимый ответ на вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Указан недопустимый вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Указано недопустимое имя пользователя. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.ProviderError:
                    return "Поставщик проверки подлинности вернул ошибку. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                case MembershipCreateStatus.UserRejected:
                    return "Запрос создания пользователя был отменен. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                default:
                    return "Произошла неизвестная ошибка. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";
            }
        }
        #endregion
	}
}