using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memos.Import;
using Canon.Data.Business;
using log4net;

namespace Canon.Data.Import
{
    public abstract class CommonParser<T> : ICommonParser<T> where T : CanonChannelMonitor
    {
        public int TryedRecords { get; set; }
        public int SuccessRecords { get; set; }

        private static ILog logger = null;

        /// <summary>
        /// Log4Net logger
        /// </summary>
        public ILog Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        public abstract List<T> ImportFile();
        public abstract T ParseRow(Object row);
        public abstract bool ExportToDb();
    }
}
