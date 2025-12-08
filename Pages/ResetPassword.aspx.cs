using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPWeBSM
{
    public partial class ResetPassword : AuthOtp
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            Page.Validate("ResetGroup");
            if (!Page.IsValid) return;
            bool success = false;
            try
            {
                string password = txtCnfPassword.Text;
                string email = Session["Email"].ToString();
                DatabaseManager.resetPassword(email, password);
                Session.Remove("IsVerified");
                Session.Remove("email");
                Session.Remove("OTP");
                Session.Abandon();
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            UiHelper.SetToast("Password Updated", "success");
            if (success) Response.Redirect("Login.aspx");
        }
    }
}