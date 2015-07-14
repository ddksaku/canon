<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminChannelsMapping.ascx.cs" Inherits="CanonWebApp.Controls.AdminChannelsMapping" %>

<%@ Register TagPrefix="Canon" TagName="LogPopupControl" Src="~/Controls/LogPopupControl.ascx" %>

<script language="javascript" type="text/javascript">
    var IsFilteredMapping = false;
    function OnMoreInfoClick(element, keyValue) {
        mappingPopup.ShowAtElement(element);
        clbPanelMapping.PerformCallback('LogHistory'+keyValue);
    }
</script>

<dev:ASPxCallbackPanel ID="clbPanelMapping" runat="server" Width="100%"
    ClientInstanceName="clbPanelMapping" HideContentOnCallback="False" oncallback="clbPanelMapping_Callback">
<ClientSideEvents EndCallback="function(s, e) {
            if(clbPanelMapping.cpResult=='OK') {
              actualizePopup.Show();
            }
        }" />
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px" width="100%">
  <tr><td colspan="4">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
  </td>
  </tr>
  <tr>
  <td width="5%">
    <dev:ASPxLabel ID="lblLastActualization" Font-Bold="True" runat="server" meta:resourcekey="LastActualization">
    </dev:ASPxLabel>
  </td>
  <td width="5%">
    <dev:ASPxLabel ID="lblLastActualizationValue" runat="server">
    </dev:ASPxLabel>
  </td>
  <td colspan="2" align="left">
    <dev:ASPxButton ID="btnActualize" runat="server" AutoPostBack="False" meta:resourcekey="ButtonLastActualization">
            <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             clbPanelMapping.PerformCallback('DoActualization');
        }" />    
    </dev:ASPxButton>
  </td>
  </tr>
  <tr>
  <td colspan="4">
    <table width="100%" cellpadding="0" cellspacing="10">
    <tr>
    <td align="left" width="1%">
        <dev:ASPxLabel ID="lblSearch" runat="server" meta:resourcekey="SearchTitle">
        </dev:ASPxLabel>
    </td>
    <td align="left" width="3%">
        <dev:ASPxTextBox ID="txtSearchParam" ClientInstanceName="searchMapText" runat="server" Width="200px" meta:resourcekey="SearchHelpHint">
        <ClientSideEvents GotFocus="function(s, e) {
	        searchMapText.SetText('');
        }" KeyPress="function(s, e) {if (e.htmlEvent.keyCode == 13) {
            IsFilteredMapping = true;clbPanelMapping.PerformCallback('Search');
        }}" />
        </dev:ASPxTextBox>
    </td>
    <td align="left" width="3%">
        <dev:ASPxButton ID="btnSearch" runat="server" AutoPostBack="False" meta:resourcekey="SearchButton">
            <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             IsFilteredMapping = true;
             clbPanelMapping.PerformCallback('Search');
        }" />    
        </dev:ASPxButton>
    </td>
    <td align="left" width="3%">
        <dev:ASPxButton ID="btnShowAll" runat="server" ClientEnabled="True" AutoPostBack="False" meta:resourcekey="ShowAllButton">
            <ClientSideEvents Click="function(s, e) {
                 e.processOnServer = false;
                 searchMapText.SetText('');
                 IsFilteredMapping = false;
                 clbPanelMapping.PerformCallback('ShowAll');
            }" Init="function(s, e) {s.SetEnabled(IsFilteredMapping)}" />    
        </dev:ASPxButton>
    </td>
    <td align="right" width="80%">
        <dev:ASPxLabel ID="lblFilterByState" runat="server" meta:resourcekey="FilterByState">
        </dev:ASPxLabel>
    </td>
    <td align="left" width="3%">
        <dev:ASPxComboBox ID="cbStates" runat="server">
        <ClientSideEvents SelectedIndexChanged="function(s, e) {clbPanelMapping.PerformCallback('StateChanged');}" />
        </dev:ASPxComboBox>
    </td>
    </tr>
    </table>
  </td>
  </tr>
  <tr><td colspan="4">
    <dev:ASPxGridView ID="gridMapping" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridMapping" 
          KeyFieldName="ProductId" 
          OnCustomButtonCallback="gridMapping_CustomButtonCallback" 
          OnCustomColumnDisplayText="gridMapping_CustomColumnDisplayText" 
          OnCustomCallback="gridMapping_CustomCallback"
          OnPageIndexChanged="gridMapping_PageIndexChanged"
          OnHtmlRowCreated="gridMapping_HtmlRowCreated"
          OnHtmlCommandCellPrepared="gridMapping_HtmlCommandCellPrepared"  >
        <Columns>
            <dev:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0">
                <HeaderTemplate>
                    <dev:ASPxCheckBox ID="selCheckbox" ClientInstanceName="gridMappingMainSelectionBox" runat="server" Text=""
                    ClientSideEvents-CheckedChanged="function(s, e) 
                        {gridMapping.PerformCallback('SelectAll'+s.GetChecked());}">
                    </dev:ASPxCheckBox>
                </HeaderTemplate>
                <HeaderStyle Paddings-PaddingTop="1" Paddings-PaddingBottom="1" 
                    HorizontalAlign="Center">
                    <Paddings PaddingTop="1px" PaddingBottom="1px"></Paddings>
                </HeaderStyle>
            </dev:GridViewCommandColumn>
            <dev:GridViewDataTextColumn PropertiesTextEdit-EncodeHtml="False" Name="colProductState" Caption="" Width="2%" FieldName="ProductState" VisibleIndex="1">
                <PropertiesTextEdit EncodeHtml="False"></PropertiesTextEdit>
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colProductEan" Caption="" Width="10%" FieldName="ProductEan" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colProductName" Caption="" Width="20%" FieldName="ProductName" VisibleIndex="3">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colActual" Caption="" Width="29%" FieldName="Actual" VisibleIndex="4">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colRecommended" Caption="" Width="29%" FieldName="Recommended" VisibleIndex="5">
            </dev:GridViewDataTextColumn>
            <dev:GridViewCommandColumn Name="colCommands" Width="5%" VisibleIndex="6">
                <CustomButtons>
                    <dev:GridViewCommandColumnCustomButton ID="btnCustomMapManual" Text="" Visibility="BrowsableRow">
                    </dev:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dev:GridViewCommandColumn>
            <dev:GridViewDataColumn Name="colHistoryLink" Caption="" VisibleIndex="7" Width="5%">
                 <DataItemTemplate>
                     <dev:ASPxHyperLink ID="hlChannelMapHistory" runat="server" Text="" NavigateUrl="javascript:void(0);">
                     </dev:ASPxHyperLink>
                 </DataItemTemplate>
            </dev:GridViewDataColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="10"></SettingsPager>
        <SettingsBehavior AllowMultiSelection="True" ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
        <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="400px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
        <ClientSideEvents SelectionChanged="function(s, e) {
             gridMappingMainSelectionBox.SetChecked(gridMapping.cpRowsCount == gridMapping.GetSelectedRowCount());
        }" />
    </dev:ASPxGridView>
  </td></tr>
  <tr>
  <td align="left" width="3%">
    <dev:ASPxButton ID="btnMapProducts" AutoPostBack="False" runat="server" 
          meta:resourcekey="ButtonMapProducts">  
          <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             clbPanelMapping.PerformCallback('MapSelected');
        }" />    
    </dev:ASPxButton>
  </td>
  <td align="left" width="3%">
    <dev:ASPxButton ID="btnUnMapProducts" AutoPostBack="False" runat="server" 
          meta:resourcekey="ButtonUnMapProducts">    
          <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             clbPanelMapping.PerformCallback('UnMapSelected');
        }" />    
    </dev:ASPxButton>
  </td>
  <td align="left" width="3%">
    <dev:ASPxButton ID="btnDeleteFromMapping" AutoPostBack="False" runat="server" 
          meta:resourcekey="ButtonDeleteFromMapping">  
          <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             clbPanelMapping.PerformCallback('ExcludeSelected');
        }" />  
    </dev:ASPxButton>
  </td>
  <td align="left">
    <dev:ASPxButton ID="btnAddToMapping" AutoPostBack="False" runat="server" 
          meta:resourcekey="ButtonAddToMapping">  
          <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             clbPanelMapping.PerformCallback('IncludeSelected');
        }" />  
    </dev:ASPxButton>
  </td>
  </tr>
  <tr><td colspan="4" align="left">
    <dev:ASPxButton ID="btnBackToChannels" AutoPostBack="True" runat="server" 
          meta:resourcekey="BackToChannels" OnClick="btnBackToChannels_Click">    
    </dev:ASPxButton>
  </td></tr>
 </table>
 
 <dev:ASPxPopupControl ID="popupImport" runat="server" 
    ClientInstanceName="mappingPopup" Width="400px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
    <ContentCollection>
    <dev:PopupControlContentControl>
	    <Canon:LogPopupControl runat="server" ID="MapHistoryCtrl" LogTypeValue="1" />
    </dev:PopupControlContentControl>
    </ContentCollection>
</dev:ASPxPopupControl>

 <dev:ASPxPopupControl ID="popupActualizeStatus" runat="server" 
    ClientInstanceName="actualizePopup" Width="400px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
    <ContentCollection>
    <dev:PopupControlContentControl>
	    <dev:ASPxLabel ID="lblActualizeMessage" runat="server">
        </dev:ASPxLabel>
    </dev:PopupControlContentControl>
    </ContentCollection>
</dev:ASPxPopupControl>

</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>