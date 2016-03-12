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
using System.Data.SqlClient;
using System.Web.Helpers;

namespace SEP.Controllers
{
    public class Students_moduleController : Controller
    {
        ///Create a instances of a DB class
        private DB2 db = new DB2();
        /// <summary>
        /// "The index method which execute initially during page loading"
        /// </summary>
        /// <param name="searchTerm">"value of the search feild set to nul to retive entire list of students"</param>
        /// <returns></returns>
        public ActionResult Index(string searchTerm = null)
        {
            string loggedUserId = Session["id"] + "";

            ///Check if the logged user has already acceppted a group request
            bool isExists = db.StudentGroupeLists.Any(m => m.status == 1 && m.StuId == loggedUserId);
            if (isExists)
            {
                return RedirectToAction("Index", "Home");

            }
            else 
{
                var modelToView = (from r in db.Students
                                   orderby r.Name ascending
                                   where (r.Name.Contains(searchTerm) || searchTerm == null)
                                   select r);

                return View(modelToView);
            }
        }

        
        
        /// <summary>
        /// "Function to search the students by name"
        /// </summary>
        /// <param name="key">"Term to be search"</param>
        /// <returns>"array of students with matching search term and no.of elements in the array"</returns>
        public ActionResult searchStudent(string key)
        {
            ///Query to serch the sudents with given search key
            string qrysearch = "select Name from dbo.Student where Name LIKE '%' + @q1 + '%'";
            
            List<object> parameterListSearch = new List<object>();
            parameterListSearch.Add(new SqlParameter("@q1", key));
            object[] p1 = parameterListSearch.ToArray();

            IList<string> rs = db.Database.SqlQuery<string>(qrysearch, p1).ToList<string>();

            Array array = rs.Cast<string>().ToArray();

            return Json(new { student = array, size = rs.Count }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// "Method to retrive the summary detail of the selected group member to display into central grid"
        /// </summary>
        /// <param name="id">"gets the id of the selected member"</param>
        /// <returns></returns>

        public ActionResult GetStudent(string selectedUser)
        {
            ///Query to retrive the data of selected student
            string qry = "select * from dbo.Student where Name = @a1";

            List<object> parameterList = new List<object>();
            parameterList.Add(new SqlParameter("@a1", selectedUser));
            object[] parameters = parameterList.ToArray();

            ///creating a instance of student class to get access to it's attributes
            Student st1 = db.Students.SqlQuery(qry, parameters).FirstOrDefault();

            ///Creating a object of type student to assign values and then return
            Student st = new Student
            {
                Name = st1.Name,
                CGPA = st1.CGPA,
                RegistrationNo = st1.RegistrationNo,
                ContactNo = st1.ContactNo,
                Email = st1.Email,
                Avatar = st1.Avatar
            };
            return Json(new { student = st }, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// "Function to add the logged in user to his group once he press 'Create my group' button"
        /// </summary>
        /// <param name="loggedUserID">"User id the logged in user"</param>
        /// <param name="currentModule">"Currently logged module"</param>
        /// <returns>"returns 1=> group been create, 2=> fails"</returns>
        public int FillMe(string loggedUserID, string currentModule)
        {
            ///Query to check if the logged in user already having a group
            string qryFind = "Select GroupNo from dbo.StudentGroupeList where StuId=@a1";

            List<object> parameterList = new List<object>();
            parameterList.Add(new SqlParameter("@a1", loggedUserID));
            object[] parametersFillMe = parameterList.ToArray();

            string value = db.Database.SqlQuery<string>(qryFind, parametersFillMe).FirstOrDefault();

            ///Check if the logged in user doesn't have any group yet
            if (value == null)
            {
                ///Query to get the member count currently available in logged in user's group.
                string qryGetCount = "SELECT COUNT(DISTINCT GroupNo) from dbo.StudentGroupeList";

                int result = db.Database.SqlQuery<int>(qryGetCount).FirstOrDefault();
                
                int no1 = Convert.ToInt32(result);
                int newGroupID = no1++;


                ///Query to insert the leader's deatails once Student press create group button
                string inputMeQry = "insert into dbo.StudentGroupeList(StuId,Module,GroupNo,status) values (@a,@b, @c,@d)";

                List<object> parameterList2 = new List<object>();
                parameterList2.Add(new SqlParameter("@a", loggedUserID));
                parameterList2.Add(new SqlParameter("@b", currentModule));
                parameterList2.Add(new SqlParameter("@c", "SE_SEP_" + newGroupID));
                parameterList2.Add(new SqlParameter("@d", 2));

                object[] parametersFillMe2 = parameterList2.ToArray();

                int resultset = db.Database.ExecuteSqlCommand(inputMeQry, parametersFillMe2);

                return 1;
            }

            else
            {
                return 2;
            }
        }


        /// <summary>
        /// "Function to Add selected student to logged in users group"
        /// </summary>
        /// <param name="studentId">"Id of the added student"</param>
        /// <param name="module">"Module Id of the logged module"</param>
        /// <param name="leaderID">"Id of the logged user"</param>
        /// <returns></returns>
        public int addToGroup(string studentId, string module, string leaderID)
        {
            string addToGroupQry1 = "SELECT GroupNo from dbo.StudentGroupeList WHERE StuId=@a";
            List<object> parameterList1 = new List<object>();
            parameterList1.Add(new SqlParameter("@a", leaderID));
            object[] parameters1 = parameterList1.ToArray();
            string leadersGroupNo = db.Database.SqlQuery<string>(addToGroupQry1, parameters1).FirstOrDefault();

            ///Check if the logged user has already created a group or not
            if (leadersGroupNo != null)
            {
                string addToGroupQry2 = "SELECT COUNT(StuId) FROM dbo.StudentGroupeList WHERE GroupNo=@b";
                List<object> parameterList2 = new List<object>();
                parameterList2.Add(new SqlParameter("@b", leadersGroupNo));
                object[] parameters2 = parameterList2.ToArray();
                int memberCount = db.Database.SqlQuery<int>(addToGroupQry2, parameters2).FirstOrDefault();

                ///Check if the member count of the logged user's group exceeds 4
                if (memberCount <= 3)
                {
                    string addToGroupQry4 = "select StuId from dbo.StudentGroupeList where StuId=@e and GroupNo=@f";
                    List<object> parameterList3 = new List<object>();
                    parameterList3.Add(new SqlParameter("@e", studentId));
                    parameterList3.Add(new SqlParameter("@f", leadersGroupNo));

                    object[] parameters3 = parameterList3.ToArray();
                    string result = db.Database.SqlQuery<string>(addToGroupQry4, parameters3).FirstOrDefault();

                    ///Check if the select user is already in the logged user's group
                    if (result == null)
                    {
                        int defaultStatus = 0;
                        string addToGroupQry3 = "insert into dbo.StudentGroupeList(StuId, Module, GroupNo,status) VALUES (@a,@b,@c,@d)";
                        List<object> parameterList4 = new List<object>();
                        parameterList4.Add(new SqlParameter("@a", studentId));
                        parameterList4.Add(new SqlParameter("@b", module));
                        parameterList4.Add(new SqlParameter("@c", leadersGroupNo));
                        parameterList4.Add(new SqlParameter("@d", defaultStatus));

                        object[] parameters4 = parameterList4.ToArray();
                        int resultSet = db.Database.ExecuteSqlCommand(addToGroupQry3, parameters4);

                        return 1;

                    }
                    else {
                        return 4;
                    }


                }

                else {
                    return 2;
                }
            }
            else {
                return 3;
            }

        }



        /// <summary>
        /// "Funtion that refreshing the group information table on adding new member to the group"
        /// </summary>
        /// <param name="studentId">"Student Id of the added member"</param>
        /// <returns>"return json object containing the avatar, name and status of the added memberSS"</returns>
        public ActionResult FillTable(string studentId)
        {
            ///Get informations of selected member
            string FillTableQry1 = "select * from dbo.Student where RegistrationNo=@a";
            List<object> parameterList1 = new List<object>();
            parameterList1.Add(new SqlParameter("@a", studentId));
            object[] parameters1 = parameterList1.ToArray();

            Student student = db.Students.SqlQuery(FillTableQry1, parameters1).FirstOrDefault();

            Student st = new Student
            {
                Name = student.Name,
                Avatar = student.Avatar
            };

            ///get the acceptance status of the selected user withing group
            string FillTableQry2 = "select * from dbo.StudentGroupeList where StuId=@b";
            List<object> parameterList2 = new List<object>();
            parameterList2.Add(new SqlParameter("@b", studentId));
            object[] parameters2 = parameterList2.ToArray();

            StudentGroupeList sgl = db.StudentGroupeLists.SqlQuery(FillTableQry2, parameters2).FirstOrDefault();

            StudentGroupeList sgl1 = new StudentGroupeList
            {
                status = sgl.status
            };

            return Json(new { student = st, acceptanceStatus = sgl1 }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// "Function to update notification table"
        /// </summary>
        /// <param name="newMemberId">"Id of the newly added student"</param>
        /// <returns></returns>
        public int UpdateNotifications(string newMemberId)
        {
            ///Get the details of the newly added user
            string updateNotificationsqry1 = "select * from dbo.Student where RegistrationNo=@a";

            List<object> parameterList1 = new List<object>();
            parameterList1.Add(new SqlParameter("@a", newMemberId));
            object[] parameters1 = parameterList1.ToArray();

            Student stu2 = db.Students.SqlQuery(updateNotificationsqry1, parameters1).FirstOrDefault();

            string memberName = stu2.Name;

            ///Takes the system date and format it according to the required format
            string time = DateTime.Now.ToString("HH:mm:ss tt");

            ///Message to be send to the newly added member
            string msg = "  " + Session["UserName"] + " request you to join his/her group CUT" + Session["Avatar"] + "CUT" + time + "CUT";

            string updateNotificationsqry3 = "insert into dbo.Requests(Name, Request, Status, Loaded) values(@q,@r,@s,@t)";

            List<object> parameterList2 = new List<object>();
            parameterList2.Add(new SqlParameter("@q", memberName));
            parameterList2.Add(new SqlParameter("@r", msg));
            parameterList2.Add(new SqlParameter("@s", 2));
            parameterList2.Add(new SqlParameter("@t", 2));
            object[] parameters2 = parameterList2.ToArray();

            int rs = db.Database.ExecuteSqlCommand(updateNotificationsqry3, parameters2);

            return 1;
        }



        /// <summary>
        /// "Refresh the group information table on page load"
        /// </summary>
        /// <returns>"returns 3 arrays including summary details of the added members"</returns>
        public ActionResult FillTableOnLoad()
        {
            int i = 0;
            string groupNumber = "p";

            ///takes the logged user's userId
            string loggedUser = (string)Session["id"];

            ///Check if the logged user is a group leader and get his Id
            bool result = db.StudentGroupeLists.Any(m => m.status == 2 && m.StuId == loggedUser);

            if (result)
            {
                var matchingTuples = from ps in db.StudentGroupeLists
                                     where (ps.StuId == loggedUser && ps.status == 2)
                                     select ps;

                ///traverse through each matching tuple
                foreach (var t in matchingTuples)
                {
                    groupNumber = t.GroupNo;
                }

                ///get the required informations to be display in the group detail table by joining student and StudentGroupeLists tables
                var mod = from m in db.StudentGroupeLists
                          join
                          pf in db.Students on m.StuId equals pf.RegistrationNo
                          where (m.GroupNo == groupNumber)
                          select new
                          {
                              avatar = pf.Avatar,
                              status = m.status,
                              Name = pf.Name
                          };

                ///get the no.of affected rows
                var count = mod.Count();

                string[] avtrs = new string[count];
                string[] names = new string[count];
                int[] status = new int[count];

                ///travers though each accepted rows
                foreach (var c in mod)
                {
                    avtrs[i] = c.avatar;
                    names[i] = c.Name;
                    status[i] = c.status;

                    i++;
                }
                return Json(new { avatarSet = avtrs, nameSet = names, statusSet = status }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { student = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// "Method to get the subject proficiency of each student to chart"
        /// </summary>
        /// <param name="stuId">"Member Id of the selected user"</param>
        /// <returns></returns>
        public ActionResult getProficiency(string stuId)
        {

            string qryProficiency = "select * from dbo.languageProficiency where RegistrationNum= @r1";
            List<object> parameterListLanguage = new List<object>();
            parameterListLanguage.Add(new SqlParameter("@r1", stuId));
            object[] r1 = parameterListLanguage.ToArray();
            IList<int> rs = db.Database.SqlQuery<int>(qryProficiency, r1).ToList<int>();

            Array arr = rs.Cast<int>().ToArray();

            return Json(new { chartData = arr, size = rs.Count }, JsonRequestBehavior.AllowGet);
        }



        // GET: Students_module/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students_module/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students_module/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RegistrationNo,Name,Email,ContactNo,CGPA,Avatar,CV,Password")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students_module/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students_module/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RegistrationNo,Name,Email,ContactNo,CGPA,Avatar,CV,Password")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students_module/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students_module/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
