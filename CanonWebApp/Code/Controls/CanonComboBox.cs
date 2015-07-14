using System;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;

namespace CanonWebApp.Code.Controls
{
    /// <summary>
    /// Combo box which support getting client's selected value.
    /// We can't override and use default protected property ClientValue because we can't preset it.
    /// This property works only after user changes item.
    /// </summary>
    public class CanonComboBox : ASPxComboBox
    {
        #region Properties

        #region ClientSelectedValue
        /// <summary>
        /// Client's selected value.
        /// </summary>
        public string ClientSelectedValue
        {
            get
            {
                string clientSelectedValue = Context.Request.Params[SelectedValueHiddenField.UniqueID];

                return
                    !String.IsNullOrEmpty(clientSelectedValue) ?
                    clientSelectedValue :
                    Convert.ToString(Value);
            }
        }
        #endregion

        #region SelectedValueHiddenField
        /// <summary>
        /// HiddenField control for storing value of selected item.
        /// </summary>
        protected HiddenField SelectedValueHiddenField
        {
            get;
            set;
        }
        #endregion

        #endregion

        #region OnLoad
        /// <summary>
        /// Creates child controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }
        #endregion

        #region CreateChildControls
        /// <summary>
        /// Creates HiddenField child control.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            HiddenField hiddenField = new HiddenField();
            hiddenField.EnableViewState = true;
            hiddenField.ID = String.Concat(ClientInstanceName, "HiddenField");

            Controls.Add(hiddenField);

            SelectedValueHiddenField = hiddenField;
        }
        #endregion

        #region Render
        /// <summary>
        /// Create script for saving selected value in the hidden field.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (SelectedItem != null)
            {
                SelectedValueHiddenField.Value = Convert.ToString(SelectedItem.Value);
            }

            ClientSideEvents.SelectedIndexChanged += String.Concat(
                "function(s, e) { var hiddenField = document.getElementById('",
                SelectedValueHiddenField.ClientID,
                "'); hiddenField.value = ",
                ClientInstanceName,
                ".GetValue(); }");

            base.Render(writer);
        }
        #endregion
    }
}