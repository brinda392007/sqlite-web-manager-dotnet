using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ASPWeBSM
{
    public partial class ForgetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private string GetUsernameByEmail(string email)
        {
            string username = "";
            DatabaseManager.Initialize();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                string query = "SELECT Username FROM Users WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        username = result.ToString();
                    }
                }
            }

            return username;
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
                string email = txtEmail.Text;
                string username = GetUsernameByEmail(email);

                if (string.IsNullOrEmpty(username))
                {
                    UiHelper.ShowToast(this, "Email not found!", "error");
                    return;
                }
                Session["OTP"] = OTP.ToString();
                Session["Email"] = email;
                
                
                SendEmail(email,username, OTP);
            }

            pnlVerify.Visible = true;
        }

        private void SendEmail(string reciever_email, string username, int OTP)
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
                msg.Body = $@"
            <h3>Hello {username},</h3>
            <p>You requested to reset your password.</p>
            <p><strong>Your 6-digit OTP is:</strong></p>
            <h2 style='color:blue;'>{OTP}</h2>
            <p>Please do not share this OTP with anyone.</p>
            <br/>
            <p>Regards,<br/>ASPWeBSM</p>
        ";
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