using System;

namespace CanonWebApp.Code.Exceptions
{
    /// <summary>
    /// Invalid old password exception.
    /// </summary>
    public class InvalidOldPasswordException : Exception
    {
        #region Constructor
        /// <summary>
        /// Creates a new instance of InvalidOldPasswordException.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="oldPassword"></param>
        public InvalidOldPasswordException(int userID, string oldPassword)
        {
            UserID = userID;
            OldPassword = oldPassword;
        }
        #endregion

        #region UserID
        /// <summary>
        /// User ID.
        /// </summary>
        public int UserID
        {
            get;
            set;
        }
        #endregion

        #region OldPassword
        /// <summary>
        /// Old password.
        /// </summary>
        public string OldPassword
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
                return String.Format("Old password '{0}' is invalid for user with ID '{1}'.", OldPassword, UserID);
            }
        }
        #endregion
    }
}