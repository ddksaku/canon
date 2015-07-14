<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminPricesImportsLog.ascx.cs" Inherits="CanonWebApp.Controls.AdminPricesImportsLog" %>

<dev:ASPxCallbackPanel ID="CallbackPricesImportsLogPanel" runat="server">
    <PanelCollection>
        <dev:PanelContent>
            <table cellpadding="5" cellspacing="0" border="0px" width="100%">
                <tr>
                    <td width="100%" align="center">
                        <dev:ASPxGridView ID="gridPricesImportsLog" runat="server" KeyFieldName="ID" Width="100%"
                            AutoGenerateColumns="false" ClientInstanceName="gridPricesImportsLog">
                            <Columns>
                                <dev:GridViewDataTextColumn Name="colDate" Caption="Datum" FieldName="DateImported" VisibleIndex="0" Width="50%">
                                    <PropertiesTextEdit DisplayFormatString="dd.MM.yyyy" />
                                </dev:GridViewDataTextColumn>
                                <dev:GridViewDataTextColumn Name="colUser" Caption="Uživatel" FieldName="User.FullName" VisibleIndex="1" Width="50%">
                                </dev:GridViewDataTextColumn>
                            </Columns>
                        </dev:ASPxGridView>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="center">
                        <dev:ASPxButton AutoPostBack="false" runat="server" ID="btnClose" Text="Zavřít">
                            <ClientSideEvents Click="function(s, e) {
	                            window.parent.pricesImportsLogPopup.Hide();
                             }" />
                        </dev:ASPxButton>
                    </td>
                </tr>
            </table>
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>