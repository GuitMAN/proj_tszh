using System;
using System.Collections.Generic;
using WebMatrix.WebData;
using System.Data;
using System.Linq;



namespace Web.Models.Repository
{

    public class Repo
    {
        private EFDbContext context = new EFDbContext();


        public void SQLstringConnect(string sqlcommand, out DataSet ds)
        {
            context.SQLStringConnect(sqlcommand, out ds);
        }
     
        public IEnumerable<Article> Articles
        {
            get { return context.Articles; }
        }

        public IEnumerable<UserProfile> UserProfile
        {
            get { return context.UserProfiles; }
        }

        public IEnumerable<Admtszh> Admtszh
        {
            get { return context.admtszh; }
        }


        public void SaveAdmtszh(Admtszh user)
        {
            if (user.id == 0)
            {
                Admtszh db = context.admtszh.Add(user);
                db.AdmtszhId = user.AdmtszhId;
            }
            else
            {
                Admtszh db = context.admtszh.Find(user.id);
                if (db != null)
                {
                    db.AdmtszhId = user.AdmtszhId;
                    db.id_uk = user.id_uk;
                    db.post = user.post;
                    db.SurName = user.SurName;
                    db.Name = user.Name;
                    db.Patronymic = user.Patronymic;

                }
                
            }
            context.SaveChanges();
        }
        public IEnumerable<MenuItem> MenuItems
        {
            get { return context.MenuItems; }
        }

        public void SaveMenuItem(MenuItem item)
        {
            if (item.id == 0)
            {
                context.MenuItems.Add(item);
            }
            else
            {
                MenuItem dbEntry = context.MenuItems.Find(item.id);
                if (dbEntry != null)
                {
                    dbEntry.id = item.id;
                    dbEntry.title = item.title;
                    dbEntry.menu_id = item.menu_id;
                    dbEntry.parent_id = item.parent_id;
                }
            }
            context.SaveChanges();
        }

        public void SaveArticle(Article article)
        {
            if (article.id == 0)
            {
                article.publicDate = DateTime.UtcNow;
                context.Articles.Add(article);              
            }
            else
            {
                Article dbEntry = context.Articles.Find(article.id);
                if (dbEntry != null)
                {
                    dbEntry.id = article.id;
                    dbEntry.title = article.title;
                    dbEntry.summary = article.summary;
                    dbEntry.content = article.content;
                    dbEntry.publicDate = DateTime.UtcNow;
                }     
            }
            context.SaveChanges();
        }

        public Article GetArticle(string str)
        {
            return context.GetArticle(str);
        }

        public IEnumerable<uk_profile> uk_profile
        {
           get {  return context.uk_profiles;  }
        }

        public void SaveUkProfile(uk_profile uk)
        {
            if (uk.id == 0)
            {
                context.uk_profiles.Add(uk);
            }
            else
            {
                uk_profile db = context.uk_profiles.Find(uk.id);
                if (db != null)
                {
                    db.Name = uk.Name;
                    db.host = uk.host;
                }
            }
            context.SaveChanges();
        }


        public IEnumerable<uk_adress> uk_adress
        {
            get { return context.uk_adresses; }
        }


        public void SaveUkAdress(uk_adress uk)
        {
            if (uk.id == 0)
            {
                context.uk_adresses.Add(uk);
            }
            else
            {
                uk_adress db = context.uk_adresses.Find(uk.id);
                if (db != null)
                {
                    db.id_uk = uk.id_uk;
                    db.City = uk.City;
                    db.Street = uk.Street;
                    db.House = uk.House;
                }
            }
            context.SaveChanges();
        }


        public uk_profile DeleteUk(int id)
        {
            uk_profile db = context.uk_profiles.Find(id);
            if (db != null)
            {
                context.uk_profiles.Remove(db);
                context.SaveChanges();
            }
            return db;
        }


        public uk_adress DeleteUkAddr(int id)
        {
            uk_adress db = context.uk_adresses.Find(id);
            if (db != null)
            {
                context.uk_adresses.Remove(db);
                context.SaveChanges();
            }
            return db;
        }

        public IEnumerable<feedback> feedback
        {
            get { return context.Feedback; }
        }

        public void SaveFeedBack(feedback message)
        {
            context.Feedback.Add(message);
            context.SaveChanges();
        }

        public void DeleteFeedBack(int id)
        {
            feedback db = context.Feedback.Find(id);
            if (db != null)
            {
                context.Feedback.Remove(db);
                context.SaveChanges();
            }
        }

