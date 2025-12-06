<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ASPWeBSM.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

        <div class="mb-8">
            <label class="block text-slate-400 text-sm font-bold mb-2">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" 
                CssClass="w-full p-3 bg-slate-900 text-white rounded border border-slate-600 focus:outline-none focus:border-orange-500 transition-colors placeholder-slate-500" 
                placeholder="••••••••"></asp:TextBox>
        </div>

        <div class="mb-4">
            <asp:Button ID="btnRegister" runat="server" Text="Initialize Account" OnClick="btnRegister_Click" 
                CssClass="w-full bg-orange-500 hover:bg-orange-600 text-white font-bold py-3 px-4 rounded transition duration-500 transform hover:scale-105 shadow-lg shadow-orange-500/30" />
        </div>

        <p class="mt-4 text-center text-sm text-slate-400">
            Already have access? <a href="Login.aspx" class="text-orange-400 hover:text-orange-300 font-semibold underline decoration-dotted">Login</a>
        </p>
    </div>
</div>
</asp:Content>
