using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPWeBSM
{
    public class AuthorizedPage : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            base.OnLoad(e);
        }
    }
}