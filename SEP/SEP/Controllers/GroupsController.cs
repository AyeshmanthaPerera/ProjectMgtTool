using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SEP.Models;
using System.Diagnostics;

namespace SEP.Controllers
{
    public class GroupsController : Controller
    {
        private DB2 db = new DB2(); 
        /// <summary>
        /// At the begining test weather the user is Lecture In charge or not
        /// </summmary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string position =(string) Session["UserName"];
            
           bool checkLecture = db.Modules.Any(ac => ac.LecturerIncharge.Equals(position));
            if (checkLecture) {
                Session["LecIN"] = true;
            }
            else
            {
                Session["LecIN"] = false;
            }
            return View(db.Groups);
        }
        /// <summary>
        /// Load the all searched items from Group table where searched from Search box
        /// </summary>
        /// <param name="Search">Selected value from search box</param>
        /// <returns>List of all project ids and Supervisors that Searched from Search box</returns>
        // GET: Groups
        [HttpPost]
        public ActionResult Index(string Search)
        {
           return View(db.Groups.Where(x => x.ProjectID.StartsWith(Search) || x.ProjectID==Search).ToList());   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Groups/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }
        /// <summary>
        /// Dynamically Share the Module IDs and superviosor names with controller
        /// </summary>
        /// <returns>Module IDs and Supervisor names</returns>
        // GET: Groups/Create
        public ActionResult Create()
        {
            ViewBag.ProjectID = new SelectList(db.Modules, "ModuleId", "ModuleId");
            ViewBag.Supervisor = new SelectList(db.Lecturers, "Name", "Name");
            
            return View();
        }

        /// <summary>
        /// Validate the Group ID and Create a Supervisor for relevant Project ID
        /// </summary>
        /// <param name="group">Group ID,Project ID,Supervisor</param>
        /// <returns>Saving Above Items</returns>
        
        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,ProjectID,Supervisor")] Group group)
        {
            if (String.IsNullOrWhiteSpace(group.GroupID))
            {
                TempData["null"] = "null";
               
                return RedirectToAction("Index", "Groups1");
            }

            else if (group.GroupID.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                TempData["letnum)"] = "letnum";
                return RedirectToAction("Index", "Groups1");
            }
            else if (group.GroupID.Length > 6 | group.GroupID.Length<3) {
                TempData["len"] = "between";
                return RedirectToAction("Index", "Groups1");
            }
           
            else if (db.Groups.Any(ac => ac.GroupID.Equals(group.GroupID)))
            {
                TempData["exist)"] = "exist";
                return RedirectToAction("Index", "Groups1");

            }
            else if (db.Groups.Any(acd => acd.ProjectID.Equals(group.ProjectID)))
            {
                TempData["NoMore)"] = "NoMore";
                return RedirectToAction("Index", "Groups1");

            }
            else if (ModelState.IsValid)
                {
                    db.Groups.Add(group);
                    db.SaveChanges();
                    ModelState.Clear();
                    return RedirectToAction("index");
             }
             return View(group);
        }


        /// <summary>
        /// Dyanamically Share the Project ID and Supervisor from the Group table with the controller
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <returns>Update the Group</returns>
        // GET: Groups/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Groups, "GroupId", "ProjectID", group.ProjectID);
            ViewBag.Supervisor = new SelectList(db.Groups, "GroupId", "Supervisor", group.Supervisor);

            return View(group);
        }
        
        /// <summary>
        /// Update the project id and Supervisor for group id to the Group Table
        /// </summary>
        /// <param name="group">Group Id get from selected row</param>
        /// <returns>Update the Selected row</returns>
        
        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,ProjectID,Supervisor")] Group group)
        {
            if(db.Groups.Any(ac=>ac.ProjectID.Equals(group.ProjectID)) && db.Groups.Any(acd => acd.Supervisor.Equals(group.Supervisor))){
                TempData["exist)"] = "exist";
                return RedirectToAction("Index", "Groups1");
            }
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Selected Group ID</param>
        /// <returns></returns>
        // GET: Groups/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        /// <summary>
        /// Delete the Group Id
        /// </summary>
        /// <param name="id">Selected Group ID</param>
        /// <returns></returns>

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Group group = db.Groups.Find(id);
          
            db.Groups.Remove(group);
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


        public JsonResult CheckForDuplication(String names)
        {
            var data = db.Groups.Where(p => p.GroupID.Equals(names, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (data != null)
            {
                return Json("Sorry, this name already exists", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet); 
            }
        }
    }
}




