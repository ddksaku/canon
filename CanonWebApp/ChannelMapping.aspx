<%@ Page Title="" Language="C#" Theme="Default" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="ChannelMapping.aspx.cs" Inherits="CanonWebApp.ChannelMapping" %>
<%@ Register TagPrefix="Canon" TagName="AdminChannelMapping" Src="~/Controls/AdminChannelsMapping.ascx" %>

<asp:Content ID="MappingContent" ContentPlaceHolderID="Content" runat="server">
    <Canon:AdminChannelMapping runat="server" ID="MappingMainCtrl" />
</asp:Content>
