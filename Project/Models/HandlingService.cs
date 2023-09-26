using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CHAMS.DAL;



namespace Project.Models
{
    public class HandlingService
    {
        //private GNSWEntities swdb = new GNSWEntities();

        private CHAMSEntities chamsdb = new  CHAMSEntities();



        public void SendEmailNotificationToChamsAdmin(string subject, string message, string EmailAddress, string sender, int Id)
        {
            AlertNotification alertNotification = new AlertNotification()
            {
                AlertType = 2,
                Subject = subject,
                Message = message,
                Sender = sender,
                Receiver = EmailAddress,
                ModifiedBy = "System",
                ModifiedDate = new DateTime?(DateTime.Now),
                SentDate = DateTime.Now,
                Status = 0
            };
            this.chamsdb.AlertNotification.AddObject(alertNotification);
            this.chamsdb.SaveChanges();
        }

        public void SendEmailNotificationToUser(string subject, string message, string EmailAddress, string sender, int Id)
        {
            AlertNotification alertNotification = new AlertNotification()
            {
                AlertType = new int?(2),
                Subject = subject,
                Message = message,
                Sender = sender,
                Receiver = EmailAddress,
                ModifiedBy = "System",
                ModifiedDate = new DateTime?(DateTime.Now),
                SentDate = DateTime.Now,
                Status = new int?(0)
            };
            this.chamsdb.AlertNotification.AddObject(alertNotification);
            this.chamsdb.SaveChanges();
        }

        public void SendSMSNotificationToAdmin(string subject, string message, string PhoneNumber, string sender, int Id)
        {
            AlertNotification alertNotification = new AlertNotification()
            {
                AlertType = new int?(1),
                Subject = subject,
                Message = message,
                Sender = sender,
                Receiver = PhoneNumber,
                ModifiedBy = "System",
                ModifiedDate = new DateTime?(DateTime.Now),
                SentDate = DateTime.Now,
                Status = new int?(0)
            };
            this.chamsdb.AlertNotification.AddObject(alertNotification);
            this.chamsdb.SaveChanges();
        }

       
    }


}