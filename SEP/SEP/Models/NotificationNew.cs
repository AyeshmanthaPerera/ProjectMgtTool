using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEP.Models
{
    public class NotificationNew
    {
        [Key]
        public int NotificationID { get; set; }
        public string Name { get; set; }
        public string Request { get; set; }
        public int Status { get; set; }
        public int Loaded { get; set; }
    }
}