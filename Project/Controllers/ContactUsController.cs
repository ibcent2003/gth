using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sw = GNSW.DAL;

namespace Project.Controllers
{
    public class ContactUsController : Controller
    {
        //
        // GET: /ContactUs/
        private PROEntities db = new PROEntities();
       // private cha.CHAMSEntities chamsdb= new cha.CHAMSEntities();
        private sw.GNSWEntities swdb = new sw.GNSWEntities();
        public ActionResult Index()
        {

            ContactUsViewModel model = new ContactUsViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ContactUsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HandlingService AppModel = new HandlingService();
                    //Alert to send to user
                    var UserAlert = swdb.Alert.Where(x => x.Id == 17).FirstOrDefault();

                    //Alert to send to admin
                    var adminAlert = swdb.Alert.Where(x => x.Id == 18).FirstOrDefault();

                    ContactUs addnew = new ContactUs
                    {
                        Fullname = model.contactusform.fullname,
                        EmailAddress = model.contactusform.EmailAddress,
                        PhoneNo = model.contactusform.MobileNumber,
                        Message = model.contactusform.Message,
                        MessageType = "Feedback",
                        SentDate = DateTime.Now,
                        IsReply = false
                    };
                    db.ContactUs.AddObject(addnew);
                    db.SaveChanges();
                    #region Send Notification to GTH User
                    AppModel.SendEmailNotificationToUser(UserAlert.SubjectEmail, UserAlert.Email.Replace("%FullName%", model.contactusform.fullname), model.contactusform.EmailAddress, "no-reply@ghanatradinghub.com", UserAlert.Id);
                 //  AppModel.SendSMSNotificationToUser(UserAlert.SubjectSms, UserAlert.Sms.Replace("%FullName%", model.contactusform.fullname), model.contactusform.MobileNumber, UserAlert.SubjectSms, UserAlert.Id);
                    #endregion

                    #region send notification to GTH Admin

                   
                    var poolUser = new List<GNSW.DAL.Users>();
                    var rl = swdb.Roles.SingleOrDefault(x => x.RoleName == "GTH Admin");

                    var GetUser = rl.Users.ToList();
                    foreach (var u in GetUser)
                    {
                        //var GetUserInformation = swdb.UserDetails.Where(x => x.UserId == u.UserId).FirstOrDefault();
                        //if (GetUserInformation != null)
                        //{
                        //    if (GetUserInformation.PhoneNumber != null)
                        //    {
                        //        AppModel.SendSMSNotificationToAdmin(adminAlert.SubjectSms, adminAlert.Sms.Replace("%Username%", GetUserInformation.FirstName), GetUserInformation.PhoneNumber, adminAlert.SubjectSms, adminAlert.Id);
                        //    }
                        //}
                        AppModel.SendEmailNotificationToChamsAdmin(adminAlert.Title, adminAlert.Email.Replace("%Username%", u.UserName), u.Memberships.Email, model.contactusform.EmailAddress, adminAlert.Id);
                    }
                    #endregion

                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Your Message has been sent successully. We'll review your message and contact you within 7 working days. <b>Thank You.</b>";
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404");
            }
        }

    }
}
