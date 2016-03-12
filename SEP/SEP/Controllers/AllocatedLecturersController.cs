using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SEP.Models;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SEP.Controllers
{
    public class AllocatedLecturersController : Controller
    {
        private DB2 db = new DB2();

        /// <summary>
        /// Check weather the user is Lecture In Charge or not.
        /// </summary>
        /// <returns>Allocated moudule</returns>
        // GET: AllocatedLecturers
        public ActionResult Index()
        {
            string position = (string)Session["UserName"];

            bool p = db.Modules.Any(ac => ac.LecturerIncharge.Equals(position));
            if (p)
            {
                Session["LecIN"] = true;   
            }
            else
            {
                Session["LecIN"] = false;
            }
            return View(db.AllocatedLecturers);
        }
        /// <summary>
        /// Search the superviosrs where Superviosr name start using Search box or load all data items in Group table whenever the 
        /// Search field is empty.
        /// </summary>
        /// <param name="Search">Text item that Search using search box</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string Search)
        {
                return View (db.AllocatedLecturers.Where(x => x.Supervisors.StartsWith(Search)|| Search==null).ToList());
        }

        /// <summary>
        /// Display the details of the Allocated Supervisors for each Groups
        /// </summary>
        /// <param name="id">Selected Group ID</param>
        /// <returns>Allocated Lectrue Module</returns>

        // GET: AllocatedLecturers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllocatedLecturers allocatedLecturers = db.AllocatedLecturers.Find(id);
            if (allocatedLecturers == null)
            {
                return HttpNotFound();
            }
            return View(allocatedLecturers);
        }

        internal bool UserExists(string username)
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// Share the Supervisors in groups table and lecturers in Lecture table after adding to list.
        /// </summary>
        /// <returns></returns>

        // GET: AllocatedLecturers/Create
        public ActionResult Create()
        {

            ViewBag.Supervisors = new SelectList(db.Groups,"Supervisor", "Supervisor");
            ViewBag.Lecturers = new SelectList(db.Lecturers, "Name", "Name");

            return View();
        }

        /// <summary>
        /// Check weather the Same Lecture allocate for same supervisor 
        /// And check the maximum number of lecturers to add for selected Supervisor.Then Send those details to notification table
        /// to load notifcations for lecturers.
        /// </summary>
        /// <param name="allocatedLecturers">Superviosr and Lecture fields</param>
        /// <returns>Allocating Lecture for a Superviosr</returns>

        // POST: AllocatedLecturers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Supervisors,Lecturers")] AllocatedLecturers allocatedLecturers)
        {
            var vari = allocatedLecturers.Supervisors;
            
            int table_rows = db.AllocatedLecturers.Count(ac => ac.Supervisors.Equals(allocatedLecturers.Supervisors));
            int MaxAllocatedLecturers = db.Modules.Max(ac => ac.MaxCount);

            if (MaxAllocatedLecturers <= table_rows)
            {
                TempData["max"] = "max";
                return RedirectToAction("fuc", "Groups1");
            }

            if (db.AllocatedLecturers.Any(ac => ac.Lecturers.Equals(allocatedLecturers.Lecturers)) &&
                db.AllocatedLecturers.Any(acs => acs.Supervisors.Equals(allocatedLecturers.Supervisors)))

            {
                TempData["exists"] = "exists";
                return RedirectToAction("Edit", "Groups1");

            }
            else if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString("HH:mm:ss tt");
                string query = "insert into  dbo.Requests(Name,Request,Status,Loaded) values(@a1,@a2,@a3,@a4)";
                List<object> parameterList = new List<object>();
                parameterList.Add(new SqlParameter("@a1", allocatedLecturers.Lecturers));
                parameterList.Add(new SqlParameter("@a2", Session["UserName"] + "Requests for a allocating on a Lecture panelCUT" + Session["Avatar"] + "CUT" + time + "CUT" + allocatedLecturers.id+"CUTAllocateLecturers"));
                parameterList.Add(new SqlParameter("@a3", 2));
                parameterList.Add(new SqlParameter("@a4", 2));
                object[] parameters123 = parameterList.ToArray();
                int rs = db.Database.ExecuteSqlCommand(query, parameters123);
                db.AllocatedLecturers.Add(allocatedLecturers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else {
                return View(allocatedLecturers);
            }
        }

        /// <summary>
        /// Share the allocatedLecturers moudle that find by selected id
        /// </summary>
        /// <param name="id">Selected id</param>
        /// <returns>Saving Supervisors for allocated lecturers</returns>

        // GET: AllocatedLecturers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllocatedLecturers allocatedLecturers = db.AllocatedLecturers.Find(id);
            if (allocatedLecturers == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supervisors = new SelectList(db.Groups, "Supervisor", "Supervisor");
            ViewBag.Lecturers = new SelectList(db.Lecturers, "Name", "Name");

           
            return View(allocatedLecturers);
        }

        /// <summary>
        /// Update the Lecturers and Superviosrs in allocatedLecturers table
        /// </summary>
        /// <param name="allocatedLecturers"></param>
        /// <returns></returns>

        // POST: AllocatedLecturers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Supervisors,Lecturers")] AllocatedLecturers allocatedLecturers)
        {
            int table_rows = db.AllocatedLecturers.Count(ac => ac.Supervisors.Equals(allocatedLecturers.Supervisors));
            int t = db.Modules.Max(ac => ac.MaxCount);


            if (db.AllocatedLecturers.Any(ac => ac.Supervisors.Equals(allocatedLecturers.Supervisors))
                && db.AllocatedLecturers.Any(acd=>acd.Lecturers.Equals(allocatedLecturers.Lecturers))) {

                TempData["exists"] = "exists";
                return RedirectToAction("Edit", "Groups1");
            }
           
            if (t <= table_rows)
            {
                TempData["max"] = "max";
                return RedirectToAction("fuc", "Groups1");
            }

            if (ModelState.IsValid)
            {
                db.Entry(allocatedLecturers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(allocatedLecturers);
        }

        /// <summary>
        /// Share the allocatedLecuters moudle where the Selected id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>allocatedLecturers module</returns>

        // GET: AllocatedLecturers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllocatedLecturers allocatedLecturers = db.AllocatedLecturers.Find(id);

           
            if (allocatedLecturers == null)
            {
                return HttpNotFound();
            }
            return View(allocatedLecturers);
        }
        /// <summary>
        /// Delete the Allocated Lecturer from Lecture panel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: AllocatedLecturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AllocatedLecturers allocatedLecturers = db.AllocatedLecturers.Find(id);
            
            db.AllocatedLecturers.Remove(allocatedLecturers);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult GetStudents(string term)
        {
            DB2 db = new DB2();
            List <string> students;

            students = db.AllocatedLecturers.Where(x => x.Supervisors.StartsWith(term)).Select(y => y.Supervisors).ToList();
            return Json(students, JsonRequestBehavior.AllowGet);
        }


    }
}
