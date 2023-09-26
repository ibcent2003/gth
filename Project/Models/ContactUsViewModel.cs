using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ContactUsViewModel
    {
        public ContactUsForm contactusform { get; set; }
    }

    public class ContactUsForm
    {
        [Required(ErrorMessage = "Please enter your full Name")]
        [Display(Name = "Full Name")]
        public string fullname { get; set; }

        [Required(ErrorMessage = "Please enter your Email Address")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter your Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }


        [Required(ErrorMessage = "Please enter your Message")]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }

}