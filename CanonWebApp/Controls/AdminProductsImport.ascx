<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminProductsImport.ascx.cs" Inherits="CanonWebApp.Controls.AdminProductsImport" %>

<dev:ASPxCallbackPanel ID="CallbackImportPanel" runat="server" Width="100%">
<PanelCollection>
<dev:PanelContent>
    <dev:ASPxUploadControl ID="UploadControl" Width="100%" ClientInstanceName="uploader" runat="server" OnFileUploadComplete="UploadControl_FileUploadComplete">
    <ValidationSettings MaxFileSize="5242880">
    </ValidationSettings>
    <ClientSideEvents FileUploadComplete="function(s, e) {
    statusLabel.SetText(e.errorText);
    if (e.isValid)
        statusLabel.mainElement.style.color = '#00CC00';
    else
        statusLabel.mainElement.style.color = '#CC0000';
    var strs = e.callbackData.split('||');
    for(i=0; i < strs.length; i++)
        errorsList.AddItem(strs[i]);
    errorsList.SetVisible(true);
    errorsLabel.SetVisible(true);
    }" />
    </dev:ASPxUploadControl>
    <dev:ASPxButton ID="btnUploadImport" runat="server" meta:resourcekey="UploadImportButton">
    <ClientSideEvents Click="function(s, e) { 
        statusLabel.SetText(''); 
        errorsList.ClearItems();
        errorsList.SetVisible(false);
        errorsLabel.SetVisible(false);
        e.processOnServer = false; 
        if (ASPxClientEdit.ValidateGroup('ImportUpload')) { uploader.UploadFile(); }}" />
    </dev:ASPxButton>
    <div style="padding-top:10px;padding-bottom:10px;">
    <dev:ASPxLabel ID="lblImportStatus" Width="100%" EnableClientSideAPI="True" ClientInstanceName="statusLabel" runat="server" Text="">
    </dev:ASPxLabel>
    </div>
    <dev:ASPxLabel ID="lblErrors" Width="100%" EnableClientSideAPI="True" ClientInstanceName="errorsLabel" runat="server" meta:resourcekey="ErrorsLabel">
    </dev:ASPxLabel>
    <dev:ASPxListBox ID="listErrors" runat="server" Width="100%" EnableClientSideAPI="True" ClientInstanceName="errorsList">
    </dev:ASPxListBox>
</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>
