<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminGroups.ascx.cs" Inherits="CanonWebApp.Controls.AdminGroups" %>

<script language="javascript" type="text/javascript">
    function ShowDeleteResult(sender) {
        if (sender.cpPGDeleteError != null && sender.cpPGDeleteError != '') {
            lblRemoveProductGroupError.SetText(sender.cpPGDeleteError);
            popupRemoveProductGroupError.Show();
        }

        if (sender.cpRGDeleteError != null && sender.cpRGDeleteError != '') {
            lblRemoveResellerGroupError.SetText(sender.cpRGDeleteError);
            popupRemoveResellerGroupError.Show();
        }

        sender.cpPGDeleteError = '';
        sender.cpRGDeleteError = '';
    }
</script>

<dev:ASPxCallbackPanel ID="clbPanelGroups" runat="server"
     ClientInstanceName="clbPanelGroups" HideContentOnCallback="False" 
    oncustomjsproperties="clbPanelGroups_CustomJSProperties" 
    oncallback="clbPanelGroups_Callback">
    <ClientSideEvents EndCallback="function(s,e) {
        ShowDeleteResult(clbPanelGroups);
        }" />
     <PanelCollection>
        <dev:PanelContent>
            <table cellpadding="5" cellspacing="0" border="0px" width="100%">
                <tr>
                    <td width="50%">
                        <table width="90%">
                            <tr>
                                <td>
                                    <dev:ASPxLabel ID="lblSubTitle1" runat="server" Text="Produktové skupiny" SkinID="AdminPageTitle">
                                    </dev:ASPxLabel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <dev:ASPxButton ID="btnAddProductGroup" runat="server" AutoPostBack="false" Text="přidat skupinu">
                                        <ClientSideEvents Click="function(s,e) {
                                            e.processOnServer = false;
                                            gridProductGroups.AddNewRow();
                                        }" />
                                    </dev:ASPxButton>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <dev:ASPxGridView ID="gridProductGroups" runat="server" Width="100%"
                                        ClientInstanceName="gridProductGroups" KeyFieldName="ID" 
                                        AutoGenerateColumns="false" 
                                        OnBeforeGetCallbackResult="gridProductGroups_BeforeGetCallbackResult" 
                                        OnCancelRowEditing="gridProductGroups_CancelRowEditing" 
                                        OnHtmlEditFormCreated="gridProductGroups_HtmlEditFormCreated" 
                                        OnHtmlRowCreated="gridProductGroups_HtmlRowCreated" 
                                        OnRowDeleting="gridProductGroups_RowDeleting" 
                                        OnRowInserting="gridProductGroups_RowInserting" 
                                        OnRowUpdating="gridProductGroups_RowUpdating" 
                                        OnRowValidating="gridProductGroups_RowValidating" 
                                        OnStartRowEditing="gridProductGroups_StartRowEditing" >
                                        <ClientSideEvents EndCallback="function(s,e) {
                                            ShowDeleteResult(gridProductGroups);
                                        }" />
                                        <Columns>
                                            <dev:GridViewCommandColumn ShowSelectCheckbox="true" VisibleIndex="0">
                                                <HeaderTemplate>
                                                    <input type="checkbox" onclick="gridProductGroups.SelectAllRowsOnPage(this.checked);" />
                                                </HeaderTemplate>
                                                <HeaderStyle HorizontalAlign="Center">
                                                    <Paddings PaddingTop="1px" PaddingBottom="1px"></Paddings>
                                                </HeaderStyle>
                                            </dev:GridViewCommandColumn>
                                            <dev:GridViewDataTextColumn Name="colName" FieldName="FileAs" Caption="Název" Width="40%" VisibleIndex="1">
                                            </dev:GridViewDataTextColumn>
                                            <dev:GridViewDataTextColumn Name="colCode" FieldName="Code" Caption="Identifikátor" Width="30%" VisibleIndex="2">
                                            </dev:GridViewDataTextColumn>
                                            <dev:GridViewCommandColumn Name="colCommands" Caption="Akce" Width="30%" VisibleIndex="3">
                                                <EditButton Visible="true"></EditButton>
                                                <DeleteButton Visible="true"></DeleteButton>
                                            </dev:GridViewCommandColumn>
                                        </Columns>
                                        
                                        <SettingsBehavior AllowMultiSelection="true" ConfirmDelete="true" />
                                        <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="400px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
                                        <SettingsText ConfirmDelete="Opravdu si přejete odebrat produktovou skupinu?" />
                                        
                                        <Templates>
                                            <EditForm>
                                                <div style="padding:4px 4px 4px 4px;">
                                                    <table width="100%" cellpadding="0" cellspacing="5" border="0px">
                                                        <tr>
                                                            <td width="40%">
                                                                <dev:ASPxLabel ID="lblName" runat="server" Font-Bold="true" Text="Název:">
                                                                </dev:ASPxLabel>
                                                            </td>
                                                            <td width="60%">
                                                                <dev:ASPxTextBox ID="txtName" runat="server" Text='<%# Bind("FileAs") %>'>
                                                                </dev:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="40%">
                                                                <dev:ASPxLabel ID="lblIdentifier" runat="server" Font-Bold="true" Text="Identifikátor:">
                                                                </dev:ASPxLabel>
                                                            </td>
                                                            <td width="60%">
                                                                <dev:ASPxTextBox ID="txtIdentifier" runat="server" Text='<%# Bind("Code") %>'>
                                                                </dev:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div style="text-align:right; padding:2px 2px 2px 2px">
                                                    <dev:ASPxGridViewTemplateReplacement ID="pgUpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dev:ASPxGridViewTemplateReplacement>
                                                    <dev:ASPxGridViewTemplateReplacement ID="pgCancelButton" ReplacementType="EditFormCancelButton" runat="server"></dev:ASPxGridViewTemplateReplacement>
                                                </div>
                                            </EditForm>
                                        </Templates>
                                        
                                    </dev:ASPxGridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <dev:ASPxButton ID="btnRemoveSellectedProductGroups" AutoPostBack="false" runat="server" Text="Smazat označené">
                                        <ClientSideEvents Click="function(s,e) {
                                            e.processOnServer = false;
                                            if (confirm('Opravdu si přejete odebrat vybrané produktové skupiny?'))
                                            {
                                                clbPanelGroups.PerformCallback('RemoveSelectedProductGroups');
                                            }
                                        }" />
                                    </dev:ASPxButton>
                                    
                                    <dev:ASPxPopupControl ID="popupRemoveProductGroupError" runat="server"
                                        HeaderText = "Smazání produktové skupiny" ClientInstanceName="popupRemoveProductGroupError"
                                        width="300px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
                                        EnableClientSideAPI="true" Modal="true">
                                        <ContentCollection>
                                            <dev:PopupControlContentControl>
                                                <table align="center">
                                                    <tr>
                                                        <td align="center">
                                                            <b>
                                                                <dev:ASPxLabel ID="lblRemoveProductGroupError" ClientInstanceName="lblRemoveProductGroupError" runat="server" />
                                                            </b>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <dev:ASPxButton ID="btnCloseRemovePGErrorPopup" runat="server" AutoPostBack="false" Text="Zavřít">
                                                                <ClientSideEvents Click="function(s,e) {
                                                                    popupRemoveProductGroupError.Hide();
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
                    </td>
                    <td width="50%" align="right" valign="top">
                        <table width="90%">
                            <tr>
                                <td align="left">
                                    <dev:ASPxLabel ID="lblSubTitle2" runat="server" Text="Skupiny reselerů" SkinID="AdminPageTitle">
                                    </dev:ASPxLabel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <dev:ASPxButton ID="btnAddResellerGroup" AutoPostBack="false" runat="server" Text="Přidat skupinu">
                                        <ClientSideEvents Click="function(s,e) {
                                            e.processOnServer = false;
                                            gridResellerGroups.AddNewRow();
                                        }" />
                                    </dev:ASPxButton>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <dev:ASPxGridView ID="gridResellerGroups" runat="server" width="100%" 
                                        ClientInstanceName="gridResellerGroups" KeyFieldName="ID" 
                                        AutoGenerateColumns="false" 
                                        OnBeforeGetCallbackResult="gridResellerGroups_BeforeGetCallbackResult" 
                                        OnCancelRowEditing="gridResellerGroups_CancelRowEditing" 
                                        OnHtmlEditFormCreated="gridResellerGroups_HtmlEditFormCreated" 
                                        OnHtmlRowCreated="gridResellerGroups_HtmlRowCreated" 
                                        OnRowDeleting="gridResellerGroups_RowDeleting" 
                                        OnRowInserting="gridResellerGroups_RowInserting" 
                                        OnRowUpdating="gridResellerGroups_RowUpdating" 
                                        OnRowValidating="gridResellerGroups_RowValidating" 
                                        OnStartRowEditing="gridResellerGroups_StartRowEditing">
                                        <ClientSideEvents EndCallback="function(s,e) {
                                            ShowDeleteResult(gridResellerGroups);
                                        }" />
                                        <Columns>
                                            <dev:GridViewCommandColumn ShowSelectCheckbox="true" VisibleIndex="0">
                                                <HeaderTemplate>
                                                    <input type="checkbox" onclick="gridProductGroups.SelectAllRowsOnPage(this.checked);" />
                                                </HeaderTemplate>
                                                <HeaderStyle HorizontalAlign="Center">
                                                    <Paddings PaddingTop="1px" PaddingBottom="1px"></Paddings>
                                                </HeaderStyle>
                                            </dev:GridViewCommandColumn>
                                            <dev:GridViewDataTextColumn Name="colName" FieldName="FileAs" Caption="Název" Width="40%" VisibleIndex="1">
                                            </dev:GridViewDataTextColumn>
                                            <dev:GridViewDataTextColumn Name="colCode" FieldName="Code" Caption="Identifikátor" Width="30%" VisibleIndex="2">
                                            </dev:GridViewDataTextColumn>
                                            <dev:GridViewCommandColumn Name="colCommands" Caption="Akce" Width="30%" VisibleIndex="3">
                                                <EditButton Visible="true"></EditButton>
                                                <DeleteButton Visible="true"></DeleteButton>
                                            </dev:GridViewCommandColumn>
                                        </Columns>
                                        
                                        <SettingsBehavior AllowMultiSelection="true" ConfirmDelete="true" />
                                        <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="400px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
                                        <SettingsText ConfirmDelete="Opravdu si přejete odebrat skupinu reselerů?" />
                                        
                                        <Templates>
                                            <EditForm>
                                                <div style="padding:4px 4px 4px 4px;">
                                                    <table width="100%" cellpadding="0" cellspacing="5" border="0px">
                                                        <tr>
                                                            <td width="40%">
                                                                <dev:ASPxLabel ID="lblName" runat="server" Font-Bold="true" Text="Název:">
                                                                </dev:ASPxLabel>
                                                            </td>
                                                            <td width="60%">
                                                                <dev:ASPxTextBox ID="txtName" runat="server" Text='<%# Bind("FileAs") %>'>
                                                                </dev:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="40%">
                                                                <dev:ASPxLabel ID="lblIdentifier" runat="server" Font-Bold="true" Text="Identifikátor:">
                                                                </dev:ASPxLabel>
                                                            </td>
                                                            <td width="60%">
                                                                <dev:ASPxTextBox ID="txtIdentifier" runat="server" Text='<%# Bind("Code") %>'>
                                                                </dev:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div style="text-align:right; padding:2px 2px 2px 2px">
                                                    <dev:ASPxGridViewTemplateReplacement ID="rgUpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dev:ASPxGridViewTemplateReplacement>
                                                    <dev:ASPxGridViewTemplateReplacement ID="rgCancelButton" ReplacementType="EditFormCancelButton" runat="server"></dev:ASPxGridViewTemplateReplacement>
                                                </div>
                                            </EditForm>
                                        </Templates>
                                        
                                    </dev:ASPxGridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <dev:ASPxButton ID="btnDeleteSelectedResellerGroups" AutoPostBack="false" runat="server" Text="Smazat označené">
                                        <ClientSideEvents Click="function(s,e) {
                                            e.processOnServer = false;
                                            if (confirm('Opravdu si přejete odebrat vybrané skupiny reselerů?'))
                                            {
                                                clbPanelGroups.PerformCallback('RemoveSelectedResellerGroups');
                                            }
                                        }" />
                                    </dev:ASPxButton>
                                    
                                    <dev:ASPxPopupControl ID="popupRemoveResellerGroupError" runat="server"
                                        HeaderText = "Smazání skupiny reselerů" ClientInstanceName="popupRemoveResellerGroupError"
                                        width="300px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
                                        EnableClientSideAPI="true" Modal="true">
                                        <ContentCollection>
                                            <dev:PopupControlContentControl>
                                                <table align="center">
                                                    <tr>
                                                        <td align="center">
                                                            <b>
                                                                <dev:ASPxLabel ID="lblRemoveResellerGroupError" ClientInstanceName="lblRemoveResellerGroupError" runat="server" />
                                                            </b>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <dev:ASPxButton ID="btnCloseRemoveRGPopup" runat="server" AutoPostBack="false" Text="Zavřít">
                                                                <ClientSideEvents Click="function(s,e) {
                                                                    popupRemoveResellerGroupError.Hide();
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
                    </td>
                </tr>
            </table>
        </dev:PanelContent>
     </PanelCollection>
</dev:ASPxCallbackPanel>