using SEP.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SEP.Controllers
{
    public class RegisterController : Controller
    {
        private DB2 db = new DB2();


        [HttpGet]
        public ActionResult Login()
        {
            TempData["Message1"] = null;
            return View();
        }


        [HttpPost]
        public ActionResult Login(string id, string Email)
        {
            TempData["Message1"] = null;


            //Find the Any student there in the database according to the given credentials
            Student student2 = db.Students.FirstOrDefault(m => m.Email.Equals(Email) && m.RegistrationNo.Equals(id));
            //Find the Any Lecture there in the database according to the given credentials
            Lecturer lecture2 = db.Lecturers.FirstOrDefault(m => m.Email.Equals(Email) && m.LecturerId.Equals(id));

            if (student2 != null && lecture2 == null)
            {
                SessionSet(student2.Name, student2.Email, student2.Avatar, student2.CGPA, student2.ContactNo, "student", student2.RegistrationNo, true);
                return RedirectToActionPermanent("Index", "Home");
            }
            else if (student2 == null && lecture2 != null)
            {
                SessionSet(lecture2.Name, lecture2.Email, lecture2.Avatar, 0.0, lecture2.ContactNo, "Lecturer", lecture2.LecturerId, true);
                if (lecture2.Module.Equals("Pending"))
                {
                    return RedirectToAction("Pending", "Register");
                }
                else
                {
                    return RedirectToActionPermanent("Index", "Home");
                }
            }
            TempData["Message1"] = " Error Login in Please Check The Credenitials";



            return View();
        }
        public void SessionSet(string ename, string Email, string Avatar, double CGPA, int ContactNo, string Position, string ResgistrationNo, bool remember1)
        {

            Session["UserName"] = ename;
            Session["Email"] = Email;
            Session["Avatar"] = Avatar;
            Session["CGPA"] = CGPA;
            Session["ContactNo"] = ContactNo;
            Session["id"] = ResgistrationNo;
            if (ename == "Auro" && remember1)
            {
                Session["Position"] = "HOD";
                FormsAuthentication.SetAuthCookie(ename, remember1);
                FormsAuthentication.RedirectFromLoginPage(ename, remember1);
            }
            else if (remember1)
            {
                Session["Position"] = Position;
                FormsAuthentication.SetAuthCookie(ename, remember1);
                FormsAuthentication.RedirectFromLoginPage(ename, remember1);
            }

        }
        [HttpGet]
        public ActionResult Forgot1() {


            return View();
        }

        [HttpPost]
        public ActionResult Forgot(string Email)
        {
            Lecturer getID = db.Lecturers.FirstOrDefault(m => m.Email.Equals(Email));
            Student getID2 = db.Students.FirstOrDefault(m => m.Email.Equals(Email));
            string id;
            if (getID.Email == null)
            {
                id = getID2.RegistrationNo;
            }
            else
            {
                id = getID.LecturerId;
            }
            return RedirectToAction("Sendin", "Mail", new { Email1 = Email, Id1 = id });
        }
        

        public ActionResult Pending()
        {
            TempData["Pending"] = "U have to waot until the Head of the department accepts u're request";

            return View();
        }

        [HttpPost]
        public ActionResult Extrnal(string Email)
        {
            Client cl1 = db.Clients.FirstOrDefault(m => m.Email.Equals(Email));

            if (cl1 != null)
            {

                return RedirectToAction("Sendin", "Mail", new { Email1 = Email, Id1 = -1 + "", client1 = cl1.Name });

            }
            else
            {
                return RedirectToAction("Sendin", "Mail", new { Email1 = Email, Id1 = 0 + "" });
            }
        }
        public ActionResult LogOut()
        {
            if (Session["UserName"] != null)
            {

                Session.Clear();
                Session.Abandon();

                return RedirectToActionPermanent("Login", "Register");
            }

            return View();
        }

    }

}
