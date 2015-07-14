<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginPage.aspx.cs" Theme="Default" Inherits="CanonWebApp.LoginPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Login - Canon Monitoring</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<table cellpadding="0" cellspacing="10" border="0" width="100%">
		    <tr>
		        <td align="center">
                    <dev:ASPxImage runat="server" ID="LogoImage" SkinID="LogoImage" meta:resourcekey="LogoImage" />
		        </td>
		    </tr>
			<tr>
				<td align="center">
					<dev:ASPxRoundPanel
						runat="server"
						ID="LoginRoundPanel"
						HeaderStyle-HorizontalAlign="Left"
						ClientInstanceName="LoginRoundPanel" HeaderText="Přihlašovací formulář">
<HeaderStyle HorizontalAlign="Left"></HeaderStyle>
						<PanelCollection>
							<dev:PanelContent ID="PanelContent1" runat="server">
								<asp:Login
									runat="server"
									ID="LoginControl"
									DestinationPageUrl="~/default.aspx"
									OnLoggedIn="LoginControl_LoggedIn"
									OnLoginError="LoginControl_LoginError">
									<LayoutTemplate>
										<asp:Panel runat="server" ID="LoginPanel" DefaultButton="LoginButton">
											<table cellpadding="0" cellspacing="6" border="0">
												<tr align="left">
													<td style="white-space: nowrap;"><asp:Localize runat="server" ID="UserNameTitle" Text="Uživatelské jméno:" />&nbsp;&nbsp;&nbsp;</td>
													<td colspan="2">
														<dev:ASPxTextBox runat="server" ID="UserName" MaxLength="50" Width="220px">
															<ValidationSettings SetFocusOnError="True" ValidationGroup="Login" ErrorDisplayMode="ImageWithTooltip" CausesValidation="True">
																<RequiredField IsRequired="True" />
																<ErrorFrameStyle Paddings-PaddingRight="5" />
															</ValidationSettings>
														</dev:ASPxTextBox>
													</td>
												</tr>
												<tr align="left">
													<td style="white-space: nowrap;"><asp:Localize runat="server" ID="PasswordTitle" Text="Heslo:" /></td>
													<td colspan="2">
														<dev:ASPxTextBox runat="server" ID="Password" MaxLength="50" Password="true" Width="220px">
															<ValidationSettings SetFocusOnError="True" ValidationGroup="Login" ErrorDisplayMode="ImageWithTooltip" CausesValidation="True">
																<RequiredField IsRequired="True" />
																<ErrorFrameStyle Paddings-PaddingRight="5" />
															</ValidationSettings>
														</dev:ASPxTextBox>
													</td>
												</tr>
												<tr align="left">
													<td>&nbsp;</td>
													<td style="white-space: nowrap;">
														<dev:ASPxCheckBox runat="server" ID="RememberMeCheckBox" Text="" Visible="false" />
													</td>
													<td align="right" style="padding: 2px 27px 2px 0px;">
														<dev:ASPxButton runat="server" ID="LoginButton" ValidationGroup="Login" CommandName="Login" Text="Přihlásit" >
															<ClientSideEvents Click="function(s, e) { if (!ASPxClientEdit.ValidateGroup('Login')) return false; }" />
														</dev:ASPxButton>
													</td>
												</tr>
												<tr align="left">
													<td>&nbsp;</td>
													<td colspan="2">
														<dev:ASPxHyperLink runat="server" ID="ForgotHyperLink" meta:resourcekey="ForgotHyperLink" Visible="false">
															 <ClientSideEvents Click="function(s, e) { ForgotPopupControl.Show(); EmailTextBox.Focus(); }" />
														</dev:ASPxHyperLink>
														
														<dev:ASPxPopupControl
															runat="server"
															ID="ForgotPopupControl"
															ClientInstanceName="ForgotPopupControl"
															Modal="True"
															PopupHorizontalAlign="WindowCenter"
															PopupVerticalAlign="WindowCenter"
															AllowDragging="True"
															EnableAnimation="False">
															<ClientSideEvents CloseUp="function(s, e) { ASPxClientEdit.ClearGroup('Remind', true); }" />
															
															<Controls>
																<dev:ASPxCallbackPanel
																	runat="server"
																	ID="ForgotCallbackPanel"
																	ClientInstanceName="ForgotCallbackPanel"
																	Width="320px"
																	OnCallback="ForgotCallbackPanel_Callback">
																	<ClientSideEvents EndCallback="function(s, e) { ForgotPopupControl.UpdateWindowPosition(); }" />
																	
																	<PanelCollection>
																		<dev:PanelContent runat="server">
																			<asp:Panel runat="server" ID="RemindPanel" DefaultButton="RemindButton">
																				<table cellpadding="0" cellspacing="6" border="0">
																					<tr>
																						<td style="white-space: nowrap;"><asp:Localize runat="server" ID="EmailTitle" meta:resourcekey="EmailTitle" />&nbsp;&nbsp;&nbsp;</td>
																						<td>
																							<dev:ASPxTextBox runat="server" ID="EmailTextBox" ClientInstanceName="EmailTextBox" MaxLength="50" Width="220px">
																								<ValidationSettings SetFocusOnError="True" ValidationGroup="Remind" ErrorDisplayMode="ImageWithTooltip" CausesValidation="True">
																									<RequiredField IsRequired="True" />
																									<RegularExpression ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" />
																									<ErrorFrameStyle Paddings-PaddingRight="5" />
																								</ValidationSettings>
																							</dev:ASPxTextBox>
																						</td>
																					</tr>
																					<tr>
																						<td>&nbsp;</td>
																						<td align="right" style="padding: 2px 27px 2px 0px;">
																							<dev:ASPxButton runat="server" ID="RemindButton" AutoPostBack="false" ValidationGroup="Remind" meta:resourcekey="RemindButton">
																								<ClientSideEvents Click="function(s, e) { if (ASPxClientEdit.ValidateGroup('Remind')) { ForgotCallbackPanel.PerformCallback('Remind'); } else return false; }" />
																							</dev:ASPxButton>
																						</td>
																					</tr>
																					<tr runat="server" id="SuccessMessageDiv" Visible="false">
																						<td align="center" colspan="2">
																							<asp:Label runat="server" ID="SuccessMessageLabel" meta:resourcekey="SuccessMessageLabel" CssClass="success-message" />
																						</td>
																					</tr>
																					<tr runat="server" id="ErrorMessageDiv" Visible="false">
																						<td align="center" colspan="2">
																							<asp:Label runat="server" ID="ErrorMessageLabel" meta:resourcekey="ErrorMessageLabel" CssClass="unsuccess-message" />
																						</td>
																					</tr>
																				</table>
																			</asp:Panel>
																		</dev:PanelContent>
																	</PanelCollection>
																</dev:ASPxCallbackPanel>
															</Controls>
														</dev:ASPxPopupControl>
													</td>
												</tr>
											</table>
										</asp:Panel>
									</LayoutTemplate>
								</asp:Login>
							</dev:PanelContent>
						</PanelCollection>
					</dev:ASPxRoundPanel>
				</td>
			</tr>
			<tr>
				<td align="center">
					<asp:Label runat="server" ID="LoginErrorLabel" Visible="false" meta:resourcekey="LoginErrorLabel" CssClass="unsuccess-message" />
				</td>
			</tr>
		</table>
   
    </div>
    </form>
</body>
</html>
