using SEP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEP.Controllers
{
    public class LecModController : Controller
    {
        private DB2 db = new DB2();
        // GET: LecMod
        public ActionResult Index()
        {
         
            var q = (from m in db.Modules
                     join l in db.Lecturers on m.ModuleId equals l.Module
                     //  join ls in db.Lec_Statuses on l.LecturerId equals ls.LectureId
                     where m.LecturerIncharge != null
                     select new ModLec { Lecturer = l, Module = m });

            return View(q);
        }

        public ActionResult ModulesLec()
        {

            var q = (from m in db.Modules
                     join l in db.Lecturers on m.ModuleId equals l.Module
                     //  join ls in db.Lec_Statuses on l.LecturerId equals ls.LectureId
                     where m.LecturerIncharge != null
                     select new ModLec { Lecturer = l, Module = m });

            return View("Index", q);
        }


        // GET: LecMod/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LecMod/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LecMod/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LecMod/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LecMod/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LecMod/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LecMod/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
