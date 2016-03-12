using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEP.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        [Required(ErrorMessage = "The Company Name Is A Required Field")]
        [StringLength(40, ErrorMessage = "This field required a smaller name than this")]
        public string Company { get; set; }
        [Required(ErrorMessage = "The  Name Is A Required Field")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Don't include numarical values or symbols in u're name")]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The Email Is A Require Field")]
        [EmailAddress(ErrorMessage = "Invalid Email Address(ex :=ab@defg.com)")]
        public string Email { get; set; }
        [Display(Name = "Contact No.")]
        [Required(ErrorMessage = "The Contact No Is A Require Field")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Please Enter A Valid Contact No (ex:-07132154000)")]
        public int ContactNo { get; set; }

    }
}