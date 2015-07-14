<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Main.Master" Theme="Default" CodeBehind="Default.aspx.cs" Inherits="CanonWebApp._Default" %>

<%@ Register TagPrefix="Canon" TagName="CanonCalendar" Src="~/Controls/CanonCalendar.ascx" %>

<asp:Content ID="DefaultContent" ContentPlaceHolderID="Content" runat="server">
<script language="javascript" type="text/javascript">
    function ValidateSelection() {
        if (gridMainMonitor.GetSelectedRowCount() == 0) {
            alert('<%= this.GetEmptySelectionMessage() %>');
            return false;
        }
        return true;
    }
</script>
	<dev:ASPxCallbackPanel
		runat="server"
		ID="DefaultPageCallbackPanel"
		ClientInstanceName="DefaultPageCallbackPanel"
		HideContentOnCallback="False"
		OnCallback="DefaultPageCallbackPanel_Callback"
		>
		<ClientSideEvents EndCallback="function(s, e) {
            if((DefaultPageCallbackPanel.cpResult != '')||(DefaultPageCallbackPanel.cpResult != '')) {
              popupExcelExport.Hide();
              window.location = DefaultPageCallbackPanel.cpResult;
            }
        }" />
		<PanelCollection>
			<dev:PanelContent ID="PanelContent1" runat="server">
            <div>
            <table cellpadding="5" cellspacing="0" width="100%">
            <tr>
            <td width="17%">
                <dev:ASPxLabel ID="lblCategory" SkinID="LocalTitle" runat="server" meta:resourcekey="lblCategory">
                </dev:ASPxLabel>
            </td>
            <td width="12%">
                <dev:ASPxLabel ID="lblChannels" SkinID="LocalTitle" runat="server" meta:resourcekey="lblChannels">
                </dev:ASPxLabel>
            </td>
            <td width="17%">
                <dev:ASPxLabel ID="lblShops" SkinID="LocalTitle" runat="server">
                </dev:ASPxLabel>
            </td>
            <td width="17%">
                <dev:ASPxLabel ID="lblProduct" SkinID="LocalTitle" runat="server" meta:resourcekey="lblProduct">
                </dev:ASPxLabel>
            </td>
            <td width="13%">
                <dev:ASPxLabel ID="lblPrices" SkinID="LocalTitle" runat="server" meta:resourcekey="lblPrices">
                </dev:ASPxLabel>
            </td>
            <td align="left" width="14%">
                <dev:ASPxLabel ID="lblPageSize" SkinID="LocalTitle" runat="server" meta:resourcekey="lblPageSize">
                </dev:ASPxLabel>
            </td>
            </tr>
            
            <tr>
            <td rowspan="3">
                <asp:ListBox ID="lstCategories" Width="100%" SelectionMode="Multiple" runat="server"></asp:ListBox>
            </td>
            <td rowspan="3" valign="top">
                <dev:ASPxRadioButtonList ID="rblShopTypes" AutoPostBack="True" runat="server" OnSelectedIndexChanged="rblShopTypes_SelectedIndexChanged">
                </dev:ASPxRadioButtonList>
            </td>
            <td rowspan="3">
                <asp:ListBox ID="lstShops" Width="100%" SelectionMode="Multiple" runat="server"></asp:ListBox>
            </td>
            <td>
                <dev:ASPxTextBox ID="txtProduct" runat="server" Width="100%">
                </dev:ASPxTextBox>
            </td>
            <td rowspan="2">
                <dev:ASPxRadioButtonList ID="rblPriceTypes" runat="server">
                </dev:ASPxRadioButtonList>
            </td>
            <td align="left">
                 <dev:ASPxComboBox ID="cbPageSize" runat="server">
                 </dev:ASPxComboBox>
            </td>
            </tr>
            
            <tr>
            <td>
                <dev:ASPxLabel ID="lblDate" SkinID="LocalTitle" runat="server" meta:resourcekey="lblDate">
                </dev:ASPxLabel>
            </td>
            <td rowspan="2" valign="bottom" align="right">
                <dev:ASPxButton ID="btnFilter" runat="server" meta:resourcekey="btnFilter" OnClick="btnFilter_Click">
                </dev:ASPxButton>
            </td>

            </tr>
                
            <tr>
            <td>
        		<Canon:CanonCalendar runat="server" ID="deDate" />
            </td>
            <td>
                <table cellpadding="0" cellspacing="0" width="100%"><tr>
                <td style="padding-bottom:3px;">
                <dev:ASPxLabel ID="lblConditions" SkinID="LocalTitle" runat="server" meta:resourcekey="lblConditions">
                </dev:ASPxLabel>
                </td></tr><tr>
                <td>
                    <dev:ASPxComboBox ID="cbConditions" runat="server">
                    </dev:ASPxComboBox>
                </td>
                </tr></table>
            </td>
            </tr>
            </table>
            </div>
            
            <dev:ASPxGridView ID="gridMainMonitor" runat="server" Width="100%" 
                  AutoGenerateColumns="False" ClientInstanceName="gridMainMonitor" 
                  KeyFieldName="ProductId"
                  OnHtmlDataCellPrepared="gridMainMonitor_HtmlDataCellPrepared"
                  OnCustomCallback="gridMainMonitor_CustomCallback"
                  OnPageIndexChanged="gridMainMonitor_PageIndexChanged"
                  OnCustomColumnSort="gridMainMonitor_CustomColumnSort" >
                <Settings ShowGroupPanel="False" />
                <SettingsPager PageSize="15"></SettingsPager>
                <SettingsBehavior AllowMultiSelection="True" ConfirmDelete="True" />
                <SettingsLoadingPanel ImagePosition="Top" />
                <ClientSideEvents  
                SelectionChanged="function(s, e) {
                 gridMainMonitorSelectionBox.SetChecked(gridMainMonitor.cpRowsCount == gridMainMonitor.GetSelectedRowCount());
                }" />
            </dev:ASPxGridView>

            <table cellpadding="10" cellspacing="0" width="100%">
            <tr>
            <td align="right" width="5%">
                <dev:ASPxButton ID="btnExportToExcel" AutoPostBack="False" runat="server" 
                      meta:resourcekey="ButtonExportToExcel">  
                      <ClientSideEvents Click="function(s, e) {
                         if (ValidateSelection())
                         {
                             e.processOnServer = false;
                             popupExcelExport.Show();
                         }
                    }" />  
                </dev:ASPxButton>
            </td>
            <td align="left">
                <dev:ASPxButton ID="btnShowChart" AutoPostBack="False" runat="server" 
                      meta:resourcekey="ButtonShowChart">  
                      <ClientSideEvents Click="function(s, e) {
                         if (ValidateSelection())
                         {
                             e.processOnServer = false;
                             DefaultPageCallbackPanel.PerformCallback('ShowChart');
                         }
                    }" />  
                </dev:ASPxButton>
            </td>
            </tr>
            </table>

            <dev:ASPxPopupControl ID="popupExcelExport" runat="server" 
            ClientInstanceName="popupExcelExport" Width="400px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
            <ContentCollection>
            <dev:PopupControlContentControl>
             <table cellpadding="5" cellspacing="0" border="0px" width="100%">
                  <tr>
                  <td width="5%" align="right">
                    <dev:ASPxLabel ID="lblFrom" runat="server" meta:resourcekey="FromLabel">
                    </dev:ASPxLabel>
                  </td>
                  <td width="45%" align="left">
                    <Canon:CanonCalendar runat="server" ID="deExportStartDate" />
                  </td>
                  <td width="5%" align="right">
                    <dev:ASPxLabel ID="lblTo" runat="server" meta:resourcekey="ToLabel">
                    </dev:ASPxLabel>
                  </td>
                  <td width="45%" align="left">
                    <Canon:CanonCalendar runat="server" ID="deExportFinishDate" />
                  </td>
                  </tr>
                  <tr>
                  <td width="50%" align="left" colspan="2">
                    <dev:ASPxButton ID="btnExportGenerate" AutoPostBack="False" runat="server" 
                          meta:resourcekey="ButtonGenerateExcel">  
                        <ClientSideEvents Click="function(s, e) {
                             e.processOnServer = false;
                             DefaultPageCallbackPanel.PerformCallback('ExportToExcel');
                        }" />  
                    </dev:ASPxButton>
                  </td>
                  <td width="50%" align="right" colspan="2">
                    <dev:ASPxButton ID="btnExportCancel" AutoPostBack="False" runat="server" 
                          meta:resourcekey="ButtonCancel">  
                        <ClientSideEvents Click="function(s, e) {
                             e.processOnServer = false;
                             popupExcelExport.Hide();
                        }" />  
                    </dev:ASPxButton>
                  </td>
                  </tr>
                </table>
            </dev:PopupControlContentControl>
            </ContentCollection>
            </dev:ASPxPopupControl>
  
			</dev:PanelContent>
		</PanelCollection>
	</dev:ASPxCallbackPanel>

</asp:Content>
