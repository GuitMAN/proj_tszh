using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Models;
using Web.Models.Repository;
using System.Data;
using WebMatrix.WebData;
using System.Web.Security;
using Web.Utils;
using Web.Filters;
using System.Net;
using System.Net.Mail;
using log4net;

namespace Web.Controllers
{

    public class AdmtszhController : Controller
    {
        Repo repository;
        private static readonly ILog Log = LogManager.GetLogger("LOGGER");
        public AdmtszhController()
        {
            repository = new Repo();
        }

        // GET: Admtszh
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult Articles()
        {
            //Article art;
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            return View(repository.Articles.Where(a => a.id_uk.Equals(admuser.id_uk)).OrderBy(p => p.id_uk));
        }

        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult EditArticle(int id = 0)
        {
            Article art;
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();
            if (id == 0)
            {
                art = new Article();
            }
            else
            {
                art = repository.Articles.Where(a => a.id_uk.Equals(admuser.id_uk)).Where(q => q.id == id).Single();
            }
            return Json(art, JsonRequestBehavior.AllowGet);
        }

        // Перегруженная версия Edit() для сохранения изменений
        [HttpPut]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult EditArticle(Article article)
        {
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();
            article.publicDate = DateTime.UtcNow;// -TimeZone.CurrentTimeZone; ;
            try
            {
                repository.SaveArticle(article);
                return Json(new string[] { "Ok", "Страница обновлена" });
            }
            catch (Exception ex)
            {
                Log.Error("Не удалось обновить статью");
                return Json(new string[] { "Error", "Не удалось обновить статью"});
            }
        }


        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult profile(string returnUrl)
        {

            Admtszh admuser = null;
            uk_profile uk = null;
            try
            {
                admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();
                ViewData["uk_name"] = uk.Name;

                //  ViewData["user_adr"] = get_adr(user.Adress);
            }
            catch (Exception ex)
            {
                Log.Error("GET Admtszh/profile Пользователь: " + WebSecurity.CurrentUserName, ex);
                ViewData["uk_name"] = "нет данных";
                //       ViewData["user_adr"] = "нет данных";
                if (admuser == null)
                    return View(new Admtszh());

            }
            //----------------------------
            return View(admuser);
        }

        [HttpGet]
        [MyAuthorize]
        public ActionResult editprof()
        {
            //Проверка на принадлежность пользователя
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            return View(admuser);
        }

        [HttpPut]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult editprof(Admtszh model)
        {
            if (ModelState.IsValid && (model.AdmtszhId == WebSecurity.CurrentUserId))
            {
                repository.SaveAdmtszh(model);
                TempData["message"] = string.Format("Ваш профиль \"{0}\" был изменен", model.id);
                return Json(new string[] { "Ok", string.Format("Ваш профиль \"{0}\" был изменен", model.id) });
            }
            return Json(new string[] { "Error", "Ошибка при изменении профиля" });
        }



        //Createprof
        [HttpPost]
        [AllowAnonymous]
        public ActionResult editprof(newAdmtszh model)
        {
            string requestDomain = Request.Headers["host"];       
            uk_profile uk = repository.uk_profile.Where(p => p.host.Equals(requestDomain)).SingleOrDefault();

            Admtszh newUser = new Admtszh();
            if (ModelState.IsValid)
            {
                newUser.AdmtszhId = WebSecurity.CurrentUserId;
                newUser.id_uk = uk.id;
                newUser.SurName = model.SurName;
                newUser.Name = model.Name;
                newUser.Patronymic = model.Patronymic;
                newUser.post = model.post;

                repository.SaveAdmtszh(newUser);
                return Json(new string[] { "Ok", string.Format("Ваш профиль \"{0}\" был изменен", newUser.id) });
            };
            //Send E-mail
            string title = "Запрос на активацию нового пользователя администрации ТСЖ";
            string message = " Пользователь"
                + "Ваши логин: " + newUser.SurName + " " + newUser.Name + " " + newUser.Patronymic
                + "\n Запросили авторизацию для получения полного доступа к функционалу Вам необходимо заполнить анкету и отправить запрос на активацию.";

            SendMail("smtp.yandex.ru", "cloudsolution@bitrix24.ru", "321654as", uk.Email, title, message);
            return Json(new string[] { "Ok", "Заявка отправлена" });
        }


