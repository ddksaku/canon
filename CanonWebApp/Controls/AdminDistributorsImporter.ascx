<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminDistributorsImporter.ascx.cs" Inherits="CanonWebApp.Controls.AdminDistributorsImporter" %>

<script language="javascript" type="text/javascript">
    function SetVisibility() {
        var uploadcount = document.getElementById('hidUploadCount').value;

        var table = document.getElementById('tblUploadControl');
        for (i = 1; i <= 5; i++) {
            table.rows[i].style.display = "none";
        }

        for (i = 1; i <= uploadcount; i++) {
            table.rows[i].style.display = "";
        }
    }

    function UploadFiles() {
        var uploadcount = document.getElementById('hidUploadCount').value;

        if (tblUploadControlRow1.style.display == "")
            distributorsUploader1.UploadFile();
        if (tblUploadControlRow2.style.display == "")
            distributorsUploader2.UploadFile();
        if (tblUploadControlRow3.style.display == "")
            distributorsUploader3.UploadFile();
        if (tblUploadControlRow4.style.display == "")
            distributorsUploader4.UploadFile();
        if (tblUploadControlRow5.style.display == "")
            distributorsUploader5.UploadFile();

        clbPanelDistributorsGrid.PerformCallback('RefreshAfterImport')
    }

    function addUpload() {
        
        var uploadcount = document.getElementById('hidUploadCount').value;
        if (uploadcount == 5) {
            return;
        }

        var rowCount = parseInt(uploadcount) + 1;
        var table = document.getElementById('tblUploadControl');
        table.rows[rowCount].style.display = "";

        document.getElementById('hidUploadCount').value = rowCount;
        hlAddFileInput.SetVisible(rowCount < 5);
    }

    function removeUpload(rowNumber) {
        var table = document.getElementById('tblUploadControl');
        var row = document.getElementById('tblUploadControlRow' + rowNumber);
        table.moveRow(row.rowIndex, 5);
        row.style.display = 'none';


        var uploadcount = document.getElementById('hidUploadCount').value;
        var rowCount = parseInt(uploadcount) - 1;
        document.getElementById('hidUploadCount').value = rowCount;
        hlAddFileInput.SetVisible(rowCount < 5);


        var cboDistributors = eval('cboDistributors' + rowNumber);
        cboDistributors.SetSelectedItem(null);

        var distributorsUploader = eval('distributorsUploader' + rowNumber);
        distributorsUploader.ClearText();
        distributorsUploader.ClearInputText();

        var lblFilename = eval('lblFilename' + rowNumber);
        lblFilename.SetVisible(false);

        var lblResult = eval('lblResult' + rowNumber);
        lblResult.SetVisible(false);    
    }

    function OnFileUploadCompleted(number, isValid, result) {
        var lblFilename = eval('lblFilename' + number);
        var distributorsUploader = eval('distributorsUploader' + number);
        var lblResult = eval('lblResult' + number);

        lblFilename.SetText(distributorsUploader.GetText());
        lblFilename.SetVisible(true);
        lblResult.SetText(result);
        lblResult.SetVisible(true);

        if (isValid) {
            lblResult.mainElement.style.color = "#00CC00";
            distributorsUploader.SetVisible(false);
        }
        else {
            lblResult.mainElement.style.color = "#ff0000";
            distributorsUploader.SetVisible(true);
        }
    }

</script>

<dev:ASPxCallbackPanel ID="clbPanelForDistributors" runat="server"
    ClientInstanceName="clbPanelForDistributors" HideContentOnCallback="False" 
    oncallback="clbPanelForDistributors_Callback">
    <PanelCollection>
        <dev2:PanelContent runat="server"></dev2:PanelContent>
    </PanelCollection>

    <ClientSideEvents EndCallback="function(s,e) {
    }" />
</dev:ASPxCallbackPanel>

<dev:ASPxCallbackPanel ID="clbPanelDistributorsImporter" runat="server"
    ClientInstanceName="clbPanelDistributorsImporter" 
    HideContentOnCallback="False" Border-BorderColor="Black"
    Border-BorderWidth="1" Paddings-Padding="5" 
    oncallback="clbPanelDistributorsImporter_Callback">
    
<Border BorderColor="Black" BorderWidth="1px"></Border>

