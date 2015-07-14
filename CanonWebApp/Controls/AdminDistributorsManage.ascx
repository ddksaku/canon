<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminDistributorsManage.ascx.cs" Inherits="CanonWebApp.Controls.AdminDistributorsManage" %>

<script language="javascript" type="text/javascript">
    function OnEditClick(visibleIndex) {
        gridDistributorsManage.StartEditRow(visibleIndex);
    }
    function OnDeleteClick(distributorName, visibleIndex) {
        if (confirm('Opravdu si přejete odebrat distributora ' + distributorName + '?'))
            gridDistributorsManage.DeleteRow(visibleIndex);
    }

    function AfterCallback(sender) {
        ShowDeleteDistributorResult(sender);
        clbPanelDistributorsImport.PerformCallback('RefreshDistributors');
    }

    function ShowDeleteDistributorResult(sender) {
        if (sender.cpDisDeleteError != null && sender.cpDisDeleteError != '') {
            lblRemoveDistributorError.SetText(sender.cpDisDeleteError);
            popupRemoveDistributorError.Show();
        }

        sender.cpDisDeleteError = '';
    }
</script>

<dev:ASPxCallbackPanel ID="clbPanelDistributorsManage" runat="server" Width="100%"
    ClientInstanceName="clbPanelDistributorsManage" 
    HideContentOnCallback="False" 
    oncallback="clbPanelDistributorsManage_Callback">
        <ClientSideEvents  EndCallback="function(s,e) {
            AfterCallback(clbPanelDistributorsManage);
        }" />
    <PanelCollection>
        <dev:PanelContent>
            <table cellpadding="5" cellspacing="0" border="0px" width="100%">
                <tr>
                    <td>
                        <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" Text="Správa distributorů">
                        </dev:ASPxLabel>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="right">
                        <dev:ASPxButton ID="btnAddNewItem" runat="server" AutoPostBack="false"  Text="Přidat distributora">
                            <ClientSideEvents Click="function(s,e) {
                                e.processOnServer = false;
                                gridDistributorsManage.AddNewRow();
                            }" />
                        </dev:ASPxButton>
                    </td>
                </tr>
                <tr>
                    <td>
                        <dev:ASPxGridView ID="gridDistributorsManage" runat="server" Width="100%"
                            KeyFieldName="ID" AutoGenerateColumns="false" 
                            ClientInstanceName="gridDistributorsManage" 
                            OnBeforeGetCallbackResult="gridDistributorsManage_BeforeGetCallbackResult" 
                            OnHtmlEditFormCreated="gridDistributorsManage_HtmlEditFormCreated" 
                            OnCancelRowEditing="gridDistributorsManage_CancelRowEditing" 
                            OnRowDeleting="gridDistributorsManage_RowDeleting" 
                            OnRowInserting="gridDistributorsManage_RowInserting" 
                            OnRowUpdating="gridDistributorsManage_RowUpdating" 
                            OnRowValidating="gridDistributorsManage_RowValidating" 
                            OnStartRowEditing="gridDistributorsManage_StartRowEditing" 
                            OnHtmlRowCreated="gridDistributorsManage_HtmlRowCreated">
                            <ClientSideEvents EndCallback="function(s,e) {
                                AfterCallback(gridDistributorsManage);
                            }" />
                            <Columns>
                                <dev:GridViewCommandColumn ShowSelectCheckbox="true" VisibleIndex="0">
                                    <HeaderTemplate>
                                        <input type="checkbox" onclick="gridDistributorsManage.SelecteAllRowsOnPage(this.checked);" style="vertical-align:middle;" />
                                    </HeaderTemplate>
                                </dev:GridViewCommandColumn>
                                <dev:GridViewDataTextColumn Name="colDistributorName" Caption="Název distributora" FieldName="FileAs" Width="20%" VisibleIndex="1">
                                </dev:GridViewDataTextColumn>
                                <dev:GridViewDataTextColumn Name="colDistributorTyp" Caption="Typ" FieldName="DistributorType.FileAs" Width="20%" VisibleIndex="2">
                                </dev:GridViewDataTextColumn>
                                <dev:GridViewDataTextColumn Name="colNote" Caption="Poznámka" FieldName="Note" Width="40%" VisibleIndex="3">
                                </dev:GridViewDataTextColumn>
                                <dev:GridViewCommandColumn Name="colCommands" Caption="Akce" Width="20%" VisibleIndex="4">
                                    <EditButton Visible="true"></EditButton>
                                    <DeleteButton Visible="true"></DeleteButton>
                                </dev:GridViewCommandColumn>
                                <%--<dev:GridViewDataColumn Name="colCommands" Caption="Akce" Width="20%" VisibleIndex="5">
                                    <DataItemTemplate>
                                        <dev:ASPxHyperLink ID="btnEdit" runat="server" Text="Edit" NavigateUrl="javascript:void(0);">
                                        </dev:ASPxHyperLink>
                                        <dev:ASPxHyperLink ID="btnDelete" Text="Delete" runat="server" NavigateUrl="javascript:void(0);" >
                                        </dev:ASPxHyperLink>
                                    </DataItemTemplate>
                                </dev:GridViewDataColumn>--%>
                            </Columns>                        
                            
                            <SettingsBehavior AllowMultiSelection="true" ConfirmDelete="true" />
                            <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="400px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
                            <SettingsText ConfirmDelete="Opravdu si přejete odebrat distributora?" />
                            
                            <Templates>
                                <EditForm>
                                    <div style="padding:4px 4px 4px 4px;">
                                        <table width="100%" cellpadding="0" cellspacing="5" border="0px">
                                            <tr>
                                                <td width="40%" colspan="2">
                                                    <dev:ASPxLabel ID="lblInformation" runat="server" Font-Bold="true" Text="Základní informace:">
                                                    </dev:ASPxLabel>
                                                </td>
                                            </tr>
                                            <tr />
                                            <tr>
                                                <td width="20%">
                                                    <dev:ASPxLabel ID="lblDistributorName" runat="server" Font-Bold="true" Text="Název:">
                                                    </dev:ASPxLabel>
                                                </td>
                                                <td  width="30%">
                                                    <dev:ASPxTextBox ID="txtDistributorName" runat="server" Text='<%# Bind("FileAs") %>'>
                                                    </dev:ASPxTextBox>
                                                </td>
                                                <td width="50%">
                                                    <dev:ASPxCheckBox ID="chkShowInImports" runat="server" Text="Zobrazovat v importech" Value='<%# Bind("ShowInImports") %>'>
                                                    </dev:ASPxCheckBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="20%">
                                                    <dev:ASPxLabel ID="lblType" runat="server" Font-Bold="true" Text="Typ:">
                                                    </dev:ASPxLabel>
                                                </td>
                                                <td width="30%">
                                                    <dev:ASPxComboBox ID="cboType" runat="server" Width="120px"
                                                        ValueField="ID" TextField="FileAs" OnInit="cboType_Init">
                                                    </dev:ASPxComboBox>
                                                </td>
                                                <td width="50%">
                                                    <dev:ASPxCheckBox ID="chkShowInReports" runat="server" Text="Zobrazovat v reportech" Value='<%# Bind("ShowInReports") %>'>
                                                    </dev:ASPxCheckBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="20%" valign="top">
                                                    <dev:ASPxLabel ID="lblNote" runat="server" Font-Bold="true" Text="Poznámka">
                                                    </dev:ASPxLabel>
                                                </td>
                                                <td width="30%" rowspan="2">
                                                    <dev:ASPxMemo ID="txtNote" runat="server" Text='<%# Bind("Note") %>' Height="80px" Width="100%">
                                                    </dev:ASPxMemo>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div style="text-align:right; padding:2px 2px 2px 2px">
                                          <dev:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dev:ASPxGridViewTemplateReplacement>
                                          <dev:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dev:ASPxGridViewTemplateReplacement>
                                     </div>
                                </EditForm>
                            </Templates>
                            
                        </dev:ASPxGridView>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <dev:ASPxButton ID="btnDeleteSelected" AutoPostBack="false" runat="server" Text="Smazat označené">
                            <ClientSideEvents Click="function(s, e) {
                                 e.processOnServer = false;
                                 if (confirm('Opravdu si přejete odebrat vybrané distributory?'))
                                 {
                                    clbPanelDistributorsManage.PerformCallback('');
                                 }          
                            }" />    
                        </dev:ASPxButton>
                        
                        <dev:ASPxPopupControl ID="popupRemoveDistributorError" runat="server"
                             HeaderText = "Smazání distributora" ClientInstanceName = "popupRemoveDistributorError" 
                             Width="300px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
                             EnableClientSideAPI="true" Modal="true">
                             <ContentCollection>
                                <dev:PopupControlContentControl>
                                    <table align="center">
                                        <tr>
                                            <td align="center">
                                                <b>
                                                    <dev:ASPxLabel ID="lblRemoveDistributorError" ClientInstanceName="lblRemoveDistributorError" runat="server">
                                                    </dev:ASPxLabel>
                                                </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <dev:ASPxButton ID="btnCloseRmoveDistributorPopup" runat="server" AutoPostBack="false" Text="Zavřít">
                                                    <ClientSideEvents Click="function(s,e) {
                                                        popupRemoveDistributorError.Hide();
                                                    }" />
                                                </dev:ASPxButton>
                                            </td>
                                        </tr>
                                    </table>
                                </dev:PopupControlContentControl>
                             </ContentCollection>
                        </dev:ASPxPopupControl>
                        
                    </td>
                </tr>
            </table>
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>