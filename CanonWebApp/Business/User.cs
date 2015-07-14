using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using Memos.Framework;
using Memos.Framework.Logging;
using Canon.Data;
using Canon.Data.Enums;
using CanonWebApp.Code;
using SessionManager=CanonWebApp.Code.SessionManager;
using Utilities=CanonWebApp.Code.Utilities;

namespace CanonWebApp.Business
{
    /// <summary>
    /// User business object class.
    /// </summary>
    public class User
    {
        #region GetAllUsers
        /// <summary>
        /// Get user's list.
        /// </summary>
        public List<Canon.Data.User> GetAllUsersList()
        {
            var users = from u in Cdb.Instance.Users
                        orderby u.FullName, u.UserName ascending
                        select u;

            return users.ToList();
        }
        #endregion

        #region GetUserByID
        /// <summary>
        /// Get user by ID.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Canon.Data.User GetUserByID(int userID)
        {
            Canon.Data.User user = ((CanonMembershipProvider) Membership.Provider).GetUserByID(userID);

            if (user == null)
            {
                Logger.Log("User with specified ID doesn't exists in database. User ID:'" + userID + "'", LogLevel.Warn);
            }

            return user;
        }
        #endregion

        #region GetUserByEmail
        /// <summary>
        /// Get user by email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static Canon.Data.User GetUserByEmail(string email)
        {
            return ((CanonMembershipProvider)Membership.Provider).GetUserByEmail(email);
        }
        #endregion

        #region GetUserByLogin
        /// <summary>
        /// Get user by login.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static Canon.Data.User GetUserByLogin(string login)
        {
            return ((CanonMembershipProvider)Membership.Provider).GetUserByLogin(login);
        }
        #endregion


