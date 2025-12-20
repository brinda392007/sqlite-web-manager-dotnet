using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.UI;

namespace ASPWebSM
{
    public partial class AboutUs : System.Web.UI.Page
    {
        // Session key for tracking email count
        private const string SESSION_EMAIL_COUNT = "EmailCount";
        private const int MAX_EMAILS_PER_SESSION = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize email count from session
                if (Session[SESSION_EMAIL_COUNT] == null)
                {
                    Session[SESSION_EMAIL_COUNT] = 0;
                }
                UpdateEmailCountDisplay();
            }
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                // Get current email count from session
                int emailCount = Session[SESSION_EMAIL_COUNT] != null
                    ? Convert.ToInt32(Session[SESSION_EMAIL_COUNT])
                    : 0;

                // Check if email limit reached
                if (emailCount >= MAX_EMAILS_PER_SESSION)
                {
                    ShowModal("error", "Email limit reached! You can send maximum 5 emails per session to prevent spam. Please try again later.");
                    btnSendMessage.Enabled = false;
                    btnSendMessage.Text = "🚫 Email Limit Reached - Try Again Later";
                    btnSendMessage.Style["background"] = "#475569";
                    btnSendMessage.Style["box-shadow"] = "none";
                    return;
                }

                // Validate form fields
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtSubject.Text) ||
                    string.IsNullOrWhiteSpace(txtMessage.Text))
                {
                    ShowModal("error", "Please fill in all fields before sending.");
                    return;
                }

                // Validate email format
                if (!IsValidEmail(txtEmail.Text))
                {
                    ShowModal("error", "Please enter a valid email address.");
                    return;
                }

                // Send email
                bool emailSent = SendEmail(txtName.Text, txtEmail.Text, txtSubject.Text, txtMessage.Text);

                if (emailSent)
                {
                    // Increment email count
                    emailCount++;
                    Session[SESSION_EMAIL_COUNT] = emailCount;

                    // Show success modal
                    int remaining = MAX_EMAILS_PER_SESSION - emailCount;
                    ShowModal("success", $"Your message has been sent successfully! We'll get back to you soon. ({remaining} emails remaining in this session)");

                    // Clear form fields
                    txtName.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtSubject.Text = string.Empty;
                    txtMessage.Text = string.Empty;

                    // Update email count display
                    UpdateEmailCountDisplay();

                    // Disable button if limit reached
                    if (emailCount >= MAX_EMAILS_PER_SESSION)
                    {
                        btnSendMessage.Enabled = false;
                        btnSendMessage.Text = "🚫 Email Limit Reached - Try Again Later";
                        btnSendMessage.Style["background"] = "#475569";
                        btnSendMessage.Style["box-shadow"] = "none";
                    }
                }
                else
                {
                    ShowModal("error", "Failed to send message. Please check your internet connection and try again.");
                }
            }
            catch (Exception ex)
            {
                ShowModal("error", "An error occurred while sending your message. Please try again later.");
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error in btnSendMessage_Click: {ex.Message}");
            }
        }

        private bool SendEmail(string senderName, string senderEmail, string subject, string message)
        {
            try
            {
                // Get SMTP credentials from AppSettings
                string smtpEmail = ConfigurationManager.AppSettings["SmtpEmail"];
                string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];

                if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpPassword))
                {
                    System.Diagnostics.Debug.WriteLine("SMTP credentials not found in configuration.");
                    return false;
                }

                // Create email message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(smtpEmail, "ASPWebSM Contact Form");
                mail.To.Add(smtpEmail); // Send to your email
                mail.Subject = $"Contact Form: {subject}";

                // Create HTML email body
                mail.Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='background: #f3f4f6; padding: 20px;'>
                            <div style='background: white; border-radius: 10px; padding: 30px; max-width: 600px; margin: 0 auto;'>
                                <h2 style='color: #f97316; border-bottom: 2px solid #f97316; padding-bottom: 10px;'>
                                    New Contact Form Submission
                                </h2>
                                
                                <div style='margin: 20px 0;'>
                                    <p style='margin: 10px 0;'><strong style='color: #0f172a;'>From:</strong> {senderName}</p>
                                    <p style='margin: 10px 0;'><strong style='color: #0f172a;'>Email:</strong> {senderEmail}</p>
                                    <p style='margin: 10px 0;'><strong style='color: #0f172a;'>Subject:</strong> {subject}</p>
                                </div>
                                
                                <div style='background: #f9fafb; border-left: 4px solid #f97316; padding: 15px; margin: 20px 0;'>
                                    <p style='margin: 0; color: #64748b; font-size: 12px; text-transform: uppercase;'>Message:</p>
                                    <p style='margin: 10px 0 0 0; color: #1e293b; white-space: pre-wrap;'>{message}</p>
                                </div>
                                
                                <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #e2e8f0;'>
                                    <p style='color: #64748b; font-size: 12px; margin: 0;'>
                                        This email was sent from the ASPWebSM contact form on {DateTime.Now:dddd, MMMM dd, yyyy 'at' hh:mm tt}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

                mail.IsBodyHtml = true;
                mail.ReplyToList.Add(new MailAddress(senderEmail, senderName)); // Allow reply to sender

                // Configure SMTP client
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Send email
                smtp.Send(mail);

                // Dispose objects
                mail.Dispose();
                smtp.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowModal(string type, string message)
        {
            hfShowModal.Value = "true";
            hfModalType.Value = type;
            hfModalMessage.Value = message;
        }

        private void UpdateEmailCountDisplay()
        {
            int emailCount = Session[SESSION_EMAIL_COUNT] != null
                ? Convert.ToInt32(Session[SESSION_EMAIL_COUNT])
                : 0;

            lblEmailCount.Text = emailCount.ToString();

            // Update button state if limit reached
            if (emailCount >= MAX_EMAILS_PER_SESSION)
            {
                btnSendMessage.Enabled = false;
                btnSendMessage.Text = "🚫 Email Limit Reached - Try Again Later";
                btnSendMessage.Style["background"] = "#475569";
                btnSendMessage.Style["box-shadow"] = "none";
            }
        }
    }
}