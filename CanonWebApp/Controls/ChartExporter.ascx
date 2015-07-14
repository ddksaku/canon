<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChartExporter.ascx.cs" Inherits="CanonWebApp.Controls.ChartExporter" %>
<%@ Register TagPrefix="Canon" TagName="CanonCalendar" Src="~/Controls/CanonCalendar.ascx" %>

<dev:ASPxCallbackPanel ID="clbPanelPicExport" runat="server" Width="100%">
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px" width="100%">
  <tr>
  <td colspan="6" align="center">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
  </td>
  </tr>
  <tr>
  <td width="44%" align="right">
  &nbsp;
  </td>
  <td width="1%" align="right">
    <dev:ASPxLabel ID="lblFrom" runat="server" meta:resourcekey="FromLabel">
    </dev:ASPxLabel>
  </td>
  <td width="5%" align="left">
    <Canon:CanonCalendar runat="server" ID="deStartDate" />
  </td>
  <td width="1%" align="right">
    <dev:ASPxLabel ID="lblTo" runat="server" meta:resourcekey="ToLabel">
    </dev:ASPxLabel>
  </td>
  <td width="5%" align="left">
    <Canon:CanonCalendar runat="server" ID="deFinishDate" />
  </td>
  <td width="44%" align="right">
  &nbsp;
  </td>
  </tr>

  <tr>
  <td width="44%" align="right">
  &nbsp;
  </td>
  <td align="left" colspan="2">
    <dev:ASPxButton ID="btnGenerate" AutoPostBack="True" runat="server" 
          meta:resourcekey="ButtonGenerateCharts" OnClick="btnGenerate_Click">  
    </dev:ASPxButton>
  </td>
  <td align="left" colspan="2">
    <dev:ASPxButton ID="btnSavePictures" AutoPostBack="True" runat="server" 
          meta:resourcekey="ButtonSavePictures" OnClick="btnSavePictures_Click">  
    </dev:ASPxButton>
  </td>
  <td align="left">
    <dev:ASPxButton ID="btnCancel" AutoPostBack="True" runat="server" 
          meta:resourcekey="ButtonCancel" OnClick="btnCancel_Click">  
    </dev:ASPxButton>
  </td>
  </tr>

  <tr><td colspan="6" align="center" style="padding-top:20px;">
    <dev2:ASPxPanel ID="panelPics" runat="server" Width="100%">
    <PanelCollection>
        <dev2:PanelContent>
        </dev2:PanelContent>
    </PanelCollection>
    </dev2:ASPxPanel>
  </td></tr>
  </table>
</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>
