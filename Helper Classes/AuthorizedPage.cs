using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPWeBSM
{
    // Instead of adding auth to every single file we inherit all the files to this class
    // in simple words if youre logged in it lets you load the page..
    // if not youre redirected to Login.aspx
    public class AuthorizedPage : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            // 🔴 Disable caching (IMPORTANT)
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            base.OnLoad(e);
        }
    }
}