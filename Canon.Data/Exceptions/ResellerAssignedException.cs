using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Data.Exceptions
{
    public class ResellerAssignedException : Exception
    {
        #region constructor

        public ResellerAssignedException(string resellergroup, int count)
        {
            ResellerGroupName = resellergroup;
            ResellersCount = count;
        }

        #endregion

        #region ResellerGroupName

        public string ResellerGroupName
        {
            get;
            set;
        }

        #endregion

        #region Rellers Count

        public int ResellersCount
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
                return string.Format("Skupinu reselerů nelze odebrat, jsou k ní přiřazeny reselery ({0})!", ResellersCount);
            }
        }

        #endregion
    }
}
