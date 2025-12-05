<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ASPWeBSM.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="flex items-center justify-center min-h-screen bg-slate-900">
        <div class="bg-slate-800 p-8 rounded-2xl shadow-2xl w-96 border border-slate-700">

            <h2 class="text-3xl font-bold mb-6 text-center text-white">
                Join the <span class="text-orange-500">Crew</span>
            </h2>

            <div class="mb-4">
                <label class="block text-slate-400 text-sm font-bold mb-2">Username</label>
                <asp:TextBox ID="txtUser" runat="server"
                    CssClass="w-full p-3 bg-slate-900 text-white rounded border border-slate-600 focus:outline-none focus:border-orange-500 transition-colors placeholder-slate-500"
                    placeholder="Username"></asp:TextBox>
            </div>

            <div class="mb-4">
                <label class="block text-slate-400 text-sm font-bold mb-2">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"
                    CssClass="w-full p-3 bg-slate-900 text-white rounded border border-slate-600 focus:outline-none focus:border-orange-500 transition-colors placeholder-slate-500"
                    placeholder="you@example.com"></asp:TextBox>
            </div>

            <div class="mb-4">
                <label class="block text-slate-400 text-sm font-bold mb-2">Password</label>
                <div class="relative">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ClientIDMode="Static"
                        CssClass="w-full p-3 bg-slate-900 text-white rounded border border-slate-600 focus:outline-none focus:border-orange-500 transition-colors placeholder-slate-500 pr-10"
                        placeholder="••••••••"></asp:TextBox>
                    <span class="absolute inset-y-0 right-0 flex items-center pr-3 cursor-pointer text-slate-400 hover:text-orange-500"
                          onclick="togglePasswordVisibility('txtPassword', 'toggleIcon1')">
                        <i id="toggleIcon1" class="fas fa-eye"></i>
                    </span>
                </div>

                <asp:RegularExpressionValidator ID="revPassword" runat="server" 
                    ControlToValidate="txtPassword"
                    ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$"
                    ErrorMessage="Please enter at least 6 characters, including a capital letter, a number, and a symbol."
                    CssClass="block text-xs text-red-500 mt-1 font-semibold"
                    Display="Dynamic">
                </asp:RegularExpressionValidator>
            </div>
            <div class="mb-4">
                <label class="block text-slate-400 text-sm font-bold mb-2">Confirm Password</label>
                <div class="relative">
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" ClientIDMode="Static"
                        CssClass="w-full p-3 bg-slate-900 text-white rounded border border-slate-600 focus:outline-none focus:border-orange-500 transition-colors placeholder-slate-500 pr-10"
                        placeholder="••••••••"></asp:TextBox>
                    <span class="absolute inset-y-0 right-0 flex items-center pr-3 cursor-pointer text-slate-400 hover:text-orange-500"
                          onclick="togglePasswordVisibility('txtConfirmPassword', 'toggleIcon2')">
                        <i id="toggleIcon2" class="fas fa-eye"></i>
                    </span>
                </div>
            </div>
            <asp:Label ID="lblMessage" runat="server" CssClass="block text-center text-sm font-semibold text-red-500 mb-6"></asp:Label>


            <div class="mb-4">
                <asp:Button ID="btnRegister" runat="server" Text="Initialize Account" OnClick="btnRegister_Click"
                    CssClass="w-full bg-orange-500 hover:bg-orange-600 text-white font-bold py-3 px-4 rounded transition duration-500 transform hover:scale-105 shadow-lg shadow-orange-500/30" />
            </div>

            <p class="mt-4 text-center text-sm text-slate-400">
                Already have access? <a href="Login.aspx" class="text-orange-400 hover:text-orange-300 font-semibold underline decoration-dotted">Login</a>
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