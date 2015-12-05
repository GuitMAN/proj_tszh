using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Models.Repository;
using WebMatrix.WebData;
using System.Web.Security;

namespace Web.Models
{
    public class HomeController : Controller
    {
        Repo repository;
        public HomeController()
        {
            repository = new Repo();
        }

        //
        // GET: /Home/        
        public ViewResult Index(string id = "1")
        {
            int i;
            try
            {
                i = Convert.ToInt32(id);
            }
            catch { return View(new Article()); }

            return View();         

        }

        public string test()
        {
            return "Тестовое сообщение";
        }
/*
        [Authorize]
        public ActionResult profile()
        {
            if (!WebSecurity.Initialized)
                return RedirectToAction("Index", "Login");
            UserProfile user;
            user = repository.UserProfile.Where(p => p.id.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            if (user.id_uk==0)
                return RedirectToAction("No_uk", "Home");
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(user.id_uk)).SingleOrDefault();
            if (uk != null)
                return Redirect("http://" + uk.host + ".мое-тсж.рф/");         
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult FeedBack()
        {
            if (!WebSecurity.Initialized)
                return RedirectToAction("Index", "Login");
            UserProfile user;
            user = repository.UserProfile.Where(p => p.id.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            if (user.id_uk == 0)
                return RedirectToAction("No_uk", "Home");

            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(user.id_uk)).SingleOrDefault();
            if (uk != null)
                return Redirect("http://" + uk.host + ".мое-тсж.рф/"); 

            return View();
        }

    //*/


        [HttpGet]
        [AllowAnonymous]
        public ActionResult AjaxHouse(string term = "")
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
