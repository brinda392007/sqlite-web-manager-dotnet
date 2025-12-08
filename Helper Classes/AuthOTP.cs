using System;
using System.Web;
using System.Web.UI;

public class AuthOtp : System.Web.UI.Page
{
    // This runs BEFORE the Page_Load of the child page (ResetPassword.aspx)
    protected override void OnLoad(EventArgs e)
    {
        // 1. Check if the "VIP Badge" exists in Session
        if (Session["IsVerified"] == null || (bool)Session["IsVerified"] == false)
        {
            // STOP! They skipped the OTP. Kick them out.
            Response.Redirect("Login.aspx");
        }

        // 2. If we are here, they are safe. Allow the page to load.
        base.OnLoad(e);
    }
}