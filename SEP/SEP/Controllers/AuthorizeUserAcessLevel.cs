using SEP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEP.Controllers
{
    public class AuthorizeUserAcessLevel : AuthorizeAttribute
    {
        DB2 Current = new DB2();
        public string UserRole { get; set; }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //var isAuthorized = base.AuthorizeCore(httpContext);
            //if (!isAuthorized)
            //{

            //    return false;
            //}

            string CurrentUserRole = (string)HttpContext.Current.Session["Position"];
            if (this.UserRole.Contains(CurrentUserRole))
            {
                return true;
            }
            else
            {
                return false;

            }
        }
    }
}