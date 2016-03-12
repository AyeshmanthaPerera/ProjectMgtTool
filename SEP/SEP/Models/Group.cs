using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEP.Models
{
    public class Group
    {

        
        [Key]
        [StringLength(14, MinimumLength = 3, ErrorMessage = "Invalid")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed.")]
        [Required(ErrorMessage ="Please fill the group id")]
        public string GroupID { get; set; }

        public string ProjectID { get; set; }
       
        public string Supervisor { get; set; }


       

    }
}