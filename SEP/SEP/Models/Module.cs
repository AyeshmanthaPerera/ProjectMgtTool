using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEP.Models
{
    public class Module
    {
        [Key]

        [Required(ErrorMessage = "* Module Code is required")]
        [Display(Name = "Module Code")]
        public string ModuleId { get; set; }
        [Display(Name = "Module Name")]
        [Required(ErrorMessage = "Enter Module Name")]
        public string Description { get; set; }
        [Display(Name = "Students Per Group")]
        [Range(1, 30, ErrorMessage = "Check the Number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Enter a valid number")]
        public int MaxCount { get; set; }
        [Required(ErrorMessage = "Select a Year")]
        public int Year { get; set; }
        [Display(Name = "Semester")]
        [Range(1, 2, ErrorMessage = "Select a Semester")]
        [Required(ErrorMessage = "Select a Semester")]
        public int semester { get; set; }
        [Display(Name = "Lecturer Incharge")]
        public String LecturerIncharge { get; set; }
        [Display(Name = "Lectures per Panel")]
        [Range(0, 30, ErrorMessage = "Check the Number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Check the number")]
        public int MaxLecPanel { get; set; }
    }
}