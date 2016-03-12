using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SEP.Models;
using System.Data.SqlClient;
using System.Diagnostics;
using Rotativa;
using PagedList;
using PagedList.Mvc;

namespace SEP.Controllers
{
    public class Students1Controller : Controller
    {
        private DB2 db = new DB2();
        /// <summary>
        /// According to the search term provide by user
        /// get the list of students to a paged
        /// list with 5 elements per each page
        /// </summary>
        /// <param name="searchterm"></param>
        /// <param name="page"></param>
        /// <returns>"IEnumarable List Of Students"</returns>
        [AuthorizeUserAcessLevel(UserRole ="HOD,Lecturer")]
        public ActionResult Index(string searchterm = null, int page = MvcApplication.Pr_one)
        {
            //Get the student modules according to the given search term
            var model = (from r in db.Students
                         orderby r.Name ascending
                         where (r.Name.StartsWith(searchterm) || string.IsNullOrEmpty(searchterm))
                         select r).ToPagedList(page, MvcApplication.Pr_five);
            return View(model);
        }

        /// <summary>
        /// "According to the given id 
        /// provide the details of the
        /// student"
        /// </summary>
        /// <param name="id"></param>
        /// <returns>"Returns the Object from Student Entity type"</returns>
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Find the students relevent to the given id
            Student student = db.Students.Find(id);
            if (string.IsNullOrEmpty(student.Name))
            {
                return HttpNotFound();
            }
            return View(student);
        }