<Paddings Padding="5px"></Paddings>

    <PanelCollection>
        <dev:PanelContent>
            <input id="hidUploadCount" name="hidUploadCount" type="hidden" value="1" />
            <table cellpadding="5" cellspacing="0" width="700px" id="tblUploadControl" enableviewstate="true">
                <tr>
                    <td width="30%">
                        <dev:ASPxLabel ID="lblDistributor" runat="server" Text="Distributor:" Font-Bold="true" />
                    </td>
                    <td width="70%">
                        <dev:ASPxLabel ID="lblUploadControl" runat="server" Text="Importní XLS soubor:" Font-Bold="true" Width="200px" />
                    </td>
                </tr>
                <tr id="tblUploadControlRow1">
                    <td valign="top">
                        <dev:ASPxComboBox ID="cboDistributors1" name="cboDistributors1" ClientInstanceName="cboDistributors1"
                            runat="server" ValueField="ID" TextField="FileAs" EnableClientSideAPI="true"
                            OnValidation="cboDistributors1_Validation" >
                            <ClientSideEvents ValueChanged="function(s,e) {
                                clbPanelForDistributors.PerformCallback('Upload1');
                            }" />
                        </dev:ASPxComboBox>
                    </td>
                    <td>
                        <dev:ASPxUploadControl ID="UploadControl1"
                            ClientInstanceName="distributorsUploader1" runat="server" width="100%"
                            ShowAddRemoveButtons="false" FileInputCount="1"
                            OnFileUploadComplete="UploadControl1_FileUploadComplete">
                            <ValidationSettings MaxFileSize="5242880" MaxFileSizeErrorText="Nahraný soubor je příliš velký" />
                            <ClientSideEvents FileUploadComplete="function(s,e) {
                                OnFileUploadCompleted(1, e.isValid, e.callbackData);
                            }" />
                        </dev:ASPxUploadControl>
                        <dev:ASPxLabel ID="lblFilename1" ClientInstanceName="lblFilename1" runat="server" ClientVisible="false"></dev:ASPxLabel><br />
                        <dev:ASPxLabel ID="lblResult1" ClientInstanceName="lblResult1" runat="server" ClientVisible="false"></dev:ASPxLabel>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr style="display:none;" id="tblUploadControlRow2">
                    <td valign="top">
                        <dev:ASPxComboBox ID="cboDistributors2" ClientInstanceName="cboDistributors2" runat="server" ValueField="ID" TextField="FileAs" ClientVisible="true" >
                            <ClientSideEvents ValueChanged="function(s,e) {
                                clbPanelForDistributors.PerformCallback('Upload2');
                            }" />
                        </dev:ASPxComboBox>
                    </td>
                    <td>
                        <dev:ASPxUploadControl ID="UploadControl2" ClientInstanceName="distributorsUploader2" runat="server" ClientVisible="true" width="100%"
                            ShowAddRemoveButtons="false" FileInputCount="1" OnFileUploadComplete="UploadControl1_FileUploadComplete">
                            <ValidationSettings MaxFileSize="5242880" MaxFileSizeErrorText="Nahraný soubor je příliš velký" />
                            <ClientSideEvents FileUploadComplete="function(s,e) {
                                OnFileUploadCompleted(2, e.isValid, e.callbackData);
                            }" />
                        </dev:ASPxUploadControl>
                        <dev:ASPxLabel ID="lblFilename2" ClientInstanceName="lblFilename2" runat="server" ClientVisible="false"></dev:ASPxLabel><br />
                        <dev:ASPxLabel ID="lblResult2" ClientInstanceName="lblResult2" runat="server" ClientVisible="false" ></dev:ASPxLabel>
                    </td>
                    <td valign="top">
                        <dev:ASPxHyperLink ID="hlRemoveFileInput2" ClientInstanceName="hlRemoveFileInput2"
                            runat="server" Text="Odstranit" Font-Underline="true" ClientVisible="true" >
                            <ClientSideEvents Click="function(s,e) {
                                removeUpload('2');
                            }" />
                        </dev:ASPxHyperLink>
                    </td>
                </tr>
                <tr style="display:none;" id="tblUploadControlRow3">
                    <td valign="top">
                        <dev:ASPxComboBox ID="cboDistributors3" ClientInstanceName="cboDistributors3" runat="server" ValueField="ID" TextField="FileAs" >
                            <ClientSideEvents ValueChanged="function(s,e) {
                                clbPanelForDistributors.PerformCallback('Upload3');
                            }" />
                        </dev:ASPxComboBox>
                    </td>
                    <td>
                        <dev:ASPxUploadControl ID="UploadControl3" ClientInstanceName="distributorsUploader3" runat="server" width="100%"
                            ShowAddRemoveButtons="false" FileInputCount="1" OnFileUploadComplete="UploadControl1_FileUploadComplete">
                            <ValidationSettings MaxFileSize="5242880" MaxFileSizeErrorText="Nahraný soubor je příliš velký" />
                            <ClientSideEvents FileUploadComplete="function(s,e) {
                                OnFileUploadCompleted(3, e.isValid, e.callbackData);
                            }" />
                        </dev:ASPxUploadControl>
                        <dev:ASPxLabel ID="lblFilename3" ClientInstanceName="lblFilename3" runat="server" ClientVisible="false"></dev:ASPxLabel><br />
                        <dev:ASPxLabel ID="lblResult3" ClientInstanceName="lblResult3" runat="server" ClientVisible="false" ></dev:ASPxLabel>
                    </td>
                    <td valign="top">
                        <dev:ASPxHyperLink ID="ASPxHyperLink1" ClientInstanceName="hlRemoveFileInput2"
                            runat="server" Text="Odstranit" Font-Underline="true" ClientVisible="true" >
                            <ClientSideEvents Click="function(s,e) {
                                removeUpload('3');
                            }" />
                        </dev:ASPxHyperLink>
                    </td>
                </tr>
                <tr style="display:none;" id="tblUploadControlRow4">
                    <td valign="top">
                        <dev:ASPxComboBox ID="cboDistributors4" ClientInstanceName="cboDistributors4" runat="server" ValueField="ID" TextField="FileAs" >
                            <ClientSideEvents ValueChanged="function(s,e) {
                                clbPanelForDistributors.PerformCallback('Upload4');
                            }" />
                        </dev:ASPxComboBox>
                    </td>
                    <td>
                        <dev:ASPxUploadControl ID="UploadControl4" ClientInstanceName="distributorsUploader4" runat="server" width="100%"
                            ShowAddRemoveButtons="false" FileInputCount="1" OnFileUploadComplete="UploadControl1_FileUploadComplete" >
                            <ValidationSettings MaxFileSize="5242880" MaxFileSizeErrorText="Nahraný soubor je příliš velký" />
                            <ClientSideEvents FileUploadComplete="function(s,e) {
                                OnFileUploadCompleted(4, e.isValid, e.callbackData);
                            }" />
                        </dev:ASPxUploadControl>
                        <dev:ASPxLabel ID="lblFilename4" ClientInstanceName="lblFilename4" runat="server" ClientVisible="false"></dev:ASPxLabel><br />
                        <dev:ASPxLabel ID="lblResult4" ClientInstanceName="lblResult4" runat="server" ClientVisible="false" ></dev:ASPxLabel>
                    </td>
                    <td valign="top">
                        <dev:ASPxHyperLink ID="ASPxHyperLink2" ClientInstanceName="hlRemoveFileInput2"
                            runat="server" Text="Odstranit" Font-Underline="true" ClientVisible="true" >
                            <ClientSideEvents Click="function(s,e) {
                                removeUpload('4');
                            }" />
                        </dev:ASPxHyperLink>
                    </td>
                </tr>
                <tr style="display:none;" id="tblUploadControlRow5">
                    <td valign="top">
                        <dev:ASPxComboBox ID="cboDistributors5" ClientInstanceName="cboDistributors5" runat="server" ValueField="ID" TextField="FileAs" >
                            <ClientSideEvents ValueChanged="function(s,e) {
                                clbPanelForDistributors.PerformCallback('Upload5');
                            }" />
                        </dev:ASPxComboBox>
                    </td>
                    <td>
                        <dev:ASPxUploadControl ID="UploadControl5" ClientInstanceName="distributorsUploader5" runat="server" width="100%"
                            ShowAddRemoveButtons="false" FileInputCount="1" OnFileUploadComplete="UploadControl1_FileUploadComplete" >
                            <ValidationSettings MaxFileSize="5242880" MaxFileSizeErrorText="Nahraný soubor je příliš velký" />
                            <ClientSideEvents FileUploadComplete="function(s,e) {
                                OnFileUploadCompleted(5, e.isValid, e.callbackData);
                            }" />
                        </dev:ASPxUploadControl>
                        <dev:ASPxLabel ID="lblFilename5" ClientInstanceName="lblFilename5" runat="server" ClientVisible="false"></dev:ASPxLabel><br />
                        <dev:ASPxLabel ID="lblResult5" ClientInstanceName="lblResult5" runat="server" ClientVisible="false" ></dev:ASPxLabel>
                    </td>
                    <td valign="top">
                        <dev:ASPxHyperLink ID="ASPxHyperLink3" ClientInstanceName="hlRemoveFileInput2"
                            runat="server" Text="Odstranit" Font-Underline="true" ClientVisible="true" >
                            <ClientSideEvents Click="function(s,e) {
                                removeUpload('5');
                            }" />
                        </dev:ASPxHyperLink>
                    </td>
                </tr>
                <tr>
                    <td>
                        <dev:ASPxHyperLink ID="hlAddFileInput" ClientInstanceName="hlAddFileInput" runat="server" Text="Přidat další soubor" Font-Underline="true">
                            <ClientSideEvents Click="function(s,e) {
                                addUpload();
                            }" />
                        </dev:ASPxHyperLink>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="right" colspan="2">
                       <dev:ASPxButton ID="ASPxButton1" AutoPostBack="false" runat="server" Text="Provést import" OnClick="btnImport_Click">
                            <ClientSideEvents Click="function(s,e) {
                               e.processOnServer = false;
                               UploadFiles();
                            }" />
                        </dev:ASPxButton>
                            
                    </td>
                </tr>
            </table>
            
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>