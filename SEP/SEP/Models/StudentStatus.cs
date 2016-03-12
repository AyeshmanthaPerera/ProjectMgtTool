using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEP.Models
{
    public class StudentStatus
    {
        [Key]
        public string ModuleId { get; set; }
        public string RegistratioNo { get; set; }
        public string Status { get; set; }
    }
}