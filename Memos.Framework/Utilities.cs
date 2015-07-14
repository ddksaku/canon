using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

namespace Memos.Framework
{
    /// <summary>
    /// Utilites class.
    /// </summary>
    public static class Utilities
    {
        #region Const

        /// <summary>
        /// Online users count key.
        /// </summary>
        const string ONLINE_USERS_COUNT_KEY = "OnlineUsersCount";

        #endregion

        // Properties

        #region OnlineUsersCount
        /// <summary>
        /// Count of online users.
        /// </summary>
        public static int OnlineUsersCount
        {
            get
            {
                int onlineUsersCount = 0;

                if (HttpContext.Current == null)
                {
                    return onlineUsersCount;
                }

                if (HttpContext.Current.Application[ONLINE_USERS_COUNT_KEY] != null)
                {
                    Int32.TryParse(
                        Convert.ToString(HttpContext.Current.Application[ONLINE_USERS_COUNT_KEY]),
                        out onlineUsersCount);

                    return onlineUsersCount;
                }

                return onlineUsersCount;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Application[ONLINE_USERS_COUNT_KEY] = value;
                }
            }
        }

        #endregion

        #region TempDirectory
        /// <summary>
        /// Temporary directory.
        /// </summary>
        public static string TempDirectory
        {
            get
            {
                string temp = HttpContext.Current.Server.MapPath("~/Temp");
                if (!Directory.Exists(temp))
                {
                    Directory.CreateDirectory(temp);
                }

                return temp;
            }
        }
        #endregion

        // Assembly version

