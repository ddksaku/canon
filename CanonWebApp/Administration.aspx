<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" Theme="Default" AutoEventWireup="true" CodeBehind="Administration.aspx.cs" Inherits="CanonWebApp.Administration" %>

<%@ Register TagPrefix="Canon" TagName="AdminLog" Src="~/Controls/AdminLog.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminDistributorsImport" Src="~/Controls/AdminDistributorsImport.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminPricesImport" Src="~/Controls/AdminPricesImport.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminResellersImport" Src="~/Controls/AdminResellersImport.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminProducts" Src="~/Controls/AdminProducts.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminDistributorsManage" Src="~/Controls/AdminDistributorsManage.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminGroups" Src="~/Controls/AdminGroups.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminCategories" Src="~/Controls/AdminCategories.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminChannels" Src="~/Controls/AdminChannels.ascx" %>
<%@ Register TagPrefix="Canon" TagName="AdminUsers" Src="~/Controls/AdminUsers.ascx" %>


<%@ Register assembly="DevExpress.Web.v8.3, Version=8.3.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxClasses" tagprefix="dxw" %>


<asp:Content ID="AdminContent" ContentPlaceHolderID="Content" runat="server">
	<dev:ASPxCallbackPanel
		runat="server"
		ID="AdministrationPageCallbackPanel"
		ClientInstanceName="AdministrationPageCallbackPanel"
		HideContentOnCallback="False" 
		OnCallback="AdministrationPageCallbackPanel_Callback">
		<PanelCollection>
			<dev:PanelContent ID="PanelContent1" runat="server">
				
				<dev:ASPxPageControl
					runat="server"
					ID="AdministrationPageControl"
					Width="100%"
					SaveStateToCookies="true" EnableHierarchyRecreation="True"
					SaveStateToCookiesID="AdministrationActiveTab">
					<ClientSideEvents ActiveTabChanged="function(s, e) { /*AdministrationTabChanged();*/ } " />
					
					<TabPages>
						<dev:TabPage Name="ImportDistributors" Text="Import dat od distributorů">
							<Controls>
								<Canon:CanonPanel runat="server" ID="DistributorsPanel">
									<Canon:AdminDistributorsImport runat="server" ID="AdminDistributorsCtrl" />
								</Canon:CanonPanel>
							</Controls>
						</dev:TabPage>
						<dev:TabPage Name="ImportPrices" Text="Import ceníku Canon">
							<Controls>
								<Canon:CanonPanel runat="server" ID="PricesPanel">
									<Canon:AdminPricesImport runat="server" ID="AdminPricesCtrl" />
								</Canon:CanonPanel>
							</Controls>
						</dev:TabPage>
						<dev:TabPage Name="ImportResellerGroups" Text="Import skupin reselerů">
							<Controls>
								<Canon:CanonPanel runat="server" ID="ProductsPanel">
									<Canon:AdminResellersImport runat="server" ID="AdminResellersCtrl" />
								</Canon:CanonPanel>
							</Controls>
						 </dev:TabPage>
						 <dev:TabPage Name="DistributorsManage" Text="Distributoři">
						    <Controls> 
						        <Canon:CanonPanel runat="server" ID="DistributorsManagePanel">
									<Canon:AdminDistributorsManage runat="server" ID="AdminDistributorsManageCtrl" />
								</Canon:CanonPanel>
						    </Controls>
						 </dev:TabPage>
						 <dev:TabPage Name="Groups" Text="Skupiny">
							<Controls>
							    <Canon:CanonPanel runat="server" ID="GroupsPanel">
							        <Canon:AdminGroups runat="server" ID="AdminGroupsCtrl" />
							    </Canon:CanonPanel>
							</Controls>
						 </dev:TabPage>
						 <dev:TabPage Name="Users" Text="Uživatelské účty">
							<Controls>
								<Canon:CanonPanel runat="server" ID="AdminUsersPanel">
									<Canon:AdminUsers runat="server" ID="AdminUsersCtrl" />
								</Canon:CanonPanel>
							</Controls>
						 </dev:TabPage>
					</TabPages>
				</dev:ASPxPageControl>
			</dev:PanelContent>
		</PanelCollection>
	</dev:ASPxCallbackPanel>

</asp:Content>
