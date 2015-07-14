using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using CanonWebApp.Code;
using Canon.Data;
using System.Configuration;
using System.Reflection;
using Memos.Framework;
using Memos.Framework.Logging;
using log4net;
using CanonWebApp.Controls;

namespace CanonWebApp
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Logger.assembly = Assembly.GetExecutingAssembly();

            CanonWebApp.Code.Utilities.InitializeCulture();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // vskarda
            Session.Timeout = 1440;

            // grid localizer
            MyGridLocalizer.Activate();
            MyWebLocalizer.Activate();
            MyEditorsLocalizer.Activate();

            Cdb.ConnectionString = ConfigurationManager.ConnectionStrings["CanonConnectionStringMain"].ToString();
            //delete old files
            ILog logger = LogManager.GetLogger(typeof(Global));
            try
            {
                int delNum = Memos.Framework.Utilities.DeleteOldFiles(
                                             Server.MapPath(CanonWebApp.Code.ConfigSettings.UploadDirectory),
                                             "*.*",
                                             DateTime.Now.AddDays(-2));
                Logger.Log(string.Format("Deleted {0} old files", delNum), LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Fatal);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}