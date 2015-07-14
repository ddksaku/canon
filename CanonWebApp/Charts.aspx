<%@ Page Title="" Language="C#" Theme="Default" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Charts.aspx.cs" Inherits="CanonWebApp.Charts" %>

<%@ Register TagPrefix="Canon" TagName="CanonChartControl" Src="~/Controls/ChartExporter.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
     <Canon:CanonChartControl ID="CanonChartCtrl" runat="server" />
</asp:Content>
