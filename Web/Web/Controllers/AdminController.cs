using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Models;
using Web.Models.Repository;
using System.Data;
//using WebMatrix.WebData;
using System.Web.Security;
//using Web.Filter;

namespace Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {

        Repo repository;
        public AdminController()
        {
            repository = new Repo();
        }

        //
        // GET: /Admin/
        public ViewResult Index()
        {
            return View();
        }

        public ViewResult Articles()
        {
            return View(repository.Articles.OrderBy(p => p.id_uk));
        }

        public ViewResult EditArticle(int id = 0)
        {
            Article art;
            if (id == 0)
            {
                art = new Article();
            }
            else
                art = repository.Articles.Where(q => q.id == id).Single();

//            UserProfile profile = repository.UserProfile.Where(i => i.id_uk(art.id_uk)).SingleOrDefault();
            IEnumerable<uk_profile> list_uk = repository.uk_profile.OrderBy(p => p.id);
            ViewData["uk_profile"] = from n in list_uk
                                     select new SelectListItem { Text = n.Name, Value = n.id.ToString() };

            return View(art);
        }

        // Перегруженная версия Edit() для сохранения изменений
        [HttpPost]
        public ActionResult EditArticle(Article article)
        {
            if (ModelState.IsValid)
            {
                article.publicDate = DateTime.UtcNow;// -TimeZone.CurrentTimeZone; ;
                repository.SaveArticle(article);
                TempData["message"] = string.Format("Изменения странице \"{0}\" были сохранены", article.title);
                
                return RedirectToAction("Index");
            }
            else
            {
                IEnumerable<uk_profile> list_uk = repository.uk_profile.OrderBy(p => p.id);
                ViewData["uk_profile"] = from n in list_uk
                                         select new SelectListItem { Text = n.Name, Value = n.id.ToString() };
                return View(article);
            }
        }

        public ViewResult CreateArt()
        {
            IEnumerable<uk_profile> list_uk = repository.uk_profile.OrderBy(p => p.id);
            ViewData["uk_profile"] = from n in list_uk
                                     select new SelectListItem { Text = n.Name, Value = n.id.ToString() };
            return View("EditArticle", new Article());
        }

        public ViewResult ViewUsers()
        {
            IEnumerable<UserProfile> users = repository.UserProfile.OrderBy(q => q.id_uk);
            return View(users);
        }
       
 

        //public ActionResult EditUser(int id = 0)
        //{
        //    UserProfile profile = repository.UserProfile.Where(i => i.UserId == id).SingleOrDefault();
 
        //    IEnumerable<uk_profile> list_uk = repository.uk_profile.OrderBy(p => p.id);
        //    ViewData["uk_profile"] = from n in list_uk
        //                       select new SelectListItem { Text = n.Name, Value = n.id.ToString() };

        //    IEnumerable<uk_adress> list_adr = repository.uk_adress.Where(p =>p.id_uk.Equals(profile.id_uk)).OrderBy(p => p.id);
        //    ViewData["uk_adress"] = from adr in list_adr
        //                             select new SelectListItem { Text = adr.City.ToString() + ", "+adr.Street.ToString() + ", " + adr.House, Value = adr.id.ToString() };

        //    //присвоить роль пользователю
        //    UserProfile_form model = new UserProfile_form(profile);

        //    model.Role = Roles.GetRolesForUser(profile.login);
        //    return View(model);
        //}

        public ActionResult EditUser(string login = "")
        {
            UserProfile profile = repository.UserProfile.Where(i => i.login.Equals(login)).SingleOrDefault();
            IEnumerable<uk_profile> list_uk = repository.uk_profile.OrderBy(p => p.id);
            ViewData["uk_profile"] = from n in list_uk
                                         select new SelectListItem { Text = n.Name, Value = n.id.ToString() };
            IEnumerable<uk_adress> list_adr;
            if (profile!=null)
            {
                list_adr = repository.uk_adress.Where(p => p.id_uk.Equals(profile.id_uk)).OrderBy(p => p.id);
               ViewData["uk_adress"] = from adr in list_adr
                                        select new SelectListItem { Text = adr.City.ToString() + ", " + adr.Street.ToString() + ", " + adr.House, Value = adr.id.ToString()};
             }
            else
            {
                List<uk_adress> list = new List<uk_adress>();
                ViewData["uk_adress"] = from adr in list.ToArray()
                                        select new SelectListItem {  Text = adr.City.ToString() + ", " + adr.Street.ToString() + ", " + adr.House, Value = adr.id.ToString() };
           
            }
               
            if (profile != null)
            {
                UserProfile_form model = new UserProfile_form(profile);

                model.Role = Roles.GetRolesForUser(profile.login);
                return View(model);
            }
            else
            //присвоить роль пользователю
            {
                UserProfile_form model = new UserProfile_form();
                model.UserId = Convert.ToInt32(Membership.GetUser(login).ProviderUserKey);
                return View(model);
            }
            
        }

        [HttpPost]
        public ActionResult EditUser(UserProfile_form model)
        { 
            webpages_UsersInRoles userrole = new webpages_UsersInRoles();
            string[] ir = Roles.GetRolesForUser(model.login);
            
            if (ir.Length>0)
                Roles.RemoveUserFromRoles(model.login,ir);
            if (model.Role!=null)
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

            IEnumerable<uk_adress> list_adr = repository.uk_adress.Where(p => p.id_uk.Equals(model.id_uk)).OrderBy(p => p.id);
            ViewData["uk_adress"] = from adr in list_adr
                                    select new SelectListItem { Text = adr.City.ToString() + ", " + adr.Street.ToString() + ", " + adr.House, Value = adr.id.ToString() };

            //присвоить роль пользователю
            model.Role = Roles.GetRolesForUser(model.login);
            return View(model);
        }

