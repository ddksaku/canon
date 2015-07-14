using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Web.Security;
using System.Web.Hosting;
using System.Linq;
using Canon.Data;
using Memos.Framework;
using CanonWebApp.Code.Exceptions;

namespace CanonWebApp.Code
{
    /// <summary>
    /// Canon membership provider.
    /// </summary>
    public class CanonMembershipProvider : MembershipProvider
    {

        #region Initialize
        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (String.IsNullOrEmpty(name))
            {
                name = "CanonMembershipProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Membership provider");
            }

            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);

            maxInvalidPasswordAttempts = Int32.Parse(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            passwordAttemptWindow = Int32.Parse(GetConfigValue(config["passwordAttemptWindow"], "10"));
            minRequiredNonAlphanumericCharacters = Int32.Parse(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            minRequiredPasswordLength = Int32.Parse(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            passwordStrengthRegularExpression = GetConfigValue(config["passwordStrengthRegularExpression"], "");
            enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));

            string tempFormat = config["passwordFormat"] ?? "Hashed";

            switch (tempFormat)
            {
                case "Hashed":
                    passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new InvalidOperationException("Password format not supported.");
            }
        }
        #endregion

        #region GetConfigValue
        /// <summary>
        /// Get confuguration value.
        /// </summary>
        /// <param name="configValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }
        #endregion


        #region Properties

        #region ApplicationName
        /// <summary>
        /// applicationName
        /// </summary>
        private string applicationName;
        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }
        #endregion

        #region EnablePasswordReset
        /// <summary>
        /// enablePasswordReset
        /// </summary>
        private bool enablePasswordReset;
        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }
        #endregion

        #region EnablePasswordRetrieval
        /// <summary>
        /// enablePasswordRetrieval
        /// </summary>
        private bool enablePasswordRetrieval;
        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }
        #endregion

        #region RequiresQuestionAndAnswer
        /// <summary>
        /// requiresQuestionAndAnswer
        /// </summary>
        private bool requiresQuestionAndAnswer;
        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }
        #endregion

        #region RequiresUniqueEmail
        /// <summary>
        /// requiresUniqueEmail
        /// </summary>
        private bool requiresUniqueEmail;
        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }
        #endregion

        #region MaxInvalidPasswordAttempts
        /// <summary>
        /// maxInvalidPasswordAttempts
        /// </summary>
        private int maxInvalidPasswordAttempts;
        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }
        #endregion

        #region PasswordAttemptWindow
        /// <summary>
        /// passwordAttemptWindow
        /// </summary>
        private int passwordAttemptWindow;
        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get { return passwordAttemptWindow; }
        }
        #endregion

        #region PasswordFormat
        /// <summary>
        /// passwordFormat
        /// </summary>
        private MembershipPasswordFormat passwordFormat;
        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }
        #endregion

        #region MinRequiredNonAlphanumericCharacters
        /// <summary>
        /// minRequiredNonAlphanumericCharacters
        /// </summary>
        private int minRequiredNonAlphanumericCharacters;
        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }
        #endregion

        #region MinRequiredPasswordLength
        /// <summary>
        /// minRequiredPasswordLength
        /// </summary>
        private int minRequiredPasswordLength;
        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }
        #endregion

        #region PasswordStrengthRegularExpression
        /// <summary>
        /// passwordStrengthRegularExpression
        /// </summary>
        private string passwordStrengthRegularExpression;
        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }
        #endregion

        #endregion


        #region CheckPassword
        /// <summary>
        /// Check password.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="dbPassword"></param>
        /// <returns></returns>
        public bool CheckPassword(string password, string dbPassword)
        {
            CanonDataContext db = Cdb.Instance;

            string pass1 = password;
            string pass2 = dbPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbPassword);
                    break;
                default:
                    break;
            }

            return String.Equals(pass1, pass2);
        }
        #endregion

        #region EncodePassword
        /// <summary>
        /// Encode password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HMACSHA1 hash = new HMACSHA1();
                    hash.Key = HexToByte("1234567812345678123512345678123456781235");
                    encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new InvalidOperationException("Unsupported password format.");
            }

            return encodedPassword;
        }
        #endregion

        #region UnEncodePassword
        /// <summary>
        /// Unencode password.
        /// </summary>
        /// <param name="encodedPassword"></param>
        /// <returns></returns>
        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new InvalidOperationException("Cannot unencode a hashed password.");
                default:
                    throw new InvalidOperationException("Unsupported password format.");
            }

            return password;
        }
        #endregion

        #region HexToByte
        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];

            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }
        #endregion

        #region GetUserByID
        /// <summary>
        /// Get user by ID.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public User GetUserByID(int userID)
        {
            CanonDataContext db = Cdb.Instance;

            var users = from u in db.Users
                        where u.UserId == userID
                        select u;

            if (users.Count() == 0)
            {
                return null;
            }

            return users.First();
        }
        #endregion

        #region GetUserByEmail
        /// <summary>
        /// Get user by email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            CanonDataContext db = Cdb.Instance;

            var users = from u in db.Users
                        where u.Email == email
                        select u;

            if (users.Count() == 0)
            {
                return null;
            }

            return users.First();
        }
        #endregion

        #region GetUserByLogin
        /// <summary>
        /// Get user by login.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public User GetUserByLogin(string login)
        {
            CanonDataContext db = Cdb.Instance;

            var users = from u in db.Users
                        where u.UserName == login
                        select u;

            if (users.Count() == 0)
            {
                return null;
            }

            return users.First();
        }
        #endregion

        #region CreateUser
        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="user"></param>
        public int CreateUser(User user)
        {
            CanonDataContext db = Cdb.Instance;

            if (Business.User.IsLoginExists(user.UserName))
            {
                throw new LoginExistsException(user.UserName);
            }

            string cleanPassword = string.Empty;
            //insert a new user
            user.IsForbidden = false;
            cleanPassword = user.Password;
            user.Password = EncodePassword(cleanPassword);
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();

            if (!string.IsNullOrEmpty(user.Email))
                EmailGateway.Send(
                   String.Empty,
                   user.Email,
                   Utilities.GetResourceString("Common", "EmailCredentialsSubject"),
                   String.Format(Utilities.GetResourceString("Common", "EmailCredentialsText"), user.UserName, cleanPassword),
                   new List<Attachment>());

            return user.UserId;
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// Update user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="previousLogin"></param>
        public void UpdateUser(User user, string previousLogin)
        {
            CanonDataContext db = Cdb.Instance;

            if (!String.Equals(user.UserName, previousLogin, StringComparison.CurrentCultureIgnoreCase))
            {
                if (Business.User.IsLoginExists(user.UserName))
                {
                    throw new LoginExistsException(user.UserName);
                }
            }

            var updatedUser = (from u in db.Users
                               where u.UserId == user.UserId
                               && ((u.IsForbidden == false)||(u.IsForbidden == null))
                               select u).Single();

            updatedUser.FullName = user.FullName;
            updatedUser.LastLogin = user.LastLogin;
            updatedUser.UserName = user.UserName;
            updatedUser.Email = user.Email;
            updatedUser.IsDailyEmail = user.IsDailyEmail;
            updatedUser.IsForbidden = false;
            updatedUser.IsActive = user.IsActive;

            if (!String.IsNullOrEmpty(user.Password))
            {
                updatedUser.Password = EncodePassword(user.Password);
            }
            db.SubmitChanges();
        }
        #endregion

        #region UpdateUserLastLogin
        /// <summary>
        /// Update user's last login.
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUserLastLogin(User user)
        {
            CanonDataContext db = Cdb.Instance;

            var updatedUser = (from u in db.Users
                               where u.UserId == user.UserId
                               select u).Single();

            updatedUser.LastLogin = DateTime.Now;

            db.SubmitChanges();
        }
        #endregion

        #region IsLoginExists
        /// <summary>
        /// Check if user's login exists.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public bool IsLoginExists(string login)
        {
            CanonDataContext db = Cdb.Instance;

            int count = db.Users.Count(u=> u.UserName == login && 
                                        ((u.IsForbidden == false) || 
                                        (u.IsForbidden == null)));

            return (count != 0);
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public void ChangePassword(int userID, string oldPassword, string newPassword)
        {
            CanonDataContext db = Cdb.Instance;

            var user = (from u in db.Users
                        where u.UserId == userID
                        select u).Single();

            if (!CheckPassword(oldPassword, user.Password))
            {
                throw new InvalidOldPasswordException(userID, oldPassword);
            }

            user.Password = EncodePassword(newPassword);

            db.SubmitChanges();

            EmailGateway.Send(
                String.Empty,
                user.Email,
                Utilities.GetResourceString("Common", "EmailPasswordChangingSubject"),
                String.Format(Utilities.GetResourceString("Common", "EmailPasswordChangingText"), newPassword),
                new List<Attachment>());
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Change password with new generated password.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>New generated password.</returns>
        public string ChangePassword(int userID)
        {
            CanonDataContext db = Cdb.Instance;

            var user = (from u in db.Users
                        where u.UserId == userID
                        select u).Single();

            string password = GeneratePassword(8);
            user.Password = EncodePassword(password);

            db.SubmitChanges();

            return password;
        }
        #endregion

        #region ValidateUser
        /// <summary>
        /// Validate user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override bool ValidateUser(string username, string password)
        {
            CanonDataContext db = Cdb.Instance;

            try
            {
                List<User> users = db.Users.Where(u => u.UserName == username).ToList();

                foreach (User user in users)
                {
                    if (user.IsForbidden == true || user.IsActive == false)
                        continue;

                    if (CheckPassword(password, user.Password))
                    {
                        SessionManager.LoggedUser = user;
                        UpdateUserLastLogin(user);
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region GeneratePassword
        /// <summary>
        /// Generates a new password.
        /// </summary>
        /// <param name="length">Length of the password.</param>
        /// <returns>New password.</returns>
        public string GeneratePassword(int length)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";

            StringBuilder passwordStringBuilder = new StringBuilder(length);
            Random random = new Random();
            int n;

            for (int i = 0; i < length; ++i)
            {
                int type = random.Next(3);
                switch (type)
                {
                    case 0:
                        n = random.Next(lower.Length - 1);
                        passwordStringBuilder.Append(lower[n]);
                        break;
                    case 1:
                        n = random.Next(upper.Length - 1);
                        passwordStringBuilder.Append(upper[n]);
                        break;
                    case 2:
                        n = random.Next(digits.Length - 1);
                        passwordStringBuilder.Append(digits[n]);
                        break;
                    default:
                        break;
                }
            }

            return passwordStringBuilder.ToString();
        }
        #endregion

        // Empty methods

        #region ChangePasswordQuestionAndAnswer
        /// <summary>
        /// ChangePasswordQuestionAndAnswer
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPasswordQuestion"></param>
        /// <param name="newPasswordAnswer"></param>
        /// <returns></returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region CreateUser
        /// <summary>
        /// CreateUser
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// DeleteUser
        /// </summary>
        /// <param name="username"></param>
        /// <param name="deleteAllRelatedData"></param>
        /// <returns></returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region FindUsersByEmail
        /// <summary>
        /// FindUsersByEmail
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region FindUsersByName
        /// <summary>
        /// FindUsersByName
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region GetAllUsers
        /// <summary>
        /// GetAllUsers
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region GetNumberOfUsersOnline
        /// <summary>
        /// GetNumberOfUsersOnline
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region GetPassword
        /// <summary>
        /// GetPassword
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string GetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region GetUser
        /// <summary>
        /// GetUser
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region GetUser
        /// <summary>
        /// GetUser
        /// </summary>
        /// <param name="providerUserKey"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region GetUserNameByEmail
        /// <summary>
        /// GetUserNameByEmail
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override string GetUserNameByEmail(string email)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region ResetPassword
        /// <summary>
        /// ResetPassword
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string ResetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region UnlockUser
        /// <summary>
        /// UnlockUser
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override bool UnlockUser(string userName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// UpdateUser
        /// </summary>
        /// <param name="user"></param>
        public override void UpdateUser(MembershipUser user)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}