﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Models;
using Web.Models.Repository;
using System.Data;
using WebMatrix.WebData;
using System.Web.Security;
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

        public ActionResult get_users()
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

    }
}