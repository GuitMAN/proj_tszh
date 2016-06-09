//using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Web.Mvc;
//using System.Data.SqlClient;
using System.Data;
//using Web.Models;
using Web.Models.Repository;


namespace Web.Controllers
{

    public class NavController : Controller
    {
        //
        // GET: /Nav/
        Repo repository;

        public NavController()
        {
            repository = new Repo();
        }

        public PartialViewResult Menu(int menu_id = 0)
        {
            //Связываемся с нашей БД
            //Repo p = new Repo();
            //Отправляем результат предствалению
            return PartialView(repository.MenuItems.Where(q => q.menu_id.Equals(menu_id)).OrderBy(a => a.id));
        }



        public PartialViewResult Menu_main(string category = null)
        {
            ViewBag.SelectedCategory = category;

            IEnumerable<string> categories = repository.Articles
                .Select(title => title.title)
                .Distinct()
                .OrderBy(x => x);
            return PartialView(categories);
        }

        public PartialViewResult menu_top(string category = null)
        {
            ViewBag.SelectedCategory = category;

            IEnumerable<string> categories = repository.Articles
                .Select(title => title.title)
                .Distinct()
                .OrderBy(x => x);
            return PartialView(categories);
        }

        public PartialViewResult menu_moder()
        {
            return PartialView();
        }
    }
}
