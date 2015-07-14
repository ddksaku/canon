<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminCategories.ascx.cs" Inherits="CanonWebApp.Controls.AdminCategories" %>

<script language="javascript" type="text/javascript">
    var IsFilteredCategory = false;
    function ShowDeleteResult(sender) {
        if (sender.cpDeleteError == null)
            return;
        if (sender.cpDeleteError != '') {
            lblProductAssignedCategoryName.SetText(sender.cpDeleteError);
            catErrorPopup.Show();
        }
        sender.cpDeleteError = '';
    }
</script>

<dev:ASPxCallbackPanel ID="clbPanelCategories" runat="server" Width="100%" 
    oncallback="clbPanelCategories_Callback" ClientInstanceName="clbPanelCategories" HideContentOnCallback="False">
    <ClientSideEvents EndCallback="function(s, e) {
            ShowDeleteResult(clbPanelCategories);
        }" />
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px">
  <tr><td colspan="4">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
  </td></tr>
  <tr><td width="10%" align="right">
    <dev:ASPxLabel ID="lblSearch" runat="server" meta:resourcekey="SearchTitle">
    </dev:ASPxLabel>
  </td><td width="30%" align="left">
    <dev:ASPxTextBox ID="txtSearchParam" ClientInstanceName="searchCatText" runat="server" Width="200px" meta:resourcekey="SearchHelpHint">
        <ClientSideEvents GotFocus="function(s, e) {
	        searchCatText.SetText('');
        }"
        KeyPress="function(s, e) {if (e.htmlEvent.keyCode == 13) {
            IsFilteredCategory = true;clbPanelCategories.PerformCallback('Search');
        }}"  />
    </dev:ASPxTextBox>
  </td><td width="10%" align="left">
    <dev:ASPxButton ID="btnSearch" runat="server" AutoPostBack="False" meta:resourcekey="SearchButton">
            <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             IsFilteredCategory = true;
             clbPanelCategories.PerformCallback('Search');
        }" />    
    </dev:ASPxButton>
  </td>
  <td width="10%" align="left">
    <dev:ASPxButton ID="btnShowAll" runat="server" ClientEnabled="True" ClientInstanceName="showAll" AutoPostBack="False" meta:resourcekey="ShowAllButton">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             searchCatText.SetText('');
             IsFilteredCategory = false;
             clbPanelCategories.PerformCallback('ShowAll');
        }" Init="function(s, e) {s.SetEnabled(IsFilteredCategory)}" />    
    </dev:ASPxButton>
  </td>
  <td width="40%" align="right">
    <dev:ASPxButton ID="btnNewItem" runat="server" AutoPostBack="False" meta:resourcekey="NewButton">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             gridCategories.AddNewRow();         
        }" />    
    </dev:ASPxButton>
  </td>
  </tr>
  <tr><td colspan="5">
    <dev:ASPxGridView ID="gridCategories" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridCategories" 
          KeyFieldName="CategoryId" OnRowDeleting="gridCategories_RowDeleting" 
          OnHtmlEditFormCreated="gridCategories_HtmlEditFormCreated" 
          OnBeforeGetCallbackResult="gridCategories_BeforeGetCallbackResult" 
          OnCancelRowEditing="gridCategories_CancelRowEditing" 
          OnRowUpdating="gridCategories_RowUpdating" 
          OnRowInserting="gridCategories_RowInserting" 
          OnRowValidating="gridCategories_RowValidating">
          <ClientSideEvents EndCallback="function(s, e) {
            ShowDeleteResult(gridCategories);
        }" />
        <Columns>
            <dev:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0" Width="5%">
                 <HeaderTemplate>
                     <input type="checkbox" onclick="gridCategories.SelectAllRowsOnPage(this.checked);" style="vertical-align:middle;"></input>
                 </HeaderTemplate>
                 <HeaderStyle Paddings-PaddingTop="1" Paddings-PaddingBottom="1" HorizontalAlign="Center"/>
            </dev:GridViewCommandColumn>
            <dev:GridViewDataTextColumn Name="colCategoryName" Caption="Category Name" Width="30%" FieldName="CategoryName" VisibleIndex="1">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colInternalId" Caption="Internal ID" Width="30%" FieldName="InternalId" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewCommandColumn Name="colCommands" Width="20%">
                <EditButton Text="Edit" Visible="True"></EditButton>
                <DeleteButton Text="Delete" Visible="True"></DeleteButton>
            </dev:GridViewCommandColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="10"></SettingsPager>
        <SettingsBehavior AllowMultiSelection="True" ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
        <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="400px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
        <Templates>
             <EditForm>
             <div style="padding:4px 4px 3px 4px">
                 <table width="100%" cellpadding="0" cellspacing="4">
                    <tr><td width="20%" align="left">
                        <dev:ASPxLabel ID="lblEfCategoryName" runat="server">
                        </dev:ASPxLabel>
                    </td><td>
                        <dev:ASPxTextBox ID="txtEfCategoryName" runat="server" Width="200px" Text='<%# Bind("CategoryName") %>'>
                        </dev:ASPxTextBox>
                    </td></tr>
                    <tr><td width="20%" align="left">
                        <dev:ASPxLabel ID="lblEfInternalId" runat="server">
                        </dev:ASPxLabel>
                    </td><td width="60%" align="left" colspan="2">
                        <dev:ASPxTextBox ID="txtEfInternalId" runat="server" Width="200px" Text='<%# Bind("InternalId") %>'>
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
                clbPanelCategories.PerformCallback('');
             }          
        }" />    
    </dev:ASPxButton>
  </td></tr>
 </table>
 
 <dev:ASPxPopupControl ID="popupDeleteError" runat="server"
    ClientInstanceName="catErrorPopup" Width="400px" HeaderText="" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
    <ContentCollection>
    <dev:PopupControlContentControl>
	    <dev:ASPxLabel ID="lblProductsAssignedExist" runat="server">
        </dev:ASPxLabel>
        &nbsp;
	    <dev:ASPxLabel ID="lblProductAssignedCategoryName" ClientInstanceName="lblProductAssignedCategoryName" runat="server">
        </dev:ASPxLabel>
    </dev:PopupControlContentControl>
    </ContentCollection>
</dev:ASPxPopupControl>

</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>