        /// <summary>
        /// Returns the create view
        /// works on the GET requests only
        /// </summary>
        /// <returns></returns>
        // GET: Students1/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// If the given details are valid
        /// create a new student works only 
        /// on POST requests
        /// </summary>
        /// <param name="student"></param>
        /// <param name="Avatar"></param>
        /// <param name="CV"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create([Bind(Include = "RegistrationNo,Name,Email,ContactNo,CGPA,Avatar,CV")] Student student, HttpPostedFileBase Avatar, HttpPostedFileBase CV)
        {

            Session["UserName"] = student.Name;
            Session["Email"] = student.Email;
            Session["CGPA"] = student.CGPA;
            Session["CV"] = student.CV;
            Session["ContactNo"] = student.ContactNo;
            Session["id"] = student.RegistrationNo;
            Session["Position"] = "student";
            var extention1 = Path.GetExtension(Avatar.FileName);
            var extention2 = Path.GetExtension(CV.FileName);

            //Check weather the model is valid and image and also the pdf format is valid
            if (ModelState.IsValid && (extention2.Equals(".PDF") || extention2.Equals(".pdf")) && (extention1.Equals(".jpg") || extention1.Equals(".jpeg") || extention1.Equals(".GIF")))
            {

                var fileName1 = Path.GetFileName(CV.FileName);
                var path1 = Path.Combine(Server.MapPath("~/Content/Images2"), fileName1);
                string[] path21 = path1.Split(new string[] { "SEP\\SEP" }, StringSplitOptions.None);
                CV.SaveAs(path1);
                student.CV = path21[MvcApplication.Pr_one] + "";


                var fileName = Path.GetFileName(Avatar.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Images2"), fileName);
                string[] path2 = path.Split(new string[] { "SEP\\SEP" }, StringSplitOptions.None);
                Avatar.SaveAs(path);
                student.Avatar = path2[MvcApplication.Pr_one] + "";


                Session["Avatar"] = student.Avatar;

                //if the model is valid adding the student to the DB
                db.Students.Add(student);
                db.SaveChanges();



                string query2 = "insert into  dbo.Notification(Name,Notification,Status,Loaded) values(@a3,@a4,@a5,@a6)";
                List<object> parameterList2 = new List<object>();
                parameterList2.Add(new SqlParameter("@a3", "Auro"));
                parameterList2.Add(new SqlParameter("@a4", student.Name + "Student Been Added To the SEP Tool CUT" + student.Avatar + "CUT" + DateTime.Now.ToString("HH:mm:ss tt")));
                parameterList2.Add(new SqlParameter("@a5", 2));
                parameterList2.Add(new SqlParameter("@a6", 2));
                object[] parameters12 = parameterList2.ToArray();
                int rs1 = db.Database.ExecuteSqlCommand(query2, parameters12);



                return RedirectToActionPermanent("Index", "Home");
            }
            else if (!(extention2.Equals(".PDF") || extention2.Equals(".pdf")))
            {
                TempData["error"] = "Please Use A Valid PDF";

            }
            else if (!(extention1.Equals(".jpg") || extention1.Equals(".jpeg") || extention1.Equals(".GIF")))
            {
                TempData["Ava"] = "Please Use A Valid Avatar";
            }

            return View(student);
        }

        /// <summary>
        /// Find if theres any student for the given id 
        /// if exists returning the Edit view
        /// works on GET methods
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Student type object</returns>
       
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Student student = db.Students.Find(id);
                if (string.IsNullOrEmpty(student.Name))
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(student);
                }
            }
        }
        /// <summary>
        /// If the given details are valid 
        /// change the attributes of the selected student object
        /// </summary>
        /// <param name="student"></param>
        /// <param name="Avatar"></param>
        /// <param name="CV"></param>
        /// <returns></returns>
        // POST: Students1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RegistrationNo,Name,Email,ContactNo,CGPA,Avatar,CV")] Student student, HttpPostedFileBase Avatar, HttpPostedFileBase CV)
        {
            try
            {
                Session["UserName"] = student.Name;
                Session["Email"] = student.Email;
                Session["CGPA"] = student.CGPA;
                Session["CV"] = student.CV;

                var extention1 = Path.GetExtension(Avatar.FileName);
                var extention2 = Path.GetExtension(CV.FileName);
                if (ModelState.IsValid && (extention2.Equals(".PDF") || extention2.Equals(".pdf")) && (extention1.Equals(".jpg") || extention1.Equals(".jpeg") || extention1.Equals(".GIF")))
                {
                    var fileName1 = Path.GetFileName(CV.FileName);
                    var path22 = Path.Combine(Server.MapPath("~/Content/Images2"), fileName1);
                    string[] path21 = path22.Split(new string[] { "SEP\\SEP" }, StringSplitOptions.None);
                    CV.SaveAs(path22);
                    student.CV = path21[MvcApplication.Pr_one] + "";

                    var fileName = Path.GetFileName(Avatar.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images2"), fileName);
                    string[] path2 = path.Split(new string[] { "SEP\\SEP" }, StringSplitOptions.None);
                    Avatar.SaveAs(path);
                    student.Avatar = path2[MvcApplication.Pr_one] + "";

                    //Save the updated student model
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else if (!(extention2.Equals(".PDF") || extention2.Equals(".pdf")))
                {
                    TempData["error"] = "Please Use A Valid PDF";

                }
                else if (!(extention1.Equals(".jpg") || extention1.Equals(".jpeg") || extention1.Equals(".GIF")))
                {
                    TempData["Ava"] = "Please Use A Valid Avatar";
                }


            }
            catch (System.Data.Entity.Core.UpdateException)
            {
                TempData["Error"] = "Error Ocured while Updating the Database Context";
                throw;
            }
            return View(student);
        }

        /// <summary>
        /// Find if theres any student for the given id 
        /// if exists returning the Delete view
        /// works on GET methods
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Student type object</returns>
        // GET: Students1/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Find students fro the given id
            else
            {
                Student student = db.Students.Find(id);
                if (string.IsNullOrEmpty(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(student);
                }
            }
        }

        /// <summary>
        /// If the userr wants to delete the student object 
        /// this will do it 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Students1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// After performing the action relate to database 
        /// to turn of the db connection
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            else
            {
                base.Dispose(disposing);
            }
        }
    }
}
