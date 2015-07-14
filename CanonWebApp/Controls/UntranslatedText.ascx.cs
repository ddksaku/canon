using System;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView;
using CanonWebApp.Code;

namespace CanonWebApp.Controls
{
    /// <summary>
    /// Untranslated text user control.
    /// </summary>
    public partial class UntranslatedText : UserControl
    {
        #region Page_Load
        /// <summary>
        /// Localizes the page and bind untranslated text grids.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Localize();

            UntranslatedTextGridView.SettingsPager.PageSize = ConfigSettings.DefaultPageSize;

            if ((!IsPostBack && !Page.IsCallback) || UntranslatedTextGridView.IsCallback)
            {
                BindUntranslatedTextGrid();
            }

            FillUntranslatedResourcesButton.ClientSideEvents.Click = String.Concat(
                "function(s, e) { if (confirm('",
                Utilities.GetResourceString("Common", "FillUntranslatedResourcesConfirmation"),
                "')) { LocalizationCallbackPanel.PerformCallback('FillUntranslatedResources'); } }");
        }
        #endregion

        #region Localize
        /// <summary>
        /// Localizes text depending on selected language.
        /// </summary>
        private void Localize()
        {
            UntranslatedTextGridView.Columns["ResourceSetField"].Caption = Utilities.GetResourceString("Headers", "ResourceSet");
            UntranslatedTextGridView.Columns["ResourceIdField"].Caption = Utilities.GetResourceString("Headers", "ResourceID");
        }
        #endregion


        #region LocalizationCallbackPanel_Callback
        /// <summary>
        /// Fill untranslated resources with default english values.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void LocalizationCallbackPanel_Callback(object source, CallbackEventArgsBase e)
        {
            if (String.Equals(e.Parameter, "FillUntranslatedResources", StringComparison.CurrentCultureIgnoreCase))
            {
                Business.Localization.FillUntranslatedResources("cs-cz");
            }
        }
        #endregion


        #region UntranslatedTextGridView_CustomCallback
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UntranslatedTextGridView_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {

        }
        #endregion

        #region UntranslatedTextGridView_HtmlDataCellPrepared
        /// <summary>
        /// Creates link for redirect to untranslated resource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UntranslatedTextGridView_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            switch (e.DataColumn.Name)
            {
                case "ResourceIdField":
                    string resourceSet = Convert.ToString(((ASPxGridView)sender).GetRowValues(e.VisibleIndex, "ResourceSet"));

                    e.Cell.Text = String.Concat(
                        "<a href='Default.aspx?ResourceSet=",
                        resourceSet,
                        "&CtlId=",
                        e.CellValue,
                        "'>",
                        e.CellValue,
                        "</a>");
                    break;
            }
        } 
        #endregion

        #region BindUntranslatedTextGrid
        /// <summary>
        /// Bind untranslated text grid control.
        /// </summary>
        private void BindUntranslatedTextGrid()
        {
            UntranslatedTextGridView.DataSource = Business.Localization.GetUntranslatedTextsList();
            UntranslatedTextGridView.DataBind();
        }
        #endregion
    }
}