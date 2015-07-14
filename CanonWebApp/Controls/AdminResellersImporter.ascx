<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminResellersImporter.ascx.cs" Inherits="CanonWebApp.Controls.AdminResellersImporter" %>

<dev:ASPxCallbackPanel ID="cblPanelResellersImporter" runat="server" Width="100%">
    <PanelCollection>
        <dev:PanelContent>
            <dev:ASPxUploadControl ID="UploadControl" Width="100%"
                ClientInstanceName="resellersUploader" runat="server" 
                OnFileUploadComplete="UploadControl_FileUploadComplete">
                <ValidationSettings MaxFileSize="5242880" MaxFileSizeErrorText="Nahraný soubor je příliš velký">
                </ValidationSettings>
                <ClientSideEvents FileUploadComplete="function(s,e) {
                    if(e.isValid)
                    {
                        resellerImportStatusLabel.SetText(e.errorText);
                        resellerImportStatusLabel.SetVisible(true);
                        resellerImportErrorsList.SetVisible(false);
                    }
                    else
                    {
                        resellerImportStatusLabel.SetVisible(false);
                        resellerImportErrorsList.SetVisible(true);
                    }
                    
                    var strs = e.callbackData.split('||');
                    for(i=0; i < strs.length; i++)
                    {
                        if(strs[i].length != 0)
                            resellerImportErrorsList.AddItem(strs[i]);
                    }
                    if(resellerImportErrorsList.GetItemCount() == 0)
                        resellerImportErrorsList.SetVisible(false);
                }" />
            </dev:ASPxUploadControl>
            
            <table>
                <tr>
                    <td width="100%" align="right">
                        <dev:ASPxButton AutoPostBack="false" ID="btnUploadImport" runat="server" Text="Importovat" meta:resourcekey="UploadImportButton">
                            <ClientSideEvents Click="function(s, e) { 
                                resellerImportStatusLabel.SetText('');
                                resellerImportErrorsList.ClearItems();
                                resellerImportErrorsList.SetVisible(false);
                                resellerImportStatusLabel.SetVisible(false);
                                e.processOnServer = false;
                                resellersUploader.UploadFile();
                                
                            }" />
                        </dev:ASPxButton>
                    </td>
                    <td align="right">
                        <dev:ASPxButton AutoPostBack="false" ID="btnClose" runat="server" Text="Zavřít">
                        </dev:ASPxButton>
                    </td>
                </tr>
            </table>
            <div style="padding-top:10px; padding-bottom:10px;">
                <dev:ASPxLabel ID="lblImportStatus" Width="100%" EnableClientSideAPI="true"
                    ClientInstanceName="resellerImportStatusLabel" runat="server" Text="" ForeColor='#00CC00' ClientVisible="false">
                </dev:ASPxLabel>
                <dev:ASPxListBox ID="listErrors" runat="server" Width="100%" EnableCallbackMode="true" ClientVisible="false"
                    ClientInstanceName="resellerImportErrorsList" >
                </dev:ASPxListBox>
            </div>
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>