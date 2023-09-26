using System.Web.Mvc;
using Project.DAL;
using System.Collections.Generic;
using System;
using PAARS.DAL;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class IndexViewModel
    {
        public List<News> category
        {
            get;
            set;
        }

        public List<News> CountNews
        {
            get;
            set;
        }

        public List<CPC> CPCList
        {
            get;
            set;
        }

        public SelectList Currencies
        {
            get;
            set;
        }

        public CurrencyConverterForm CurrencyConverterForm
        {
            get;
            set;
        }

        public List<ExchangeRateView> CurrencyList
        {
            get;
            set;
        }

        public DutyFeedbackForm Dutyfeedbacform
        {
            get;
            set;
        }

        public Event EventInformation
        {
            get;
            set;
        }

        public ContactForm feedbackform
        {
            get;
            set;
        }

        public bool HasNewEvent
        {
            get;
            set;
        }

        public string IpAddress
        {
            get;
            set;
        }

        public Location locationForm
        {
            get;
            set;
        }

        public string masterURL
        {
            get;
            set;
        }

        public News news
        {
            get;
            set;
        }

        public List<string> newsCategory
        {
            get;
            set;
        }

        public DateTime newsdate
        {
            get;
            set;
        }

        public List<DateTime> newslist
        {
            get;
            set;
        }

        public List<News> NewsList
        {
            get;
            set;
        }

        public string PicturePath
        {
            get;
            set;
        }

        public List<Testimonials> Testimonials
        {
            get;
            set;
        }
    }
   
    public class CurrencyConverterForm
    {
        public int Country { get; set; }
        public double Amount { get; set; }
    }

    public class ContactForm
    {
        [Required(ErrorMessage = "Please enter your full Name")]
        [Display(Name = "Full Name")]
        public string full_name { get; set; }

        [Required(ErrorMessage = "Please enter your Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }


        [Required(ErrorMessage = "Please enter your Message")]
        [Display(Name = "Message")]
        public string MessageInput { get; set; }
    }

    public class Location
    {
        public string CityName
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }

        public string CountryName
        {
            get;
            set;
        }

        public string IPAddress
        {
            get;
            set;
        }

        public string Latitude
        {
            get;
            set;
        }

        public string Longitude
        {
            get;
            set;
        }

        public string RegionName
        {
            get;
            set;
        }

        public string TimeZone
        {
            get;
            set;
        }

        public string ZipCode
        {
            get;
            set;
        }
    }

  

}