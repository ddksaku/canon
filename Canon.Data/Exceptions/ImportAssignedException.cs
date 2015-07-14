using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Data.Exceptions
{
    public class ImportAssignedException : Exception
    {
        #region Constructor

        public ImportAssignedException(string distributor, int count)
        {
            DistributorName = distributor;
            ImportCount = count;
        }

        #endregion

        #region Distributor Name

        public string DistributorName
        {
            get;
            set;
        }

        #endregion

        #region Import Count

        public int ImportCount
        {
            get;
            set;
        }

        #endregion

        #region Message

        public override string Message
        {
            get
            {
                return string.Format("Distributora nelze odebrat, jsou k němu přiřazeny importy ({0})!", ImportCount);
            }
        }

        #endregion
    }
}
