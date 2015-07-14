<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminProductDetails.ascx.cs" Inherits="CanonWebApp.Controls.AdminProductDetails" %>

<%@ Register TagPrefix="Canon" TagName="ProductPrices" Src="~/Controls/ProductPrices.ascx" %>

<script language="javascript" type="text/javascript">
    var IsFilteredDetails = false;
</script>

<dev:ASPxCallbackPanel ID="clbPanelDetails" runat="server" Width="80%" 
    oncallback="clbPanelDetails_Callback" ClientInstanceName="clbPanelDetails" HideContentOnCallback="False">
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px">
  <tr>
  <td colspan="4">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
  </td>
  </tr>
  <tr>
  <td colspan="4" align="left" valign="top">
    <table width="100%">
     <tr>
     <td align="left" width="40%" valign="top">
      <dev:ASPxDataView ID="dvGeneralInfo" runat="server" Paddings-PaddingLeft="0px" Width="100%" AllowPaging="False" RowPerPage="1" ColumnCount="1">

<Paddings PaddingLeft="0px"></Paddings>

      <ItemStyle Width="100%" Paddings-Padding="5px" Height="100%" >
<Paddings Padding="5px"></Paddings>
          </ItemStyle>
      <ItemTemplate>
        <table cellpadding="5" cellspacing="0" border="0px" width="100%">
          <tr>
            <td colspan="2" align="left">
              <dev:ASPxLabel ID="lblGeneralTitle" Font-Bold="True" runat="server">
              </dev:ASPxLabel>
            </td>
          </tr>
          <tr>
            <td align="left" width="50%">
              <dev:ASPxLabel ID="lblProductName" Font-Bold="True" runat="server">
              </dev:ASPxLabel>
            </td>
            <td align="right">
              <dev:ASPxLabel ID="lblProductNameValue" runat="server" Text='<%# Bind("ProductName") %>'>
              </dev:ASPxLabel>
            </td>
          </tr>
          <tr>
            <td align="left">
              <dev:ASPxLabel ID="lblProductEan" Font-Bold="True" runat="server">
              </dev:ASPxLabel>
            </td>
            <td align="right">
              <dev:ASPxLabel ID="lblProductEanValue" runat="server" Text='<%# Bind("ProductCode") %>'>
              </dev:ASPxLabel>
            </td>
          </tr>
          <tr>
            <td align="left">
              <dev:ASPxLabel ID="lblCategory" Font-Bold="True" runat="server">
              </dev:ASPxLabel>
            </td>
            <td align="right">
              <dev:ASPxLabel ID="lblCategoryValue" runat="server" Text='<%# Bind("Category.CategoryName") %>'>
              </dev:ASPxLabel>
            </td>
          </tr>
          <tr>
            <td align="left">
              <dev:ASPxLabel ID="lblRecommendedPrice" Font-Bold="True" runat="server">
              </dev:ASPxLabel>
            </td>
            <td align="right">
              <dev:ASPxLabel ID="lblRecommendedPriceValue" runat="server" Text='<%# Bind("CurrentPrice") %>'>
              </dev:ASPxLabel>
            </td>
          </tr>
        </table>
      </ItemTemplate>
      </dev:ASPxDataView>
     </td>
     <td valign="top">
      <Canon:ProductPrices ID="ProductPricesCtrl" runat="server" />
     </td>
    </tr>
   </table>
  </td>
  </tr>
  <tr>
  <td align="left" colspan="4">
    <dev:ASPxLabel ID="lbl" Font-Bold="True" runat="server" meta:resourcekey="MappingToChannelsLabel">
    </dev:ASPxLabel>
  </td>
  </tr>
  <tr>
  <td width="10%" align="right">
    <dev:ASPxLabel ID="lblSearch" runat="server" meta:resourcekey="SearchTitle">
    </dev:ASPxLabel>
  </td>
  <td width="10%" align="left">
    <dev:ASPxTextBox ID="txtSearchParam" ClientInstanceName="searchDetailsText" runat="server" Width="200px" meta:resourcekey="SearchHelpHint">
        <ClientSideEvents GotFocus="function(s, e) {
	        searchDetailsText.SetText('');
        }" KeyPress="function(s, e) {if (e.htmlEvent.keyCode == 13) {
            IsFilteredDetails = true;clbPanelDetails.PerformCallback('Search');
        }}" />
    </dev:ASPxTextBox>
  </td>
  <td width="10%" align="left">
    <dev:ASPxButton ID="btnSearch" runat="server" AutoPostBack="False" meta:resourcekey="SearchButton">
            <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             IsFilteredDetails = true;
             clbPanelDetails.PerformCallback('Search');
        }" />    
    </dev:ASPxButton>
  </td>
  <td width="70%" align="left">
    <dev:ASPxButton ID="btnShowAll" runat="server" ClientEnabled="True" ClientInstanceName="showAllDetails" AutoPostBack="False" meta:resourcekey="ShowAllButton">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             searchDetailsText.SetText('');
             IsFilteredDetails = false;
             clbPanelDetails.PerformCallback('ShowAll');
        }" Init="function(s, e) {s.SetEnabled(IsFilteredDetails)}" />    
    </dev:ASPxButton>
  </td>
  </tr>
  <tr><td colspan="4" valign="top">
    <dev:ASPxGridView ID="gridDetails" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridDetails" 
          KeyFieldName="MapId" 
          OnCustomUnboundColumnData="gridDetails_CustomUnboundColumnData">
        <Columns>
            <dev:GridViewDataTextColumn Name="colChannelName" Caption="" Width="35%" FieldName="Channel.ChannelName" VisibleIndex="0">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colChannelType" Caption="" Width="15%" FieldName="Channel.Enum.Name" VisibleIndex="1">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colProductName" Caption="" Width="35%" FieldName="MonitoredName" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colPriceVat" UnboundType="Decimal" Caption="" Width="15%" FieldName="CurrentPrice" VisibleIndex="3">
            </dev:GridViewDataTextColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="10"></SettingsPager>
        <SettingsBehavior ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
    </dev:ASPxGridView>
  </td></tr>
  <tr><td colspan="4" align="left" valign="top">
    <dev:ASPxButton ID="btnBackToProducts" AutoPostBack="True" runat="server" 
          meta:resourcekey="BackToProducts" OnClick="btnBackToProducts_Click">    
    </dev:ASPxButton>
  </td></tr>
 </table>
</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>

