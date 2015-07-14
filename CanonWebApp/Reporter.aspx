<%@ Page Language="C#" Theme="Default" EnableEventValidation="false" AutoEventWireup="true"
    CodeBehind="Reporter.aspx.cs" Inherits="CanonWebApp.Reporter" MasterPageFile="~/Main.Master" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v8.3.Export, Version=8.3.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView.Export" TagPrefix="dxwgv" %>

<%@ Register Src="Controls/AjaxTest.ascx" TagName="AjaxTest" TagPrefix="uc2" %>
<%@ Register Src="Controls/CanonCalendar.ascx" TagName="CanonCalendar" TagPrefix="uc1" %>
<%@ Register Src="Controls/SearchBox.ascx" TagName="SearchBox" TagPrefix="uc3" %>
<asp:Content runat="server" ContentPlaceHolderID="Content">
    <link href="App_Themes/Default/Reports.css" rel="stylesheet" type="text/css" />
    <script src="JavaScript/Portamenta_Special.js" type="text/javascript"></script>
    <script src="JavaScript/Portamenta_Core.js" type="text/javascript"></script>
    <script src="JavaScript/Memos_SearchBox.js" type="text/javascript"></script>
    <script src="ListBoxData.js" type="text/javascript"></script>

<asp:Panel runat="server" ID="reportForm">
    <div id="report">
        <div id="x">
        </div>
        <asp:Label runat="server" ID="lblText"></asp:Label>
        <table class="reportHeader">
            <tr>
                <td>
                </td>
                <th>
                    Od:
                </th>
                <td>
                    <uc1:CanonCalendar ID="ccDateFrom" runat="server" />
                </td>
                <th>
                    Do:
                </th>
                <td>
                    <uc1:CanonCalendar ID="ccDateTo" runat="server" />
                </td>
                <td>
                    <asp:LinkButton runat="server" ID="btnGenerateExcel" Text="Data pro pivot" OnClick="btnGenerateExcel_Click" />
                </td>
                <th class="rbl">
                    Hodnoty zobrazit jako:
                </th>
                <td>
                    <asp:RadioButtonList runat="server" ID="rblValueDisplayMode" CssClass="rbl" RepeatLayout="Table" RepeatColumns="3"  >
                    <asp:ListItem Selected="True" Value="1" Text="Absolutní hodnoty"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Procenta"></asp:ListItem>
                    <asp:ListItem Value="3" Text="Pøírùstky"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
        <div class='reportPanel' id="reportPanel">
            <a class="toggle" onclick="return TogglePanel('<%= ID + "_form" %>')" href="#"></a>
            <div class="form" id='<%= ID + "_form" %>'>
                <div class="left">
                    <h4>
                        Svislá osa</h4>
                    <h5>
                        Sledovaná hodnota</h5>
                    <asp:DropDownList ID="ddlYValueType" runat="server">
                        <asp:ListItem Value="4" Selected="True">Obrat</asp:ListItem>
                        <asp:ListItem Value="1">Prodan&#253;ch kusù</asp:ListItem>
                    </asp:DropDownList>
                    <h5>
                        Omezit pouze na</h5>
                    <asp:DropDownList ID="ddlYFilterType" runat="server">
                        <asp:ListItem Selected="True" Value="-">Neomezovat</asp:ListItem>
                        <asp:ListItem Value="VAD">Distributora</asp:ListItem>
                        <asp:ListItem Value="ProductGroupDesc">Produktovou skupinu</asp:ListItem>
                        <asp:ListItem Value="DistributorTypeJoined">Typ distributorù</asp:ListItem>
                        <asp:ListItem Value="ReselerJoined">Reselera</asp:ListItem>
                        <asp:ListItem Value="ReselerGroupJoined">Skupinu reselerù</asp:ListItem>
                        <asp:ListItem Value="ReselerCountryJoined">Zemi reselera</asp:ListItem>
                        <asp:ListItem Value="ProductJoined">Produkt</asp:ListItem>
                    </asp:DropDownList>
                    <h5>
                        Zpøesnìní omezení</h5>
                    <asp:Panel runat="server" ID="pnlYFilter">
                    </asp:Panel>
                    <h5>
                        Seskupovat dle</h5>
                    <asp:DropDownList ID="ddlYGroupBy" runat="server" >
                        <asp:ListItem Selected="True" Value="VAD">Distributorù</asp:ListItem>
                        <asp:ListItem Value="ProductGroupDesc">Produktov&#253;ch skupin</asp:ListItem>
                        <asp:ListItem Value="DistributorTypeJoined">Typu distributorù</asp:ListItem>
                        <asp:ListItem Value="ReselerGroupJoined">Skupin reselerù</asp:ListItem>
                        <asp:ListItem Value="ReselerJoined">Reselerù</asp:ListItem>
                        <asp:ListItem Value="ProductJoined">Produktù</asp:ListItem>
                    </asp:DropDownList>
                    <h5>
                        Zobrazené øady</h5>
                    <asp:ListBox ID="lbYHaving" runat="server" SelectionMode="Multiple" Rows="8"></asp:ListBox>
                    <br />
                    <br />
                    <asp:CheckBox ID="chbShowSum" runat="server" Text="Zobrazovat jen sumu" /><br />
                    <asp:CheckBox ID="chbTop" ValidationGroup="topCount" runat="server" Text="Jen TOP" />
                    <asp:TextBox ID="txtTop" runat="server" MaxLength="3" Rows="10" Columns="3" Text="3"></asp:TextBox><br />
                    <asp:RequiredFieldValidator Display="Dynamic" ID="RequiredFieldValidator1" runat="server"  ControlToValidate="txtTop" EnableClientScript="true" ValidationGroup="topCount" ErrorMessage="Zadejte omezení poètu øad k zobrazení"></asp:RequiredFieldValidator>
                    <asp:RangeValidator runat="server" ErrorMessage="Zadané èíslo musí být v rozsahu 1 - 999" MinimumValue="1" MaximumValue="999" ControlToValidate="txtTop" Display="Dynamic" EnableClientScript="true" ></asp:RangeValidator>
                               </div>
                <div class="right">
                    <h4>
                        Vodorovná osa</h4>
                    <h5>
                        Sledovaná hodnota</h5>
                    <asp:DropDownList ID="ddlXValueType" runat="server">
                        <asp:ListItem Value="Time" Selected="True">Èas</asp:ListItem>
                        <asp:ListItem Value="Data">Data</asp:ListItem>
                    </asp:DropDownList>
                    <h5>
                        Seskupovat dle</h5>
                    <asp:DropDownList ID="ddlXGroupByTime" runat="server">
                        <asp:ListItem Selected="True" Value="1">Dnù</asp:ListItem>
                        <asp:ListItem Value="7">T&#253;dnù</asp:ListItem>
                        <asp:ListItem Value="3">Mìs&#237;cù</asp:ListItem>
                        <asp:ListItem Value="4">Rokù</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlXGroupByData" runat="server" Enabled="True" Style="display: none;">
                        <asp:ListItem Selected="True" Value="VAD">Distributorù</asp:ListItem>
                        <asp:ListItem Value="ProductGroupDesc">Produktov&#253;ch skupin</asp:ListItem>
                        <asp:ListItem Value="DistributorTypeJoined">Typu distributorù</asp:ListItem>
                        <asp:ListItem Value="ReselerJoined">Reselerù</asp:ListItem>
                        <asp:ListItem Value="ProductJoined">Produktù</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:LinkButton runat="server" ID="lbFormAction" OnClick="lbFormAction_Click"></asp:LinkButton>
                <ul>
                    <li>
                        <asp:LinkButton runat="server" ID="btnGenerate" Text="Generovat report" OnClick="btnGenerate_Click" />
                    </li>
                </ul>
                <asp:TextBox runat="server" ID="txtScreenWidth" style="display: none;"></asp:TextBox>
                <asp:TextBox runat="server" ID="txtScreenHeight" style="display: none;"></asp:TextBox>
            </div>
        </div>
        <asp:Panel runat="server" ID="pnlGraph">
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlTable" class="grid" Visible="False" >
            <h3>Tabulka</h3>
            <asp:GridView runat="server" ID="gvGrid"></asp:GridView>
            <dev:ASPxGridView ID="gvTable" ClientInstanceName="grid" Visible="false" runat="server" Width="100%" AutoGenerateColumns="True">
            </dev:ASPxGridView>
             <dxwgv:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="gvTable"></dxwgv:ASPxGridViewExporter>
        </asp:Panel>
        <asp:Literal runat="server" ID="ltrJS"></asp:Literal>
    </div>
    </asp:Panel>
</asp:Content>
