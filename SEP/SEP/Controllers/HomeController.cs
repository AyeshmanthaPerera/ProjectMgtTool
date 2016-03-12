using SEP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace SEP.Controllers
{
    public class HomeController : Controller
    {
        [AuthorizeUserAcessLevel(Roles = "Lecturer,student,HOD", UserRole = "student,HOD,Lecturer")]
        public ActionResult Index()
        {
           
            return View();


        }
        //[AuthorizeUserAcessLevel(UserRole ="Manager")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}