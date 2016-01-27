﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Models;
using Web.Models.Repository;
using System.Data;
using WebMatrix.WebData;
using System.Web.Security;
using System.Web;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
//using Web.Filter;
//using Microsoft.AspNet.Identity;

namespace Web.Controllers
{
    [Authorize(Roles = "Moder")]
    public class AdmtszhController : Controller
    {
        Repo repository;

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

        [Authorize]
        public ActionResult profile(string returnUrl)
        {
            //---------------------------
            //Проверка на авторизацию
            if (!WebSecurity.IsAuthenticated && !WebSecurity.Initialized)
                return RedirectToAction("Login", "Home");
            //Проверка на принадлежность пользователя
            Admtszh admuser = null;
            uk_profile uk = null;
            string requestDomain = Request.Headers["host"];
            try
            {
                admuser = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                uk = repository.uk_profile.Where(p => p.host == requestDomain).SingleOrDefault();
                ViewData["uk_name"] = uk.Name;
                
              //  ViewData["user_adr"] = get_adr(user.Adress);
            }
            catch (Exception ex)
            {
                ViewData["uk_name"] = "нет данных";
         //       ViewData["user_adr"] = "нет данных";
                if (admuser == null)
                   return View(new Admtszh());

            }
            //----------------------------
            return View(admuser);
        }

