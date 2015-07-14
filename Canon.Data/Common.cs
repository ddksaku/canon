using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Canon.Data
{
    public sealed class WebVariables
    {
        public static int LoggedUserId
        {
            get;
            set;
        }

        public static string ServerLanguage
        {
            get;
            set;
        }

        public static ILog Logger
        {
            get;
            set;
        }
    }
}
