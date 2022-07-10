using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RepositoryLayer.Services
{
    public class EmailService
    {
        public static void SendEmail(string email, string token, string Firstname)
        {
            using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential("testingemailvinay@gmail.com", "obpwhgpaasbhfvjg");
                MailMessage msgObj = new MailMessage();
                msgObj.To.Add(email);
                msgObj.IsBodyHtml = true;
                msgObj.From = new MailAddress("testingemailvinay@gmail.com");
                msgObj.Subject = "Password Reset Link";
                msgObj.Body = "<html><body><p><b>Hello " + $"{Firstname}" + "</b>,<br/>Please click the below link to Reset Your Password.<br/>" +
                   $"www.fundooapp.com/reset-password/{token}" +
                   "<br/><br/><br/><b>Thanks&Regards </b><br/><b>Mail Team(donot - reply to this mail)</b></p></body></html>";
                client.Send(msgObj);
            }
        }
    }
}
