<%@ Page Title="Analyze Database" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Analyze.aspx.cs" Inherits="ASPWeBSM.Analyze" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="min-h-screen bg-slate-900 py-10">

        <div class="mb-6 flex items-center justify-between">
            <a href="Default.aspx"
                class="flex items-center gap-2 text-slate-400 hover:text-orange-400">← Back to Command Center
            </a>

            <asp:Label ID="lblDbName" runat="server"
                CssClass="rounded bg-slate-800 px-4 py-2 font-mono text-sm text-orange-400">
            </asp:Label>
        </div>

        <div class="mb-6 rounded-xl border border-slate-700 bg-slate-800 p-6 shadow-xl">
            <h2 class="mb-2 text-2xl font-bold text-orange-400">Table Operations</h2>
            <p class="mb-4 text-sm text-slate-400">
                Select the operations you want to generate for each table.
            </p>

            <asp:Label ID="lblMessage" runat="server"
                CssClass="mb-4 block font-mono text-sm text-orange-300"></asp:Label>

            <asp:Panel ID="pnlTables" runat="server">
                <asp:Repeater ID="rptTables" runat="server">
                    <HeaderTemplate>
                        <table class="w-full text-left text-sm text-slate-300">
                            <thead class="bg-slate-700 text-xs uppercase text-slate-400">
                                <tr>
                                    <th class="px-4 py-3">Table</th>
                                    <th class="px-4 py-3 text-center">Select</th>
                                    <th class="px-4 py-3 text-center">Insert</th>
                                    <th class="px-4 py-3 text-center">Update</th>
                                    <th class="px-4 py-3 text-center">Delete</th>
                                    <th class="px-4 py-3 text-center">Select By Id</th>
                                    <th class="px-4 py-3 text-center">Select all</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="border-b border-slate-700 hover:bg-slate-750">
                            <td class="px-4 py-3 font-mono text-orange-300">
                                <asp:HiddenField ID="hfTableName" runat="server"
                                    Value='<%# Eval("TableName") %>' />
                                <%# Eval("TableName") %>
                            </td>
                            <td class="px-4 py-3 text-center">
                                <asp:CheckBox ID="chkSelect" runat="server" />
                            </td>
                            <td class="px-4 py-3 text-center">
                                <asp:CheckBox ID="chkInsert" runat="server" />
                            </td>
                            <td class="px-4 py-3 text-center">
                                <asp:CheckBox ID="chkUpdate" runat="server" />
                            </td>
                            <td class="px-4 py-3 text-center">
                                <asp:CheckBox ID="chkDelete" runat="server" />
                            </td>
                            <td class="px-4 py-3 text-center">
                                <asp:CheckBox ID="chkSelectById" runat="server" />
                            </td>
                            <td class="px-4 py-3 text-center">
                                <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelectAll_CheckedChanged" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>

            <div class="mt-6 flex flex-wrap gap-3">
                <asp:Button ID="btnGenerateSp" runat="server" Text="Create SP"
                    OnClick="btnGenerateSp_OnClick"
                    CssClass="rounded bg-orange-600 px-4 py-2 text-sm font-semibold text-white shadow hover:bg-orange-500" />

                <asp:Button ID="btnGenerateMethods" runat="server" Text="Create Methods"
                    OnClick="btnGenerateMethods_OnClick"
                    CssClass="rounded border border-slate-600 bg-slate-800 px-4 py-2 text-sm font-semibold text-slate-200 hover:bg-slate-700" />

                <asp:Button ID="btnGenerateBoth" runat="server" Text="Create Both"
                    OnClick="btnGenerateBoth_OnClick"
                    CssClass="rounded bg-gradient-to-r from-orange-500 to-orange-600 px-4 py-2 text-sm font-semibold text-white shadow-lg hover:from-orange-400 hover:to-orange-500" />
            </div>
        </div>
    </div>
</asp:Content>

