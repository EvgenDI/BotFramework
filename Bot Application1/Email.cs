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
    public class Email
    {
        public static bool correct(string email)
        {
            if (Regex.IsMatch(email, @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task SendEmailAsync(EntityRecommendation eemail, string cod)
        {

            MailAddress from = new MailAddress("super.geroi2018@yandex.ru", "BotEvgen");
            MailAddress to = new MailAddress(eemail.Entity);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Редактирование электронной почты";
            m.Body = cod;
            SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 25);
            smtp.Credentials = new NetworkCredential("super.geroi2018@yandex.ru", "1234Qwer");
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
        }



    }
}