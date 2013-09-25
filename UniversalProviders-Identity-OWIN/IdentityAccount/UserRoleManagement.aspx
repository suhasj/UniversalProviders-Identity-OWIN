<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserRoleManagement.aspx.cs" Inherits="UniversalProviders_Identity_OWIN.IdentityAccount.UserRoleManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">

    <div>
        <h3>Add user to role
        </h3>
        <asp:TextBox runat="server" ID="Username" placeholder="user name" />
        <asp:TextBox runat="server" ID="AddRoleName" placeholder="Role name" />
        <asp:Button Text="Add User to Role" ID="AddUserRole" OnClick="AddUserRole_Click" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
