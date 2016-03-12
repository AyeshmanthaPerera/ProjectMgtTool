using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEP.Models;
using System.Diagnostics;

namespace SEP.Controllers
{
    public class LecturePanelController : Controller
    {
        private DB2 db = new DB2();

        /// <summary>
        /// Check weather the user is Lecture In Charge or not by using the Sessions
        /// Share the Group module with the controller.
        /// </summary>
        /// <returns>Group Module</returns>
        
            // GET: LecturePanel
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

            return View(db.Groups);
        }
        /// <summary>
        /// Display the alllocated Lecturers for each Lecture Panel
        /// Query : Check weather the superviosr in Allocated Lecturer table is equal to the supervisors in Groups table.
        /// Create the list object as mymodel and add selected lists to the mymodel list
        /// </summary>
        /// <param name="id">Selected Panel ID</param>
        /// <returns>mymodel list</returns>
        /// 
        // GET: LecturePanel/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            IList<object> mymodel = new List<object>();
            try
            {

                mymodel.Add(db.AllocatedLecturers.Where(acd => acd.Supervisors.Equals(group.Supervisor)).ToList());
                mymodel.Add(db.Groups.Where(ac => ac.GroupID.Equals(id)).ToList());

            }
            catch (System.Reflection.TargetException t)
            {
                Debug.Write(t);
            }

            if (group == null)
            {
                return HttpNotFound();
            }
            return View(mymodel);
        }

       /// <summary>
       /// Share the group module to relevant id with controller
       /// </summary>
       /// <param name="id">Selected Group ID</param>
       /// <returns>group module for selected row(group ID)</returns>


        // GET: LecturePanel/Delete/5
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
        /// Delete the Lecture Panel
        /// </summary>
        /// <param name="id">Selected Group ID</param>
        /// <returns>Deleting the Lectrue Panel</returns>
        // POST: LecturePanel/Delete/5
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
    }
}
