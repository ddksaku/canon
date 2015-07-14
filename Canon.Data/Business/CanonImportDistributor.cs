using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Data.Business
{
    public class CanonImportDistributor : ImportDistributor
    {
        public static void InsertImportDistributor(ImportDistributor newValue)
        {
            CanonDataContext db = Cdb.Instance;
            db.ImportDistributors.InsertOnSubmit(newValue);
            db.SubmitChanges();
        }

        public static void RemoveImportDistributor(int id)
        {
            CanonDataContext db = Cdb.Instance;
            ImportDistributor import = db.ImportDistributors.FirstOrDefault(d => d.ID == id);
            if (import != null)
            {
                import.IsDeleted = true;
                db.SubmitChanges();
            }
        }

    }
}
