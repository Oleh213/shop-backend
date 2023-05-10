using System;
using System.Net.Mail;
using sushi_backend.Interfaces;
using WebShop.Models;

namespace sushi_backend.BusinessLogic
{
	public class EmailSenderBL : IEmailSender
	{
        public bool SentEmail(string Message)
		{
            MailMessage mail = new MailMessage();
            mail.To.Add("olegantemenuk5@gmail.com");
            mail.From = new MailAddress("oa792289@gmail.com", "Umami sushi");
            mail.Subject = "New order!";
            mail.Body = Message;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587; // 25 465
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com";
            smtp.Credentials = new System.Net.NetworkCredential("oa792289@gmail.com", "dqajjwwbrcuurqgf");
            smtp.Send(mail);

            return true;
        }
	}
}

