<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminProductMapping.ascx.cs" Inherits="CanonWebApp.Controls.AdminProductMapping" %>
<script language="javascript" type="text/javascript">
    var IsFilteredProdMapping = false;
</script>

<dev:ASPxCallbackPanel ID="clbPanelProductMapping" runat="server" Width="100%" 
    ClientInstanceName="clbPanelProductMapping" HideContentOnCallback="False" oncallback="clbPanelProductMapping_Callback">
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px">
  <tr><td colspan="4">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
  </td>
  </tr>
  <tr><td colspan="4" align="left">
    <dev:ASPxLabel ID="lblMapInfo" Font-Bold="True" runat="server" meta:resourcekey="MapInfo">
    </dev:ASPxLabel>
  </td>
  </tr>
  <tr>
  <td colspan="4" align="left">
    <dev:ASPxGridView ID="gridCurrentMapping" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridCurrentMapping" 
          KeyFieldName="ProductId" >
        <Columns>
            <dev:GridViewDataTextColumn Name="colProductEan" Caption="" Width="10%" FieldName="ProductEan" VisibleIndex="0">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colProductName" Caption="" Width="20%" FieldName="ProductName" VisibleIndex="1">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colActual" Caption="" Width="30%" FieldName="Actual" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colRecommended" Caption="" Width="30%" FieldName="Recommended" VisibleIndex="3">
            </dev:GridViewDataTextColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="20"></SettingsPager>
        <SettingsBehavior AllowMultiSelection="False" ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
        <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="400px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
    </dev:ASPxGridView>
  </td>
  </tr>
  <tr><td colspan="4" align="left">
    <dev:ASPxLabel ID="lblChangeMapping" Font-Bold="True" runat="server" meta:resourcekey="ChangeMapping">
    </dev:ASPxLabel>
  </td>
  </tr>
  <tr>
  <td width="5%" align="right">
    <dev:ASPxLabel ID="lblSearch" runat="server" meta:resourcekey="SearchTitle">
    </dev:ASPxLabel>
  </td>
  <td width="5%" align="left">
    <dev:ASPxTextBox ID="txtSearchParam" ClientInstanceName="searchPMapText" runat="server" Width="200px" meta:resourcekey="SearchHelpHint">
        <ClientSideEvents GotFocus="function(s, e) {
	        searchPMapText.SetText('');
        }" KeyPress="function(s, e) {if (e.htmlEvent.keyCode == 13) {
            IsFilteredProdMapping = true;clbPanelProductMapping.PerformCallback('Search');
        }}" />
    </dev:ASPxTextBox>
  </td>
  <td width="5%" align="left">
    <dev:ASPxButton ID="btnSearch" runat="server" AutoPostBack="False" meta:resourcekey="SearchButton">
            <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             IsFilteredProdMapping = true;
             clbPanelProductMapping.PerformCallback('Search');
        }" />    
    </dev:ASPxButton>
  </td>
  <td width="85%" align="left">
    <dev:ASPxButton ID="btnShowAll" runat="server" ClientEnabled="True" AutoPostBack="False" meta:resourcekey="ShowAllButton">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             searchPMapText.SetText('');
             IsFilteredProdMapping = false;
             clbPanelProductMapping.PerformCallback('ShowAll');
        }" Init="function(s, e) {s.SetEnabled(IsFilteredProdMapping)}" />    
    </dev:ASPxButton>
  </td>
  </tr>
  <tr><td colspan="4">
    <dev:ASPxGridView ID="gridPMapping" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridPMapping" 
          KeyFieldName="RelId" 
          OnCustomButtonCallback="gridPMapping_CustomButtonCallback">
        <Columns>
            <dev:GridViewDataTextColumn Name="colProductName" Caption="" Width="40%" FieldName="ProductName" VisibleIndex="1">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colRelevancePercent" Caption="" Width="5%" FieldName="RelevancePercent" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colDescription" Caption="" Width="40%" FieldName="ProductDesc" VisibleIndex="3">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colPriceVat" Caption="" Width="5%" FieldName="PriceVat" VisibleIndex="4">
            </dev:GridViewDataTextColumn>
            <dev:GridViewCommandColumn Name="colCommands" Width="10%">
                <CustomButtons>
                    <dev:GridViewCommandColumnCustomButton ID="btnCustomChooseAndSave" Text="" Visibility="BrowsableRow">
                    </dev:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dev:GridViewCommandColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="10"></SettingsPager>
        <SettingsBehavior AllowMultiSelection="False" ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
        <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="400px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
    </dev:ASPxGridView>
  </td></tr>
  <tr><td colspan="4" align="left">
    <dev:ASPxButton ID="btnBackToMapping" AutoPostBack="True" runat="server" 
          meta:resourcekey="BackToMapping" OnClick="btnBackToMapping_Click">    
    </dev:ASPxButton>
  </td></tr>
 </table>
</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>