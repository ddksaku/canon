<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminPricesImport.ascx.cs" Inherits="CanonWebApp.Controls.AdminPricesImport" %>

<%@ Register TagPrefix="Canon" TagName="AdminPricesImporter" Src="~/Controls/AdminPricesImporter.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminPricesImportsLog" Src="~/Controls/AdminPricesImportsLog.ascx" %>

<dev:ASPxCallbackPanel ID="clbPanelPricesImport" runat="server" 
     ClientInstanceName="clbPanelPricesImport" HideContentOnCallback="False" 
    oncallback="clbPanelPricesImport_Callback">
    <PanelCollection>
        <dev:PanelContent>
            <table cellpadding="5" cellspacing="0" border="0px">
                <tr>
                    <td colspan="6">
                        <dev:ASPxLabel ID="lblPageTitle" runat="server" Text="Import ceníku Canon" SkinID="AdminPageTitle">
                        </dev:ASPxLabel>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="right">
                        <dev:ASPxButton ID="btnLog" AutoPostBack="false" runat="server" Text="Log">
                            <ClientSideEvents Click="function(s, e) {
	                            pricesImportsLogPopup.Show();
                             }" />
                        </dev:ASPxButton>
                        
                        <dev:ASPxPopupControl ID="popupImportsLog" runat="server"
                             HeaderText="Historie importů a doporučených cen"
                             ClientInstanceName="pricesImportsLogPopup"
                             Width="300px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
                             EnableClientSideAPI="true" Modal="true" >
                             <ContentCollection>
                                <dev:PopupControlContentControl>
                                    <Canon:AdminPricesImportsLog runat="server" ID="importsLogControl" />
                                </dev:PopupControlContentControl>
                             </ContentCollection>
                        </dev:ASPxPopupControl>
                    </td>
                    <td align="right">
                        <dev:ASPxButton ID="btnImport" AutoPostBack="False" runat="server" Text="Importovat ceník">
                             <ClientSideEvents Click="function(s, e) {
                            pricesImportPopup.Show();
                             }" />
                        </dev:ASPxButton>
                        
                        <dev:ASPxPopupControl ID="popupImport" runat="server"
                            HeaderText="Import produktů a doporučených cen"
                            ClientInstanceName="pricesImportPopup" Width="400px" PopupHorizontalAlign="WindowCenter" 
                            PopupVerticalAlign="WindowCenter" EnableClientSideAPI="true" Modal="true">
                             <ClientSideEvents CloseUp="function(s,e) {
                                clbPanelPricesImport.PerformCallback('UpdateGrid');
                             }" />
                                <ContentCollection>
                                    <dev:PopupControlContentControl>
                                        <Canon:AdminPricesImporter runat="server" ID="importControl" />
                                    </dev:PopupControlContentControl>
                                </ContentCollection>
                            </dev:ASPxPopupControl>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <dev:ASPxGridView ID="gridPrices" runat="server" Width="100%" KeyFieldName="ID" SettingsPager-PageSize="50"
                             AutoGenerateColumns="false" ClientInstanceName="gridPrices">
                             <Columns>
                                <dev:GridViewDataTextColumn Name="colProductName" Caption="Název" Width="30%" FieldName="ProductName" VisibleIndex="0">
                                </dev:GridViewDataTextColumn>
                                <dev:GridViewDataTextColumn Name="colProductCode" Caption="MCode" Width="15%" FieldName="ProductCode" VisibleIndex="1">
                                </dev:GridViewDataTextColumn>
                                <dev:GridViewDataTextColumn Name="colGroup" Caption="Produktová skupina" Width="25%" FieldName="ProductGroup.FileAs" VisibleIndex="2">
                                </dev:GridViewDataTextColumn>
                                <dev:GridViewDataTextColumn Name="colCurrentPrice" Caption="Doporučená cena" Width="15%" FieldName="CurrentPrice" VisibleIndex="3">
                                </dev:GridViewDataTextColumn>
                                <dev:GridViewDataTextColumn Name="colProductType" Caption="Typ produktu" Width="15%" FieldName="ProductType.Type" VisibleIndex="4">
                                </dev:GridViewDataTextColumn>
                             </Columns>
                             
                             <Settings ShowFilterRow="true" />
                             <Settings ShowFilterBar="Auto" />
                             
                        </dev:ASPxGridView>
                    </td>
                </tr>
            </table>
        </dev:PanelContent>
    </PanelCollection>


</dev:ASPxCallbackPanel>
