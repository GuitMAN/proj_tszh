using System;
using Microsoft.Web.WebPages.OAuth;
using Web.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using WebMatrix.WebData;
using System.Threading;


namespace Web
{
    public static class AuthConfig
    {

        public static void RegisterAuth()
        {

  
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // следует обновить сайт. Дополнительные сведения: http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "",
            //    appSecret: "");

            //OAuthWebSecurity.RegisterGoogleClient();

            //System.Data.Entity.Database.SetInitializer<EFDbContext>(null);

            //try
            //{
            //    using (var context = new EFDbContext())
            //    {
            //        if (!context.Database.Exists())
            //        {
            //            // Создание базы данных SimpleMembership без схемы миграции Entity Framework
            //            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
            //        }
            //    }

            //    WebSecurity.InitializeDatabaseConnection("EFDbContext", "UserAccount", "id", "Login", autoCreateTables: true);
            //}
            //catch (Exception ex)
            //{
            //    throw new InvalidOperationException("Не удалось инициализировать базу данных ASP.NET Simple Membership. Чтобы получить дополнительные сведения, перейдите по адресу: http://go.microsoft.com/fwlink/?LinkId=256588", ex);
            //}
        }
    }

}
