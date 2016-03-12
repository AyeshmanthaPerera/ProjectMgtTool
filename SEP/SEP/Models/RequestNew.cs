using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEP.Models
{
    public class RequestNew
    {
        [Key]
        public int RequsetId { get; set; }
        public string Name { get; set; }
        public string Request { get; set; }
        public int Status { get; set; }
        public int Loaded { get; set; }
    }

}