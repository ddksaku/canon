<%@ Page Title="" Language="C#" Theme="Default" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ProductMapping.aspx.cs" Inherits="CanonWebApp.ProductMapping" %>
<%@ Register TagPrefix="Canon" TagName="AdminProductMapping" Src="~/Controls/AdminProductMapping.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <Canon:AdminProductMapping runat="server" ID="ProductMappingCtrl" />
</asp:Content>
