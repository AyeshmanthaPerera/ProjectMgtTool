using SEP.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEP.Controllers
{
    public class DropdwnController : Controller
    {
        /// <summary>
        /// Instance from the database modle
        /// </summary>
        DB2 pk = new DB2();

        /// <summary>
        /// Get the notifications related to the logged in user 
        /// and send them as Json to the ajax request to display
        /// </summary>
        /// <returns></returns>
        public ActionResult Not()
        {  

            List<object> GetNotify = new List<object>();
            GetNotify.Add(new SqlParameter("@a1", (string)Session["UserName"]));
            GetNotify.Add(new SqlParameter("@a3", MvcApplication.Pr_two));
            GetNotify.Add(new SqlParameter("@a2", MvcApplication.Pr_two));
            object[] Notification = GetNotify.ToArray();
            

            IList<string> Notify = pk.Database.SqlQuery<string>("select Notification from dbo.Notification where Name = @a1 and Status= @a2 and Loaded = @a3", Notification).ToList<string>();

            string[] pcount = new string[Notify.Count];
            string[] avatar = new string[Notify.Count];
            string[] noti = new string[Notify.Count];
            string[] time = new string[Notify.Count];

            //Using the Global Variable ZERO
            if (Notify.Count == MvcApplication.Pr_Zero)
            {
                return Json(new { data4 =  MvcApplication.Pr_one});
            }
            Notify.CopyTo(pcount, MvcApplication.Pr_Zero);
            for (int i = Notify.Count-1; i >= MvcApplication.Pr_Zero; i--)
            {
                string[] p1 = pcount[i].Split(new string[] { "CUT" }, StringSplitOptions.None);
                noti[i] = p1[MvcApplication.Pr_Zero];
                avatar[i] = p1[MvcApplication.Pr_one];
                time[i] = p1[MvcApplication.Pr_two];
            }

            for (int i = MvcApplication.Pr_Zero; i < Notify.Count; i++)
            {
                string UpdateNotifi = "Update  dbo.Notification set Loaded = @a3 where Notification = @a2";
                List<object> Upddatenotifi = new List<object>();
                Upddatenotifi.Add(new SqlParameter("@a3", 1));
                Upddatenotifi.Add(new SqlParameter("@a2", pcount[i]));
                object[] paraUpddatenotifi = Upddatenotifi.ToArray();
                int rs1 = pk.Database.ExecuteSqlCommand(UpdateNotifi, paraUpddatenotifi);


            }


            return Json(new { data = noti, data2 = avatar, data3 = time });
        }
        /// <summary>
        /// Loading the request that are related to the logged in person
        /// </summary>
        /// <returns></returns>
        public ActionResult NotLec()
        {

            List<object> parameterList3 = new List<object>();
            parameterList3.Add(new SqlParameter("@a1", (string)Session["UserName"]));
            parameterList3.Add(new SqlParameter("@a3", MvcApplication.Pr_two));
            parameterList3.Add(new SqlParameter("@a2", MvcApplication.Pr_two));
            object[] parameters1231 = parameterList3.ToArray();

            IList<string> Request = pk.Database.SqlQuery<string>("select Request from dbo.Requests where Name = @a1 and Status= @a2 and Loaded = @a3", parameters1231).ToList<string>();
            string[] p = new string[Request.Count];
            string[] avatar = new string[Request.Count];
            string[] noti = new string[Request.Count];
            string[] time = new string[Request.Count];

            Request.CopyTo(p, MvcApplication.Pr_Zero);

            if (Request.Count == MvcApplication.Pr_Zero)
            {
                return Json(new { data4 = MvcApplication.Pr_one });
            }
            for (int i = MvcApplication.Pr_Zero; i < Request.Count; i++)
            {

                string[] p1 = p[i].Split(new string[] { "CUT" }, StringSplitOptions.None);
                noti[i] = p1[MvcApplication.Pr_Zero];
                avatar[i] = p1[MvcApplication.Pr_one];
                time[i] = p1[MvcApplication.Pr_two];
            }

            for (int i = MvcApplication.Pr_one; i < Request.Count; i++)
            {
                string query2 = "Update  dbo.Requests set Loaded = @a3 where Request = @a2";
                List<object> parameterList2 = new List<object>();
                parameterList2.Add(new SqlParameter("@a3", MvcApplication.Pr_one));
                parameterList2.Add(new SqlParameter("@a2", p[i]));

                object[] parameters12 = parameterList2.ToArray();
                int rs1 = pk.Database.ExecuteSqlCommand(query2, parameters12);


            }

            return Json(new { data = noti, data2 = avatar, data3 = time });


        }
        /// <summary>
        /// Specifies what to be happened when a request accepted
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public ActionResult AddLec(string Name)
        {
            Debug.Write(Name+"Awaaaaaaaaaaaaaaaaaaaaaa");
            string query1 = "Update  dbo.Lecturer set Module=@a3 where Name = @a2";
            List<object> parameterList1 = new List<object>();
            parameterList1.Add(new SqlParameter("@a3", "Accepted"));
            parameterList1.Add(new SqlParameter("@a2", Session["UserName"]));
            object[] parameters1 = parameterList1.ToArray();
            int rs2 = pk.Database.ExecuteSqlCommand(query1, parameters1);


            Debug.Write(Name);
            //Delete from request table
            string query2 = "Delete from dbo.Requests where Name = @a2";
            List<object> parameterList2 = new List<object>();
            parameterList2.Add(new SqlParameter("@a2", Name));

            object[] parameters12 = parameterList2.ToArray();
            int rs1 = pk.Database.ExecuteSqlCommand(query2, parameters12);
            return Json(new { data = rs1 });

        }
        /// <summary>
        /// Specifies what to be happeendd when a request rejected
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public ActionResult RemoveLec(string Name)
        {

            Debug.Write(Name + "Awaaaaaaaaaaaaaaaaaaaaaa");
            Debug.Write(Name);
            //Update Request table
            string query2 = "Delete from dbo.Requests where Name = @a2";
            List<object> parameterList2 = new List<object>();
            parameterList2.Add(new SqlParameter("@a2", Name));
            object[] parameters12 = parameterList2.ToArray();
            int rs1 = pk.Database.ExecuteSqlCommand(query2, parameters12);

            //Delete from lecturer table
            string query1 = "Delete From  dbo.Lecturer  where Name = @a2";
            List<object> parameterList1 = new List<object>();
            parameterList1.Add(new SqlParameter("@a2", Name));
            object[] parameters1 = parameterList1.ToArray();
            int rs2 = pk.Database.ExecuteSqlCommand(query1, parameters1);


            return Json(new { data = rs1 });

        }
        /// <summary>
        /// Update the status and the loaded status of the request
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateReq()
        {
            //Update Request table
            string query2 = "Update  dbo.Requests set Loaded = @a3 where Name = @a2";
            List<object> parameterList2 = new List<object>();
            parameterList2.Add(new SqlParameter("@a3", MvcApplication.Pr_two));
            parameterList2.Add(new SqlParameter("@a2", (string)Session["UserName"]));
            object[] parameters12 = parameterList2.ToArray();
            int rs1 = pk.Database.ExecuteSqlCommand(query2, parameters12);

            return Json(new { data = rs1 });
        }
        /// <summary>
        ///Update the status and the loaded status of the Notification table
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateNotifi()
        {
            //Update Notification table 
            string query2 = "Update  dbo.Notification set Loaded = @a3 where Name = @a2";
            List<object> parameterList2 = new List<object>();
            parameterList2.Add(new SqlParameter("@a3", MvcApplication.Pr_two));
            parameterList2.Add(new SqlParameter("@a2", (string)Session["UserName"]));
            object[] parameters12 = parameterList2.ToArray();

            try
            {
                int rs1 = pk.Database.ExecuteSqlCommand(query2, parameters12);
                return Json(new { data = rs1 });
            }
            catch (Exception)
            {

                return Json(new { data = 1 });
            }

          
        }


    }
}
