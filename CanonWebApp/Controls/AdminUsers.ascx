<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminUsers.ascx.cs" Inherits="CanonWebApp.Controls.AdminUsers" %>

<%@ Register TagPrefix="Canon" TagName="AdminChangePassword" Src="~/Controls/AdminChangePassword.ascx" %>

<script language="javascript" type="text/javascript">
    var IsFilteredUsers = false;
    var ListBoxStates = '';
    function UpdateSelectedList(listbox) {
        ListBoxStates = '';
        len = listbox.length;
        for (var j = 0; j < len; j++) {
            if (listbox.options[j].selected)
                ListBoxStates += listbox.options[j].value + '=1;';
            else
                ListBoxStates += listbox.options[j].value + '=0;';
        }
        txtHidden.SetText(ListBoxStates);
    }
</script>

<dev:ASPxCallbackPanel ID="clbPanelUsers" runat="server" Width="100%" 
    oncallback="clbPanelUsers_Callback" ClientInstanceName="clbPanelUsers" HideContentOnCallback="False">
<PanelCollection>
<dev:PanelContent>
 <table cellpadding="5" cellspacing="0" border="0px" width="100%">
  <tr><td colspan="4">
    <dev:ASPxLabel ID="lblPageTitle" SkinID="AdminPageTitle" runat="server" Text="Správa uživatelů">
    </dev:ASPxLabel>
  </td></tr>
  <tr><td width="100%" align="right">
     <dev:ASPxButton ID="btnNewItem" runat="server" AutoPostBack="False" Text="Přidat užvatele">
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             gridUsers.AddNewRow();         
        }" />    
    </dev:ASPxButton>
  </td></tr>
  <tr><td colspan="5">
    <dev:ASPxGridView ID="gridUsers" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridUsers" 
          KeyFieldName="UserId" OnRowDeleting="gridUsers_RowDeleting" 
          OnHtmlEditFormCreated="gridUsers_HtmlEditFormCreated" 
          OnBeforeGetCallbackResult="gridUsers_BeforeGetCallbackResult" 
          OnCancelRowEditing="gridUsers_CancelRowEditing" 
          OnRowUpdating="gridUsers_RowUpdating" 
          OnRowInserting="gridUsers_RowInserting" 
          OnRowValidating="gridUsers_RowValidating" 
          OnStartRowEditing="gridUsers_StartRowEditing" 
          OnCustomCallback="gridUsers_CustomCallback">
        <Columns>
            <dev:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0" Width="5%">
                 <HeaderTemplate>
                     <input type="checkbox" onclick="gridUsers.SelectAllRowsOnPage(this.checked);" style="vertical-align:middle;"></input>
                 </HeaderTemplate>
                 <HeaderStyle Paddings-PaddingTop="1" Paddings-PaddingBottom="1" 
                     HorizontalAlign="Center">
