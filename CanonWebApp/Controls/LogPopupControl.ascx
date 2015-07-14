<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogPopupControl.ascx.cs" Inherits="CanonWebApp.Controls.LogPopupControl" %>

<dev:ASPxGridView ID="gridLogRecords" runat="server" Width="100%" OnHtmlRowPrepared="gridLogRecords_HtmlRowPrepared">
    <Settings ShowGroupPanel="False" />
    <SettingsPager PageSize="10"></SettingsPager>
</dev:ASPxGridView>

