<%@ Page Title="" Language="C#" Theme="Default" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="CanonWebApp.ProductDetails" %>
<%@ Register TagPrefix="Canon" TagName="AdminProductDetails" Src="~/Controls/AdminProductDetails.ascx" %>

<asp:Content ID="DetailsContent" ContentPlaceHolderID="Content" runat="server">
    <Canon:AdminProductDetails runat="server" ID="ProductDetailsCtrl" />
</asp:Content>
