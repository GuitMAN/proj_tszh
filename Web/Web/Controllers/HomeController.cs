using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Filters;
using Web.Models.Repository;
using Web.Utils;
using WebMatrix.WebData;
using log4net;
using System.Web.Security;

namespace Web.Models
{

    public class HomeController : Controller
    {
        Repo repository;

        private static readonly ILog Log = LogManager.GetLogger("LOGGER");

        public HomeController()
        {
            repository = new Repo();
        }

        //
        // GET: /Home/        
        public ActionResult Index()
        {
            return View();         
        }

        public ActionResult Article()
        {
            return View();
        }

        [HttpGet]
        [MyAuthorize(Roles = "User")]
        public ActionResult FeedBack()
        {
            UserProfile user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            //----------------------------
            feedback mess = new feedback();
            mess.status = false;
            mess.id_uk = user.id_uk;
            mess.id_user = WebSecurity.CurrentUserId;
            return View(mess);
        }


        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult readFeedBack()
        {
            return View();
        }




        [HttpGet]
        [AllowAnonymous]
        public JsonResult getarticle(string id = "Главная")
        {
            string requestDomain = Request.Headers["host"];
            uk_profile uk = repository.uk_profile.Where(p => p.host.Equals(requestDomain)).SingleOrDefault();
            int uk_id;
            if (uk == null)
                uk_id = 0;
            else
                uk_id = uk.id; 
            Article art = repository.Articles.Where(t => t.title.Equals(id)).Where(u => u.id_uk.Equals(uk_id)).SingleOrDefault();
            if (art == null)
            {
                art = new Article();
            }
            art.summary = requestDomain;
            return Json(art, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [MyAuthorize(Roles = "Moder")]
        public ActionResult edituk_tpl()
        {
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();
            return View();

        }

        [MyAuthorize]
        [HttpGet]
        public ActionResult No_uk_tpl()
        {
            UserProfile user;
            user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            if (user != null)
            {
                return View();
            }
            else
            {
                Log.Warn("Профиль пользователя " + WebSecurity.CurrentUserName + " уже создан");
                return new HttpStatusCodeResult(403, "User`s profile is created");
            }
        }

        [MyAuthorize]
        public ActionResult new_operprof_tpl()
        {

            int count = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).Count();
            if (count == 0)
            {
                return View();
            }
            Log.Warn("Профиль пользователя " + WebSecurity.CurrentUserName + " уже создан");
            Log.Warn("Доступная роли: " + Roles.GetRolesForUser());
            return new HttpStatusCodeResult(403, "User`s profile is created");
        }



        [HttpGet]
        [AllowAnonymous]
        public ActionResult test()
        {
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            string requestDomain = Request.Headers["host"];
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();

            Log.Warn("Ошибка доступа Home/test. requestDomain: " + requestDomain + " <> " + uk.host);
            Log.Warn("Пользователь: " + WebSecurity.CurrentUserName);
            System.Web.HttpRequestBase ss = Request;
       //     return new HttpStatusCodeResult(404, "Fucking duck");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult test(string author, string text)
        {
            Admtszh admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            string requestDomain = Request.Headers["host"];
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(admuser.id_uk)).SingleOrDefault();

            Log.Warn("Ошибка доступа Home/test. requestDomain: " + requestDomain + " <> " + uk.host);
            Log.Warn("Пользователь: " + WebSecurity.CurrentUserName);
            System.Web.HttpRequestBase ss = Request;
           return new HttpStatusCodeResult(404, "Fucking duck");
           // return Json(author);
        }


        [MyAuthorize]
        [HttpGet]
        public ActionResult send_profile()
        {
            //---------------------------

            UserProfile user = null;
            UserProfile_nouk_form model = new UserProfile_nouk_form();
            //user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            model.UserId = user.UserId;
            model.SurName = user.SurName;
            model.Name = user.Name;
            model.Patronymic = user.Patronymic;
            model.Personal_Account = user.Personal_Account;
            model.Adress = user.Adress;
            model.Apartment = user.Apartment;
            model.Email = user.Email;
            model.phone = user.phone;
            return View(model);
        }

        [MyAuthorize(Roles = "User")]
        [HttpGet]
        public ActionResult edituserprof_tpl()
        {
            //---------------------------
            //Test Autorize
            UserProfile user = null;
            try
            {
                user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error("GET Home/edituserprof_tpl", ex);
                user = new UserProfile();
                user.UserId = WebSecurity.CurrentUserId;
            }
            return View(user);
        }

        [HttpGet]
        [MyAuthorize(Roles = "User")]
        public ActionResult EditArticle_tpl()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult AjaxHouse(string term = "")
        {
            if (term=="") return null;
            IEnumerable<string> categories = repository.uk_adress
                .Select(p => p.House)
                .Where(p => p.ToUpper().Contains(term.ToUpper()))
                .Distinct()
                .OrderBy(x => x);

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        #region Вспомогательные методы
        public string get_adr(int id_adr=0)
        {
            if (id_adr == 0) return "Адрес не назначен";
            uk_adress adr = repository.uk_adress.Where(id => id.id.Equals(id_adr)).SingleOrDefault();
            if (adr == null)
                return "Ошибка! Адрес не найден!";
            return adr.City + ", " + adr.Street + ", " + adr.House;
        }



        #endregion
    }
}