        [HttpPost]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult readFeedBack()
        {
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();

            IEnumerable<feedback> list = repository.feedback.Where(p => p.id_uk.Equals(uk.id)).OrderByDescending(p => p.datetime);
            return Json(list);
        }


        [MyAuthorize(Roles = "Moder")]
        [HttpPost]
        public ActionResult DelMess(int id = -1)
        {
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();
            if ((id == -1) || (id == 0))
                return Json("Error", "Нельзя удалить сообщение");
            try
            {
                repository.DeleteFeedBack(id);
            }
            catch
            {
                return Json("Error", "Ошибка при удалении сообщения");
            }
            return Json("Ok", "Сообщение удалено");
        }




        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult ViewUsers()
        {

            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();

            IEnumerable<UserProfile> users = repository.UserProfile.Where(p => p.id_uk.Equals(uk.id)).OrderBy(p => p.SurName);
            return View(users);
        }

        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult EditUser(int id = 0)
        {
            // Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();

            // uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();
            UserProfile profile = repository.UserProfile.Where(i => i.UserId == id).SingleOrDefault();
            if (id > 0)
            {
                string[] ir = Roles.GetUsersInRole(profile.login);
                foreach (string r in ir)
                {
                    if (r == "Admin")
                        return Json("Error", "Запрещено! Данный пользователь имеет более высокии привилегии");
                }

                IEnumerable<uk_profile> list_uk = repository.uk_profile.OrderBy(p => p.id);
                ViewData["uk_profile"] = from n in list_uk
                                         select new SelectListItem { Text = n.Name, Value = n.id.ToString() };

                IEnumerable<uk_adress> list_adr = repository.uk_adress.Where(p => p.id_uk.Equals(profile.id_uk)).OrderBy(p => p.id);
                ViewData["uk_adress"] = from adr in list_adr
                                        select new SelectListItem { Text = adr.City.ToString() + ", " + adr.Street.ToString() + ", " + adr.House, Value = adr.id.ToString() };

                //присвоить роль пользователю
                UserProfile_form model = new UserProfile_form(profile);
                model.Role = Roles.GetRolesForUser(profile.login);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return View(new UserProfile_form());
            }
        }

        [HttpPut]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult EditUser(UserProfile_form model)
        {
            //         Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            //         uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();

            webpages_UsersInRoles userrole = new webpages_UsersInRoles();
            string[] ir = Roles.GetRolesForUser(model.login);

            if (ir.Length > 0)
                Roles.RemoveUserFromRoles(model.login, ir);
            if (model.Role != null)
                Roles.AddUserToRoles(model.login, model.Role);

            UserProfile profile = new UserProfile(model);
            repository.SaveUserRole(userrole);
            if (ModelState.IsValid)
            {
                repository.SaveUser(profile);
                TempData["message"] = string.Format("Изменения странице \"{0}\" были сохранены", model.login);
            }

            IEnumerable<uk_profile> list_uk = repository.uk_profile.OrderBy(p => p.id);
            ViewData["uk_profile"] = from n in list_uk
                                     select new SelectListItem { Text = n.Name, Value = n.id.ToString() };

            IEnumerable<uk_adress> list_adr = repository.uk_adress.Where(p => p.id.Equals(model.Adress));
            ViewData["uk_adress"] = from adr in list_adr
                                    select new SelectListItem { Text = adr.City.ToString() + ", " + adr.Street.ToString() + ", " + adr.House, Value = adr.id.ToString() };

            //присвоить роль пользователю
            model.Role = Roles.GetRolesForUser(model.login);

            return Json(model);
        }


        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult ViewCounter()
        {
            return View();
        }

