using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Project.Areas.Setup.Models
{
    public class TestimonialViewModel
    {
        public string PicturePath
        {
            get;
            set;
        }

        public IList<Testimonials> Rows
        {
            get;
            set;
        }

        public TestimonialForm Testimonialform
        {
            get;
            set;
        }

        public Testimonials Testimonials
        {
            get;
            set;
        }

       

    }
    public class TestimonialForm
    {
        [Display(Name = "Content")]
        [Required(ErrorMessage = "Please enter the Content")]
        public string Content
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        [Display(Name = "Is Deleted")]
        public bool IsDeleted
        {
            get;
            set;
        }

        [Display(Name = "Is Published")]
        public bool IsPublished
        {
            get;
            set;
        }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter the Name")]
        public string Name
        {
            get;
            set;
        }

        [Display(Name = "Position")]

        public string Position
        {
            get;
            set;
        }


    }
}