        #region InsertUser
        /// <summary>
        /// Insert new User.
        /// </summary>
        /// <param name="user"></param>
        public static int InsertUser(Canon.Data.User user)
        {
            int result = ((CanonMembershipProvider)Membership.Provider).CreateUser(user);
            CleanUsersCache();
            return result;
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// Update user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="previousLogin"></param>
        public static void UpdateUser(Canon.Data.User user, string previousLogin)
        {
            ((CanonMembershipProvider)Membership.Provider).UpdateUser(user, previousLogin);

            CleanUsersCache();
        }
        #endregion

        #region DeleteUserById
        /// <summary>
        /// Delete user by id.
        /// </summary>
        /// <param name="userId"></param>
        public static void DeleteUserById(int userId)
        {
            //user deletion
            CanonDataContext db = Cdb.Instance;
            Canon.Data.User user = db.Users.First(u => u.UserId==userId);
            //db.Users.DeleteOnSubmit(user);
            user.IsForbidden = true;
            db.SubmitChanges();
            CleanUsersCache();
        }
        #endregion

        #region UpdateCategoryRelation
        public static void UpdateCategoryRelation(int userId, int catId, bool relationExist)
        {
            CanonDataContext db = Cdb.Instance;
            try
            {
                UsersCategory uc = db.UsersCategories.First(u => u.UserId == userId && u.CategoryId == catId);
                if (!relationExist)
                    db.UsersCategories.DeleteOnSubmit(uc);
            }
            catch (Exception)
            {
                //there is no suc relation
                if (relationExist)
                {
                    UsersCategory newUc = new UsersCategory();
                    newUc.CategoryId = catId;
                    newUc.UserId = userId;
                    db.UsersCategories.InsertOnSubmit(newUc);
                }
            }
            db.SubmitChanges();
        }
        #endregion

        #region UpdateRightsRelation
        public static void UpdateRightsRelation(int userId, int rightId, bool relationExist)
        {
            CanonDataContext db = Cdb.Instance;
            try
            {
                UsersRight uc = db.UsersRights.First(u => u.UserId == userId && u.Rights == rightId);
                if (!relationExist)
                    db.UsersRights.DeleteOnSubmit(uc);
            }
            catch (Exception ex)
            {
                //there is no such relation
                if (relationExist)
                {
                    UsersRight newUc = new UsersRight();
                    newUc.Rights = rightId;
                    newUc.UserId = userId;
                    db.UsersRights.InsertOnSubmit(newUc);
                }
            }
            db.SubmitChanges();
        }
        #endregion

        #region SendRemindPasswordEmail
        /// <summary>
        /// Change email with the password to the user.
        /// </summary>
        /// <param name="email"></param>
        public static bool SendRemindPasswordEmail(string email)
        {
            Canon.Data.User user = GetUserByEmail(email);

            if (user != null)
            {
                string password = ((CanonMembershipProvider)Membership.Provider).ChangePassword(user.UserId);

                string changePasswordUrl = String.Concat(
                    HttpContext.Current.Request.Url.Scheme,
                    "://",
                    HttpContext.Current.Request.Url.Host,
                    ":",
                    HttpContext.Current.Request.Url.Port,
                    HttpContext.Current.Request.ApplicationPath,
                    "/Default.aspx?changepassword=yes");

                try
                {
                    EmailGateway.Send(
                        String.Empty,
                        email,
                        Utilities.GetResourceString("Common", "EmailRemindEmailSubject"),
                        String.Format(Utilities.GetResourceString("Common", "EmailRemindEmailText"), password, changePasswordUrl, user.UserName),
                        new List<Attachment>(),
                        true);
                }
                catch
                {
                    return false;
                }

                return true;
            }

            return false;
        }
        #endregion

        #region IsLoginExists
        /// <summary>
        /// Check if user's login already exists.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static bool IsLoginExists(string login)
        {
            return ((CanonMembershipProvider)Membership.Provider).IsLoginExists(login);
        } 
        #endregion

        #region ChangePassword
        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public static void ChangePassword(int userID, string oldPassword, string newPassword)
        {
            ((CanonMembershipProvider)Membership.Provider).ChangePassword(userID, oldPassword, newPassword);
        }
        #endregion

        #region CleanUsersCache
        /// <summary>
        /// Clean cache for user objects.
        /// </summary>
        public static void CleanUsersCache()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Cache.Remove(getUsersListKey);
            }
        }
        #endregion

        #region GetUsersList
        /// <summary>
        /// Cache key for GetUsersList method.
        /// </summary>
        const string getUsersListKey = "GetUsersListKey";
        /// <summary>
        /// Get users list.
        /// </summary>
        /// <returns></returns>
        public object GetUsersList()
        {
            object usersList = HttpContext.Current.Cache.Get(getUsersListKey);

            if (usersList == null)
            {
                CanonDataContext db = Cdb.Instance;

                var users = from u in db.Users
                            orderby u.FullName, u.UserName ascending
                            select new { u.UserId, Name = u.FullName };

                usersList = users.ToList();

                HttpContext.Current.Cache.Insert(getUsersListKey, usersList);
            }

            return usersList;
        }
        #endregion

        public static Dictionary<UserRightsEnum, string> GetAllRightsList()
        {
            Dictionary<UserRightsEnum, string> result = new Dictionary<UserRightsEnum, string>(5);
            var enums = Cdb.Instance.Enums.Where(r => r.EnumType == 4);
            foreach (Canon.Data.Enum en in enums)
                result.Add((UserRightsEnum)en.EnumId, en.NameCz);
                           
            return result;
        }

        public static Dictionary<UserRightsEnum, string> GetRightsList(int userId)
        {
            Dictionary<UserRightsEnum, string> result = new Dictionary<UserRightsEnum, string>(6);
            var enums = Cdb.Instance.UsersRights.Where(r => r.UserId == userId);
            foreach (Canon.Data.UsersRight en in enums)
                result.Add((UserRightsEnum)en.Rights, en.Enum.NameCz);
            return result;
        }

    }
}