        #region GetVersion
        /// <summary>
        /// Gets the version of the applicaiton from assembly.
        /// </summary>
        /// <returns>Version of the application.</returns>
        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        } 
        #endregion

        // Controls methods

        #region GetPostBackControl
        /// <summary>
        /// Returns control that caused postback.
        /// </summary>
        /// <param name="page">Page, whare we want to look for the control that caused postback</param>
        /// <returns>Returns control that caused postback or null if no postback occured</returns>
        public static Control GetPostBackControl(Page page)
        {
            if (page == null)
            {
                return null;
            }

            Control control = null;

            string ctrlname;

            if (page.IsCallback)
            {
                //__CALLBACKID is valid for devexpress callback and can be obtain using RenderUtils.CallbackControlIDParamName
                ctrlname = page.Request.Params.Get("__CALLBACKID");
            }
            else
            {
                ctrlname = page.Request.Params.Get(Page.postEventSourceID);
            }

            if (String.IsNullOrEmpty(ctrlname) == false)
            {
                //javascript submit
                control = page.FindControl(ctrlname);
            }
            else
            {
                //html submit
                foreach (string ctl in page.Request.Form)
                {
                    Control c;

                    if ((ctl.LastIndexOf(".x") > 0) || (ctl.LastIndexOf(".y") > 0))
                    {
                        //Submit of image button
                        c = page.FindControl(ctl.Substring(0, ctl.Length - 2));
                    }
                    else
                    {
                        //Submit of button
                        c = page.FindControl(ctl);
                    }
                   
                    if (c != null && c is IButtonControl)
                    {
                        control = c;
                        break;
                    }
                }
            }
            return control;
        }
        #endregion

        #region FindControlRecursive
        /// <summary>
        /// Finds the control specified by its ID in the control hiearchy.
        /// System.Web.UI.Control.FindControl() doesn't work, because it searches only on the first level
        /// </summary>
        /// <param name="root">Root of hiearchy, where searching starts</param>
        /// <param name="id">Id of the control we want to look for</param>
        /// <returns>Returns System.Web.UI.Control object or null if no contorl found</returns>
        public static Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
            {
                return root;
            }

            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
        #endregion

        #region FindChildControlsByType
        /// <summary>
        /// Returns an enumerable for child controls of given type.
        /// </summary>
        /// <typeparam name="T">Type of controls to search.</typeparam>
        /// <param name="control">Starting point.</param>
        /// <returns></returns>
        public static IEnumerable<T> FindChildControlsByType<T>(Control control) where T : class
        {
            if (control is T)
            {
                yield return control as T;
            }

            foreach (Control childControl in control.Controls)
            {
                foreach (T foundControl in FindChildControlsByType<T>(childControl))
                {
                    yield return foundControl;
                }
            }
        }
        #endregion

        #region SetListControlByValue
        /// <summary>
        /// Sets selected item for System.Web.UI.ListControl if we know text value of the item
        /// </summary>
        /// <param name="listControl">ListControl we want to set selected item</param>
        /// <param name="textValue">Text value of the item, we want to select</param>
        /// <param name="logIfNotFound">If true and textValue doesn't exist in the listControl, then we log it</param>
        public static void SetListControlByValue(ListControl listControl, object textValue, bool logIfNotFound)
        {
            if ((textValue == null) || (listControl == null))
                return;

            if (listControl.Items.FindByValue(textValue.ToString()) != null)
            {
                listControl.SelectedValue = textValue.ToString();
            }
            else
            {
                if (logIfNotFound)
                {

                }
            }
        }

        public static void SetListControlByValue(ListControl listControl, object textValue)
        {
            SetListControlByValue(listControl, textValue, true);
        }

        #endregion

        #region FocusControlOnPageLoad
        /// <summary>
        /// Inserts client script into the page, which handles that page is scrolled to the specified control
        /// </summary>
        /// <param name="ClientID">Id of the control, that the page will scroll to</param>
        /// <param name="page">Page, where the script is inserted</param>
        public static void FocusControlOnPageLoad(string ClientID, Page page)
        {
            if (page.ClientScript.IsClientScriptBlockRegistered("controlFocus"))
                return;

            page.ClientScript.RegisterClientScriptBlock(typeof(string), "controlFocus",
            @"<script> 
                  function ScrollView()
                  {
                     var el = document.getElementById('" + ClientID + @"')
                     if (el != null)
                     {        
                        el.scrollIntoView();
                        el.focus();
                     }
                  }

                  window.onload = ScrollView;

                  </script>");

        }

        #endregion

        //String functions

        #region TruncateString
        /// <summary>
        /// Truncates string to fit only excepted number of letters
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string TruncateString(string initial, int maxLength)
        {
            if (initial.Length <= maxLength)
                return initial;
            else
                return initial.Substring(0, maxLength);
        }
        #endregion

        #region ListToString
        public static string ListToString(List<int> list)
        {
            string result = string.Empty;
            foreach (int i in list)
                result += i.ToString() + ",";
            if (!string.IsNullOrEmpty(result))
                return result.Substring(0, result.Length - 1);
            return result;
        }
        #endregion

        //File functions

        #region DeleteOldFiles
        /// <summary>
        /// Deletes old files
        /// </summary>
        /// <param name="path">Directory name where to delete</param>
        /// <param name="mask">Mask as "*.*", "*.xls" and so on</param>
        /// <param name="ticksToDelete">Number of ticks to delete</param>
        /// <returns>Count of deleted files</returns>
        public static int DeleteOldFiles(string path, string mask, DateTime timeToDelete)
        {
            //  Make  a  reference  to  a  directory.
            DirectoryInfo di = new DirectoryInfo(path);
            if (di.Exists == false) throw new FileNotFoundException(string.Format("Directory {0} doesn't exists.",
                                                                    path));
            int numberOfDeleted = 0;

            //delete old files
            FileInfo[] files = di.GetFiles(mask);
            foreach (FileInfo fiNext in files)
            {
                if (fiNext.Exists)
                {
                    DateTime thisFileTime = fiNext.LastWriteTime;
                    //logger.Warn(string.Format("File {0}, this ticks {1}, to delete {2}", fiNext.Name, thisFileTime.ToString(), timeToDelete.ToString()));
                    if (thisFileTime < timeToDelete)
                    {
                        System.IO.File.Delete(System.IO.Path.Combine(path, fiNext.Name));
                        numberOfDeleted++;
                    }//if ticks
                }//if exist
            }//foreach

            return numberOfDeleted;
        }
        #endregion

    }
}