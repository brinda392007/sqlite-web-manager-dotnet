using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

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
                    ShowStatus("error",
                        "Email limit reached! You can send maximum 5 emails per session to prevent spam. Please try again later.");
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
                    ShowStatus("error", "Please fill in all fields before sending.");
                    return;
                }

                // Create mailto link
                string mailtoLink = string.Format(
                    "mailto:team@aspwebsm.com?subject={0}&body={1}",
                    System.Uri.EscapeDataString(txtSubject.Text),
                    System.Uri.EscapeDataString(
                        $"Name: {txtName.Text}\nEmail: {txtEmail.Text}\n\nMessage:\n{txtMessage.Text}"
                    )
                );

                // Register client script to open mailto link
                string script = $"window.location.href = '{mailtoLink}';";
                ClientScript.RegisterStartupScript(this.GetType(), "mailto", script, true);

                // Increment email count
                emailCount++;
                Session[SESSION_EMAIL_COUNT] = emailCount;

                // Show success message
                int remaining = MAX_EMAILS_PER_SESSION - emailCount;
                ShowStatus("success",
                    $"Email sent successfully! You have {remaining} emails remaining in this session.");

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
            catch (Exception ex)
            {
                ShowStatus("error", "An error occurred while processing your request. Please try again.");
                // Log the error (you can add logging here)
                System.Diagnostics.Debug.WriteLine($"Error in btnSendMessage_Click: {ex.Message}");
            }
        }

        private void ShowStatus(string type, string message)
        {
            pnlStatus.Visible = true;
            lblStatus.Text = message;

            if (type == "success")
            {
                pnlStatus.Style["background"] = "rgba(249, 115, 22, 0.1)";
                pnlStatus.Style["border-color"] = "#f97316";
                lblStatus.Style["color"] = "#fb923c";
                lblStatus.Text = "✓ " + message;
            }
            else // error
            {
                pnlStatus.Style["background"] = "rgba(239, 68, 68, 0.1)";
                pnlStatus.Style["border-color"] = "#ef4444";
                lblStatus.Style["color"] = "#fca5a5";
                lblStatus.Text = "⚠ " + message;
            }

            // Auto-hide status message after 5 seconds
            string hideScript = @"
                setTimeout(function() {
                    var statusPanel = document.getElementById('" + pnlStatus.ClientID + @"');
                    if (statusPanel) {
                        statusPanel.style.display = 'none';
                    }
                }, 5000);
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "hideStatus", hideScript, true);
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