<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminResellersImport.ascx.cs" Inherits="CanonWebApp.Controls.AdminResellersImport" %>

<%@ Register TagPrefix="Canon" TagName="AdminResellersImporter" Src="~/Controls/AdminResellersImporter.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminResellersImportsLog" Src="~/Controls/AdminResellersImportsLog.ascx" %>

<dev:ASPxCallbackPanel ID="clbPanelResellersImport" runat="server"
     ClientInstanceName="clbPanelResellersImport" 
    HideContentOnCallback="False" oncallback="clbPanelResellersImport_Callback">
    <PanelCollection>
        <dev:PanelContent>
            <table cellpadding="5" cellspacing="0" border="0">
                <tr>
                    <td colspan="6">
                        <dev:ASPxLabel ID="lblPageTitle" runat="server" Text="Import skupin reselerů" SkinID="AdminPageTitle">
                        </dev:ASPxLabel>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="right">
                        <dev:ASPxButton ID="btnLog" AutoPostBack="false" runat="server" Text="Log">
                            <ClientSideEvents Click="function(s, e) {
	                            resellersImportsLogPopup.Show();
                                 }" />
                        </dev:ASPxButton>
                        
                        <dev:ASPxPopupControl ID="popupImportsLog" runat="server"
                             HeaderText="Historie importů reselerů"
                             ClientInstanceName="resellersImportsLogPopup"
                             Width="300px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
                             EnableClientSideAPI="true" Modal="true" >
                             
                             <ContentCollection>
                                <dev:PopupControlContentControl>
                                    <Canon:AdminResellersImportsLog runat="server" ID="importsLogControl" />
                                </dev:PopupControlContentControl>
                             </ContentCollection>
                        </dev:ASPxPopupControl>
                        
                    </td>
                    <td align="right">
                        <dev:ASPxButton ID="btnImport" AutoPostBack="false" runat="server" Text="Importovat reselery">
                            <ClientSideEvents Click="function(s, e) {
	                            resellersImportPopup.Show();
                             }" />
                        </dev:ASPxButton>
                        
                        <dev:ASPxPopupControl ID="popupImport" runat="server"
                            HeaderText="Import reselerů"
                            ClientInstanceName="resellersImportPopup" Width="400px" PopupHorizontalAlign="WindowCenter" 
                            PopupVerticalAlign="WindowCenter" EnableClientSideAPI="true" Modal="true">
                             <ClientSideEvents CloseUp="function(s,e){
                                clbPanelResellersImport.PerformCallback('UpdateGrid');
                               }" />
                                <ContentCollection>
                                    <dev:PopupControlContentControl>
                                        <Canon:AdminResellersImporter runat="server" ID="importControl" />
                                    </dev:PopupControlContentControl>
                                </ContentCollection>
                        </dev:ASPxPopupControl>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <dev:ASPxGridView ID="gridResellers" runat="server" Width="100%" SettingsPager-PageSize="50"
                            AutoGenerateColumns="false" KeyFieldName="ID" ClientInstanceName="gridResellers">
                            <Columns>
                                <dev:GridViewDataTextColumn Name="colName" Caption="Název" FieldName="FileAs" Width="40%" VisibleIndex="0" />
                                <dev:GridViewDataTextColumn Name="colICO" Caption="IČO" FieldName="IdentificationNumber" Width="20%" VisibleIndex="1" />
                                <dev:GridViewDataTextColumn Name="colResellerGroup" Caption="Skupina reselerů" FieldName="ResellerGroup.Code" Width="20%" VisibleIndex="2" />
                                <dev:GridViewDataTextColumn Name="colCountry" Caption="Země" FieldName="Country.Code" Width="20%" VisibleIndex="3" />
                            </Columns>
                            
                            <Settings ShowFilterRow="true" />
                        </dev:ASPxGridView>
                    </td>
                </tr>
            </table>
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>