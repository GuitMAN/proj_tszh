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
            if (WebSecurity.IsAuthenticated && WebSecurity.Initialized)
                return RedirectToAction("Error_401", "Login");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model, string ReturnUrl)
        {
            //на всякий случай )) 
            WebSecurity.Logout();
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                uk_profile uk = null;
                try
                {
                    string requestDomain =Request.Headers["host"];
                    UserProfile user = repository.UserProfile.Where(p => p.login.Equals(model.UserName)).SingleOrDefault();
                    if (user != null)
                    {
                        uk = repository.uk_profile.Where(p => p.id.Equals(user.id_uk)).SingleOrDefault();
                        if (requestDomain.Equals(uk.host))
                        {
                            return RedirectToAction("Index", "User");
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "User");
                    }
                 
                    TempData["message"] = string.Format("Хост: \"{0}\" ", requestDomain);
                   
                }
                catch
                {
                    string requestDomain = Request.Headers["host"];
                    ModelState.AddModelError("", "Логин следует вводить с учетом регистра");
                    TempData["message"] = string.Format("Хост: \"{0}\" ", requestDomain);
                }        
                WebSecurity.Logout();           
            }
            ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно.");        
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {           
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid )
            {   
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.RequireRoles();
                    WebSecurity.Login(model.UserName, model.Password);
                    

                    return RedirectToAction("Index", "User");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("Ошибка при регистрации: ", ErrorCodeToString(e.StatusCode));                       
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult LogoOut(string ReturnUrl)
        {
            WebSecurity.Logout();
            return RedirectToLocal(ReturnUrl);
        }


        [Authorize]
        public string getUser()
        {
            int us_id = WebSecurity.CurrentUserId;
            string user = repository.UserProfile.Where(p => p.id.Equals(us_id)).SingleOrDefault().login;
            
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
        [ValidateAntiForgeryToken]
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

  //                      changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный текущий пароль или недопустимый новый пароль.");
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
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Не удалось создать локальную учетную запись. Возможно, учетная запись \"{0}\" уже существует.", User.Identity.Name));
                    }
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
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