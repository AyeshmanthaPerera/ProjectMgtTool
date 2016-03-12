using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEP.Models
{

    public class Lecturer
    {
        [Key]
        [Required(ErrorMessage = "The Lecture ID Is A Required Field")]
        [StringLength(10, ErrorMessage = "Please Enter a Valid Lecture ID")]
        [RegularExpression(@"^LID+\d{7}$",ErrorMessage ="Please Enter A Valid Lecture ID (ex:- LIDXXXXXXX)")]
        public string LecturerId { get; set; }
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
        [RegularExpression(@"^\d{9}$",ErrorMessage ="Please Enter A Valid Contact No (ex:-07132154000)")]
        public int ContactNo { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Don't include numarical values or symbols in the module")]
        public string Module { get; set; }
        public string Qualification { get; set; }
        [Required(ErrorMessage = "The Avatar Is A Require Field")]
       
        public string Avatar { get; set; }


    }
}