        public ActionResult DetailUser(int id = 0)
        {
            UserProfile profile = repository.UserProfile.Where(i => i.id == id).SingleOrDefault();
            return View(profile);
        }

        public ActionResult DeleteUser(int id = 0)
        {
            if (id != 0)
                repository.DeleteUser(id);
            return RedirectToAction("ViewUsers");

        }

        public ActionResult ViewUk()
        {
            IEnumerable<uk_profile> uk = repository.uk_profile.OrderBy(p => p.id);
            return View(uk);
        }

        public ActionResult CreateUk()
        {
           return  View(new uk_profile());
        }

        [HttpPost]
        public ActionResult CreateUk(uk_profile uk)
        {
            uk.RegDate = DateTime.UtcNow;
            repository.SaveUkProfile(uk);
            //Создать роль пользователей
            return RedirectToAction("ViewUk");
        }

        public ActionResult EditUk(int id = 0)
        {
            uk_profile uk = repository.uk_profile.Where(p => p.id.Equals(id)).SingleOrDefault();
            
            return View(uk); 
        }
        [HttpPost]
        public ActionResult EditUk(uk_profile uk = null)
        {
            repository.SaveUkProfile(uk);
        //обновить роль пользователей
            return View(uk);
        }

        public ActionResult DeleteUk(int id = 0)
        {
            uk_profile del_uk = repository.DeleteUk(id);
            if (del_uk != null)
            {
                TempData["message"] = string.Format("ТСЖ \"{0}\"был удален", del_uk.Name);
            }
            return RedirectToAction("ViewUk");
        }

        public ActionResult ViewAddrUk(int id = 0)
        {
            return View(repository.uk_adress.Where(p =>p.id_uk == id));
        }


        public ActionResult DeleteAddr(int id = 0)
        {
            uk_adress del_adr = repository.DeleteUkAddr(id);
            if (del_adr != null)
            {
                TempData["message"] = string.Format("Адрес \"{0}\" \"{1}\" был удален", del_adr.Street, del_adr.House);
            }
            return RedirectToAction("ViewUk"); 
        }

        public ActionResult EditAddr(int id = 0)
        {   
            uk_adress uk = repository.uk_adress.Where(p => p.id == id).SingleOrDefault();
            return View(uk);
        }
        
        public ActionResult CreateAddr(int id = 0)
        {
            uk_adress uk = new uk_adress();
            uk.id_uk = id;
            return View(uk);
        }

        [HttpPost]
        public ActionResult CreateAddr(uk_adress adr)
        {
            adr.id = 0;
            repository.SaveUkAdress(adr);
            TempData["message"] = string.Format("Новый адрес создан");
            return RedirectToAction("EditUk", new { id = adr.id_uk });
        }

        [HttpPost]
        public ActionResult EditAddr(uk_adress adr)
        {
            repository.SaveUkAdress(adr);
            TempData["message"] = string.Format("Адрес был изменен");
            return RedirectToAction("EditUk", new { id = adr.id_uk });
        }

        public ActionResult ViewRoles()
        {
            IEnumerable<webpages_Roles> listRole = repository.webpages_Roles;
            //Roles.GetAllRoles();
            return View(listRole);
        }

        public ActionResult EditRole(int id = 0)
        {
            if (id == 0) return View(new webpages_Roles());
            
            return View(repository.webpages_Roles.Where(p => p.RoleId.Equals(id)).SingleOrDefault());
        }

        [HttpPost]
        public ActionResult EditRole(webpages_Roles role)
        {
            repository.SaveRole(role);
            TempData["message"] = string.Format("Роль \"{0}\" изменена", role.RoleName);
            return View(role);
        }

        public ActionResult DeleteRole(string id = "")
        {
            Roles.DeleteRole(id);
            TempData["message"] = string.Format("Роль \"{0}\" удалена", id);
            return RedirectToAction("ViewRoles");
        }

        
        public ActionResult ViewUserInRoles(int id = -1)
        {
            List<UserInRole_model_> list = new List<UserInRole_model_>();
            DataSet ds;
            //MemberShip.GetAllUsers() не пашет, лень ковыряться
            //Поэтому обращаемся на прямую в базу )) 
            //Необходимо дополнить страничный вывод
            repository.SQLstringConnect("SELECT login FROM UserAccount", out ds);
            //Заполняем наш массив данными из таблшицы
            foreach (DataRow dr in  ds.Tables[0].Rows)
            {
                UserInRole_model_ res = new UserInRole_model_() ;
                res.UserName = dr["Login"].ToString();
                res.RoleName = Roles.GetRolesForUser(res.UserName);
                list.Add(res);
            }

            return View(list.OrderBy(p => p.UserName).ToArray());
        }

        public ActionResult DeleteAccount(string id)
        {
            
            //Membership.FindUsersByEmail("");
            //repository.DeleteAccount(id);
            Membership.DeleteUser(id);
            return View("ViewAllUsers");
        }

    }
}
