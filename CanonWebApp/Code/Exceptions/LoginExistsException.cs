using System;

namespace CanonWebApp.Code.Exceptions
{
    /// <summary>
    /// Login exists exception.
    /// </summary>
    public class LoginExistsException : Exception
    {
        #region Constructor
        /// <summary>
        /// Creates a new instance of LoginExistsException.
        /// </summary>
        /// <param name="login"></param>
        public LoginExistsException(string login)
        {
            Login = login;
        }
        #endregion

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

        #region Message
        /// <summary>
        /// Message of the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return String.Format("User login '{0}' already exists in the database.", Login);
            }
        }
        #endregion
    }
}