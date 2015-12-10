using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;


namespace Web.Models
{
    public class EFDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<uk_profile> uk_profiles { get; set; }
        public DbSet<feedback> Feedback { get; set; }
        public DbSet<uk_adress> uk_adresses { get; set; }
        public DbSet<Admtszh> admtszh { get; set; }
        public DbSet<webpages_Roles> webpages_roles { get; set; }
        public DbSet<webpages_UsersInRoles> webpages_usersinroles { get; set; }
        public DbSet<Counter> counter { get; set; }
        public DbSet<Counter_data> counter_data { get; set; }


        //Вывод статьи через вызов процедуры
        public Article GetArticle(string str)
        {
            //Вывод результата
            Article res = new Article();
            DataSet ds;
                  
            string ex = SQLStringConnect("GetArticle @str='" + str + "';", out ds);
            if (ex != "")
            { 
                res.content = ex;
                return res;
            }
            
            //Заполняем наш массив данными из таблшицы
            foreach (DataRow dr in  ds.Tables[0].Rows)
            {
                res.id = Convert.ToInt32(dr["id"].ToString());
                res.title = dr["title"].ToString();
                res.summary = dr["summary"].ToString();
                res.content = dr["content"].ToString();
                res.publicDate = Convert.ToDateTime(dr["publicDate"]);
            }

            return res;
        }

        public string SQLStringConnect(string sqlcommand)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EFDbContext"];
            //Открываем соедиенние с БД
            try
            {
                SqlConnection con = new SqlConnection(connectionString.ConnectionString);
                SqlCommand sqlcomm = new SqlCommand(sqlcommand, con);
                con.Open();
                int scalarReturned = sqlcomm.ExecuteNonQuery();
                con.Close();
                //закрываем соединение с БД

            }
            catch (Exception ex)
            {
                string str = "АХТУНГ!!!: " + ex.Message + "; Метод, вызвавший исключение:  " + ex.TargetSite;

                return str;

            };
            return "";
        }

        public string SQLStringConnect(string sqlcommand, out DataSet ds)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EFDbContext"];
            //Открываем соедиенние с БД
            try
            {
                SqlConnection con = new SqlConnection(connectionString.ConnectionString);
                SqlDataAdapter adap = new SqlDataAdapter(sqlcommand, con);
                //Получаем массив данных
                ds = new DataSet();
                adap.Fill(ds);
                con.Close();
                //закрываем соединение с БД
                
            }
            catch (Exception ex)
            {              
                string str = "АХТУНГ!!!: "+ex.Message + "; Метод, вызвавший исключение:  " + ex.TargetSite;
                ds = new DataSet();
                return str; 
           
            };
   
            return "";
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Counter_data>().Property(p => p.data).HasPrecision(15, 3);
            base.OnModelCreating(modelBuilder);
        }

        
 
    }
}