<Paddings PaddingTop="1px" PaddingBottom="1px"></Paddings>
                 </HeaderStyle>
            </dev:GridViewCommandColumn>
            <dev:GridViewDataTextColumn Name="colUsername" Caption="Uživatelské jméno" Width="25%" FieldName="UserName" VisibleIndex="1">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colFullName" Caption="Jméno" Width="25%" FieldName="FullName" VisibleIndex="2">
            </dev:GridViewDataTextColumn>
            <dev:GridViewDataTextColumn Name="colEmail" Caption="Email" Width="25%" FieldName="Email" VisibleIndex="3">
            </dev:GridViewDataTextColumn>
            <dev:GridViewCommandColumn Name="colCommands" Width="20%" Caption="Akce">
                <EditButton Visible="True"></EditButton>
                <DeleteButton Visible="True"></DeleteButton>
            </dev:GridViewCommandColumn>
        </Columns>
        
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="10"></SettingsPager>
        <SettingsBehavior AllowMultiSelection="True" ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
        <SettingsEditing Mode="PopupEditForm" PopupEditFormWidth="600px" PopupEditFormHorizontalAlign="Center" PopupEditFormVerticalAlign="Middle" />
        <SettingsText CommandUpdate="Uložit a zpět" CommandCancel="Storno" />
        <SettingsText ConfirmDelete="Opravdu si přejete odebrat uživatele?" />
        
        <Templates>
             <EditForm>
             <div style="padding:4px 4px 3px 4px">
                 <table width="100%" cellpadding="0" cellspacing="4">
                    <tr>
                    <td colspan="2" align="left">
                        <dev:ASPxLabel ID="lblEfGeneralInfo" Text="Základní informace:" TabIndex="-1" runat="server">
                        </dev:ASPxLabel>
                    </td>
                    <td align="left" colspan="2">
                        <dev:ASPxLabel ID="lblEfRights" Text="Práva:" TabIndex="-1" runat="server">
                        </dev:ASPxLabel>
                    </td>
                    </tr>
                    <tr>
                    <td width="15%" align="left">
                        <dev:ASPxLabel ID="lblEfUsername" Text="Uživatelské jméno:" TabIndex="-1" runat="server">
                        </dev:ASPxLabel>
                    </td>
                    <td width="20%" align="right">
                        <dev:ASPxTextBox ID="txtEfUsername" TabIndex="50" runat="server" Width="200px" Text='<%# Bind("UserName") %>'>
                        </dev:ASPxTextBox>
                    </td>
                    <td rowspan="5" align="left" valign="top">
                        <asp:CheckBoxList ID="chblUserRights" Height="200px" TabIndex="56" Width="100%" runat="server" OnInit="chblUserRights_Init"></asp:CheckBoxList>
                    </td>
                    </tr>
                    <tr>
                    <td width="15%" align="left">
                        <dev:ASPxLabel ID="lblEfFullName" Text="Jméno:" TabIndex="-1" runat="server">
                        </dev:ASPxLabel>
                    </td>
                    <td width="20%" align="right">
                        <dev:ASPxTextBox ID="txtEfFullName" TabIndex="51" runat="server" Width="200px" Text='<%# Bind("FullName") %>'>
                        </dev:ASPxTextBox>
                    </td>
                    </tr>
                    <tr>
                    <td width="15%" align="left">
                        <dev:ASPxLabel ID="lblEfEmail" Text="Email:" TabIndex="-1" runat="server">
                        </dev:ASPxLabel>
                    </td>
                    <td width="20%" align="right">
                        <dev:ASPxTextBox ID="txtEfEmail" TabIndex="52" runat="server" Width="200px" Text='<%# Bind("Email") %>'>
                        </dev:ASPxTextBox>
                    </td>
                    </tr>
                    <tr>
                    <td width="15%" align="left">
                        <dev:ASPxLabel ID="lblEfPassword" Text="Heslo:" TabIndex="-1" runat="server">
                        </dev:ASPxLabel>
                    </td>
                    <td width="20%" align="right">
                        <dev:ASPxTextBox ID="txtEfPassword" TabIndex="53" runat="server" Password="True" Width="200px" Text='<%# Bind("Password") %>'>
                        </dev:ASPxTextBox>
                        <dev:ASPxButton ID="btnChangePassword" runat="server" AutoPostBack="false" Text="Změnit heslo">
                            <ClientSideEvents Click="function(s,e) {
                                
                                popupChangePassword.Show();
                            }" />
                        </dev:ASPxButton>
                        
                        <dev:ASPxPopupControl ID="popupChangePassword" ClientInstanceName="popupChangePassword" runat="server"
                            HeaderText="Změnit heslo" Width="300px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
                            EnableClientSideAPI="true" Modal="true" >
                            <ContentCollection>
                                <dev:PopupControlContentControl>
                                    <Canon:AdminChangePassword ID="changePasswordCtrl" runat="server">
                                    </Canon:AdminChangePassword>
                                </dev:PopupControlContentControl>
                            </ContentCollection>
                        </dev:ASPxPopupControl>
                    </td>
                    </tr>
                    <tr>
                    <td colspan="2" align="left">
                        <dev:ASPxCheckBox ID="chkIsActive" Text="Aktivní účet" TabIndex="54" runat="server" Value='<%# Bind("IsActive") %>'>
                        </dev:ASPxCheckBox>
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
  </td></tr>
  <tr><td colspan="5" align="left">
    <dev:ASPxButton ID="btnDeleteSelected" AutoPostBack="False" runat="server" Text="Smazat označené" >
        <ClientSideEvents Click="function(s, e) {
             e.processOnServer = false;
             if (confirm('Opravdu si přejete odebrat vybrané uživatele?'))
             {
                clbPanelUsers.PerformCallback('');
             }          
        }" />    
    </dev:ASPxButton>
  </td></tr>
 </table>
</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>
