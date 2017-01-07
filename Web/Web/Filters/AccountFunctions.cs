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
        public bool getAccount(string Login, out string RegData)
        {

            DataSet ds;
            repository.SQLstringConnect("SELECT CreateDate FROM webpages_Membership WHERE UserId = (SELECT id FROM UserAccount WHERE Login = "+Login+")", out ds);
            if (ds.Tables.Count == 1)
            {

                RegData = ds.Tables[0].Rows[0]["CreateDate"].ToString();
                //Заполняем наш массив данными из таблшицы
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    RegData = dr["CreateDate"].ToString();
                }
            }
            RegData = null;
            return false;
        }

    }
}