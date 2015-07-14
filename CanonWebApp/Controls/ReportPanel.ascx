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
<h4>Svisl� osa</h4>
<h5>Sledovan� hodnota</h5>
<asp:DropDownList ID="ddlYValueType" runat="server">
    <asp:ListItem Value="4" Selected="True">Obrat</asp:ListItem>
    <asp:ListItem Value="1">Prodan&#253;ch kus�</asp:ListItem>
</asp:DropDownList>
<h5>Omezit pouze na</h5>
<asp:DropDownList ID="ddlYFilterType" runat="server">
    <asp:ListItem Selected="True">Neomezovat</asp:ListItem>
    <asp:ListItem>Distributora</asp:ListItem>
    <asp:ListItem>Produktovou skupinu</asp:ListItem>
    <asp:ListItem>Typ distributor�</asp:ListItem>
    <asp:ListItem>Reselera</asp:ListItem>
    <asp:ListItem>Skupinu reseler�</asp:ListItem>
    <asp:ListItem>Zemi reselera</asp:ListItem>
    <asp:ListItem>Produkt</asp:ListItem>
</asp:DropDownList>
<%-- --%>
<h5>Zp�esn�n� omezen�</h5>
<asp:ListBox ID="lbYFilter" runat="server" SelectionMode="Multiple">
    <asp:ListItem>prvni</asp:ListItem>
    <asp:ListItem>druhy</asp:ListItem>
    <asp:ListItem>treti</asp:ListItem>
</asp:ListBox>
<h5>Seskupovat dle</h5>
<asp:DropDownList ID="ddlYGroupBy" runat="server">
    <asp:ListItem Selected="True" Value="VAD">Distributor�</asp:ListItem>
    <asp:ListItem Value="ProductGroup">Produktov&#253;ch skupin</asp:ListItem>
    <asp:ListItem>Typu distributor�</asp:ListItem>
    <asp:ListItem Value="ReselerGroup">Skupin reseler�</asp:ListItem>
    <asp:ListItem Value="Ico">Reseler�</asp:ListItem>
    <asp:ListItem Value="Mcode">Produkt�</asp:ListItem>
</asp:DropDownList>
<h5>Zobrazen� �ady</h5>
    <asp:ListBox ID="lbYHaving" runat="server"></asp:ListBox><br />
    <br />
    <asp:CheckBox ID="chbShowSum" runat="server" Text="Zobrazovat jen sumu" /><br />
    <asp:CheckBox ID="chbTop" runat="server" Text="Jen TOP" />
    <asp:TextBox ID="txtTop" runat="server" MaxLength="3" Rows="10"></asp:TextBox><br />
</div>
<div class="right">
    <h4>Vodorovn� osa</h4>
    <h5>Sledovan� hodnota</h5>
    <asp:DropDownList ID="ddlXValueType" runat="server">
        <asp:ListItem Selected="True">�as</asp:ListItem>
        <asp:ListItem Value="Data">Data</asp:ListItem>
    </asp:DropDownList>
    <h5>Seskupovat dle</h5>
    <asp:DropDownList ID="ddlXGroupBy" runat="server">
        <asp:ListItem Selected="True">Dn�</asp:ListItem>
        <asp:ListItem>T&#253;dn�</asp:ListItem>
        <asp:ListItem>M�s&#237;c�</asp:ListItem>
    </asp:DropDownList>
    <h5>Seskupovat dle</h5>
    <asp:DropDownList ID="DropDownList6" runat="server">
        <asp:ListItem Selected="True">Distributor�</asp:ListItem>
        <asp:ListItem>Produktov&#253;ch skupin</asp:ListItem>
        <asp:ListItem>Typu distributor�</asp:ListItem>
        <asp:ListItem>Reseler�</asp:ListItem>
        <asp:ListItem>Produkt�</asp:ListItem>
    </asp:DropDownList>
<asp:LinkButton runat="server" ID="btnGenerate" Text="Generate" 
    onclick="btnGenerate_Click" />
</div>
</div>
</div>
<asp:Panel runat="server" ID="pnlGraph" ></asp:Panel>