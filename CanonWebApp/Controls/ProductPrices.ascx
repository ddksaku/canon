<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductPrices.ascx.cs" Inherits="CanonWebApp.Controls.ProductPrices" %>

<%@ Register TagPrefix="Canon" TagName="LogPopupControl" Src="~/Controls/LogPopupControl.ascx" %>

<dev:ASPxCallbackPanel ID="CallbackHistoryPanel" ClientInstanceName="RecomPriceCallbackPanel" 
    runat="server" Width="100%" OnCallback="CallbackHistoryPanel_Callback" HideContentOnCallback="False">
<PanelCollection>
<dev:PanelContent>

<div style="text-align:left;width:100%;">
    <dev:ASPxLabel ID="lblPriceTitle" Font-Bold="True" runat="server" meta:resourcekey="PageTitle">
    </dev:ASPxLabel>
</div>
<div style="width:100%;">
    <dev:ASPxGridView ID="gridPriceHistory" runat="server" Width="100%" 
          AutoGenerateColumns="False" ClientInstanceName="gridPriceHistory" 
          KeyFieldName="PriceId"
          OnRowUpdating="gridPriceHistory_RowUpdating">
        <Columns>
            <dev:GridViewDataDateColumn Name="colPriceDate" Caption="" ReadOnly="True" Width="40%" FieldName="ChangeDate" VisibleIndex="0">
            <PropertiesDateEdit DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"></PropertiesDateEdit>
             <EditItemTemplate>
                <div style="padding-left:7px;">
                <dev:ASPxLabel ID="lblPriceDate" runat="server" Text="" Value='<%# ((DateTime)Eval("ChangeDate")).ToString("dd.MM.yyyy") %>'>
                </dev:ASPxLabel>
                </div>
             </EditItemTemplate>
            </dev:GridViewDataDateColumn>
            <dev:GridViewDataSpinEditColumn Name="colPriceValue" Caption="" Width="40%" FieldName="Price" VisibleIndex="1">
                <PropertiesSpinEdit DisplayFormatString="g">
                </PropertiesSpinEdit>
            </dev:GridViewDataSpinEditColumn>
            <dev:GridViewCommandColumn Name="colCommands" Width="20%">
            <EditButton Visible="True"></EditButton>
            </dev:GridViewCommandColumn>
        </Columns>
        <Settings ShowGroupPanel="False" />
        <SettingsPager PageSize="5"></SettingsPager>
        <SettingsBehavior ConfirmDelete="True" />
        <SettingsLoadingPanel ImagePosition="Top" />
        <SettingsEditing Mode="Inline" />
    </dev:ASPxGridView>
</div>
<div style="text-align:right;width:100%;">
    <dev:ASPxButton ID="btnShowPriceLog" runat="server" AutoPostBack="False"  meta:resourcekey="PriceLogButton">
       <ClientSideEvents Click="function(s, e) {
            recomHistoryPopup.Show();
            RecomPriceCallbackPanel.PerformCallback();
        }" />    
    </dev:ASPxButton>
</div>
<dev:ASPxPopupControl ID="popupRecomHistory" runat="server" 
ClientInstanceName="recomHistoryPopup" Width="500px" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableClientSideAPI="True" Modal="True">
<ContentCollection>
<dev:PopupControlContentControl>
    <Canon:LogPopupControl runat="server" ID="RecommendedHistoryCtrl" LogTypeValue="4" />
</dev:PopupControlContentControl>
</ContentCollection>
</dev:ASPxPopupControl>

</dev:PanelContent>
</PanelCollection>
</dev:ASPxCallbackPanel>
