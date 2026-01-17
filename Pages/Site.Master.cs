using System;
using System.Web.UI;

namespace ASPWeBSM
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Determine which page we are on
            string currentPage = Request.Url.AbsolutePath.ToLower();
            bool isAuthPage = currentPage.Contains("login.aspx") || currentPage.Contains("register.aspx");

            // 2. If on Login or Register, disable the click completely
            if (isAuthPage)
            {
                lnkHeader.Enabled = false;
                lnkHeader.Style["cursor"] = "default";
                lnkHeader.Style["pointer-events"] = "none"; // Extra safety for CSS
                lnkHeader.Style["text-decoration"] = "none";
            }

            // 3. Hide Logout button if not logged in
            btnLogOut.Visible = (Session["UserId"] != null);
        }

        protected void lnkHeader_Click(object sender, EventArgs e)
        {
            // This only runs if the button is Enabled (not on Login/Register)
            if (Session["UserId"] != null)
            {
                // REDIRECT to your dashboard - double check spelling of "Defual.aspx"
                Response.Redirect(ResolveUrl("Default.aspx"));
            }
            // If Session is null (like first-time visitor on AboutUs), it does nothing.
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect(ResolveUrl("Login.aspx"));
        }
    }
}