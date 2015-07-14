using System;

namespace Memos.Administration
{
    /// <summary>
    /// AdministrationUser user business object class.
    /// </summary>
    public class AdministrationUser
    {
        #region AdministrationUser
        /// <summary>
        /// Administration user constructor.
        /// </summary>
        public AdministrationUser(string login, string password)
        {
            Login = login;
            Password = password;
        } 
        #endregion

        #region Properties

        #region Login
        /// <summary>
        /// Login of the user.
        /// </summary>
        public string Login
        {
            get;
            set;
        }
        #endregion

        #region Password
        /// <summary>
        /// Password of the user.
        /// </summary>
        public string Password
        {
            get;
            set;
        }
        #endregion

        #endregion

        #region ValidateUser
        /// <summary>
        /// Validate administration user.
        /// </summary>
        /// <returns></returns>
        public bool ValidateUser()
        {
            if (String.Equals(Login, ConfigSettings.Login) &&
                String.Equals(Password, ConfigSettings.Password))
            {
                return true;
            }

            return false;
        } 
        #endregion
    }
}