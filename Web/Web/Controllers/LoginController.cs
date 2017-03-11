using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Web.Models;
using Web.Models.Repository;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Web.Controllers
{

    [Authorize]
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
        [AllowAnonymous]
 //       [ValidateJsonAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                
                uk_profile uk = null;
                try
                {
                    Account_model result = new Account_model();
                    result.id = WebSecurity.GetUserId(model.UserName);
                    result.Login = model.UserName;
                    result.Roles = Roles.GetRolesForUser(model.UserName);
                    string requestDomain = Request.Headers["host"];

                    foreach (var role in result.Roles)
                    {
                        if (role.Equals("User"))
                        {
                            UserProfile user = repository.UserProfile.Where(p => p.UserId.Equals(result.id)).SingleOrDefault();
                            if (user != null)
                            {
                                uk = repository.uk_profile.Where(p => p.id.Equals(user.id_uk)).SingleOrDefault();

                                if (requestDomain.Equals(uk.host))
                                {
                                    //User have direct company
                                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                                    return Json(result);
                                    //        return new HttpStatusCodeResult(200, "{id:"+ WebSecurity.CurrentUserId.ToString() + "}");
                                }
                                else
                                {
                                    //User have no current direct company
                                    //TempData["message"] = string.Format("Хост: \"{0}\" ", requestDomain);
                                    WebSecurity.Logout();
                                    return Json(new string[] { "Error", "Имя пользователя или пароль не принадлежат данному домену" });
                                }
                            }
                        }
                        if (role.Equals("Moder"))
                        {
                             Admtszh admtszh = repository.Admtszh.Where(p => p.AdmtszhId.Equals(result.id)).SingleOrDefault();
                            if (admtszh != null)
                            {
                                uk = repository.uk_profile.Where(p => p.id.Equals(admtszh.id_uk)).SingleOrDefault();

                                if (requestDomain.Equals(uk.host))
                                {
                                    //User have direct company
                                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                                    return Json(result);
                                    //        return new HttpStatusCodeResult(200, "{id:"+ WebSecurity.CurrentUserId.ToString() + "}");
                                }
                                else
                                {
                                    //User have no current direct company
                                    //TempData["message"] = string.Format("Хост: \"{0}\" ", requestDomain);
                                    WebSecurity.Logout();
                                    return Json(new string[] { "Error", "Имя пользователя или пароль не принадлежат данному домену" });

                                }

                            }
                      
                        }
                        if (role.Equals("Admin"))
                        {
                            Admtszh admtszh = repository.Admtszh.Where(p => p.AdmtszhId.Equals(result.id)).SingleOrDefault();
                            if (admtszh != null)
                            {
                                uk = repository.uk_profile.Where(p => p.id.Equals(admtszh.id_uk)).SingleOrDefault();

                                if (requestDomain.Equals(uk.host))
                                {
                                    //User have direct company
                                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                                    return Json(result);
                                    //        return new HttpStatusCodeResult(200, "{id:"+ WebSecurity.CurrentUserId.ToString() + "}");
                                }
                                else
                                {
                                    //User have no current direct company
                                    //TempData["message"] = string.Format("Хост: \"{0}\" ", requestDomain);
                                    WebSecurity.Logout();
                                    return Json(new string[] { "Error", "Имя пользователя или пароль не принадлежат данному домену" });

                                }

                            }

                        }
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
        [AllowAnonymous]
        public ActionResult Register(RegisterModel model)
        {
            if (WebSecurity.UserExists(model.UserName))
            {
                return Json("Error", "Пользователь с таким логином уже существует");
            }

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

                        //Send E-mail
                        string title = "Добро пожаловать!";
                        string message = "Вы зарегистрировались в системе http://mytsn.ru\n"
                            + "Ваши логин: " + model.UserName 
                            +"\n Пароль: " + model.Password 
                            + "\nДля получения полного доступа к функционалу Вам необходимо заполнить анкету и отправить запрос на активацию.";

                        SendMail("smtp.yandex.ru", "cloudsolution@bitrix24.ru", "321654as", model.UserName, title, message);

                        WebSecurity.Login(model.UserName, model.Password);
                        Account_model result = new Account_model();
                        result.id = WebSecurity.CurrentUserId;
                        result.Login = WebSecurity.CurrentUserName;
                        result.Roles = Roles.GetRolesForUser();
                        return Json(result);

                        
                    }
                    catch (MembershipCreateUserException ex)
                    {
                        ModelState.AddModelError("Ошибка при регистрации: ", ErrorCodeToString(ex.StatusCode));
                        Logger.Log.Error("Ошибка при регистрации: ", ex);
                        return Json("Error", "Ошибка при регистрации: " + ErrorCodeToString(ex.StatusCode));
                    }
                }
                if (string.IsNullOrEmpty(model.UserName))
                {
                    return Json("Error", "Введите E-mail");
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    return Json("Error", "Пустой пароль");
                }

            }
            WebSecurity.Logout();
            return Json("Error", "Ошибка при регистрации");
        }

        [AllowAnonymous]
        public ActionResult LogoOut(string ReturnUrl)
        {
            WebSecurity.Logout();
            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RecoverPassSendMail()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RecoverPassSendMail(string email)
        {
            //  Regex regex = new Regex(@"/^(?:[a-z0-9]+(?:[-_]?[a-z0-9]+)?@[a-z0-9]+(?:\.?[a-z0-9]+)?\.[a-z]{2,5})$/i", RegexOptions.IgnoreCase);

            //if (regex.IsMatch(email))
            {
                string date;
                Filters.AccountFunctions func = new Filters.AccountFunctions(repository);
                if (func.getAccount(email, out date))
                {
                    string token = WebSecurity.GeneratePasswordResetToken(email);

                    //Send E-mail
                    string title = "Восстановление пароля";
                    string message = "Для восставноления пароля пройдет по ссылке ниже\n"
                        + "http://mytsn.ru/#/recoverpass/" + token;
                    //    + "http://mytsn.ru/Login/RecoverPass/" + getMd5Hash(email+date);

                    SendMail("smtp.yandex.ru", "cloudsolution@bitrix24.ru", "321654as", email, title, message);
                    return Json(new string[] { "Ok", "На Ваш E-mail отправлено письмо с инструкцией по восстановлению пароля." });
                }
            }

            return  Json(new string[] { "Error", "Пользователь с указанным E-mail: " + email + " не найден" });
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult RecoverPass()
        {
         //   LocalPasswordModel model = new  LocalPasswordModel();
            // используем поле OldPassword для хранения token пользователя
          //  model.OldPassword = token;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //        [ValidateAntiForgeryToken]
        public ActionResult RecoverPass(LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // В ряде случаев при сбое ChangePassword породит исключение, а не вернет false.
                bool changePasswordSucceeded;
                try
                {
                    // model.OldPassword - token пользователя, сгенерированный в вызывающем методе 
                    changePasswordSucceeded = WebSecurity.ResetPassword(model.OldPassword, model.NewPassword);
                    //  changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return Json(new string[] { "Ok", "Пароль успешно изменен" });
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный текущий пароль или недопустимый новый пароль.");
                    return Json(new string[] { "Error", "Неправильный текущий пароль или недопустимый новый пароль." });
                }
            }
            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return Json(new string[] { "Error", "Пароль изменить не удалось." });
        }


        public string getUser()
        {
            int us_id = WebSecurity.CurrentUserId;
            string user = repository.UserProfile.Where(p => p.UserId.Equals(us_id)).SingleOrDefault().login;
            
            return user;
        }

        
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


        public static void SendMail(string smtpServer, string from, string password,
        string mailto, string caption, string message, string attachFile = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = caption;
                mail.Body = message;
                if (!string.IsNullOrEmpty(attachFile))
                    mail.Attachments.Add(new Attachment(attachFile));
                SmtpClient client = new SmtpClient();
                client.Host = smtpServer;
                client.Port = 587;
                client.EnableSsl = true;
                client.Timeout = 15000;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(from, password);//.Split('@')[0]
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception ex)
            {
                Utils.Log.Write(ex);
                //ModelState.AddModelError("City", "УК или ТСЖ не найдена");
            }
        }



        #endregion
    }
}