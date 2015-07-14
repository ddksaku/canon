<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CanonCalendar.ascx.cs" Inherits="CanonWebApp.Controls.CanonCalendar" %>

<table cellpadding="0" cellspacing="5" width="100%" border="0">
<tr>
<td align="right">
    <dev:ASPxImage ID="imgPreviousDay" Cursor="pointer" SkinID="Previous" runat="server">
    </dev:ASPxImage>
</td>
<td width="5%">
    <dev:ASPxDateEdit ID="deMainDate" runat="server">
    </dev:ASPxDateEdit>
</td>
<td align="left">
    <dev:ASPxImage ID="imgNextDay" Cursor="pointer" SkinID="Next" runat="server">
    </dev:ASPxImage>
</td>
</tr>
</table>