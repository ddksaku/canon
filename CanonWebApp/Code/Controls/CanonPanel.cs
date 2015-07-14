using System;
using System.Web.UI.WebControls;

namespace CanonWebApp.Code.Controls
{
    /// <summary>
    /// Panel with default button that does not do post back
    /// of the page after Enter is pressed in that panel.
    /// </summary>
    public class CanonPanel : Panel
    {
        #region Properties

        #region DefaultButton
        /// <summary>
        /// Default button.
        /// </summary>
        public override string DefaultButton
        {
            get
            {
                return String.Concat("DefaultButton", ID);
            }
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
            ID = Guid.NewGuid().ToString();

            EnsureChildControls();

            base.OnLoad(e);
        }
        #endregion

        #region CreateChildControls
        /// <summary>
        /// Creates default button child control.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Button defaultButton = new Button();
            defaultButton.ID = String.Concat("DefaultButton", ID);
            defaultButton.Style.Add("height", "0");
            defaultButton.Style.Add("visibility", "hidden");
            defaultButton.Attributes.Add("onclick", "return false;");

            Controls.Add(defaultButton);
        }
        #endregion
    }
}