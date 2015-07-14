<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminLog.ascx.cs" Inherits="CanonWebApp.Controls.AdminLog" %>

<%@ Register TagPrefix="Canon" TagName="LogPopupControl" Src="~/Controls/LogPopupControl.ascx" %>
<%@ Register TagPrefix="Canon" TagName="CanonCalendar" Src="~/Controls/CanonCalendar.ascx" %>

<script language="javascript" type="text/javascript">
    function OnMainHistoryErrorsClick(element, keyValue) {
        logStatusPopup.Show();
        clbPanelLog.PerformCallback('MainLogErrors' + keyValue);
    }
</script>

<dev:ASPxCallbackPanel ID="clbPanelLog" runat="server" Width="100%" 
    OnCallback="clbPanelLog_Callback" ClientInstanceName="clbPanelLog" HideContentOnCallback="False">
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px">
  <tr><td colspan="3">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
  </td></tr>
  <tr>
  <td width="2%" align="right">
    <dev:ASPxLabel ID="lblState" Font-Bold="True" runat="server" meta:resourcekey="StateTitle">
    </dev:ASPxLabel>
  </td>
  <td width="93%" align="left">
      <dev:ASPxComboBox ID="cbState" runat="server" ClientInstanceName="cbState">
      <ClientSideEvents SelectedIndexChanged="function(s, e) {
                clbPanelLog.PerformCallback('SelectedIndexChanged' + s.GetSelectedItem().value);
        }" />
      </dev:ASPxComboBox>
  </td>
  <td width="5%" align="right">
  	  <Canon:CanonCalendar runat="server" ID="deLogDate" ParentPanel="clbPanelLog" />
  </td>
  </tr>
  <tr><td colspan="3">
    <dev:ASPxGridView ID="gridMainLog" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridMainLog" 
          KeyFieldName="LogId"
          OnHtmlRowPrepared="gridMainLog_HtmlRowPrepared"
          OnHtmlRowCreated="gridMainLog_HtmlRowCreated">
        <Columns>
            <dev:GridViewDataTextColumn Name="colChannelName" Caption="" Width="30%" FieldName="ChannelName" VisibleIndex="0">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colChannelType" Caption="" Width="30%" FieldName="" VisibleIndex="1">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colLogState" Caption="" Width="30%" FieldName="" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataColumn Name="colHistoryLink" Caption="" VisibleIndex="3" Width="2%">
                 <DataItemTemplate>
                     <dev:ASPxHyperLink ID="hlLogHistory" runat="server" Text="" NavigateUrl="javascript:void(0);">
                     </dev:ASPxHyperLink>
                 </DataItemTemplate>
            </dev:GridViewDataColumn>
            <dev:GridViewDataColumn Name="colToChannelLink" Caption="" VisibleIndex="4" Width="2%">
                 <DataItemTemplate>
                     <dev:ASPxHyperLink ID="hlToChannelLink" runat="server" Text="" NavigateUrl="javascript:void(0);">
                     </dev:ASPxHyperLink>
                 </DataItemTemplate>
            </dev:GridViewDataColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="10"></SettingsPager>
        <SettingsLoadingPanel ImagePosition="Top" />
    </dev:ASPxGridView>
  </td></tr>
 </table>
 
<dev:ASPxPopupControl ID="popupLogStatus" runat="server" 
ClientInstanceName="logStatusPopup" Width="400px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
<ContentCollection>
<dev:PopupControlContentControl>
    <table cellpadding="5" cellspacing="0" width="100%" style="margin-bottom:10px;">
    <tr>
    <td width="80%" align="left">
        <dev:ASPxLabel ID="lblTryedAmountHeader" Font-Bold="True" runat="server" meta:resourcekey="lblTryedAmountHeader">
        </dev:ASPxLabel>
    </td>
    <td align="right">
        <dev:ASPxLabel ID="lblTryedAmount" runat="server">
        </dev:ASPxLabel>
    </td>
    </tr>
    <tr>
    <td width="80%" align="left">
        <dev:ASPxLabel ID="lblSuccessAmountHeader" Font-Bold="True" runat="server" meta:resourcekey="lblSuccessAmountHeader">
        </dev:ASPxLabel>
    </td>
    <td align="right">
        <dev:ASPxLabel ID="lblSuccessAmount" runat="server">
        </dev:ASPxLabel>
    </td>
    </tr>
    </table>
    <Canon:LogPopupControl runat="server" ID="MainLogErrorsCtrl" LogTypeValue="5" />
</dev:PopupControlContentControl>
</ContentCollection>
</dev:ASPxPopupControl>

</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>