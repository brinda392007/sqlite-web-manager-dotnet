<%@ Page Title="ResetPassword" Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="ASPWeBSM.ResetPassword" %>

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
</asp:Content>
