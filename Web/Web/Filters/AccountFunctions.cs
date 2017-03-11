using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using Web.Models;
using Web.Models.Repository;

namespace Web.Filters
{
    public class AccountFunctions
    {
        Repo repository;

        public AccountFunctions(Repo insert_repo)
        {
            repository = insert_repo;
        }

        public bool getAccount(string Login, out string RegData)
        {

            DataSet ds;
            string sql_str = "SELECT CreateDate FROM [dbo].[webpages_Membership] WHERE UserId = (SELECT id FROM [dbo].[UserAccount] WHERE Login = '" + Login + "')";
            repository.SQLstringConnect(sql_str, out ds);
            RegData = null;
            if (ds.Tables[0].Rows.Count == 1)
            {
                Logger.Log.Info("User: " + Login + " - get CreateDate");
                //RegData = ds.Tables[0].Rows[0]["CreateDate"].ToString();
                //Заполняем наш массив данными из таблшицы
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    RegData = dr["CreateDate"].ToString();
                    return true;
                }
                
            }              
            return false;
        }

    }
}