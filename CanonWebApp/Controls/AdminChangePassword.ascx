<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminChangePassword.ascx.cs" Inherits="CanonWebApp.Controls.AdminChangePassword" %>

<dev:ASPxCallbackPanel ID="CallbackChangePasswordPanel" HideContentOnCallback="False"
    ClientInstanceName="clbPanelChangePassword" runat="server" 
    oncallback="CallbackChangePasswordPanel_Callback">
    <ClientSideEvents EndCallback="function(s,e) {
        if (clbPanelChangePassword.cpChangePasswordError != null && clbPanelChangePassword.cpChangePasswordError != '') {
            lblError.SetText(clbPanelChangePassword.cpChangePasswordError);
            lblError.SetVisible(true);
        }

        clbPanelChangePassword.cpChangePasswordError = '';
    }" />
    
    <PanelCollection>
        <dev:PanelContent>
            <table cellpadding="5" cellspacing="5" border="0px" width="300px">
                <tr>
                    <td width="150px" align="left">
                        <dev:ASPxLabel runat="server" ID="lblOldPassword" Text="Původní heslo:"></dev:ASPxLabel>
                    </td>
                    <td width="150px" align="left">
                        <dev:ASPxTextBox runat="server" ID="txtOldPassword" Password="true">
                        </dev:ASPxTextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <dev:ASPxLabel runat="server" ID="lblNewPassword" Text="Nové heslo:"></dev:ASPxLabel>
                    </td>
                    <td align="left">
                        <dev:ASPxTextBox runat="server" ID="txtNewPassword" Password="true">
                        </dev:ASPxTextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <dev:ASPxLabel runat="server" ID="lblNewPasswordConfirm" Text="Nové heslo (Kontrola):"></dev:ASPxLabel>
                    </td>
                    <td align="left">
                        <dev:ASPxTextBox runat="server" ID="txtNewPasswordConfirm" Password="true">
                        </dev:ASPxTextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" colspan="2">
                        <dev:ASPxLabel ID="lblError" ClientInstanceName="lblError" runat="server" ClientVisible="false"></dev:ASPxLabel>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <dev:ASPxButton runat="server" ID="btnChangePassword" ClientInstanceName="btnChangePassword" Text="Změnit heslo" AutoPostBack="false">
                            <ClientSideEvents Click="function(s,e) {
                                clbPanelChangePassword.PerformCallback('ChangePassword');
                            }" />
                        </dev:ASPxButton>
                    </td>
                    <td align="left">
                        <dev:ASPxButton runat="server" ID="btnCancel" Text="Zrušit" AutoPostBack="false">
                            <ClientSideEvents Click="function(s,e) {
                                lblError.SetVisible(false);
                                window.parent.popupChangePassword.Hide();
                            }" />
                        </dev:ASPxButton>
                        
                    </td>
                </tr>
            </table>
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>