using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1
{
    [Serializable]
    public class Email
    {
        private const string emailRegExPattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
        private string host;
        private string user;
        private string password;
        private string from;
        private int port;



        public Email(string host, int port, string user, string password,string from)
        {
            this.host = host;
            this.port = port;
            this.user = user;
            this.password = password;
            this.from = from;
        }



        public bool Correct(string email)
        {
            if (Regex.IsMatch(email,emailRegExPattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public async Task SendEmailAsync(EntityRecommendation email, string cod)
        {
            //MailAddress from = new MailAddress("super.geroi2018@yandex.ru", "BotEvgen");
            //MailAddress to = new MailAddress(email.Entity);
            //MailMessage m = new MailMessage(from, to);
            //m.Subject = "Редактирование электронной почты";
            //m.Body = cod;
            //SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 25);
            //smtp.Credentials = new NetworkCredential("super.geroi2018@yandex.ru", "1234Qwer");
            //smtp.EnableSsl = true;
            //await smtp.SendMailAsync(m);
            MailAddress fromMail = new MailAddress(from, "BotEvgen");
            MailAddress toMail = new MailAddress(email.Entity);
            MailMessage m = new MailMessage(fromMail, toMail);
            m.Subject = "Редактирование электронной почты";
            m.Body = cod;
            SmtpClient smtp = new SmtpClient(host, port);
            smtp.Credentials = new NetworkCredential(user, password);
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
        }
    }
}