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
{/// <summary>
/// 
/// </summary>
    public class ModulesController : Controller
    {
        private DB2 db = new DB2();

        /// <summary>
        /// Get all modules details in the database
        /// </summary>
        /// <returns>List of all modules details </returns>
        public ActionResult Index()
        {
            return View(db.Modules.ToList());
        }
        /// <summary>
        /// Get modules details in database
        /// </summary>
        /// <param name="searchString">modulename</param>
        /// <returns>all the details of the module according to the paramatere status</returns>
        public ActionResult ViewAll(string searchString)
        {
            var names = from m in db.Modules
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                names = names.Where(s => s.Description.Contains(searchString) || s.ModuleId.Contains(searchString));
            }
            return View(names);
        }

        /// <summary>
        /// get modules details
        /// </summary>
        /// <param name="id">moduleCode</param>
        /// <returns>details of the module according to the param</returns>
        // GET: Modules/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }
        /// <summary>
        /// get values need for creating a new module
        /// </summary>
        /// <returns></returns>
        // GET: Modules/Create
        public ActionResult Create()
        {
            ComboValues(); //call the method where the comboBox values get 
            return View();
        }
        /// <summary>
        /// create new module.set the values to the database
        /// </summary>
        /// <param name="module">colomn values of new module</param>
        /// <returns></returns>
        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ModuleId,Description,MaxCount,Year,semester,LecturerIncharge,MaxLecPanel")] Module module)
        {
            if (db.Modules.Any(m => m.ModuleId.Equals(module.ModuleId))) //check modulecode already in the database table
            {
                TempData["ErrMsg1"] = "Module Code Already Exists!";             
            }
            if (ModelState.IsValid)
            {
                try
                {
                    db.Modules.Add(module);
                    db.SaveChanges();
                    TempData["Msg1"] = "Successfully Inserted";
                    return RedirectToAction("ViewAll");
                }
                catch(Exception ex)
                {                   
                    ComboValues();
                    throw ex;
                }
            }
            else
            {
                ComboValues();
                return View(module);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Modules/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }

            ComboValues();
            return View(module);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModuleId,Description,MaxCount,Year,semester,LecturerIncharge,MaxLecPanel")] Module module)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(module).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Msg1"] = "Successfully Updted!";
                    return RedirectToAction("ViewAll");
                }
                catch(Exception ex)
                {
                    ComboValues();
                    TempData["Msg1"]="Error in Updating!";
                    return View(module);
                    throw ex;
                }
            }
            else
            {
                ComboValues();
                return View(module);
            }
        }


        /// <summary>
        /// Get module details
        /// </summary>
        /// <returns>list of modules that do not have a lecturer incharge</returns>
        [HttpGet]
        public ActionResult ViewLecIC()
        {
            ViewBag.Modules = db.Modules;
            var details = db.Modules.Where(d => d.LecturerIncharge == null).ToList();
            ViewBag.Lecturers = db.Lecturers;
            return View(details);
        }
        /// <summary>
        /// Set Lecturer Incharge for a module
        /// </summary>
        /// <param name="module">LecturerIncharge name</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ViewLecIC([Bind(Include = "LecturerIncharge")] Module module)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(module).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ViewAll");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return View(module);
        }

        /// <summary>
        /// get ist of module details that not assigned a LecturerIncharge
        /// </summary>
        /// <param name="id">ModuleCode</param>
        /// <returns>View of the module details according to the param</returns>
        public ActionResult AssignLecIC(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            ComboValues();
            return View(module);
        }
        /// <summary>
        /// Assign LecturerIncharge for the module
        /// </summary>
        /// <param name="module">moduleCode</param>
        /// <returns></returns>
        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignLecIC([Bind(Include = "ModuleId,Description,MaxCount,Year,semester,LecturerIncharge,MaxLecPanel")] Module module)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(module).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ViewAll");
                }
                catch(Exception ex)
                {
                    ComboValues();
                    return View(module);
                   // throw ex;
                }

            }
            else
            {
                ComboValues();
                return View(module);
            }
        }
        /// <summary>
        /// get the values according to the id
        /// </summary>
        /// <param name="id">module code</param>
        /// <returns>details of the module according to the param</returns>
        // GET: Modules/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }
        /// <summary>
        /// delete the module 
        /// </summary>
        /// <param name="id">moduleCode</param>
        /// <returns>delete the module according to the param</returns>
        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Module module = db.Modules.Find(id);
            db.Modules.Remove(module);
            db.SaveChanges();
            return RedirectToAction("ViewAll");
        }
        /// <summary>
        /// get lecturer details and assign items for the year list 
        /// </summary>
        public void ComboValues()
        {
            ViewBag.Lecturers = db.Lecturers;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "1", Value = "1" });
            items.Add(new SelectListItem { Text = "2", Value = "2" });
            items.Add(new SelectListItem { Text = "3", Value = "3" });
            items.Add(new SelectListItem { Text = "4", Value = "4" });
            ViewBag.Year = items;

        }
        /// <summary>
        /// close the db connection
        /// </summary>
        /// <param name="disposing"></param>
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
