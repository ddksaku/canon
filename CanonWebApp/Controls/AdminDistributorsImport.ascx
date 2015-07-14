<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminDistributorsImport.ascx.cs"
    Inherits="CanonWebApp.Controls.AdminDistributorsImport" %>
<%@ Register TagPrefix="Canon" TagName="AdminDistributorsImporter" Src="~/Controls/AdminDistributorsImporter.ascx" %>
<style type="text/css">
    .style1
    {
        height: 24px;
    }
</style>

<script language="javascript" type="text/javascript">
    function OnRemoveDistributorClick(element, keyvalue) {
        if(confirm('Opravdu si přejete odstranit distributora?'))
        {
            gridDistributors.PerformCallback('RemoveDistributor' + keyvalue);
        }
    }
</script>

<dev:ASPxCallbackPanel ID="clbPanelDistributorsImport" runat="server" ClientInstanceName="clbPanelDistributorsImport"
    HideContentOnCallback="False" OnCallback="clbPanelDistributorsImport_Callback">
    <PanelCollection>
        <dev:PanelContent>
            <table width="100%" cellpadding="5" cellspacing="0" border="0px">
                <tr>
                    <td colspan="6">
                        <dev:ASPxLabel ID="lblPageTitle" runat="server" Text="Import dat od distributorů"
                            SkinID="AdminPageTitle">
                        </dev:ASPxLabel>
                    </td>
                </tr>
                <tr>
                    <td width="100%">
                        <Canon:AdminDistributorsImporter ID="importControl" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        <dev:ASPxLabel ID="lblImportHistory" Text="Historie importů od distributorů" runat="server"
                            Font-Bold="true">
                        </dev:ASPxLabel>
                    </td>
                </tr>
                <tr>
                    <td width="100%">
                    </td>
                </tr>
            </table>
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>
<dev:ASPxCallbackPanel ID="clbPanelDistributorsGrid" runat="server" ClientInstanceName="clbPanelDistributorsGrid"
    HideContentOnCallback="False" 
    oncallback="clbPanelDistributorsGrid_Callback">
    <PanelCollection>
        <dev:PanelContent>
            <dev:ASPxGridView ID="gridDistributors" KeyFieldName="ID" runat="server" Width="100%"  SettingsPager-PageSize="50"
                AutoGenerateColumns="false" ClientInstanceName="gridDistributors" OnCustomCallback="gridDistributors_CustomCallback"
                OnHtmlRowCreated="gridDistributors_HtmlRowCreated">
                <Columns>
                    <dev:GridViewDataTextColumn Name="colDateDistributor" Caption="Datum" Width="12%"
                        FieldName="DateImported" VisibleIndex="0">
                        <PropertiesTextEdit DisplayFormatString="dd.MM.yyyy">
                        </PropertiesTextEdit>
                    </dev:GridViewDataTextColumn>
                    <dev:GridViewDataTextColumn Name="colDistributorName" Caption="Distributor" Width="25%"
                        FieldName="Distributor.FileAs" VisibleIndex="1">
                    </dev:GridViewDataTextColumn>
                    <dev:GridViewDataTextColumn Name="colFileName" Caption="Soubor" Width="15%" FieldName="FileName"
                        VisibleIndex="2">
                    </dev:GridViewDataTextColumn>
                    <dev:GridViewDataTextColumn Name="colDateFrom" Caption="Data od" Width="12%" FieldName="DateFrom"
                        VisibleIndex="3">
                        <PropertiesTextEdit DisplayFormatString="dd.MM.yyyy">
                        </PropertiesTextEdit>
                    </dev:GridViewDataTextColumn>
                    <dev:GridViewDataTextColumn Name="colDateTo" Caption="Data do" Width="12%" FieldName="DateTo"
                        VisibleIndex="4">
                        <PropertiesTextEdit DisplayFormatString="dd.MM.yyyy">
                        </PropertiesTextEdit>
                    </dev:GridViewDataTextColumn>
                    <dev:GridViewDataTextColumn Name="colImporter" Caption="Importoval" Width="12%" FieldName="User.FullName"
                        VisibleIndex="5">
                    </dev:GridViewDataTextColumn>
                    <dev:GridViewDataColumn Name="colAction" Caption="Odstranit" Width="12%" VisibleIndex="6">
                        <DataItemTemplate>
                            <dev:ASPxHyperLink ID="hlRemoveDistributor" Text="Odstranit" runat="server" Font-Underline="true"
                                Visible='<%# !(bool)Eval("IsDeleted") %>'>
                            </dev:ASPxHyperLink>
                            <dev:ASPxLabel ID="lblRemoved" runat="server" Text="Odstraněno" ForeColor="GrayText"
                                Visible='<%# Eval("IsDeleted") %>'>
                            </dev:ASPxLabel>
                        </DataItemTemplate>
                    </dev:GridViewDataColumn>
                </Columns>
                <Settings ShowFilterRow="true" />
            </dev:ASPxGridView>
        </dev:PanelContent>
    </PanelCollection>
</dev:ASPxCallbackPanel>
