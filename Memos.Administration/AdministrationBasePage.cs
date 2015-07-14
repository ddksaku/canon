using System;
using System.Web.UI;

namespace Memos.Administration
{
    /// <summary>
    /// Base class for all pages in the Memos administration application.
    /// </summary>
    public class AdministrationBasePage : Page
    {
        #region OnLoad
        /// <summary>
        /// Logout if logged user is null.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (SessionManager.LoggedUser != null)
            {
                AdministrationUser user = SessionManager.LoggedUser;

                if (user.ValidateUser())
                {
                    return;
                }
            }

            Response.Redirect("Login.aspx");
        }
        #endregion
    }
}