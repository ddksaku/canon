using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Data.Business
{
    public class CanonImportPriceList : ImportPriceList
    {
        public static void InsertImportPriceList(ImportPriceList newValue)
        {
            CanonDataContext db = Cdb.Instance;
            db.ImportPriceLists.InsertOnSubmit(newValue);
            db.SubmitChanges();
        }
    }
}
