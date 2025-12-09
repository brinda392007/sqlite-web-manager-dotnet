<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="ASPWeBSM.ForgetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="fixed inset-0 z-50 flex items-center justify-center bg-gray-900/50 backdrop-blur-sm">
                <div class="flex flex-col items-center gap-4 rounded-lg bg-slate-700 p-6 shadow-xl">
                    <div class="h-10 w-10 animate-spin rounded-full border-4 border-orange-500 border-t-transparent"></div>
                    <span class="font-medium">Please Wait</span>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <div class="flex min-h-screen items-center justify-center bg-slate-900">
        <div class="w-96 rounded-2xl border border-slate-700 bg-slate-800 p-8 shadow-2xl">
            <h2 class="mb-6 text-center text-3xl font-bold text-white">Forgot <span class="text-orange-500">Password</span></h2>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <asp:Panel ID="pnlReset" runat="server" Visible="true">

                        <div class="mb-6">
                            
                            <asp:TextBox ID="txtEmail" runat="server"
                                CssClass="w-full rounded border border-slate-600 bg-slate-900 p-3 text-white placeholder-slate-500 outline-none transition-colors focus:border-orange-500"
                                placeholder="Enter your email"
                                ValidationGroup="EmailStep"></asp:TextBox>

                            <asp:RequiredFieldValidator
                                ID="RequiredFieldValidator1"
                                runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="Please Enter Email"
                                ForeColor="Red"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>


                            <!-- REGEX VALIDATOR (Optional, matches same style) -->
                            <asp:RegularExpressionValidator
                                ID="revEmail"
                                runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="Invalid Email Format"
                                ForeColor="Red"
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                Display="Dynamic" />
                        </div>

                        <div class="mb-4">
                            <!-- BUTTON: Must match ValidationGroup -->
                            <asp:Button ID="btnSendOTP" runat="server" Text="Send OTP" OnClick="btnSendOTP_Click"
                                CssClass="w-full transform rounded bg-orange-500 px-4 py-3 font-bold text-white shadow-lg shadow-orange-500/30 transition duration-500 hover:scale-105 hover:bg-orange-600" />
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlVerify" runat="server" Visible="false">

                        <div class="relative mb-6">

                            <asp:TextBox ID="txtOTP" runat="server"
                                CssClass="validate-me w-full rounded border border-slate-600 bg-slate-900 p-3 text-white placeholder-slate-500 outline-none transition-colors focus:border-orange-500"
                                placeholder="Enter the 6-digit Code"></asp:TextBox>

                            <asp:RequiredFieldValidator 
                                ID="RequiredFieldValidator2" 
                                runat="server" 
                                ControlToValidate="txtOTP" 
                                ErrorMessage="Please Enter OTP" 
                                ForeColor="Red" 
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>


                            <asp:RegularExpressionValidator
                                ID="revOTP"
                                runat="server"
                                ControlToValidate="txtOTP"
                                ErrorMessage="Must be 6 digits"
                                ValidationExpression="^[0-9]{6}$"
                                ForeColor="Red"
                                Display="Dynamic"/>
                        </div>

                        <div class="mb-4">
                            <asp:Button ID="btnVerify" runat="server" Text="Verify Code" OnClick="btnVerify_Click"
                                ValidationGroup="VerifyStep"
                                CssClass="w-full transform rounded bg-orange-500 px-4 py-3 font-bold text-white shadow-lg shadow-orange-500/30 transition duration-500 hover:scale-105 hover:bg-orange-600" />
                        </div>

                    </asp:Panel>

                    <asp:Label ID="lblMessage" runat="server" CssClass="mt-2 block text-center text-sm text-red-500" />

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>
    </div>
</asp:Content>
