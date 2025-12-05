<%@ Page Title="ResetPassword" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="ASPWeBSM.ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="flex min-h-screen items-center justify-center bg-slate-900">
        <div class="w-96 rounded-2xl border border-slate-700 bg-slate-800 p-8 shadow-2xl">
            <h2 class="mb-6 text-center text-3xl font-bold text-white">Reset <span class="text-orange-500">Password</span></h2>

            <asp:Panel ID="pnlChange" runat="server" Visible="true">

                <div class="relative mb-6">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                        CssClass="validate-me w-full rounded border border-slate-600 bg-slate-900 p-3 text-white placeholder-slate-500 outline-none transition-colors focus:border-orange-500"
                        placeholder="Enter New Password"
                        ValidationGroup="ResetGroup">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator runat="server" ID="rfvP1"
                        ControlToValidate="txtPassword"
                        ErrorMessage="Password is required"
                        Display="Dynamic"
                        CssClass="absolute -top-3 left-3 bg-slate-800 px-1 text-xs text-red-500"
                        ValidationGroup="ResetGroup">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="relative mb-6">
                    <asp:TextBox ID="txtCnfPassword" runat="server" TextMode="Password"
                        CssClass="validate-me w-full rounded border border-slate-600 bg-slate-900 p-3 text-white placeholder-slate-500 outline-none transition-colors focus:border-orange-500"
                        placeholder="Confirm Password"
                        ValidationGroup="ResetGroup">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator runat="server" ID="rfvP2"
                        ControlToValidate="txtCnfPassword"
                        ErrorMessage="Confirmation is required"
                        Display="Dynamic"
                        CssClass="absolute -top-3 left-3 bg-slate-800 px-1 text-xs text-red-500"
                        ValidationGroup="ResetGroup">
                    </asp:RequiredFieldValidator>

                    <asp:CompareValidator ID="CompareValidator1" runat="server"
                        ControlToValidate="txtCnfPassword"
                        ControlToCompare="txtPassword"
                        ErrorMessage="Passwords do not match"
                        Display="Dynamic"
                        CssClass="absolute -top-3 left-3 bg-slate-800 px-1 text-xs text-red-500"
                        ValidationGroup="ResetGroup">
                    </asp:CompareValidator>
                </div>

                <div class="mb-4">
                    <asp:Button ID="btnResetPassword" runat="server" Text="Update Password" OnClick="btnResetPassword_Click"
                        ValidationGroup="ResetGroup"
                        CssClass="w-full transform rounded bg-orange-500 px-4 py-3 font-bold text-white shadow-lg shadow-orange-500/30 transition duration-500 hover:scale-105 hover:bg-orange-600" />
                </div>
            </asp:Panel>
        </div>
    </div>

    <script type="text/javascript">
        function ValidatorUpdateDisplay(val) {
            var ctrl = document.getElementById(val.controltovalidate);

            // Handle the error message visibility
            val.style.display = val.isvalid ? "none" : "block";

            if (val.isvalid) {
                // VALID STATE:
                // 1. Remove Red styles
                ctrl.classList.remove("border-red-500");
                ctrl.classList.remove("focus:border-red-500"); // Stop forcing red on focus

                // 2. Add Standard styles (Slate border, Orange focus)
                ctrl.classList.add("border-slate-600");
                ctrl.classList.add("focus:border-orange-500");
            } else {
                // INVALID STATE:
                // 1. Remove Standard styles
                ctrl.classList.remove("border-slate-600");
                ctrl.classList.remove("focus:border-orange-500"); // Remove the orange focus!

                // 2. Add Red styles
                ctrl.classList.add("border-red-500");
                ctrl.classList.add("focus:border-red-500"); // Keep it red even when typing
            }
        }
    </script>

</asp:Content>
