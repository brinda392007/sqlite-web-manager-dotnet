<%@ Page Title="ResetPassword" Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="ASPWeBSM.ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="flex min-h-screen items-center justify-center bg-slate-900">
        <div class="w-96 rounded-2xl border border-slate-700 bg-slate-800 p-8 shadow-2xl">
            <h2 class="mb-6 text-center text-3xl font-bold text-white">Reset <span class="text-orange-500">Password</span></h2>

            <asp:Panel ID="pnlChange" runat="server" Visible="true">

                <div class="relative mb-6">

                    <div class="relative">
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ClientIDMode="Static"
                            CssClass="placeholder-slate-500 w-full rounded border border-slate-600 bg-slate-900 p-3 pr-10 text-white transition-colors focus:border-orange-500 focus:outline-none"
                            placeholder="••••••••"></asp:TextBox>
                        <span class="absolute inset-y-0 right-0 flex z-10 cursor-pointer items-center pr-3 text-slate-400 hover:text-orange-500"
                            onclick="togglePasswordVisibility('txtPassword', 'toggleIconPass')">
                            <i id="toggleIconPass" class="fas fa-eye"></i>
                        </span>
                    </div>

                    <asp:RequiredFieldValidator runat="server" ID="rfvP1"
                        ControlToValidate="txtPassword"
                        ErrorMessage="Password is required"
                        Display="Dynamic"
                        ForeColor="Red">
                    </asp:RequiredFieldValidator>

                    <asp:RegularExpressionValidator ID="revPassword" runat="server"
                        ControlToValidate="txtPassword"
                        ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$"
                        ErrorMessage="Please enter at least 6 characters, including a capital letter, a number, and a symbol."
                        CssClass="mt-1 block text-xs font-semibold text-red-500"
                        Display="Dynamic">
                    </asp:RegularExpressionValidator>
                </div>

                <div class="mb-6">
                    <div class="relative">
                        <asp:TextBox ID="txtCnfPassword" runat="server" TextMode="Password" ClientIDMode="Static"
                            CssClass="w-full rounded border border-slate-600 bg-slate-900 p-3 pr-10 text-white placeholder-slate-500 transition-colors focus:border-orange-500 focus:outline-none"
                            placeholder="••••••••"></asp:TextBox>
                        <span class="absolute inset-y-0 right-0 flex cursor-pointer items-center pr-3 text-slate-400 hover:text-orange-500"
                            onclick="togglePasswordVisibility('txtCnfPassword', 'toggleIcon1')">
                            <i id="toggleIcon1" class="fas fa-eye"></i>
                        </span>
                    </div>

                    <asp:RequiredFieldValidator runat="server" ID="rfvP2"
                        ControlToValidate="txtCnfPassword"
                        ErrorMessage="Confirmation is required"
                        Display="Dynamic"
                        ForeColor="Red">
                    </asp:RequiredFieldValidator>

                    <asp:CompareValidator ID="CompareValidator1" runat="server"
                        ControlToValidate="txtCnfPassword"
                        ControlToCompare="txtPassword"
                        ErrorMessage="Passwords do not match"
                        Display="Dynamic"
                        ForeColor="Red">
                    </asp:CompareValidator>
                </div>

                <div class="mb-4">
                    <asp:Button ID="btnResetPassword" runat="server" Text="Update Password" OnClick="btnResetPassword_Click"
                        CssClass="w-full transform rounded bg-orange-500 px-4 py-3 font-bold text-white shadow-lg shadow-orange-500/30 transition duration-500 hover:scale-105 hover:bg-orange-600" />
                </div>
            </asp:Panel>
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
