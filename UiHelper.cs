using System;
using System.Web;
using System.Web.UI;

namespace ASPWeBSM
{
    public static class UiHelper
    {
        // Show toast immediately on current page
        public static void ShowToast(Page page, string message, string type = "info")
        {
            if (page == null) return;

            message = (message ?? string.Empty).Replace("'", "\\'");
            type = (type ?? "info").Replace("'", "");

            string script = $"showToast('{message}', '{type}');";
            ScriptManager.RegisterStartupScript(page, page.GetType(),
                Guid.NewGuid().ToString("N"), script, true);
        }

        // Store toast in session for after Redirect
        public static void SetToast(string message, string type = "info")
        {
            var ctx = HttpContext.Current;
            if (ctx?.Session == null) return;

            ctx.Session["ToastMessage"] = message;
            ctx.Session["ToastType"] = type;
        }

        // Show and clear toast from session (call in Page_Load)
        public static void ShowSessionToast(Page page)
        {
            var ctx = HttpContext.Current;
            if (ctx?.Session == null) return;

            var msg = ctx.Session["ToastMessage"] as string;
            if (!string.IsNullOrEmpty(msg))
            {
                var type = ctx.Session["ToastType"] as string ?? "info";
                ShowToast(page, msg, type);

                ctx.Session.Remove("ToastMessage");
                ctx.Session.Remove("ToastType");
            }
        }
    }
}
