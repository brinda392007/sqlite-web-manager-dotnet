<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="ASPWeBSM.ForgetPassword" %>

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

                        <!-- WRAPPER: Must be 'relative' so the validator can float inside it -->
                        <div class="relative mb-6">

                            <!-- TEXTBOX -->
                            <!-- Added 'validate-me' class for JS hook -->
                            <asp:TextBox ID="txtEmail" runat="server"
                                CssClass="validate-me w-full rounded border border-slate-600 bg-slate-900 p-3 text-white placeholder-slate-500 outline-none transition-colors focus:border-orange-500"
                                placeholder="Enter your email"
                                ValidationGroup="EmailStep"></asp:TextBox>

                            <!--VALIDATOR -->
                            <!--absolute: takes it out of flow
                                -top-3: pulls it up onto the border
                                left-3: adds left spacing
                                bg-slate-800: MATCHES YOUR CARD BACKGROUND (This masks the line behind it)
                            -->
                            <asp:RequiredFieldValidator
                                ID="rfvEmail"
                                runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="Email is Required"
                                Display="Dynamic"
                                CssClass="absolute -top-3 left-3 bg-slate-800 px-1 text-xs text-red-500"
                                ValidationGroup="EmailStep" />

                            <!-- REGEX VALIDATOR (Optional, matches same style) -->
                            <asp:RegularExpressionValidator
                                ID="revEmail"
                                runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="Invalid Email Format"
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                Display="Dynamic"
                                CssClass="absolute -top-3 left-3 bg-slate-800 px-1 text-xs text-red-500"
                                ValidationGroup="EmailStep" />
                        </div>

                        <div class="mb-4">
                            <!-- BUTTON: Must match ValidationGroup -->
                            <asp:Button ID="btnSendOTP" runat="server" Text="Send OTP" OnClick="btnSendOTP_Click"
                                ValidationGroup="EmailStep"
                                CssClass="w-full transform rounded bg-orange-500 px-4 py-3 font-bold text-white shadow-lg shadow-orange-500/30 transition duration-500 hover:scale-105 hover:bg-orange-600" />
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlVerify" runat="server" Visible="false">

                        <div class="relative mb-6">

                            <asp:TextBox ID="txtOTP" runat="server"
                                CssClass="validate-me w-full rounded border border-slate-600 bg-slate-900 p-3 text-white placeholder-slate-500 outline-none transition-colors focus:border-orange-500"
                                placeholder="Enter the 6-digit Code"
                                ValidationGroup="VerifyStep"></asp:TextBox>

                            <asp:RequiredFieldValidator
                                ID="rfvOTP"
                                runat="server"
                                ControlToValidate="txtOTP"
                                ErrorMessage="Code is Required"
                                Display="Dynamic"
                                CssClass="absolute -top-3 left-3 bg-slate-800 px-1 text-xs text-red-500"
                                ValidationGroup="VerifyStep" />

                            <asp:RegularExpressionValidator
                                ID="revOTP"
                                runat="server"
                                ControlToValidate="txtOTP"
                                ErrorMessage="Must be 6 digits"
                                ValidationExpression="^[0-9]{6}$"
                                Display="Dynamic"
                                CssClass="absolute -top-3 left-3 bg-slate-800 px-1 text-xs text-red-500"
                                ValidationGroup="VerifyStep" />
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
    <script type="text/javascript">
        function ValidatorUpdateDisplay(val) {
            var ctrl = document.getElementById(val.controltovalidate);

            // Handle the visibility of the error message
            val.style.display = val.isvalid ? "none" : "block";

            if (val.isvalid) {
                // VALID? Back to Normal (Slate border, Orange Focus)
                ctrl.classList.remove("border-red-500");
                ctrl.classList.remove("focus:border-red-500"); // Remove Red Focus

                ctrl.classList.add("border-slate-600");
                ctrl.classList.add("focus:border-orange-500"); // Add Orange Focus
            } else {
                // INVALID? Turn Red (Red border, Red Focus)
                ctrl.classList.remove("border-slate-600");
                ctrl.classList.remove("focus:border-orange-500"); // Remove Orange Focus

                ctrl.classList.add("border-red-500");
                ctrl.classList.add("focus:border-red-500");    // Add Red Focus
            }
        }
    </script>
</asp:Content>
