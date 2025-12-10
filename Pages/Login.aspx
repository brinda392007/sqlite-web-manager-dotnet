<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ASPWeBSM.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="flex min-h-screen items-center justify-center bg-slate-900">
        <div class="w-96 rounded-2xl border border-slate-700 bg-slate-800 p-8 shadow-2xl">
            <h2 class="mb-6 text-center text-3xl font-bold text-white">Access <span class="text-orange-500">Control</span>
            </h2>
            <div class="mb-4">
                <label class="mb-2 block text-sm font-bold text-slate-400">Username</label>
                <asp:TextBox ID="txtUser" runat="server"
                    CssClass="placeholder-slate-500 w-full rounded border border-slate-600 bg-slate-900 p-3 text-white transition-colors focus:border-orange-500 focus:outline-none"
                    placeholder="Username"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUser" ErrorMessage="Please Enter Username" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator
                    ID="regexValidatorForUSername"
                    runat="server"
                    ControlToValidate="txtUser"
                    ErrorMessage="Username should be longer than 6 letters and less than 15"
                    ValidationExpression="^[A-Za-z_]{7,15}$"
                    CssClass="mt-1 block text-xs font-semibold text-red-500"
                    Display="Dynamic">
                </asp:RegularExpressionValidator>
            </div>
            <div class="mb-8">
                <label class="mb-2 block text-sm font-bold text-slate-400">Password</label>
                <div class="relative">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ClientIDMode="Static"
                        CssClass="placeholder-slate-500 w-full rounded border border-slate-600 bg-slate-900 p-3 pr-10 text-white transition-colors focus:border-orange-500 focus:outline-none"
                        placeholder="••••••••"></asp:TextBox>
                    <span class="absolute inset-y-0 right-0 z-10 flex cursor-pointer items-center pr-3 text-slate-400 hover:text-orange-500"
                        onclick="togglePasswordVisibility('txtPassword', 'toggleIconLogin')">
                        <i id="toggleIconLogin" class="fas fa-eye"></i>
                    </span>
                </div>
                <asp:RegularExpressionValidator ID="revPassword" runat="server"
                    ControlToValidate="txtPassword"
                    ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$"
                    ErrorMessage="Please enter at least 6 characters, including a capital letter, a number, and a symbol."
                    CssClass="mt-1 block text-xs font-semibold text-red-500"
                    Display="Dynamic">
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword" ErrorMessage="Please Enter Password" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            <div class="mb-4">
                <asp:Button ID="btnLogin" runat="server" Text="Enter System" OnClick="btnLogin_Click"
                    CssClass="w-full transform rounded bg-orange-500 px-4 py-3 font-bold text-white shadow-lg shadow-orange-500/30 transition duration-500 hover:scale-105 hover:bg-orange-600" />
            </div>
            <p class="mt-4 text-center text-sm text-slate-400">
                Need an account? <a href="Register.aspx" class="font-semibold text-orange-400 underline decoration-dotted hover:text-orange-300">Register</a><br />
                <a href="ForgetPassword.aspx" class="font-semibold text-orange-400 underline decoration-dotted hover:text-orange-300">Forgot Password?</a>
            </p>
        </div>
    </div>
    <script type="text/javascript">
        function togglePasswordVisibility(passwordTextBoxId, iconId) {
            var passwordInput = document.getElementById(passwordTextBoxId);
            var icon = document.getElementById(iconId);
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash');
            } else {
                passwordInput.type = 'password';
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
            }
        }
    </script>
</asp:Content>
