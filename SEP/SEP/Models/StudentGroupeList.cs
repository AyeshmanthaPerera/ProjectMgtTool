using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace SEP.Models
{
    public class StudentGroupeList
    {
        [Key]
        public int Id { get; set; }
        public string StuId { get; set; }
        public string Module { get; set; }
        public string GroupNo { get; set; }
        public int status { get; set; }
    }
}