using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using SEP.Models;
using System.Data.SqlClient;
using System.Diagnostics;
namespace SEP.Controllers
{
    public class ValidateController : Controller
    {
        // GET: Validate
        public void SessionSet(string ename)
        {

            try
            {
                Session["UserName"] = ename;
            }
            catch (System.NullReferenceException)
            {

                Debug.Write("Aul wadee");
            }
        }
        [HttpPost]
        public string ImageUpload(HttpPostedFileBase Avatar)
        {
            string[] path2 = { };
            var fileName = Path.GetFileName(Avatar.FileName);
            Debug.Write(fileName);
            try
            {
                var path = Path.Combine(Server.MapPath("~/Content/Images2"), fileName);
                path2 = path.Split(new string[] { "SEP\\SEP" }, StringSplitOptions.None);
                Avatar.SaveAs(path);
             
            }
            catch (System.NullReferenceException)
            {

             
            }
            return path2[MvcApplication.Pr_one] + "";
        }
    }
}