        [Authorize]
        public ActionResult editprof()
        {
            if (!WebSecurity.Initialized)
                return RedirectToAction("Index", "Login");
            //Проверка на принадлежность пользователя
            Admtszh user = null;
            try
            {
                user = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            }
            catch
            {

            }
            uk_profile uk;
            string requestDomain = Request.Headers["host"];
            if (user == null)
            {
                user = new Admtszh();
                uk = repository.uk_profile.Where(p => p.host.Equals(requestDomain)).SingleOrDefault();
                if (uk != null)
                    user.id_uk = uk.id;

            }
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public ActionResult editprof(Admtszh model)
        {
            if (!WebSecurity.Initialized)
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {

                repository.SaveAdmtszh(model);
                TempData["message"] = string.Format("Изменения в профиле были сохранены");
            }
            return View(model);
        }


        [HttpGet]
        [Authorize]
        public ActionResult readFeedBack()
        {
            if (!WebSecurity.Initialized)
                return RedirectToAction("Index", "Login");
            //Проверка на принадлежность пользователя
            Admtszh user = null;
            string requestDomain = Request.Headers["host"];
            uk_profile uk;
            try
            {
                user = repository.Admtszh.Where(p => p.AdmtszhId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
                uk = repository.uk_profile.Where(p => p.id.Equals(user.id_uk)).SingleOrDefault();
            }
            catch
            {
                return RedirectToAction("editprof", "Admtszh");
            }
            //var user_ = UserManager.FindById(User.Identity.GetUserId());

            //     if (user == null)
            //    {
            //        return RedirectToAction("editprof", "Admtszh");
            //    }

            IEnumerable<feedback> list = repository.feedback.Where(p =>p.id_uk.Equals(uk.id)).OrderByDescending(p => p.datetime);
            return View(list);
        }

        public ActionResult DelMess(int id = -1)
        {
            if ((id==-1)||(id==0))
                return RedirectToAction("readFeedBack");
            try
            {
                repository.DeleteFeedBack(id);
            }
            catch
            { 
            }
            return RedirectToAction("readFeedBack");
        }



        public ActionResult ViewUsers()
        {
            if (!WebSecurity.Initialized)
                return RedirectToAction("Index", "Login");
            //Проверка на принадлежность пользователя
            UserProfile user = null;
            string requestDomain = Request.Headers["host"];
            uk_profile uk;
            user = repository.UserProfile.Where(p => p.UserId.Equals(WebSecurity.CurrentUserId)).SingleOrDefault();
            uk = repository.uk_profile.Where(p => p.host.Equals(requestDomain)).SingleOrDefault();
            if (uk != null)
            {
                if (!uk.host.Equals(requestDomain)) 
                {
                    return RedirectToAction("editprof", "Admtszh");
                }
            }
            else
            {
               return RedirectToAction("editprof", "Admtszh"); 
            }

            IEnumerable<webpages_UsersInRoles> uir = repository.webpages_UsersInRoles;
            ViewData["uir"] = uir.ToList();

            IEnumerable <UserProfile> users = repository.UserProfile.Where(p => p.id_uk.Equals(uk.id)).OrderBy(p => p.SurName);

            //ViewData["uk_adress"];

            return View(users);
        }


        public ActionResult EditUser(int id = 0)
        {
           
            UserProfile profile = repository.UserProfile.Where(i => i.id == id).SingleOrDefault();
            string[] ir = Roles.GetUsersInRole(profile.login);
            foreach (string r in ir)
            {
                if (r=="Admin")
                    return Redirect("Запрещено");
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

            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(UserProfile_form model)
        {
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

            return View(model);
        }

        [AllowAnonymous]
 //       [JsonNetFilter]
        public JsonResult ViewCounters(int type = 0)
        {
            Admtszh admuser = null;
            uk_profile uk = null;
            //Counter_model model = new Counter_model();
            IEnumerable<UserProfile> users = null;
            string requestDomain = Request.Headers["host"];
            try
            {
                uk = repository.uk_profile.Where(p => p.host == requestDomain).SingleOrDefault();
                users = repository.UserProfile.Where(p => p.id_uk.Equals(1));//uk.id));
            }
            catch
            { }

            //To do add array of user's counters 
            List < Counter_model> model = new List<Counter_model>();
            IEnumerable<Counter> ListCounter = null;
            IEnumerable<Counter_data> ListData = null;
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
 
                foreach (var user in users)
                {
                    Counter_model temp = new Counter_model();
                    temp.Name = user.SurName + " " + user.Name;
                    //temp.street = get_adr(user.Adress);
                    temp.flat = user.Apartment;
                    //IEnumerable<Counter_data> Data = 
                    DateTime d_start = new DateTime(2015,12,1);
                    DateTime d_end = d_start.AddMonths(1);
                    try
                    {
                        temp.gas =      ListData.Where(m => m.id.Equals(ListCounter.Where(p => p.UserId.Equals(user.UserId)).Where(t => t.type.Equals(1)).FirstOrDefault().id)).Where(d =>d.write >= d_start).Where(d => d.write < d_end).FirstOrDefault().data;
                    }
                    catch
                    {

                    }
                    try
                    {
                        temp.energo =   ListData.Where(m => m.id.Equals(ListCounter.Where(p => p.UserId.Equals(user.UserId)).Where(t => t.type.Equals(2)).FirstOrDefault().id)).Where(d => d.write >= d_start).Where(d => d.write < d_end).FirstOrDefault().data;
                    }
                    catch
                    {

                    }
                    try
                    {
                        temp.cw =       ListData.Where(m => m.id.Equals(ListCounter.Where(p => p.UserId.Equals(user.UserId)).Where(t => t.type.Equals(3)).FirstOrDefault().id)).Where(d => d.write >= d_start).Where(d => d.write < d_end).FirstOrDefault().data;
                    }
                    catch
                    {

                    }
                    try
                    {
                        temp.hw =       ListData.Where(m => m.id.Equals(ListCounter.Where(p => p.UserId.Equals(user.UserId)).Where(t => t.type.Equals(4)).FirstOrDefault().id)).Where(d => d.write >= d_start).Where(d => d.write < d_end).FirstOrDefault().data;
                    }
                    catch
                    {

                    }
                    try
                    {
                        temp.month =    ListData.Where(m => m.id.Equals(ListCounter.Where(p => p.UserId.Equals(user.UserId)).Where(t => t.type.Equals(1)).FirstOrDefault().id)).FirstOrDefault().write.GetValueOrDefault();
                    }
                    catch { }
                   
                        model.Add(temp);
                    
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }




        public JsonResult get_users()
        {

            IEnumerable<UserProfile> users = null;
            uk_profile uk = null;
            string requestDomain = Request.Headers["host"];
            try
            {
                uk = repository.uk_profile.Where(p => p.host == requestDomain).SingleOrDefault();
                users = repository.UserProfile.Where(u => u.id_uk.Equals(uk.id));

            }
            catch
            { }


            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public string get_adr(int id_adr = 0)
        {
            if (id_adr == 0) return "Адрес не назначен";
            uk_adress adr = repository.uk_adress.Where(id => id.id.Equals(id_adr)).SingleOrDefault();
            if (adr == null)
                return "Ошибка! Адрес не найден!";
            return adr.City + ", " + adr.Street + ", " + adr.House;
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