<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MasterHeader.ascx.cs"
    Inherits="CanonWebApp.Controls.MasterHeader" %>

<table cellpadding="0" cellspacing="0" border="0" width="100%">
    <tr>
        <td>
            <div class="content-header">
                <a runat="server" href="~/Default.aspx">
                    <dev:ASPxImage runat="server" ID="LogoImage" SkinID="LogoImage" meta:resourcekey="LogoImage" />
                </a>
            </div>
        </td>
        <td align="right" style="padding-right: 40px;">
            <table cellpadding="0" cellspacing="0" border="0" xheight="117px">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="right" valign="bottom" style="padding-bottom: 20px;">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td style="white-space: nowrap;">
                                    <dev:ASPxLabel runat="server" ID="UserNameLabel" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right" valign="bottom" style="padding-bottom: 20px;">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td style="white-space: nowrap;">
                                <dev:ASPxCallbackPanel ID="clbPanelActualization" ShowLoadingPanel="False" ClientInstanceName="clbCheckIfReady" OnCallback="clbCheckIfReady_Callback" HideContentOnCallback="False" runat="server" Width="100%">
                                <PanelCollection>
                                    <dev:PanelContent>
                                        <dev:ASPxImage ID="imgActualizationState" SkinID="Actualization" meta:resourcekey="ActualizationImage" runat="server" Visible="False">
                                        <ClientSideEvents Click="function(s, e) { clbCheckIfReady.PerformCallback('GoToFinished'); }" />
                                        </dev:ASPxImage>
                                        <dev:ASPxLabel runat="server" ID="lblSideWorkIsReady" />
                                        <dev:ASPxTimer ID="timeCheckIfReady" runat="server" Interval="30000">
                                            <ClientSideEvents Tick="function(s, e) {
	                                                clbCheckIfReady.PerformCallback('CheckActualization');
                                                }" />
                                        </dev:ASPxTimer>
                                    </dev:PanelContent>
                                </PanelCollection>
                                </dev:ASPxCallbackPanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="content-menu">
                <table cellpadding="0" cellspacing="0" border="0" width="100%" height="26px">
                    <tr>
                        <td>
                            <div class="menu-grey" style="width: 40px;">
                            </div>
                        </td>
                        <td>
                            <div runat="server" id="BorderLeftDiv" class="menu-border">
                            </div>
                        </td>
                        <td class="menu-orange" runat="server" id="HomeLinkRow">
                            <asp:HyperLink runat="server" ID="HomeHyperLink" SkinID="Menu" meta:resourcekey="HomeHyperLink"
                                NavigateUrl="~/Default.aspx" />
                        </td>
                        <td runat="server" id="AdministrationSeparatorRow">
                            <div class="menu-separator">
                            </div>
                        </td>
                        <td class="menu-orange" runat="server" id="AdministrationLinkRow">
                            <asp:HyperLink runat="server" ID="AdministrationHyperLink" SkinID="Menu" meta:resourcekey="AdministrationHyperLink"
                                NavigateUrl="~/Administration.aspx" />
                        </td>
                        <td runat="server" id="LocalizationSeparatorRow" visible="false">
                            <div class="menu-separator">
                            </div>
                        </td>
                        <td class="menu-orange" runat="server" id="LocalizationLinkRow" visible="false">
                            <asp:HyperLink runat="server" ID="LocalizationHyperLink" SkinID="Menu" meta:resourcekey="LocalizationHyperLink"
                                NavigateUrl="~/Localization/Default.aspx" />
                        </td>
                        <td>
                            <div runat="server" id="BorderRightDiv" class="menu-border">
                            </div>
                        </td>
                        <td width="100%">
                            <div class="menu-grey" style="width: 100%; text-align: right;">
                                <div class="menu-date">
                                    <asp:Label runat="server" ID="CurrentDateTimeLabel" />
                                </div>
                            </div>
                        </td>
                        <td>
                            <div class="menu-border">
                            </div>
                        </td>
                        <td class="menu-orange">
                            <asp:LinkButton runat="server" ID="LogoutLinkButton" SkinID="Menu" meta:resourcekey="LogoutLinkButton"
                                OnClick="LogoutLinkButton_Click" />
                        </td>
                        <td>
                            <div class="menu-border">
                            </div>
                        </td>
                        <td>
                            <div class="menu-grey" style="width: 40px;">
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>
