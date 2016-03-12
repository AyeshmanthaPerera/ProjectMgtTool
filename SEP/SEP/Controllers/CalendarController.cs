using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHTMLX.Scheduler;
using DHTMLX.Common;
using DHTMLX.Scheduler.Data;
using DHTMLX.Scheduler.Controls;
using System.Data.Entity;

using System.Data.Linq;
using SEP.Models;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SEP.Controllers
{
    public class CalendarController : Controller
    {
        private DB2 db = new DB2();
        /// <summary>
        /// set the values for design view of the calendar 
        /// </summary>
        /// <returns>view of the calendar</returns>
        public ActionResult Index()
        {
            if ((string)Session["Position"] == "student")
            {
                ViewBag.StMsg = "Click on the Calendar to add a Request for a Supervisor Meeting";
            }
            //Being initialized in that way, scheduler will use CalendarController.Data as a the datasource and CalendarController.Save to process changes
            var scheduler = new DHXScheduler(this);
            scheduler.Skin = DHXScheduler.Skins.Flat;
            scheduler.InitialDate = new DateTime();
            scheduler.Config.multi_day = true;
            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;

            return View(scheduler);
        }
        /// <summary>
        /// get database data accrding to the position
        /// </summary>
        /// <returns>details to the calander view according to the position of the user</returns>
        public ContentResult Data()
        {
            //if the user is a student
            if ((string)Session["Position"] == "student") 
            {

                var supervisor = GetStudentData();//calling this method to get the supervisor model 
                var supervisorData = from m in db.Event
                                     where m.LecturerId == supervisor.LecturerId
                                     select m;  //get data from the table according to the supervisor id

                var data = new SchedulerAjaxData(supervisorData);
                return (ContentResult)data;
            }
            else
            {
                //if the user is a lecturer
                string lecturerId = ((string)Session["id"]);

                //get the data from the table according to the session id
                var appoinmentData = from m in db.Event
                                     where m.LecturerId == lecturerId
                                     select m; 

                var data = new SchedulerAjaxData(appoinmentData);
                return (ContentResult)data;

            }
        }

        public JavaScriptResult OutputJavaScriptAlert()
        {
            string a = "alert('this is alert')";
            return JavaScript(a);
        }


        /// <summary>
        /// save the data to the database from the calendar view
        /// </summary>
        /// <param name="id">event id</param>
        /// <param name="actionValues">save or update or delete</param>
        /// <returns></returns>
        public ContentResult Save(int? id, FormCollection actionValues)
        {
            var action = new DataAction(actionValues);
            var changedEvent = (Event)DHXEventsHelper.Bind(typeof(Event), actionValues);
            var data = new CalendarDataContext();

            try
            {
                switch (action.Type)
                {
                    case DataActionTypes.Insert:
                        //do insert
                        //action.TargetId = changedEvent.id;

                        try
                        {
                            if ((string)Session["Position"] == "student")
                            {
                                var supervisorId = GetStudentData();    //calling this method to get the supervisorId
                                changedEvent.LecturerId = supervisorId.LecturerId.ToString();

                                //insert the data to the request table
                                string time = DateTime.Now.ToString("HH:mm:ss tt");
                                string query = "insert into  dbo.Requests(Name,Request,Status,Loaded) values(@a1,@a2,@a3,@a4)";
                                List<object> parameterList = new List<object>();
                                parameterList.Add(new SqlParameter("@a1", changedEvent.LecturerId));
                                parameterList.Add(new SqlParameter("@a2", Session["UserName"] + "Requests for a meeting on" + changedEvent.start_date + "CUT" + Session["Avatar"] + "CUT" + time + "CUT" + changedEvent.id + "CUTAppoinment"));
                                parameterList.Add(new SqlParameter("@a3", 2));
                                parameterList.Add(new SqlParameter("@a4", 2));
                                object[] parameters123 = parameterList.ToArray();
                                int rs = db.Database.ExecuteSqlCommand(query, parameters123);

                                data.Events.InsertOnSubmit(changedEvent);
                                break;
                            }
                            else
                            {
                                //if the user is a lecturer 
                                string lecturerId = (string)Session["id"];
                                changedEvent.LecturerId = lecturerId;
                                data.Events.InsertOnSubmit(changedEvent);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    case DataActionTypes.Delete:
                        //do delete
                        if ((string)Session["Position"] == "student")
                        {
                            TempData["MsgDlt"] = "You Dont have permission to delete the Appoinment!";
                            RedirectToAction("Index");
                            break;
                        }
                        else
                        {
                            changedEvent = data.Events.SingleOrDefault(ev => ev.id == action.SourceId);
                            data.Events.DeleteOnSubmit(changedEvent);
                            break;
                     
                        }
                    default:
                        //do update
                        try
                        {
                            if ((string)Session["Position"] == "student")
                            {
                                TempData["MsgDlt"] = "You Dont have permission!";
                                RedirectToAction("Index");
                                break;
                            }
                            else
                            {
                                var eventToUpdate = data.Events.SingleOrDefault(ev => ev.id == action.SourceId);
                                DHXEventsHelper.Update(eventToUpdate, changedEvent, new List<string>() { "id" });//update all properties, except for id
                                break;

                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                }

                data.SubmitChanges();
                action.TargetId = changedEvent.id;
            }
            catch (Exception e)
            {
                action.Type = DataActionTypes.Error;
            }
            return (ContentResult)new AjaxSaveResponse(action);
        }
        /// <summary>
        /// get lecturer details when the user is a student
        /// </summary>
        /// <returns>Lecturer ID</returns>
        public Lecturer GetStudentData()
        {
            string studentId = ((string)Session["id"]);
            Group supervisorName = (from Y in db.StudentGroupeLists
                                    join B in db.Groups on Y.GroupNo equals B.GroupID
                                    where (Y.StuId == studentId)
                                    select B).FirstOrDefault<Group>();

            Lecturer lecturerId = (from m in db.Lecturers
                                   where m.Name == supervisorName.Supervisor
                                   select m).FirstOrDefault<Lecturer>();
            return lecturerId;

        }
    }
}

