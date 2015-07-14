<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UntranslatedText.ascx.cs" Inherits="CanonWebApp.Controls.UntranslatedText" %>

<dev:ASPxCallbackPanel
	runat="server"
	ID="LocalizationCallbackPanel"
	ClientInstanceName="LocalizationCallbackPanel"
	OnCallback="LocalizationCallbackPanel_Callback">
	<ClientSideEvents EndCallback="function(s, e) { UntranslatedTextGridView.PerformCallback(''); }" />
	
	<PanelCollection>
		<dev:PanelContent runat="server">
			<Canon:CanonPanel runat="server" ID="LocalizationDetailsPanel">
				<table cellpadding="0" cellspacing="0" border="0">
					<tr>
						<td style="text-align: left; padding: 10px 0px;">
							<h3>
								<asp:Localize runat="server" ID="UntranslatedResourcesTitle" meta:resourcekey="UntranslatedResourcesTitle" />
							</h3>
						</td>
					</tr>
					<tr>
						<td style="text-align: left; padding: 0px 0px 10px 0px;">
							<dev:ASPxButton runat="server" ID="FillUntranslatedResourcesButton" AutoPostBack="false" meta:resourcekey="FillUntranslatedResourcesButton" />
						</td>
					</tr>
				</table>

				<dev:ASPxGridView
					runat="server"
					ID="UntranslatedTextGridView"
					ClientInstanceName="UntranslatedTextGridView"
					Width="500px"
					OnCustomCallback="UntranslatedTextGridView_CustomCallback"
					OnHtmlDataCellPrepared="UntranslatedTextGridView_HtmlDataCellPrepared">
					
					<Settings ShowFilterRow="false" />
					<SettingsBehavior AllowSort="true" AllowDragDrop="false" />
					<SettingsPager Mode="ShowPager" />
					<Styles AlternatingRow-Enabled="False" />

					<Columns>
						<dev:GridViewDataColumn FieldName="ResourceSet" Name="ResourceSetField" />
						<dev:GridViewDataColumn FieldName="ResourceId" Name="ResourceIdField" />
					</Columns>
				</dev:ASPxGridView>
			</Canon:CanonPanel>
		</dev:PanelContent>
	</PanelCollection>
</dev:ASPxCallbackPanel>