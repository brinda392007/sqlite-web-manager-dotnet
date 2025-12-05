using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPWeBSM
{
    public partial class ForgetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSendOTP_Click(object sender, EventArgs e)
        {
            Page.Validate("EmailStep"); // Manually trigger the group
            if (!Page.IsValid) return;
            pnlReset.Visible = false;
            //send otp logic

            int OTP = GenerateRandomOTP();
            if (OTP == 0)
            {
                UiHelper.ShowToast(this, "Some error occured", "error");
            }
            else
            {
                Session["OTP"] = OTP.ToString();
                Session["Email"] = txtEmail.Text;
                
                SendEmail(txtEmail.Text, OTP);
            }

            pnlVerify.Visible = true;
        }

        private void SendEmail(string reciever_email, int OTP)
        {
            try
            {
                //create the client
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                //enable secure sedning
                client.EnableSsl = true;
                //use the network for delevery
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //do not use windows credentials
                client.UseDefaultCredentials = false;

                //gmail and app pass
                string appPass = ConfigurationManager.AppSettings["SmtpPassword"];
                string email = ConfigurationManager.AppSettings["SmtpEmail"];

                client.Credentials = new NetworkCredential(email, appPass);

                //structuring the email
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(email);
                msg.To.Add(reciever_email);
                msg.Subject = "Password Reset OTP";
                msg.Body = "This is your six digit OTP: " + OTP;
                msg.IsBodyHtml = true;

                //send the email
                client.Send(msg);

                //Show toast
                UiHelper.ShowToast(this, "OTP Sent!", "success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                UiHelper.ShowToast(this, "Error Occured.. Try again", "error");
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            Page.Validate("VerifyStep");
            if (!Page.IsValid) return;
            if (Session["OTP"].Equals(txtOTP.Text))
            {
                Session["IsVerified"] = true;
                UiHelper.SetToast("OTP verified", "success");
                Response.Redirect("ResetPassword.aspx");
            }
        }

        int GenerateRandomOTP()
        {
            int OTP = 0;
            OTP = new Random().Next(100000, 999999);
            return OTP;
        }
    }
}