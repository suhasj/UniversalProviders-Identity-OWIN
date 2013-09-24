<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RoleManagement.aspx.cs" Inherits="UniversalProviders_Identity_OWIN.Account.RoleManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
    <div>
        <h2> Create role</h2>
        <asp:TextBox runat="server" Id="RoleName"  placeholder="Role name" />  
        <asp:Button Text="AddRole" ID="AddRole" OnClick="AddRole_Click" runat="server"/>
    </div>

    <div>
        <h2>Add User to role</h2>
        <asp:TextBox runat="server" Id="Username" placeholder="user name"/>  
        <asp:TextBox runat="server" ID="AddRoleName" placeholder="Role name"/>  
        <asp:Button Text="Add User to Role" ID="AddUserRole" OnClick="AddUserRole_Click" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
