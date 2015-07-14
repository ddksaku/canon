<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminChannels.ascx.cs" Inherits="CanonWebApp.Controls.AdminChannels" %>

<%@ Register TagPrefix="Canon" TagName="LogPopupControl" Src="~/Controls/LogPopupControl.ascx" %>

<script language="javascript" type="text/javascript">
    var IsFiltered = false;
    function OnChannelHistoryClick(element, keyValue) {
        historyPopup.ShowAtElement(element);
        clbPanelChannels.PerformCallback('ChannelHistory' + keyValue);
    }
</script>

<dev:ASPxCallbackPanel ID="clbPanelChannels" runat="server" Width="100%" 
    oncallback="clbPanelChannels_Callback" ClientInstanceName="clbPanelChannels" HideContentOnCallback="False">
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px">
  <tr><td colspan="4">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
  </td></tr>
  <tr>
  <td width="10%" align="right">
    <dev:ASPxLabel ID="lblSearch" runat="server" meta:resourcekey="SearchTitle">
    </dev:ASPxLabel>
  </td>
  <td width="30%" align="left">
    <dev:ASPxTextBox ID="txtSearchParam" ClientInstanceName="searchChText" runat="server" Width="200px" meta:resourcekey="SearchHelpHint">
        <ClientSideEvents GotFocus="function(s, e) {
	        searchChText.SetText('');
        }" KeyPress="function(s, e) {if (e.htmlEvent.keyCode == 13) {
            IsFiltered = true;clbPanelChannels.PerformCallback('Search');
        }}" />
    </dev:ASPxTextBox>
  </td>
  <td width="10%" align="left">
    <dev:ASPxButton ID="btnSearch" runat="server" AutoPostBack="False" meta:resourcekey="SearchButton">
            <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             IsFiltered = true;
             clbPanelChannels.PerformCallback('Search');
        }" />    
    </dev:ASPxButton>
  </td>
  <td width="10%" align="left">
    <dev:ASPxButton ID="btnShowAll" runat="server" ClientEnabled="True" ClientInstanceName="showAll" AutoPostBack="False" meta:resourcekey="ShowAllButton">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             searchChText.SetText('');
             IsFiltered = false;
             clbPanelChannels.PerformCallback('ShowAll');
        }" Init="function(s, e) {s.SetEnabled(IsFiltered)}" />    
    </dev:ASPxButton>
  </td>
  <td width="40%" align="right">
    <dev:ASPxButton ID="btnNewItem" runat="server" AutoPostBack="False" meta:resourcekey="NewButton">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             gridChannels.AddNewRow();         
        }" />    
    </dev:ASPxButton>
  </td>
  </tr>
  <tr><td colspan="5">
    <dev:ASPxGridView ID="gridChannels" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridChannels" 
          KeyFieldName="ChannelId" OnRowDeleting="gridChannels_RowDeleting" 
          OnHtmlEditFormCreated="gridChannels_HtmlEditFormCreated" 
          OnBeforeGetCallbackResult="gridChannels_BeforeGetCallbackResult" 
          OnCancelRowEditing="gridChannels_CancelRowEditing" 
          OnRowUpdating="gridChannels_RowUpdating" 
          OnRowInserting="gridChannels_RowInserting" 
          OnRowValidating="gridChannels_RowValidating" 
          OnCustomButtonCallback="gridChannels_CustomButtonCallback"
          OnHtmlRowCreated="gridChannels_HtmlRowCreated"
          OnStartRowEditing="gridChannels_StartRowEditing" >
        <Columns>
            <dev:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0">
                 <HeaderTemplate>
                     <input type="checkbox" onclick="gridChannels.SelectAllRowsOnPage(this.checked);" style="vertical-align:middle;"></input>
                 </HeaderTemplate>
                 <HeaderStyle Paddings-PaddingTop="1" Paddings-PaddingBottom="1" HorizontalAlign="Center"/>
            </dev:GridViewCommandColumn>
            <dev:GridViewDataTextColumn Name="colChannelName" Caption="Channel Name" Width="20%" FieldName="ChannelName" VisibleIndex="1">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colChannelType" Caption="Channel Type" Width="10%" FieldName="ChannelType" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colChannelUrl" Caption="Url" Width="30%" FieldName="Url" VisibleIndex="3">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataCheckColumn Name="colChannelActive" Caption="Active" Width="5%" FieldName="IsActive" VisibleIndex="4">
            </dev:GridViewDataCheckColumn>
            <dev:GridViewDataTextColumn Name="colChannelReportingTo" Caption="Reporting To" Width="10%" FieldName="ReportingTo" VisibleIndex="5">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colChannelTypeHidden" Caption="" Width="0%" FieldName="ChannelType" VisibleIndex="6" Visible="False">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colChannelInfoTypeHidden" Caption="" Width="0%" FieldName="InfoType" VisibleIndex="7" Visible="False">
            </dev:GridViewDataTextColumn>
            <dev:GridViewCommandColumn Name="colCommands" Width="18%" VisibleIndex="7">
                <EditButton Text="Edit" Visible="True"></EditButton>
                <DeleteButton Text="Delete" Visible="True"></DeleteButton>
                <CustomButtons>
                    <dev:GridViewCommandColumnCustomButton ID="btnCustomMap" Text="Map" Visibility="BrowsableRow">
                    </dev:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dev:GridViewCommandColumn>
            <dev:GridViewDataColumn Name="colHistoryLink" Caption="" VisibleIndex="8" Width="2%">
                 <DataItemTemplate>
                     <dev:ASPxHyperLink ID="hlChannelHistory" runat="server" Text="" NavigateUrl="javascript:void(0);">
                     </dev:ASPxHyperLink>
                 </DataItemTemplate>
            </dev:GridViewDataColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="10"></SettingsPager>
        <SettingsBehavior AllowMultiSelection="True" ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
        <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="400px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
        <Templates>
             <EditForm>
             <div style="padding:4px 4px 3px 4px">
                 <dev:ASPxLabel ID="lblEfChannelInfo" Font-Bold="True" runat="server">
                 </dev:ASPxLabel>
                 <table width="100%" cellpadding="0" cellspacing="4">
                    <tr><td width="20%" align="left">
                        <dev:ASPxLabel ID="lblEfName" runat="server">
                        </dev:ASPxLabel>
                    </td><td>
                        <dev:ASPxTextBox ID="txtEfName" runat="server" Width="200px" Text='<%# Bind("ChannelName") %>'>
                        </dev:ASPxTextBox>
                    </td><td>
                        <dev:ASPxCheckBox ID="chbEfActive" runat="server" Value='<%# Bind("IsActive") %>'>
                        </dev:ASPxCheckBox>
                    </td></tr>
                    <tr><td width="20%" align="left">
                        <dev:ASPxLabel ID="lblEfType" runat="server">
                        </dev:ASPxLabel>
                    </td><td width="60%" align="left" colspan="2">
                        <dev:ASPxComboBox ID="cmbEfType" runat="server" Width="200px" OnInit="cmbEfType_Init">
                        </dev:ASPxComboBox>
                    </td></tr>
                    <tr><td width="20%" align="left">
                        <dev:ASPxLabel ID="lblEfUrl" runat="server">
                        </dev:ASPxLabel>
                    </td><td width="60%" align="left" colspan="2">
                        <dev:ASPxTextBox ID="txtEfUrl" runat="server" Width="200px" Text='<%# Bind("Url") %>'>
                        </dev:ASPxTextBox>
                    </td></tr>
                    <tr><td width="20%" align="left">
                        <dev:ASPxLabel ID="lblEfInfoType" runat="server">
                        </dev:ASPxLabel>
                    </td><td width="60%" align="left" colspan="2">
                        <dev:ASPxComboBox ID="cmbEfInfoType" runat="server" Width="200px" OnInit="cmbEfInfoType_Init">
                        </dev:ASPxComboBox>
                    </td></tr>
                    <tr><td width="20%" align="left">
                        <dev:ASPxLabel ID="lblReportingTo" runat="server">
                        </dev:ASPxLabel>
                    </td><td width="60%" align="left" colspan="2">
                        <dev:ASPxTextBox ID="txtReportingTo" runat="server" Width="200px"  Text='<%# Bind("ReportingTo") %>'>
                        </dev:ASPxTextBox>
                    </td></tr>
                 </table>
             </div>
             <div style="text-align:right; padding:2px 2px 2px 2px">
                 <dev:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dev:ASPxGridViewTemplateReplacement>
                 <dev:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dev:ASPxGridViewTemplateReplacement>
             </div>
             </EditForm>
        </Templates>
    </dev:ASPxGridView>
  </td></tr>
  <tr><td colspan="5" align="left">
    <dev:ASPxButton ID="btnDeleteSelected" AutoPostBack="False" runat="server" 
          meta:resourcekey="DeleteSelectedButton">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             if (confirm('Do you wish to delete all selected channels?'))
             {
                clbPanelChannels.PerformCallback('');
             }          
        }" />    
    </dev:ASPxButton>
  </td></tr>
 </table>
 
  <dev:ASPxPopupControl ID="popupChannelHistory" runat="server" 
    ClientInstanceName="historyPopup" Width="400px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
    <ContentCollection>
    <dev:PopupControlContentControl>
	    <Canon:LogPopupControl runat="server" ID="ChannelHistoryCtrl" LogTypeValue="2" />
    </dev:PopupControlContentControl>
    </ContentCollection>
  </dev:ASPxPopupControl>

</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>
