using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CanonWebApp.Business;
using Memos.Framework.Logging;
using CanonWebApp.Code.Exceptions;
using System.Drawing;

namespace CanonWebApp.Controls
{
    public partial class AdminChangePassword : System.Web.UI.UserControl
    {
        public void SetUserId(int id)
        {
            UID = id;
        }

        public int UID
        {
            get
            {
                return Convert.ToInt32(ViewState["UID"]);
            }
            set
            {
                ViewState["UID"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void CallbackChangePasswordPanel_Callback(object source, DevExpress.Web.ASPxClasses.CallbackEventArgsBase e)
        {
            if (e.Parameter != null && e.Parameter == "ChangePassword")
            {
                string oldPassword = txtOldPassword.Text;
                string newPassword = txtNewPassword.Text;
                string newPasswordConfirm = txtNewPasswordConfirm.Text;

                lblError.ForeColor = Color.Red;

                if (UID != 0)
                {
                    if (newPassword.Trim() == string.Empty)
                    {
                        CallbackChangePasswordPanel.JSProperties["cpChangePasswordError"] = "Zadejte nové heslo.";
                        return;
                    }

                    if (newPassword != newPasswordConfirm)
                    {
                        CallbackChangePasswordPanel.JSProperties["cpChangePasswordError"] = "Zadaná hesla nejsou shodná.";
                        return;
                    }

                    try
                    {
                        User.ChangePassword(UID, oldPassword, newPassword);
                    }
                    catch(Exception ex)
                    {
                        if (ex is InvalidOldPasswordException)
                        {
                            CallbackChangePasswordPanel.JSProperties["cpChangePasswordError"] = "Zadejte původní heslo správně";
                        }
                        else
                        {
                            Logger.Log(string.Format("exception {0}", ex.ToString()), LogLevel.Error);
                        }

                        return;
                    }

                    lblError.ForeColor = Color.LightGreen;
                    CallbackChangePasswordPanel.JSProperties["cpChangePasswordError"] = "Heslo bylo úspěšně změněno";
                }
            }
        }
    }
}