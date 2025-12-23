<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="ASPWeBSM.Upload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="flex min-h-screen flex-col items-center bg-slate-900 pt-20">

        <div class="mb-6 w-full max-w-lg">
            <a href="Default.aspx" class="flex items-center gap-2 text-slate-400 transition hover:text-orange-500">
                ← Return to Command Center
            </a>
        </div>

        <div class="w-full max-w-lg rounded-xl border border-slate-700 bg-slate-800 p-8 shadow-2xl">
            <h2 class="mb-2 text-3xl font-bold text-white">Data Ingestion</h2>
            <p class="mb-8 text-sm text-slate-400">Upload your database files.</p>

            <div class="mb-6">
                <label class="mb-2 block text-sm font-bold text-orange-400">Select Target File</label>

                <asp:FileUpload ID="fileUpload"  runat="server"   
                    CssClass="block w-full cursor-pointer
                        rounded border-slate-600 bg-slate-900
                        text-sm text-slate-400
                        file:mr-4 file:border-0
                        file:bg-orange-600 file:px-4 file:py-2
                        file:text-sm file:font-semibold file:text-white
                        hover:text-orange-500 focus:outline-none :file:bg-orange-500" />
            </div>

            <asp:Button ID="btnUpload" runat="server" Text="Initiate Upload" OnClick="btnUpload_Click"
                CssClass="w-full rounded bg-orange-600 px-4 py-3 font-bold text-white shadow-lg shadow-orange-900/20 transition duration-300 hover:bg-orange-500" />

            <asp:Label ID="lblStatus" runat="server" CssClass="mt-4 block text-center font-mono text-sm"></asp:Label>
        </div>
    </div>
</asp:Content>