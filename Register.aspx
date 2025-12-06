<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ASPWeBSM.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="flex min-h-screen items-center justify-center bg-slate-900">
        <div class="w-96 rounded-2xl border border-slate-700 bg-slate-800 p-8 shadow-2xl">

            <h2 class="mb-6 text-center text-3xl font-bold text-white">Join the <span class="text-orange-500">Crew</span>
            </h2>

            <div class="mb-4">
                <label class="mb-2 block text-sm font-bold text-slate-400">Username</label>
                <asp:TextBox ID="txtUser" runat="server"
                    CssClass="w-full rounded border border-slate-600 bg-slate-900 p-3 text-white placeholder-slate-500 transition-colors focus:border-orange-500 focus:outline-none"
                    placeholder="Username"></asp:TextBox>

                <asp:RequiredFieldValidator ID="reqUsername"  runat="server" ControlToValidate="txtUser" ErrorMessage="Please Enter username" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="mb-4">
                <label class="mb-2 block text-sm font-bold text-slate-400">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"
                    CssClass="w-full rounded border border-slate-600 bg-slate-900 p-3 text-white placeholder-slate-500 transition-colors focus:border-orange-500 focus:outline-none"
                    placeholder="you@example.com"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="RequiredFieldValidator1"  runat="server" ControlToValidate="txtEmail" ErrorMessage="Please Enter Email" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="mb-4">
                <label class="mb-2 block text-sm font-bold text-slate-400">Password</label>
                <div class="relative">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ClientIDMode="Static"
                        CssClass="w-full rounded border border-slate-600 bg-slate-900 p-3 pr-10 text-white placeholder-slate-500 transition-colors focus:border-orange-500 focus:outline-none"
                        placeholder="••••••••"></asp:TextBox>
                    <span class="absolute inset-y-0 right-0 flex cursor-pointer items-center pr-3 text-slate-400 hover:text-orange-500"
                        onclick="togglePasswordVisibility('txtPassword', 'toggleIcon1')">
                        <i id="toggleIcon1" class="fas fa-eye"></i>
                    </span>
                </div>

                <asp:RegularExpressionValidator ID="revPassword" runat="server"
                    ControlToValidate="txtPassword"
                    ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$"
                    ErrorMessage="Please enter at least 6 characters, including a capital letter, a number, and a symbol."
                    CssClass="mt-1 block text-xs font-semibold text-red-500"
                    Display="Dynamic">
                </asp:RegularExpressionValidator>
            </div>
            <div class="mb-4">
                <label class="mb-2 block text-sm font-bold text-slate-400">Confirm Password</label>
                <div class="relative">
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" ClientIDMode="Static"
                        CssClass="w-full rounded border border-slate-600 bg-slate-900 p-3 pr-10 text-white placeholder-slate-500 transition-colors focus:border-orange-500 focus:outline-none"
                        placeholder="••••••••"></asp:TextBox>
                    <span class="absolute inset-y-0 right-0 flex cursor-pointer items-center pr-3 text-slate-400 hover:text-orange-500"
                        onclick="togglePasswordVisibility('txtConfirmPassword', 'toggleIcon2')">
                        <i id="toggleIcon2" class="fas fa-eye"></i>
                    </span>
                </div>

            </div>
            <asp:Label ID="lblMessage" runat="server" CssClass="mb-6 block text-center text-sm font-semibold text-red-500"></asp:Label>


            <div class="mb-4">
                <asp:Button ID="btnRegister" runat="server" Text="Initialize Account" OnClick="btnRegister_Click"
                    CssClass="w-full transform rounded bg-orange-500 px-4 py-3 font-bold text-white shadow-lg shadow-orange-500/30 transition duration-500 hover:scale-105 hover:bg-orange-600" />
            </div>

            <p class="mt-4 text-center text-sm text-slate-400">
                Already have access? <a href="Login.aspx" class="font-semibold text-orange-400 underline decoration-dotted hover:text-orange-300">Login</a>
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
