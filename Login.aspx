<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ASPWeBSM.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="flex items-center justify-center min-h-screen bg-slate-900">
        <div class="bg-slate-800 p-8 rounded-2xl shadow-2xl w-96 border border-slate-700">

            <h2 class="text-3xl font-bold mb-6 text-center text-white">
                Access <span class="text-orange-500">Control</span>
            </h2>

            <div class="mb-4">
                <label class="block text-slate-400 text-sm font-bold mb-2">Username</label>
                <asp:TextBox ID="txtUser" runat="server"
                    CssClass="w-full p-3 bg-slate-900 text-white rounded border border-slate-600 focus:outline-none focus:border-orange-500 transition-colors placeholder-slate-500"
                    placeholder="Username"></asp:TextBox>
            </div>

            <div class="mb-8">
                <label class="block text-slate-400 text-sm font-bold mb-2">Password</label>
                <div class="relative">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ClientIDMode="Static"
                        CssClass="w-full p-3 bg-slate-900 text-white rounded border border-slate-600 focus:outline-none focus:border-orange-500 transition-colors placeholder-slate-500 pr-10"
                        placeholder="••••••••"></asp:TextBox>
                    
                    <span class="absolute inset-y-0 right-0 flex items-center pr-3 cursor-pointer text-slate-400 hover:text-orange-500"
                          onclick="togglePasswordVisibility('txtPassword', 'toggleIcon')">
                        <i id="toggleIcon" class="fas fa-eye"></i>
                    </span>
                </div>
            </div>
            <div class="mb-4">
                <asp:Button ID="btnLogin" runat="server" Text="Enter System" OnClick="btnLogin_Click"
                    CssClass="w-full bg-orange-500 hover:bg-orange-600 text-white font-bold py-3 px-4 rounded transition duration-500 transform hover:scale-105 shadow-lg shadow-orange-500/30" />
            </div>

            <p class="mt-4 text-center text-sm text-slate-400">
                Need access? <a href="Register.aspx" class="text-orange-400 hover:text-orange-300 font-semibold underline decoration-dotted">Register here</a>
            </p>
        </div>
    </div>

    <script type="text/javascript">
        function togglePasswordVisibility(passwordTextBoxId, iconId) {
            // Find the elements using their Static IDs
            var passwordInput = document.getElementById(passwordTextBoxId);
            var icon = document.getElementById(iconId);

            if (passwordInput.type === 'password') {
                passwordInput.type = 'text'; // Change to text mode
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash'); // Change icon to 'eye-slash'
            } else {
                passwordInput.type = 'password'; // Change back to password mode
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye'); // Change icon back to 'eye'
            }
        }
    </script>
</asp:Content>