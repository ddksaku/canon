<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchBox.ascx.cs" Inherits="CanonWebApp.Controls.SearchBox" %>
<asp:Literal runat="server" ID="ltrJS"></asp:Literal>
<div class="canvasOuter" runat="server" id="outer">
    <input type="text" class="text" runat="server" id="txtKeyword" autocomplete="off" />
    <div runat="server" id="canvas" class="canvas">
        <asp:HiddenField runat="server" ID="hf"/>
        <table class="dropdown">
            <caption>
            </caption>
            <colgroup>
                <col class="chb" />
                <col class="value" />
            </colgroup>
            <%-- 
            <thead style="display: none;">
                <asp:Repeater runat="server" ID="rptFavorites">
                    <ItemTemplate>
                        <tr>
                            <th>
                                <input type="checkbox" class="NoAjax"  />
                            </th>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem,"Text") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </thead>
            --%>
            <tbody>
                <asp:Repeater runat="server" ID="rptResults">
                    <ItemTemplate>
                        <tr>
                            <th>
                                <input type="checkbox" ajax="no" />
                            </th>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem,"Text") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
            <tfoot style="display: none;">
                <tr>
                    <th>
                        <input type="checkbox" />
                    </th>
                    <td>
                        All
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
</div>
