using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEP.Models
{
    public class Student
    {
        [Key]
        [Required(ErrorMessage = "The Lecture ID Is A Required Field")]
       
        [RegularExpression(@"^IT+\d{8}$", ErrorMessage = "Please Enter A Valid Lecture ID (ex:- LIDXXXXXXX)")]
        public string RegistrationNo { get; set; }
        [Required(ErrorMessage = "The Name Is A Required Field")]
        [DataType(DataType.Text, ErrorMessage = "Don't include numarical values or symbols in u're name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Don't include numarical values or symbols in u're name")]
        [StringLength(40, ErrorMessage = "This field required a smaller name than this")]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The Email Is A Require Field")]
        [EmailAddress(ErrorMessage = "Invalid Email Address(ex :=ab@defg.com)")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The Contact No Is A Require Field")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Please Enter A Valid Contact No (ex:-07132154000)")]
        public int ContactNo { get; set; }
        
        [RegularExpression(@"^([0-3]?.[0-9][0-9]$)|^[0-4]?$",ErrorMessage =("Please Enter A Vali GPA"))]
        [Required(ErrorMessage = "The CGPA is A Required Field")]
        public double CGPA { get; set; }
        [Required(ErrorMessage = "The Avatar Is A Require Field")]
        public string Avatar { get; set; }
        [Required(ErrorMessage = "The CV Is A Require Field")]
        public string CV { get; set; }

    }
}