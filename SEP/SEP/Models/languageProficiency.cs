using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SEP.Models
{
    public class LanguageProficiency
    {
        [Key]
        public string RegistrationNum { get; set; }
        public int Java { get; set; }
        public int Csharp { get; set; }
        public int CPlusPlus { get; set; }

        public int php { get; set; }
        public int VB { get; set; }
        public int android { get; set; }
        public int ios { get; set; }
    }
}