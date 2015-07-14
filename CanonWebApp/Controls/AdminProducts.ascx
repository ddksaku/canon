<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminProducts.ascx.cs" Inherits="CanonWebApp.Controls.AdminProducts" %>

<%@ Register TagPrefix="Canon" TagName="AdminProductsImport" Src="~/Controls/AdminProductsImport.ascx" %>
<%@ Register TagPrefix="Canon" TagName="LogPopupControl" Src="~/Controls/LogPopupControl.ascx" %>

<script language="javascript" type="text/javascript">
    var IsFilteredProducts = false;
</script>

<dev:ASPxCallbackPanel ID="clbPanelProducts" runat="server" Width="60%" 
    oncallback="clbPanelProducts_Callback" ClientInstanceName="clbPanelProducts" HideContentOnCallback="False">
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px">
  <tr><td colspan="6">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
  </td></tr>
  <tr>
  <td width="10%" align="right">
    <dev:ASPxLabel ID="lblSearch" runat="server" meta:resourcekey="SearchTitle">
    </dev:ASPxLabel>
  </td>
  <td width="30%" align="left">
    <dev:ASPxTextBox ID="txtSearchParam" ClientInstanceName="searchProductText" runat="server" Width="200px" meta:resourcekey="SearchHelpHint">
        <ClientSideEvents GotFocus="function(s, e) {
	        searchProductText.SetText('');
        }" KeyPress="function(s, e) {if (e.htmlEvent.keyCode == 13) {
            IsFilteredProducts = true;clbPanelProducts.PerformCallback('Search');
        }}" />
    </dev:ASPxTextBox>
  </td>
  <td width="10%" align="left">
    <dev:ASPxButton ID="btnSearch" runat="server" AutoPostBack="False" meta:resourcekey="SearchButton">
            <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             IsFilteredProducts = true;
             clbPanelProducts.PerformCallback('Search');
        }" />    
    </dev:ASPxButton>
  </td>
  <td width="10%" align="left">
    <dev:ASPxButton ID="btnShowAll" runat="server" ClientEnabled="True" ClientInstanceName="showAllProducts" AutoPostBack="False" meta:resourcekey="ShowAllButton">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             searchProductText.SetText('');
             IsFilteredProducts = false;
             clbPanelProducts.PerformCallback('ShowAll');
        }" Init="function(s, e) {s.SetEnabled(IsFilteredProducts)}" />    
    </dev:ASPxButton>
  </td>
  <td width="40%" align="right">
    <dev:ASPxButton ID="btnShowLog" AutoPostBack="False" runat="server" meta:resourcekey="ShowLogButton">
        <ClientSideEvents Click="function(s, e) {
               importHistoryPopup.Show();
        }" />
    </dev:ASPxButton>
  </td>
  <td width="10%" align="right">
    <dev:ASPxButton ID="btnImport" AutoPostBack="False" runat="server" meta:resourcekey="ImportProductsButton">
        <ClientSideEvents Click="function(s, e) {
	    importPopup.Show();
    }" />
    </dev:ASPxButton>
    <dev:ASPxPopupControl ID="popupImport" runat="server" 
        ClientInstanceName="importPopup" Width="400px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
        <ClientSideEvents CloseUp="function(s,e){ clbPanelProducts.PerformCallback('UpdateGrid'); }" />
        <ContentCollection>
        <dev:PopupControlContentControl>
    	    <Canon:AdminProductsImport runat="server" ID="UsersControl" />
        </dev:PopupControlContentControl>
        </ContentCollection>
    </dev:ASPxPopupControl>
  </td>
  </tr>
  <tr><td colspan="6">
    <dev:ASPxGridView ID="gridProducts" runat="server" Width="100%"  SettingsPager-PageSize="50"
          AutoGenerateColumns="False" ClientInstanceName="gridProducts" 
          KeyFieldName="ProductId"
          OnCustomButtonCallback="gridProducts_CustomButtonCallback">
        <Columns>
            <dev:GridViewDataTextColumn Name="colProductName" Caption="" Width="35%" FieldName="ProductName" VisibleIndex="0">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colProductCode" Caption="" Width="15%" FieldName="ProductCode" VisibleIndex="1">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colCategory" Caption="" Width="25%" FieldName="Category.CategoryName" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colCurrentPrice" Caption="" Width="15%" FieldName="CurrentPrice" VisibleIndex="3">
            </dev:GridViewDataTextColumn>
            <dev:GridViewCommandColumn Name="colCommands" Width="10%">
                <CustomButtons>
                    <dev:GridViewCommandColumnCustomButton ID="btnCustomDetails" Text="" Visibility="BrowsableRow">
                    </dev:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dev:GridViewCommandColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="10"></SettingsPager>
        <SettingsBehavior ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
    </dev:ASPxGridView>
  </td></tr>
 </table>
 
  
  <dev:ASPxPopupControl ID="popupProductImportHistory" runat="server" 
    ClientInstanceName="importHistoryPopup" Width="400px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
    <ContentCollection>
    <dev:PopupControlContentControl>
	    <Canon:LogPopupControl runat="server" ID="ProductImportHistoryCtrl" LogTypeValue="3" />
    </dev:PopupControlContentControl>
    </ContentCollection>
  </dev:ASPxPopupControl>

</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>

