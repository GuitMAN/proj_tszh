using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Web.Models;
using Web.Models.Repository;
using WebMatrix.WebData;
using Web.Utils;


namespace Web.Controllers
{

    public class UserController : Controller
    {
        Repo repository;
        //идентификатор УК ТСЖ
        //const string domain = ".мое-тсж.рф"; 
        private Repo repo;
        public UserController(Repo repo)
        {
            repository = repo;
        }

        public UserController()
        {
            repository = new Repo();
        }
        //
        // GET: /uk/User/
        [AllowAnonymous]
        public ActionResult Index(string id = "Главная")
        {
            string requestDomain = Request.Headers["host"];
            uk_profile uk = repository.uk_profile.Where(p => p.host.Equals(requestDomain)).SingleOrDefault();
            int uk_id;
            if (uk == null)
                uk_id = 0;
            else
                uk_id = uk.id;
            Article art = repository.Articles.Where(t => t.title.Equals(id)).Where(u => u.id_uk.Equals(uk_id)).SingleOrDefault();

            return View(art);
        }


        public ActionResult No_uk()
        {
            if (!WebSecurity.IsAuthenticated)
                return RedirectToAction("Index", "Login");
            UserProfile user;
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                int id = user.UserId;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return View();
            }

            return View();

        }

        [Authorize]
        public ActionResult profile(string returnUrl)
        {
            //---------------------------
            //Test Autorize
            if (!WebSecurity.IsAuthenticated)
                return RedirectToAction("Index", "Login");
            //Проверка на принадлежность пользователя
            UserProfile user = null;
            uk_profile uk = null;
            string requestDomain = Request.Headers["host"];
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                uk = repository.uk_profile.Where(p => p.id == user.id_uk).SingleOrDefault();
                ViewData["uk_name"] = uk.Name;
                ViewData["user_adr"] = get_adr(user.Adress);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return Redirect("send_profile");
            }
            //----------------------------
            return View(user);
        }

        [Authorize]
        [HttpGet]
        public ActionResult send_profile()
        {
            //---------------------------
            //Test Autorize
            if (!WebSecurity.IsAuthenticated)
                return RedirectToAction("Index", "Login");
            UserProfile user = null;
            UserProfile_nouk_form model = new UserProfile_nouk_form();
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                model.UserId = user.UserId;
                model.SurName = user.SurName;
                model.Name = user.Name;
                model.Patronymic = user.Patronymic;
                model.Personal_Account = user.Personal_Account;
                model.Adress = get_adr(user.Adress);
                model.Apartment = user.Apartment;
                model.Email = user.Email;
                model.phone = user.phone;

            }
            catch (Exception ex)
            {
                Log.Write(ex);
                model.UserId = WebSecurity.CurrentUserId;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult send_profile(UserProfile_nouk_form model)
        {
            //---------------------------
            //Проверка на авторизацию
            if (!WebSecurity.IsAuthenticated && !WebSecurity.Initialized)
                return RedirectToAction("Index", "Login");
            UserProfile user = new UserProfile();
            string requestDomain = Request.Headers["host"];
            uk_profile uk = repository.uk_profile.Select(p => p).Where(p => p.host.Equals(requestDomain)).SingleOrDefault();
            if (uk == null)
                return RedirectToAction("Index", "Login");

            string title;
            string message;
            model.UserId = WebSecurity.CurrentUserId;
            if (ModelState.IsValid)
            {
                user.id_uk = uk.id;
                //user.Adress = model.Adress;
                user.Apartment = model.Apartment;
                user.Email = model.Email;
                user.login = WebSecurity.CurrentUserName;
                user.Name = model.Name;
                user.Patronymic = model.Patronymic;
                user.Personal_Account = model.Personal_Account;
                user.phone = model.phone;
                user.SurName = model.SurName;
                user.UserId = model.UserId;
                title = "Заявка на активацию нового пользователя";
                message = "Анкетные данные: <br>Ф.И.О.:\n" + model.SurName + " " + model.Name + " " + model.Patronymic + "\nE-mail: " +
                     model.Email + "\nЛицевой счет: " + model.Personal_Account + "\nТелефон: " + model.phone + "\nДомашний адрес: " + model.Adress + ", Квартира: " + model.Apartment;
                feedback mess = new feedback();
                mess.id_uk = uk.id;
                mess.id_user = user.UserId;
                mess.title = title;
                mess.message = message;
                try
                {
                    FeedBack_from_nouk(mess);
                    SendMail("smtp.yandex.ru", "cloudsolution@bitrix24.ru", "321654as", uk.Email, title, message);
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
                repository.SaveUser(user);
                string[] res = { "Ok", "Ваша заявка отправлена: ", message };
                return Json(res);
            }

            return Json(new string[] { "Error", "Заполните все поля" });
        }


        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult FeedBack()
        {
            //---------------------------
            //Test Autorize
            //if (!WebSecurity.IsAuthenticated)
            //    return RedirectToAction("Index", "Login");
            //---------------------------
            //Проверка на принадлежность пользователя
            UserProfile user = null;
            uk_profile uk = null;
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                if (user == null)
                    return RedirectToAction("No_uk");
                if (user.id_uk == 0)
                    return RedirectToAction("No_uk");
                string requestDomain = Request.Headers["host"];
                uk = repository.uk_profile.Where(p => p.id == user.id_uk).SingleOrDefault();
                if (!requestDomain.Equals(uk.host))
                {
                    //                   return Redirect("http://" + uk.host);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return RedirectToAction("LogoOut", "Login");
            }
            //----------------------------

            feedback mess = new feedback();
            mess.status = false;
            mess.id_uk = user.id_uk;
            mess.id_user = WebSecurity.CurrentUserId;
            return View(mess);
        }

        // Перегруженная версия для сохранения изменений
        [HttpPost]
        [Authorize]
        [ValidateInput(true)]
        public ActionResult FeedBack(feedback mess)
        {
            //---------------------------
            //Test Autorize
            //if (!WebSecurity.IsAuthenticated)
            //    return RedirectToAction("Index", "Login");
            //Проверка на принадлежность пользователя
            UserProfile user = null;
            uk_profile uk = null;
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                if (user == null)
                    return RedirectToAction("No_uk");
                if (user.id_uk == 0)
                    return RedirectToAction("No_uk");
                string requestDomain = Request.Headers["host"];
                uk = repository.uk_profile.Where(p => p.id == user.id_uk).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                TempData["message"] = string.Format("Ошибка доступа \"{0}\"", ex.Message);
                string[] res = { "Error", string.Format("Ошибка доступа \"{0}\"", ex.Message) };
                return Json(res);
            }
            //------------------------------------
            mess.datetime = DateTime.UtcNow;
            if (string.IsNullOrEmpty(mess.title))
            {
                string[] res = { "Error", "Вы не заполнили тему сообщения" };
                ModelState.AddModelError("message", "Пустое сообщение");
                return Json(res);
            }
            else if (string.IsNullOrEmpty(mess.message))
            {
                string[] res = { "Error", "Пустое сообщение" };
                ModelState.AddModelError("message", "Пустое сообщение");
                return Json(res);
            }
            else if (mess.message.Length > 2000)
            {
                string[] res = { "Error", "Недопустимая длина строки" };
                ModelState.AddModelError("Error", "Недопустимая длина строки");
                return Json(res);
            }

            if (ModelState.IsValid)
            {
                mess.id_uk = uk.id;
                mess.status = false;
                mess.id_user = WebSecurity.CurrentUserId;
                mess.message = Regex.Replace(mess.message, @"(\r\n)", "<br>");
                if (uk.Email != null)
                    SendMail("smtp.yandex.ru", "cloudsolution@bitrix24.ru", "321654as", uk.Email, mess.title, mess.message);
                repository.SaveFeedBack(mess);
                TempData["message"] = string.Format("Ваша заявка отправлена", mess.title);
                string[] res = { "Ok", "Ваша заявка отправлена: ", mess.title };
                return Json(res);
            }
            return Json(new string[] { "Error", "Ошибка" });
        }

        [Authorize]
        private ActionResult FeedBack_from_nouk(feedback mess)
        {
            //---------------------------
            //Test Autorize
            //if (!WebSecurity.IsAuthenticated)
            //    return RedirectToAction("Index", "Login");
            //Проверка на принадлежность пользователя
            UserProfile user = null;
            uk_profile uk = null;
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();

                uk = repository.uk_profile.Where(p => p.id.Equals(user.id_uk)).SingleOrDefault();
                if (uk.id != 0)
                {
                    //генерация исключения
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return Redirect("/Login/LogoOut");
            }
            //------------------------------------
            mess.datetime = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                mess.status = false;
                mess.id_uk = uk.id;
                mess.id_user = WebSecurity.CurrentUserId;
                if (uk.Email != null)
                    SendMail("smtp.yandex.ru", "cloudsolution@bitrix24.ru", "321654as", uk.Email, "Обращение от " + user.SurName + user.Name + ": " + mess.title, mess.message);
                repository.SaveFeedBack(mess);
                return RedirectToAction("Index");
            }
            return View(mess);
        }

        [HttpGet]
        [Authorize]
        public ActionResult SeekAdress()
        {
            //---------------------------
            //Test Autorize
            //if (!WebSecurity.IsAuthenticated)
            //    return RedirectToAction("Index", "Login");
            return View();
        }

        // Перегруженная версия для сохранения изменений
        [HttpPost]
        [Authorize]
        public ActionResult SeekAdress(seek_adress model)
        {
            //---------------------------
            //Test Autorize
            //if (!WebSecurity.IsAuthenticated)
            //    return RedirectToAction("Index", "Login");
            uk_adress adress;
            if (ModelState.IsValid)
            {
                adress = repository.uk_adress.Where(c => c.City.Equals(model.City)).Where(s => s.Street.Equals(model.Street)).Where(h => h.House.Equals(model.House)).SingleOrDefault();
                if (adress == null)
                {
                    ModelState.AddModelError("City", "УК или ТСЖ не найдена");
                }
                else
                {
                    string str = repository.uk_profile.Where(p => p.id.Equals(adress.id_uk)).SingleOrDefault().Name;
                    ModelState.AddModelError("City", str);
                }
            }
            return View(model);
        }
           

        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult editprof()
        {
            //---------------------------
            //Test Autorize
            //if (!WebSecurity.IsAuthenticated)
            //    return RedirectToAction("Index", "Login");
            //Проверка на принадлежность пользователя
            UserProfile user = null;
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            if (user == null)
            {
                user = new UserProfile();
                
            }
            return View(user);
        }


        [HttpPut]
        [Authorize(Roles = "User")]
        public ActionResult editprof(UserProfile model)
        {
            //---------------------------
            //Test Autorize
            if (!WebSecurity.IsAuthenticated)
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {

                repository.SaveUser(model);
                TempData["message"] = string.Format("Ваш профиль \"{0}\" был изменен", model.login);
                return Json(new string[] { "Ok", string.Format("Ваш профиль \"{0}\" был изменен", model.login) });
            };
            return Json(new string[] { "Error", "Ошибка при изменении профиля"});
        }


        //Вывести все счетчики пользователя
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult ViewMeters()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult ViewMeters(int id = 0)
        {
            //Test Autorize
            if (!WebSecurity.IsAuthenticated)
                return Json(new string[] { "Error", "Ошибка авторизации" });

            IEnumerable<Counter> counter;
            if (id == 0)
            {
                counter = repository.Counter.Where(i => i.UserId.Equals(WebSecurity.CurrentUserId));
            }
            else
            {
                counter = repository.Counter.Where(i => i.UserId.Equals(id));
            }
            return Json(counter);
        }

        //Добавление счетчика
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult AddMeter()
        {
            //Test Autorize
            //if (!WebSecurity.IsAuthenticated)
            //    return RedirectToAction("Index", "Login");

            Counter_model_add model = new Counter_model_add();         
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult AddMeter(Counter_model_add model_add)
        {
            //Test Autorize
            //if (!WebSecurity.IsAuthenticated)
            //    return RedirectToAction("Index", "Login");

            Counter meter = new Counter();
            meter.UserId = WebSecurity.CurrentUserId;
            if (model_add.Name==null) { Json("Введите название"); }
            meter.Name = model_add.Name;
            if (model_add.Serial == null) { Json("Введите серийный номер"); }
            meter.Serial = model_add.Serial;
            //if (model_add.Name == null) { Json("Введите название счетчика") }
            meter.Status = false;
            if (model_add.Type == 0) { Json("Выберите тип"); }
            meter.Type = model_add.Type;
            if (model_add.Measure == null) { Json("Введите название счетчика"); }
            meter.Measure = model_add.Measure;
            if (model_add.DateOfReview == null) { Json("Выберите дату проверки счетчика"); }
            meter.DateOfReview = model_add.DateOfReview;


            if (ModelState.IsValid)
            {
                repository.SaveCounter(meter);
                //TempData["message"] = string.Format("Газовый счетчик успешно добавлен(обновлен)");
            }

            //int li = repository.context.Database.SqlQuery<int>("LAST_INSERT_ID()").FirstOrDefault();

            Counter_data model_data = new Counter_data();

            if (meter.id != 0)
            {
                model_data.data = model_add.firstdata;
                model_data.write = DateTime.UtcNow;
                model_data.status = false;
                model_data.id = meter.id;
                repository.SaveCounder_data(model_data);
                return Json("Ok");
            }
            return Json("Error");           
        }


        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult AddValueMeter(Counter_data model_data)
        {
            //Test Autorize
            if (!WebSecurity.IsAuthenticated)
                return RedirectToAction("Index", "Login");

            //int li = repository.context.Database.SqlQuery<int>("LAST_INSERT_ID()").FirstOrDefault();

            if (model_data.id != 0)
            {
              //  model_data.data = model_add.firstdata;
                model_data.write = DateTime.UtcNow;
                model_data.status = false;
              //  model_data.id = meter.id;
                repository.SaveCounder_data(model_data);
                return Json("Ok");
            }
            return Json("Error");
        }



        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult ViewDataMeters()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult ViewDataMeters(int month = 0, int year = 0)
        {

            UserProfile user = null;
            uk_profile uk = null;
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                if (user == null)
                    return RedirectToAction("No_uk");
                if (user.id_uk == 0)
                    return RedirectToAction("No_uk");
                string requestDomain = Request.Headers["host"];
                uk = repository.uk_profile.Where(p => p.id == user.id_uk).SingleOrDefault();
                if (!requestDomain.Equals(uk.host))
                {
                    //                   return Redirect("http://" + uk.host);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                TempData["message"] = string.Format("Ошибка доступа \"{0}\"", ex.Message);
                string[] res = { "Error", string.Format("Ошибка доступа \"{0}\"", ex.Message) };
                return Json(res);
            }
            //------------------------------------
            //To do add array of user's counters 

            Counter_user_viewdata model = new  Counter_user_viewdata();
            IEnumerable<Counter> ListCounter = null;
            IEnumerable<Counter_data> ListData = null;

            using (var context = new EFDbContext())
            {
                ListCounter = context.Database.SqlQuery<Counter>("SELECT * FROM [dbo].[Counter] WHERE UserId IN  ( " + WebSecurity.CurrentUserId.ToString() + " )").ToArray();

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
            }
            uk_adress adr;
            {
                Counter_user_viewdata temp = new Counter_user_viewdata();

                if (year == 0) year = DateTime.Now.Year;
                if (month == 0) month = DateTime.Now.Month;
                DateTime d_start = new DateTime(year, 1, 1);
                DateTime d_end = d_start.AddMonths(12).AddDays(-1);
                bool status = true;
                try
                {
                    IEnumerable<Counter> counters = ListCounter.Where(t => t.Type.Equals(1));
                    temp.gasi = new List<meter_model>();
                    foreach (Counter counter in counters)
                    {
                        meter_model cp = new meter_model();
                        cp.counter = counter;
                        cp.ListData = ListData.Where(m => m.id.Equals(counter.id)).Where(d => d.write >= d_start).Where(d => d.write < d_end);
                        temp.gasi.Add(cp);
                    }
                }
                catch
                {

                }
                try
                {
                    IEnumerable<Counter> counters = ListCounter.Where(t => t.Type.Equals(2));
                    temp.energoi = new List<meter_model>();
                    foreach (Counter counter in counters)
                    {
                        meter_model cp = new meter_model();
                        cp.counter = counter;
                        cp.ListData = ListData.Where(m => m.id.Equals(counter.id)).Where(d => d.write >= d_start).Where(d => d.write < d_end);
                        temp.energoi.Add(cp);
                    }
                }
                catch
                {

                }
                try
                {
                    IEnumerable<Counter> counters = ListCounter.Where(t => t.Type.Equals(3));
                    temp.cwi = new List<meter_model>();
                    foreach (Counter counter in counters)
                    {
                        meter_model cp = new meter_model();
                        cp.counter = counter;
                        cp.ListData = ListData.Where(m => m.id.Equals(counter.id)).Where(d => d.write >= d_start).Where(d => d.write < d_end);
                        temp.cwi.Add(cp);
                    }
                }
                catch
                {
                }
                try
                {
                    IEnumerable<Counter> counters = ListCounter.Where(t => t.Type.Equals(4));
                    temp.hwi = new List<meter_model>();
                    foreach (Counter counter in counters)
                    {
                        meter_model cp = new meter_model();
                        cp.counter = counter;
                        cp.ListData = ListData.Where(m => m.id.Equals(counter.id)).Where(d => d.write >= d_start).Where(d => d.write < d_end);
                        temp.hwi.Add(cp);
                    }

                }
                catch
                {
                }
                model=temp;                         
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult AddCounterMonthValue(int type = 0)
        {
            if (type == 0) { return View("Error"); }
            Counter_data model;
            Counter counter = repository.Counter.Where(u => u.UserId.Equals(WebSecurity.CurrentUserId)).Where(p => p.Type.Equals(type)).SingleOrDefault();
            model = new Counter_data();
            model.id = counter.id;
            return View(model);
        }


        #region Вспомогательные методы
        public string get_adr(int id_adr = 0)
        {
            if (id_adr == 0) return "Адрес не назначен";
            uk_adress adr = repository.uk_adress.Where(id => id.id.Equals(id_adr)).SingleOrDefault();
            if (adr == null)
                return "Ошибка! Адрес не найден!";
            return adr.City + ", " + adr.Street + ", " + adr.House;
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
                Log.Write(ex);
              //  ModelState.AddModelError("City", "УК или ТСЖ не найдена");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AjaxStreet(string term = "")
        {
            IEnumerable<string> categories = repository.uk_adress
                .Select(p => p.Street)
                .Where(p => p.ToUpper().Contains(term.ToUpper()))
                .Distinct()
                .OrderBy(x => x);

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        #endregion
	}
}