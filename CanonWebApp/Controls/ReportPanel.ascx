<%@ Control Language="C#" AutoEventWireup="true" Inherits="CanonWebApp.Controls.ReportPanel" Codebehind="ReportPanel.ascx.cs" %>
<script language="javascript" type="text/javascript" >

    function TogglePanel(id) {
        var o = document.getElementById(id);
        if (o.style.display == "block" || o.style.display == "")
        {
            o.style.display = "none";
        }
        else {
            o.style.display = "block";
        } 
    }

</script>
<div class='<%= CssClass %>'>
<a class="toggle" onclick="TogglePanel('<%= ID + "_form" %>')" href="#">
</a>
<div class="form" id='<%= ID + "_form" %>'>
<div class="left">
<h4>Svislá osa</h4>
<h5>Sledovaná hodnota</h5>
<asp:DropDownList ID="ddlYValueType" runat="server">
    <asp:ListItem Value="4" Selected="True">Obrat</asp:ListItem>
    <asp:ListItem Value="1">Prodan&#253;ch kusù</asp:ListItem>
</asp:DropDownList>
<h5>Omezit pouze na</h5>
<asp:DropDownList ID="ddlYFilterType" runat="server">
    <asp:ListItem Selected="True">Neomezovat</asp:ListItem>
    <asp:ListItem>Distributora</asp:ListItem>
    <asp:ListItem>Produktovou skupinu</asp:ListItem>
    <asp:ListItem>Typ distributorù</asp:ListItem>
    <asp:ListItem>Reselera</asp:ListItem>
    <asp:ListItem>Skupinu reselerù</asp:ListItem>
    <asp:ListItem>Zemi reselera</asp:ListItem>
    <asp:ListItem>Produkt</asp:ListItem>
</asp:DropDownList>
<%-- --%>
<h5>Zpøesnìní omezení</h5>
<asp:ListBox ID="lbYFilter" runat="server" SelectionMode="Multiple">
    <asp:ListItem>prvni</asp:ListItem>
    <asp:ListItem>druhy</asp:ListItem>
    <asp:ListItem>treti</asp:ListItem>
</asp:ListBox>
<h5>Seskupovat dle</h5>
<asp:DropDownList ID="ddlYGroupBy" runat="server">
    <asp:ListItem Selected="True" Value="VAD">Distributorù</asp:ListItem>
    <asp:ListItem Value="ProductGroup">Produktov&#253;ch skupin</asp:ListItem>
    <asp:ListItem>Typu distributorù</asp:ListItem>
    <asp:ListItem Value="ReselerGroup">Skupin reselerù</asp:ListItem>
    <asp:ListItem Value="Ico">Reselerù</asp:ListItem>
    <asp:ListItem Value="Mcode">Produktù</asp:ListItem>
</asp:DropDownList>
<h5>Zobrazené øady</h5>
    <asp:ListBox ID="lbYHaving" runat="server"></asp:ListBox><br />
    <br />
    <asp:CheckBox ID="chbShowSum" runat="server" Text="Zobrazovat jen sumu" /><br />
    <asp:CheckBox ID="chbTop" runat="server" Text="Jen TOP" />
    <asp:TextBox ID="txtTop" runat="server" MaxLength="3" Rows="10"></asp:TextBox><br />
</div>
<div class="right">
    <h4>Vodorovná osa</h4>
    <h5>Sledovaná hodnota</h5>
    <asp:DropDownList ID="ddlXValueType" runat="server">
        <asp:ListItem Selected="True">Èas</asp:ListItem>
        <asp:ListItem Value="Data">Data</asp:ListItem>
    </asp:DropDownList>
    <h5>Seskupovat dle</h5>
    <asp:DropDownList ID="ddlXGroupBy" runat="server">
        <asp:ListItem Selected="True">Dnù</asp:ListItem>
        <asp:ListItem>T&#253;dnù</asp:ListItem>
        <asp:ListItem>Mìs&#237;cù</asp:ListItem>
    </asp:DropDownList>
    <h5>Seskupovat dle</h5>
    <asp:DropDownList ID="DropDownList6" runat="server">
        <asp:ListItem Selected="True">Distributorù</asp:ListItem>
        <asp:ListItem>Produktov&#253;ch skupin</asp:ListItem>
        <asp:ListItem>Typu distributorù</asp:ListItem>
        <asp:ListItem>Reselerù</asp:ListItem>
        <asp:ListItem>Produktù</asp:ListItem>
    </asp:DropDownList>
<asp:LinkButton runat="server" ID="btnGenerate" Text="Generate" 
    onclick="btnGenerate_Click" />
</div>
</div>
</div>
<asp:Panel runat="server" ID="pnlGraph" ></asp:Panel>