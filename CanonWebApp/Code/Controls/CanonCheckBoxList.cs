using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;

namespace CanonWebApp.Code.Controls
{
    /// <summary>
    /// Check box list with dev express checkboxes.
    /// </summary>
    public class CanonCheckBoxList : WebControl
    {
        #region Properties

        #region Values
        /// <summary>
        /// Values for filling list with ASPxCheckBoxes.
        /// </summary>
        public Dictionary<string, string> Values
        {
            get;
            set;
        }
        #endregion

        #region CheckedChangedScript
        /// <summary>
        /// Script for client side event CheckedChanged.
        /// </summary>
        public string CheckedChangedScript
        {
            get;
            set;
        }
        #endregion

        #region CheckBoxesList
        /// <summary>
        /// List of check boxes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ASPxCheckBox> CheckBoxesList
        {
            get
            {
                return Memos.Framework.Utilities.FindChildControlsByType<ASPxCheckBox>(this);
            }
        } 
        #endregion

        #region SelectedTexts
        /// <summary>
        /// List of texts of selected check boxes.
        /// </summary>
        public List<string> SelectedTexts
        {
            get
            {
                List<string> selectedValues = new List<string>();

                foreach (ASPxCheckBox checkBox in CheckBoxesList)
                {
                    if (checkBox.Checked)
                    {
                        selectedValues.Add(checkBox.Text);
                    }
                }

                return selectedValues;
            }
        }
        #endregion

        #region SelectedValues
        /// <summary>
        /// List of values of selected check boxes.
        /// </summary>
        public List<string> SelectedValues
        {
            get
            {
                List<string> selectedValues = new List<string>();

                foreach (ASPxCheckBox checkBox in CheckBoxesList)
                {
                    if (checkBox.Checked)
                    {
                        selectedValues.Add(checkBox.ID.Replace("CheckBox", String.Empty));
                    }
                }

                return selectedValues;
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

            Table table = new Table();
            table.CellPadding = 0;
            table.CellSpacing = 0;
            table.BorderWidth = 0;

            foreach (KeyValuePair<string, string> value in Values)
            {
                ASPxCheckBox checkBox = new ASPxCheckBox();
                checkBox.ID = String.Concat("CheckBox", value.Key);
                checkBox.Text = value.Value;

                checkBox.ClientSideEvents.CheckedChanged = CheckedChangedScript;

                TableRow tableRow = new TableRow();

                TableCell checkBoxCell = new TableCell();
                checkBoxCell.Controls.Add(checkBox);

                tableRow.Cells.Add(checkBoxCell);

                table.Rows.Add(tableRow);
            }

            Controls.Add(table);
        }
        #endregion
    }
}