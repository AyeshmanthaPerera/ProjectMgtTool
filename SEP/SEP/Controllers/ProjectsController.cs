using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEP.Models;

namespace SEP.Controllers
{
    public class ProjectsController : Controller
    {
        private DB2 db = new DB2();

        // GET: Projects
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public ActionResult Index(string searchString)
        {
            var names = from m in db.Projects
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                names = names.Where(s => s.Name.Contains(searchString));
            }
            return View(names);


         //   return View(db.Projects.ToList());
        }

            // GET: Projects/Details/5
            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        public ActionResult CreateExternal() {

            ViewBag.ModuleName = db.Modules;
            TempData["ProjectCreate"] = null;
            return View();
        }

        // GET: Projects/Create
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.ModuleName = db.Modules;
            if ((string)Session["Position"] == "Lecturer") {
                ViewBag.Client = (string)Session["UserName"];
            }
            TempData["ProjectCreate"] = null;

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,ModuleId,Name,Description,Client,PreferedTechnologies")] Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   
                    ViewBag.ModuleName = db.Modules;
                    db.Projects.Add(project);
                    db.SaveChanges();
                   
                    if (Session["UserName"] == null)
                    {
                        return RedirectToAction("Login", "Register");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch
                {
                    
                    return View(project);
                }          
            } else {
                if ((string)Session["Position"] == "Lecturer")
                {
                    ViewBag.Client = (string)Session["UserName"];
                }
                ViewBag.ModuleName = db.Modules;
                return View(project);
                }
        }

        // GET: Projects/Edit/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.ModuleName = db.Modules;

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectId,ModuleId,Name,Description,Client,PreferedTechnologies")] Project project)
        {
            if (ModelState.IsValid)
            {
             try
                {
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.ModuleName = db.Modules;
                    return View("error",new HandleErrorInfo(e,"Projects","Edit"));
                    throw e;
                }                       
            }
            else
            {
                return View(project);
            }

        }

        // GET: Projects/Delete/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
