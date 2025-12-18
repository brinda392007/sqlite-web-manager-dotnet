<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASPWeBSM.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="min-h-screen bg-slate-900 p-8 text-white">

        <div class="mb-8 flex items-center justify-between border-b border-slate-700 pb-4">
            <h1 class="text-3xl font-bold text-orange-500">Command Center</h1>
            <asp:Label ID="lblWelcome" runat="server" CssClass="font-mono text-slate-400"></asp:Label>
        </div>

       <div class="grid grid-cols-1 gap-8 md:grid-cols-2">


            <div class="rounded-lg border border-slate-700 bg-slate-800 p-6 shadow-xl">
                <div class="mb-6 flex items-center justify-between">
                    <h2 class="text-xl font-bold text-orange-400">Uploads</h2>
                    <asp:Button ID="btnRefreshList" runat="server" Text="↻ Sync" OnClick="btnRefreshList_Click"
                        CssClass="cursor-pointer rounded border border-slate-600 bg-transparent px-2 py-1 text-xs text-slate-400 hover:text-white" />
                </div>

                <asp:UpdatePanel ID="UpdatePanelFiles" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="overflow-x-auto">
                            <table class="w-full text-left text-slate-300">
                                <thead class="bg-slate-700 text-xs text-slate-400 uppercase">
                                    <tr>
                                        <th class="px-6 py-3">File Name</th>
                                        <th class="px-6 py-3">Size</th>
                                        <th class="px-6 py-3">Uploaded</th>
                                        <th class="px-6 py-3 text-right">Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptFiles" runat="server" OnItemCommand="rptFiles_ItemCommand">
                                        <ItemTemplate>
                                            <tr class="border-b border-slate-700 transition-colors hover:bg-slate-750">
                                                <td class="px-6 py-4 font-medium">
                                                    <a href='<%# "Analyze.aspx?uploadId=" + Eval("Id") %>'
                                                        class="text-orange-400 hover:text-orange-300 underline decoration-dotted">
                                                        <%# Eval("FileName") %>
                                                    </a>
                                                </td>

                                                <td class="px-6 py-4">
                                                    <%# FormatSize(Eval("Size")) %>
                                                </td>
                                                <td class="px-6 py-4 text-sm text-slate-500">
                                                    <%# Eval("UploadedAt") %>
                                                </td>
                                                <td class="space-x-2 px-6 py-4 text-right">
                                                    <a href='Download.ashx?id=<%# Eval("Id") %>' target="_blank"
                                                        class="inline-block bg-orange-500 text-white px-4 py-2 rounded-md border-0 cursor-pointer no-underline font-medium hover:bg-orange-400">Download
                                                    </a>

                                                    <asp:LinkButton ID="btnDelete" runat="server"
                                                        
                                                        CommandName="DeleteFile"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        CssClass="bg-red-500 text-white px-4 py-2 rounded-md border-0 cursor-pointer no-underline font-medium hover:bg-red-400"
                                                        OnClientClick="return confirm('Are you sure you want to purge this file?');">
                                            Purge
                                                    </asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>

                            <asp:Label ID="lblEmpty" runat="server" Text="No data artifacts found." Visible="false" CssClass="block py-8 text-center text-slate-500 italic"></asp:Label>
                        </div>



                    </ContentTemplate>
                </asp:UpdatePanel>


            </div>
            <!-- Generated files panel (paste below your Uploads panel) -->
<div class="rounded-lg border border-slate-700 bg-slate-800 p-6 shadow-xl">
    <div class="mb-6 flex items-center justify-between">
        <h2 class="text-xl font-bold text-orange-400">Generated Files</h2>
        <asp:Button ID="btnRefreshGenerated" runat="server" Text="↻ Sync" OnClick="btnRefreshGenerated_Click"
            CssClass="cursor-pointer rounded border border-slate-600 bg-transparent px-2 py-1 text-xs text-slate-400 hover:text-white" />
    </div>

    <asp:UpdatePanel ID="UpdatePanelGenerated" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="overflow-x-auto">
                <table class="w-full text-left text-slate-300">
                    <thead class="bg-slate-700 text-xs text-slate-400 uppercase">
                        <tr>
                            <th class="px-6 py-3">File Name</th>
                            <th class="px-6 py-3">Operations</th>
                            <th class="px-6 py-3">Generated</th>
                            <th class="px-6 py-3 text-right">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptGenerated" runat="server" OnItemCommand="rptGenerated_ItemCommand">
                            <ItemTemplate>
                                <tr class="border-b border-slate-700 transition-colors hover:bg-slate-750">
                                    <td class="px-6 py-4 font-medium text-white">
                                        <%# Eval("FileName") %>
                                    </td>
                                    <td class="px-6 py-4 text-sm text-slate-400">
                                        <%# Eval("OperationsInfo") %>
                                    </td>
                                    <td class="px-6 py-4 text-sm text-slate-500">
                                        <%# Eval("GeneratedDate") %>
                                    </td>
                                    <td class="space-x-2 px-6 py-4 text-right whitespace-nowrap">

                                        <a href='GeneratedDownload.ashx?id=<%# Eval("FileID") %>' target="_blank"
                                            class="inline-block bg-orange-500 text-white px-4 py-2 rounded-md border-0 cursor-pointer no-underline font-medium hover:bg-orange-400">Download
                                        </a>

                                        <asp:LinkButton ID="btnPurgeGenerated" runat="server"
                                            CommandName="DeleteFile"
                                            CommandArgument='<%# Eval("FileID") %>'
                                            CssClass="bg-red-500 text-white px-4 py-2 rounded-md border-0 cursor-pointer no-underline font-medium hover:bg-red-400"
                                            OnClientClick="return confirm('Are you sure you want to purge this generated file?');">
                                            Purge
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

                <asp:Label ID="lblGeneratedEmpty" runat="server" Text="No generated files found." Visible="false" CssClass="block py-8 text-center text-slate-500 italic"></asp:Label>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

            <div class="rounded-lg border border-slate-700 bg-slate-800 p-6 shadow-xl">
                <h2 class="mb-4 text-xl font-bold text-orange-400">Quick Actions</h2>
                <a href="Upload.aspx" class="mb-3 block w-full rounded border border-slate-600 bg-slate-700 py-3 text-center text-white transition hover:bg-slate-600">Upload New File
                </a>
                <a href="#" class="block w-full rounded border border-slate-600 bg-slate-700 py-3 text-center text-white transition hover:bg-slate-600">View Analytics
                </a>
            </div>
          <div class="rounded-lg border border-slate-700 bg-slate-800 p-6 shadow-xl">
    <h2 class="mb-4 text-xl font-bold text-orange-400">Application Logs</h2>

    <asp:UpdatePanel ID="upLogs" runat="server">
        <ContentTemplate>
            <div class="max-h-[200px] overflow-y-auto rounded border border-slate-700 bg-slate-900 p-3">
                <asp:Repeater ID="rptLogs" runat="server">
                    <ItemTemplate>
                        <div class="mb-1 text-sm">
                            <span class="<%# Eval("ColorClass") %>">
                                [<%# Eval("Time") %>]
                            </span>
                            <span class="text-slate-200">
                                <%# Eval("Message") %>
                            </span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoLogs" runat="server"
                    Text="No activity yet."
                    Visible="false"
                    CssClass="text-sm text-slate-500 italic">
                </asp:Label>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

        </div>
    </div>
</asp:Content>
