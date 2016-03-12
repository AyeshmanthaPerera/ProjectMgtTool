using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEP.Models;
using System.IO;
using System.Data.SqlClient;
using System.Diagnostics;
using PagedList;

namespace SEP.Controllers
{
    public class LecturersController : Controller
    {
        ValidateController Validater = new ValidateController();
        private DB2 db = new DB2();
        /// <summary>
        /// According to the search term provide by user
        /// get the list of lecturers to a paged
        /// list with 5 elements per each page
        /// </summary>
        /// <param name="searchterm"></param>
        /// <param name="page"></param>
        /// <returns>"IEnumarable List Of Lecturers"</returns>
        [AuthorizeUserAcessLevel(UserRole = "Lecturer,HOD")]
        public ActionResult Index(string searchterm = null, int page = MvcApplication.Pr_one)
        {
            //Gett the fields according to the given search term
            var model = (from r in db.Lecturers
                         orderby r.Name ascending
                         where (r.Name.Contains(searchterm) || string.IsNullOrEmpty(searchterm))
                         select r).ToPagedList(page, MvcApplication.Pr_five);
            return View(model);

        }

        /// <summary>
        /// "According to the given id 
        /// provide the details of the
        /// Lecturer"
        /// </summary>
        /// <param name="id"></param>
        /// <returns>"Returns the Object from Lecturer Entity type"</returns>
        // GET: Lecturers/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecturer lecturer = db.Lecturers.Find(id);
            if (string.IsNullOrEmpty(lecturer.Name))
            {
                return HttpNotFound();
            }
            return View(lecturer);
        }
        /// <summary>
        /// Returns the create view
        /// works on the GET requests only
        /// </summary>
        /// <returns></returns>
        // GET: Lecturers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Lecturers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// If the given details are valid
        /// create a new Lecturer works only 
        /// on POST requests
        /// </summary>
        /// <param name="lecturer"></param>
        /// <param name="Avatar"></param>
        /// <param name="CV"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LecturerId,Name,Email,ContactNo,Module,Qualification,Avatar")] Lecturer lecturer, HttpPostedFileBase Avatar)
        {
            var extension = Path.GetExtension(Avatar.FileName);
            if (ModelState.IsValid && Avatar != null && (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".GIF")))
            {
                string path = Server.MapPath("~/Content/Images2");
                //calling the image upload method in ValidateController
                lecturer.Avatar = Validater.ImageUpload(Avatar, path);
                lecturer.Qualification = "Lecturer";
                lecturer.Module = "Pending";
                //calling the RegisterSessionSet to set the sessions 
                RegisterSessionSet(lecturer.Name, lecturer.Email, lecturer.Avatar, 0.0, lecturer.ContactNo, "Lecturer", lecturer.LecturerId);


                //Save Changes in DB
                db.Lecturers.Add(lecturer);

                RequestNew LecReq = new RequestNew { Name = "Auro", Request = lecturer.Name + " Lecturer Been Added To the SEP Tool As a lectureCUT" + lecturer.Avatar + "CUT" + MvcApplication.time, Status = MvcApplication.Pr_two, Loaded = MvcApplication.Pr_two };
                db.RequestsNew.Add(LecReq);
                db.SaveChanges();


                return RedirectToActionPermanent("Pending", "Register");
            }
            else if (!(extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".GIF")))
            {

                TempData["error"] = "Please Use A Valid Avatar";

            }

            return View(lecturer);

        }

        public void RegisterSessionSet(string ename, string Email, string Avatar, double CGPA, int ContactNo, string Position, string ResgistrationNo)
        {

            Session["UserName"] = ename;
            Session["Email"] = Email;
            Session["Avatar"] = Avatar;
            Session["CGPA"] = CGPA;
            Session["ContactNo"] = ContactNo;
            Session["id"] = ResgistrationNo;
            Session["Position"] = Position;

        }

        // GET: Lecturers/Edit/5
        /// <summary>
        /// Find if theres any Lecturers for the given id 
        /// if exists returning the Edit view
        /// works on GET methods
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Lecturer type object</returns>
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                //Finding the Lecturer for the given Id
                Lecturer lecturer = db.Lecturers.Find(id);
                if (lecturer == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(lecturer);

                }
            }
        }

        // POST: Lecturers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// If the given details are valid 
        /// change the attributes of the selected Lecturer object
        /// </summary>
        /// <param name="lecturer"></param>
        /// <param name="Avatar"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LecturerId,Name,Email,ContactNo,Module,Qualification,Avatar")] Lecturer lecturer, HttpPostedFileBase Avatar)
        {
            try
            {
                lecturer.Qualification = "HOD";
                var extension = Path.GetExtension(Avatar.FileName);

                //To check the Model is Valid and the Image is Valid
                if (ModelState.IsValid && (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".GIF")))
                {
                    string path = Server.MapPath("~/Content/Images2");

                    var fileName = Path.GetFileName(Avatar.FileName);
                    //Inserting the Image File
                    lecturer.Avatar = Validater.ImageUpload(Avatar, path);
                    //If the Changes are valid saving them to the DB
                    db.Entry(lecturer).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else if ((!(extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".GIF"))))
                {
                    Debug.Write("Invalid");
                    TempData["error"] = "Please Use A Valid Avatar";
                    return View(lecturer);

                }
            }
            catch (System.NullReferenceException)
            {
                TempData["error"] = "Please Use A Valid Avatar";

            }

            if (!ModelState.IsValid) {
                Debug.Write("Invalid mate");
                return View(lecturer);
            }
            return ViewBag;
        }



        /// <summary>
        /// Find if theres any Lecturers for the given id 
        /// if exists returning the Delete view
        /// works on GET methods
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Lecturer type object</returns>
        // GET: Lecturers/Delete/5
        [AuthorizeUserAcessLevel(UserRole = "HOD")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                //Find the Lecture from the DB
                Lecturer lecturer = db.Lecturers.Find(id);
                if (string.IsNullOrEmpty(lecturer.Name))
                {
                    return HttpNotFound();
                }
                else
                {
                    TempData["Message2"] = "Lec";

                    return View(lecturer);
                }
            }
        }

        /// <summary>
        /// If the user wants to delete the Lecturer object 
        /// this will do it 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Lecturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUserAcessLevel(UserRole = "HOD")]
        public ActionResult DeleteConfirmed(string id)
        {
            //Find Any lectures for the given ID
            Lecturer lecturer = db.Lecturers.Find(id);

            //Remove Lecturer from the DB
            db.Lecturers.Remove(lecturer);
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
