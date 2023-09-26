using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Models
{
    public class EventViewModel
    {
        public IList<Event> Rows { get; set; }

        public EventForm eventform { get; set; }
    }

    public class EventForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Event Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the Event Venue")]
        [Display(Name = "Venue")]
        public string Venue { get; set; }

        [Required(ErrorMessage = "Please enter the Event Date")]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Please enter the Event Time")]
        [Display(Name = "Event Time")]
        public string EventTime { get; set; }

       
        [Display(Name = "Has Ended?")]
        public bool HasEnded { get; set; }

    }
}