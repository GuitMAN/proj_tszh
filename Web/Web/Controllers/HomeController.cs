﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Models.Repository;
using Web.Utils;
using WebMatrix.WebData;

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
        public ActionResult Index()
        {
            return View();         
        }

        public ActionResult Article()
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
        public ActionResult edituk_tpl()
        {
            return View();
        }

        public ActionResult No_uk_tpl()
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



        [HttpGet]
        [AllowAnonymous]
        public ActionResult test()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult test(string author, string text)
        {
            System.Web.HttpRequestBase ss = Request;
            //on.

             return new HttpStatusCodeResult(404, "Fucking duck");
           // return Json(author);
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
