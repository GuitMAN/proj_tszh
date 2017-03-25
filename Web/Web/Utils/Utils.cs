using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Web.Models;
using Web.Models.Repository;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using log4net;

namespace Web.Utils
{
    //public class Log
    //{
    //    private static object sync = new object();
    //    public static void Write(Exception ex)
    //    {
    //        try
    //        {
    //            // Путь .\\Log
    //            string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
    //            if (!Directory.Exists(pathToLog))
    //                Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
    //            string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log",
    //            "",//AppDomain.CurrentDomain.FriendlyName, 
    //            DateTime.Now));
    //            string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3}\r\n",
    //            DateTime.Now, ex.TargetSite.DeclaringType, ex.TargetSite.Name, ex.Message);
    //            lock (sync)
    //            {
    //                File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
    //            }
    //        }
    //        catch
    //        {
    //            // Перехватываем все и ничего не делаем
    //        }
    //    }
    //}

//    public static void SendMail(string smtpServer, string from, string password,
//string mailto, string caption, string message, string attachFile = null)
//    {
//        try
//        {
//            MailMessage mail = new MailMessage();
//            mail.From = new MailAddress(from);
//            mail.To.Add(new MailAddress(mailto));
//            mail.Subject = caption;
//            mail.Body = message;
//            if (!string.IsNullOrEmpty(attachFile))
//                mail.Attachments.Add(new Attachment(attachFile));
//            SmtpClient client = new SmtpClient();
//            client.Host = smtpServer;
//            client.Port = 587;
//            client.EnableSsl = true;
//            client.Timeout = 15000;
//            client.UseDefaultCredentials = false;
//            client.Credentials = new NetworkCredential(from, password);//.Split('@')[0]
//            client.DeliveryMethod = SmtpDeliveryMethod.Network;
//            client.Send(mail);
//            mail.Dispose();
//        }
//        catch (Exception ex)
//        {
//            Logger.Log.Error("Не удалось отправить письмо на адрес " + mailto, ex);
//        }
//    }
}