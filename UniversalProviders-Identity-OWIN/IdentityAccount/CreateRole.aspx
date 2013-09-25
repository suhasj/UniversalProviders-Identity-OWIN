<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateRole.aspx.cs" Inherits="UniversalProviders_Identity_OWIN.IdentityAccount.CreateRole" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
    <h3> Create Role</h3>
    <asp:ValidationSummary runat="server" />    

    <div>
        Enter role name
        <asp:TextBox runat="server" Id="RoleName"/>
        <div>
            <asp:Button Text="Add Role" ID="AddRole" OnClick="AddRole_Click" runat="server" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
