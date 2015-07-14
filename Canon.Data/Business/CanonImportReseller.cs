using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Data.Business
{
    public class CanonImportReseller : ImportReseller
    {
        public static void InsertImportReseller(ImportReseller newValue)
        {
            CanonDataContext db = Cdb.Instance;
            db.ImportResellers.InsertOnSubmit(newValue);
            db.SubmitChanges();
        }
    }
}
