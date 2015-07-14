<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminPricesImporter.ascx.cs" Inherits="CanonWebApp.Controls.AdminPricesImporter" %>

<dev:ASPxCallbackPanel ID="cblPanelPricesImporter" runat="server"  Width="100%">
    <PanelCollection>
        <dev:PanelContent>
            <dev:ASPxUploadControl ID="UploadControl" Width="100%"
                ClientInstanceName="pricesUploader" runat="server" 
                OnFileUploadComplete="UploadControl_FileUploadComplete">
                <ValidationSettings MaxFileSize="5242880" MaxFileSizeErrorText="Nahraný soubor je příliš velký">
                </ValidationSettings>
                <ClientSideEvents FileUploadComplete="function(s, e) {
                    if(e.isValid)
                    {
                        priceImportStatusLabel.SetText(e.errorText);
                        priceImportStatusLabel.SetVisible(true);
                        priceImportErrorsList.SetVisible(false);
                    }
                    else
                    {
                        priceImportStatusLabel.SetVisible(false);
                        priceImportErrorsList.SetVisible(true);
                    }
                    
                    
                    var strs = e.callbackData.split('||');
                    for(i=0; i < strs.length; i++)
                    {
                        if(strs[i].length != 0)
                            priceImportErrorsList.AddItem(strs[i]);
                    }
                    if(priceImportErrorsList.GetItemCount() == 0)
                        priceImportErrorsList.SetVisible(false);
                    
                }" />
            </dev:ASPxUploadControl>
            
            <table>
                <tr>
                    <td width="100%" align="right">
                        <dev:ASPxButton ID="btnUploadImport" ClientInstanceName="btnUploadImport" runat="server" Text="Importovat" meta:resourcekey="UploadImportButton">
                            <ClientSideEvents Click="function(s, e) {
                                priceImportStatusLabel.SetText(''); 
                                priceImportErrorsList.ClearItems();
                                priceImportErrorsList.SetVisible(false);
                                priceImportStatusLabel.SetVisible(false);
                                e.processOnServer = false;
                                
                                pricesUploader.UploadFile();
                                
                            }" />
                        </dev:ASPxButton>
                    </td>
                    <td align="right">
                        <dev:ASPxButton AutoPostBack="false" ClientInstanceName="btnClose" ID="btnClose" runat="server" Text="Zavřít">
                        </dev:ASPxButton>
                    </td>
                </tr>
            </table>
            <div style="padding-top:10px; padding-bottom:10px;">
                <dev:ASPxLabel ID="lblImportStatus" Width="100%" EnableClientSideAPI="true"
                    ClientInstanceName="priceImportStatusLabel" runat="server" Text="" ForeColor='#00CC00' ClientVisible="false">
                </dev:ASPxLabel>
                <dev:ASPxListBox ID="listErrors" runat="server" Width="100%" EnableClientSideAPI="true"
                    ClientInstanceName="priceImportErrorsList" ClientVisible="false" >
                </dev:ASPxListBox>
            </div>
            
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>