        public void SaveUser(UserProfile user)
        {
            if (user.id == 0)
            {
                 UserProfile db = context.UserProfiles.Add(user);
                 db.UserId = user.UserId;
                 db.login = user.login;

           }
           else
           {
               UserProfile db = context.UserProfiles.Find(user.id);
               if (db != null)
               {
                   db.UserId = WebSecurity.CurrentUserId;
                   db.id_uk = user.id_uk;
                   db.login = user.login;
                   db.mobile = user.mobile;
                   db.Name = user.Name;
                   db.Patronymic = user.Patronymic;
                   db.Personal_Account = user.Personal_Account;
                   db.phone = user.phone;
                   db.SurName = user.SurName;
                   db.Adress = user.Adress;
                   db.Apartment = user.Apartment;
                   db.Email = user.Email;

               }
           }
           context.SaveChanges();
        }

        public UserProfile DeleteUser(int id)
        {
            UserProfile db = context.UserProfiles.Find(id);
            if (db != null)
            {
                context.UserProfiles.Remove(db);
                context.SaveChanges();
            }
            return db;
        }

        public bool DeleteAccount(string id)
        {


            return true;
        }


        public IEnumerable<webpages_Roles> webpages_Roles
        {
            get { return context.webpages_roles; }
        }

        public void SaveRole(webpages_Roles role)
        {
            if (role.RoleId == 0)
            {
                context.webpages_roles.Add(role);
            }
            else
            {
                webpages_Roles db = context.webpages_roles.Find(role.RoleId);
                if (db != null)
                {
                    db.RoleName = role.RoleName;
                }
            }
            context.SaveChanges();
        }

        public webpages_Roles DeleteRole(int id)
        {
            webpages_Roles db = context.webpages_roles.Find(id);
            if (db != null)
            {
                context.webpages_roles.Remove(db);
                context.SaveChanges();
            }
            return db;
        }

        public IEnumerable<webpages_UsersInRoles> webpages_UsersInRoles
        {
            get { return context.webpages_usersinroles; }
        }



        public void SaveUserRole(webpages_UsersInRoles user)
        {
            webpages_UsersInRoles db = context.webpages_usersinroles.Find(user.UserId);

            if (db == null)
            {
                db = new webpages_UsersInRoles();
                db.UserId = user.UserId;
                db.RoleId = user.RoleId;
                context.webpages_usersinroles.Add(db);
            }
            else
            {
               db.UserId = user.UserId;
               db.RoleId = user.RoleId;
            }
            context.SaveChanges();
        }
        public webpages_UsersInRoles DeleteUserRole(int id)
        {
            webpages_UsersInRoles db = context.webpages_usersinroles.Find(id);
            if (db != null)
            {
                context.webpages_usersinroles.Remove(db);
                context.SaveChanges();
            }
            return db;
        }

        public IEnumerable<Counter> Counter
        {
            get { return context.counter; }
        }

        public void SaveCounter (Counter cou)
        {
            Counter db = context.counter.Find(cou.id);
           
            
            if (db == null)
            {
               context.counter.Add(cou);
            }
            else
            {
                db.DateOfReview = cou.DateOfReview;
                db.place = cou.place;
                db.serial = cou.serial;
                db.status = cou.status;
                db.type = cou.type;
                db.UserId = cou.UserId;
            }
            context.SaveChanges();
        }

        public void SaveCounder_data(Counter_data cou)
        {
            DateTime seek = DateTime.Today;
            int month = seek.Month;
            int year = seek.Year;
            DataSet t;
            string sql = "SELECT * FROM [dbo].[Counter_data] WHERE id = " + cou.id.ToString() + " AND write >= '" + year.ToString() + "." + month.ToString() + ".01'";
            if (context.SQLStringConnect(sql, out t) != null)
            {
                if (t.Tables.Count == 0)
                    context.SQLStringConnect("INSERT INTO [dbo].[Counter_data] ([id],[write],[data]) VALUES    ( " + cou.id + "   , '" + cou.write + "' , '" + cou.data + "') ");
                else
                    context.SQLStringConnect("UPDATE [dbo].[Counter_data] SET write = '" + cou.write + "' , data = '" + cou.data + "' WHERE id = " + cou.id + " AND write >= '" + year + "." + month + ".01'");
            }
        }

    }
}