        [HttpPost]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult ViewCounters(int month = 0, int year = 0)
        {
            IEnumerable<UserProfile> users = null;
            //To do add array of user's counters 
            List<Counter_model> model = new List<Counter_model>();
            IEnumerable<Counter> ListCounter = null;
            IEnumerable<Counter_data> ListData = null;
            Admtszh admuser;
            string requestDomain = Request.Headers["host"];
            try
            {
                admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();
                users = repository.UserProfile.Where(p => p.id_uk.Equals(uk.id));
            }
            catch (Exception ex)
            {
                Log.Error(Request.ToString() + " User: " + WebSecurity.CurrentUserName);
            }

            using (var context = new EFDbContext())
            {
                string u = "";
                foreach (var item in users)
                {
                    if (!u.Equals("")) { u = u + ","; }
                    u = u + item.UserId.ToString();
                }
                ListCounter = context.Database.SqlQuery<Counter>("SELECT * FROM [dbo].[Counter] WHERE UserId IN  ( " + u + " )").ToArray(); //repository.Counter.Where(u => u.UserId.Equals(it.id)).Where(p => p.type.Equals(type));

                if (ListCounter.Count() != 0)
                {
                    string res = "";
                    foreach (var item in ListCounter)
                    {
                        if (!res.Equals("")) { res = res + ","; }
                        res = res + item.id.ToString();
                    }
                    ListData = context.Database.SqlQuery<Counter_data>("SELECT * FROM [dbo].[Counter_data] WHERE id IN  ( " + res + " )").ToArray();
                }
                else
                {
                    ListData = new List<Counter_data>().ToArray();
                }
                uk_adress adr;
                foreach (var user in users)
                {
                    Counter_model temp = new Counter_model();
                    temp.Name = user.SurName + " " + user.Name;

                    adr = context.uk_adresses.Where(pu => pu.id.Equals(user.Adress)).FirstOrDefault();
                    temp.street = adr.Street;
                    temp.house = adr.House;
                    temp.flat = user.Apartment;
                    if (year == 0) year = DateTime.Now.Year;
                    if (month == 0) month = DateTime.Now.Month;
                    DateTime d_start = new DateTime(year, month, 1);
                    DateTime d_end = d_start.AddMonths(1);
                    bool status = true;

                    for (int j = 1; j <= 4; j++)
                    {
                        if (j == 1) temp.gasi = new List<count_place>();
                        if (j == 2) temp.energoi = new List<count_place>();
                        if (j == 3) temp.cwi = new List<count_place>();
                        if (j == 4) temp.hwi = new List<count_place>();

                            IEnumerable<Counter> counters = ListCounter.Where(p => p.UserId.Equals(user.UserId)).Where(t => t.Type.Equals(j));
                        foreach (Counter counter in counters)
                        {
                            Counter_data t_data = ListData.Where(m => m.id.Equals(counter.id)).Where(d => d.write >= d_start).Where(d => d.write < d_end).SingleOrDefault();
                            //                                foreach (var it in t_data)
                            if (t_data != null)
                            {
                                count_place cp = new count_place();
                                cp.id = t_data.id;
                                cp.data = t_data.data;
                                cp.place = ListCounter.Where(p => p.id.Equals(t_data.id)).FirstOrDefault().Name;
                                status = t_data.status;

                                if (j == 1) temp.gasi.Add(cp);
                                if (j == 2) temp.energoi.Add(cp);
                                if (j == 3) temp.cwi.Add(cp);
                                if (j == 4) temp.hwi.Add(cp);
                                cp = null;
                            }
                        }
                     }


                    if ((temp.gasi.Count == 0) && (temp.energoi.Count == 0) && (temp.cwi.Count == 0) && (temp.hwi.Count == 0))
                    {
                        // temp = null;
                    }
                    else
                    {
                        temp.month = d_start;
                        temp.status = status;
                        model.Add(temp);

                    }
                    temp = null;
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult SetsSatusCounterData(int id, int year, int month, bool status)
        {
            Admtszh admuser = null;
            // uk_profile uk = null;
            string result = null;
            using (var context = new EFDbContext())
            {

                IEnumerable<Counter> ListCounters = repository.Counter.Where(u => u.UserId.Equals(id));
                if (ListCounters.Count() != 0)
                {
                    string res = "";
                    foreach (var item in ListCounters)
                    {
                        if (!res.Equals("")) { res = res + ","; }
                        res = res + item.id.ToString();
                    }
                    result = context.SQLStringConnect("UPDATE[dbo].[Counter_data] SET status = 1 WHERE id IN( " + res + ")");
                }
            }
            return Json(result);
        }

        [HttpPost]
        [Route("route/{data}")]
        public JsonResult TheAction(string data)
        {
            string _jsonObject = data.Replace(@"\", string.Empty);
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, string> jsonObject = serializer.Deserialize<Dictionary<string, string>>(_jsonObject);
            return Json("Ok");
      
        }

        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult EditUk()
        {
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();
            return Json(uk, JsonRequestBehavior.AllowGet);
        }

        [HttpPut]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult EditUk(uk_profile uk = null)
        {
            repository.SaveUkProfile(uk);
            return Json("Ok");
        }



        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult ViewAddrUk()
        {
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            return Json(repository.uk_adress.Where(p => p.id_uk.Equals(admuser.id_uk)), JsonRequestBehavior.AllowGet);
        }


        [MyAuthorize(Roles = "Moder")]
        public JsonResult get_users()
        {
            IEnumerable<UserProfile> users = null;
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            users = repository.UserProfile.Where(u => u.id_uk.Equals(admuser.id_uk));
            return Json(users, JsonRequestBehavior.AllowGet);
        }


        [MyAuthorize(Roles = "Moder")]
        public string get_adr(int id_adr = 0)
        {
            if (id_adr == 0) return "Адрес не назначен";
            uk_adress adr = repository.uk_adress.Where(id => id.id.Equals(id_adr)).SingleOrDefault();
            if (adr == null)
                return "Ошибка! Адрес не найден!";
            return adr.City + ", " + adr.Street + ", " + adr.House;
        }

        public string getModerLogin(int id)
        {
            List<UserInRole_model_> list = new List<UserInRole_model_>();
            DataSet ds;
            //MemberShip.GetAllUsers() не пашет, лень ковыряться
            //Поэтому обращаемся на прямую в базу )) 
            //Необходимо дополнить страничный вывод
            repository.SQLstringConnect("SELECT login FROM UserAccount WHERE id IN  ( " + id + " )", out ds);
            //Заполняем наш массив данными из таблшицы
            string login = "-1";
            if (ds.Tables[0].Rows.Count == 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    //UserInRole_model_ res = new UserInRole_model_();
                    login = dr["Login"].ToString();
                }
            }
            return login;
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
                Log.Error("POST Admtszh/SendMail. Пользователь: " + WebSecurity.CurrentUserName, ex);

            }
        }

    }

    /*
        public class JsonNetFilterAttribute : ActionFilterAttribute
        {
            private const string _dateFormat = "";

            public override void OnActionExecuted(ActionExecutedContext filterContext)
            {
                if (filterContext.Result is JsonResult == false)
                    return;

                filterContext.Result = new JsonNetResult((JsonResult)filterContext.Result);
            }

            private class JsonNetResult : JsonResult
            {
                public JsonNetResult(JsonResult jsonResult)
                {
                    this.ContentEncoding = jsonResult.ContentEncoding;
                    this.ContentType = jsonResult.ContentType;
                    this.Data = jsonResult.Data;
                    this.JsonRequestBehavior = jsonResult.JsonRequestBehavior;
                    this.MaxJsonLength = jsonResult.MaxJsonLength;
                    this.RecursionLimit = jsonResult.RecursionLimit;
                }

                public override void ExecuteResult(ControllerContext context)
                {
                    if (context == null)
                        throw new ArgumentNullException("context");

                    if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet
                        && String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("GET not allowed! Change JsonRequestBehavior to AllowGet.");

                    var response = context.HttpContext.Response;

                    response.ContentType = String.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

                    if (this.ContentEncoding != null)
                        response.ContentEncoding = this.ContentEncoding;

                    if (this.Data != null)
                    {
                        var isoConvert = new IsoDateTimeConverter();
                        isoConvert.DateTimeFormat = _dateFormat;
                        response.Write(JsonConvert.SerializeObject(this.Data, isoConvert));
                    }
                }
            }
        }
        //*/
}