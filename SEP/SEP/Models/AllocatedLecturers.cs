using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEP.Models
{
    public class AllocatedLecturers
    {
        [Key]
        [Required(ErrorMessage ="Please fill")]
        public int id { get; set;}

        [Display(Name = "Supervisor")]
        public string Supervisors { get; set;}
     
        [Display(Name = "Lecturer")]
        public string Lecturers { get; set;}
        public IEnumerable<SelectListItem> Lecturer { get; set; }

    }
}