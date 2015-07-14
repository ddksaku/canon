using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxTabControl;
using Memos.Framework.Logging;
using Canon.Data.Import;

namespace CanonWebApp.Code
{
    /// <summary>
    /// Utilites class.
    /// </summary>
    public static class Utilities
    {
        // Resource string

        #region GetResourceString
        /// <summary>
        /// Returns a string value stored in resources.
        /// </summary>
        /// <param name="control">Current page or usercontrol</param>
        /// <param name="resourceKey">Resource key.</param>
        /// <returns>String value stored in resources</returns>
        public static string GetResourceString(TemplateControl control, string resourceKey)
        {
            string resourceString = (string)HttpContext.GetLocalResourceObject(control.AppRelativeVirtualPath, resourceKey);
            if (resourceString != null)
            {
                return resourceString;
            }

            Logger.Log(String.Format("String '{0}' was not found in resources", resourceKey), LogLevel.Warn);
            return String.Format("[{0}]", resourceKey);
        }
        #endregion

        #region GetResourceString
        /// <summary>
        /// Returns a string value stored in resources.
        /// </summary>
        /// <param name="classKey">Resource class key.</param>
        /// <param name="resourceKey">Resource key.</param>
        /// <returns>String value stored in resources</returns>
        public static string GetResourceString(string classKey, string resourceKey)
        {
            string resourceString = null;

            try
            {
                resourceString = (string)HttpContext.GetGlobalResourceObject(classKey, resourceKey);
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                Logger.Log(String.Format("Missing global resource '{0}'", classKey), LogLevel.Error);
            }

            if (resourceString != null)
            {
                return resourceString;
            }

            Logger.Log(String.Format("String '{0}' was not found in resources", resourceKey), LogLevel.Warn);
            return String.Format("[{0}.{1}]", classKey, resourceKey);
        }
        #endregion

        #region InitializeCulture
        /// <summary>
        /// Initialize culture.
        /// </summary>
        public static void InitializeCulture()
        {
            string language = SessionManager.CurrentLanguage;

            if (String.IsNullOrEmpty(language))
            {
                return;
            }

            //if (language.Length < 3)
            //{
            //    language = String.Concat(language, "-", language.ToUpper());
            //}

            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
            }
            catch (Exception exc)
            {
                Logger.Log(String.Format("Can't set culture for language '{0}'. Detail: {1}", language, exc.ToString()), LogLevel.Error);
                language = ConfigSettings.DefaultLanguage;
            }
        }
        #endregion

        // Common

        #region SetControlsEnabled
        /// <summary>
        /// Set web controls enabled / disabled.
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="controls"></param>
        public static void SetControlsEnabled(bool enabled, params WebControl[] controls)
        {
            foreach (WebControl control in controls)
            {
                control.Enabled = enabled;
            }
        } 
        #endregion

        #region SetControlsVisibility
        /// <summary>
        /// Set controls visible / invisible.
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="controls"></param>
        public static void SetControlsVisibility(bool visible, params Control[] controls)
        {
            foreach (Control control in controls)
            {
                control.Visible = visible;
            }
        }
        #endregion

        #region GetEditFormComboValue
        /// <summary>
        /// Searchs for combo by FindEditFormTemplateControl and returns selected value
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public static object GetEditFormComboValue(ASPxGridView grid, string controlName)
        {
            ASPxComboBox cbo = (ASPxComboBox)grid.FindEditFormTemplateControl(controlName);
            if (cbo != null)
                if (cbo.SelectedItem == null)
                    return null;
                else
                    return cbo.SelectedItem.Value;
            return null;
        }
        #endregion

        // Strings

        /// <summary>
        /// Builds string from List[string] using defined separator
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string BuildStringWithSeparator(List<string> list, string separator)
        {
            StringBuilder builder = new StringBuilder(100);
            int amount = list.Count;
            for (int i = 0; i < amount; i++)
            {
                if (i > 0) builder.Append(separator);
                builder.Append(list[i]);
            }
            return builder.ToString();
        }

        public static List<string> ConvertImportErrorsToStrings(List<ImportErrorMessage> list)
        {
            List<string> res = new List<string>(20);
            foreach (ImportErrorMessage iem in list)
            {
                string message = Utilities.GetResourceString("Validators", iem.Id);
                if (iem.Parameters.Count > 0)
                    message = string.Format(message, iem.Parameters.ToArray());
                res.Add(message);
            }
            return res;
        }

    }
}