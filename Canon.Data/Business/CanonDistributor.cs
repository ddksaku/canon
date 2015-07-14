using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Exceptions;

namespace Canon.Data.Business
{
    public class CanonDistributor : Distributor
    {
        public static void DeleteDistributorById(int id)
        {
            CanonDataContext db = Cdb.Instance;
            Distributor distributor = db.Distributors.FirstOrDefault(d => d.ID == id);
            
            if(distributor.ImportDistributors.Count > 0)
            {
                throw new ImportAssignedException(distributor.FileAs, distributor.ImportDistributors.Count);
            }

            db.Distributors.DeleteOnSubmit(distributor);
            db.SubmitChanges();
        }

        public static void InsertOrUpdateDistributor(CanonDistributor newValue)
        {
            CanonDataContext db = Cdb.Instance;
            
            Distributor distributor = null;
            if (newValue.ID == -1)
            {
                distributor = new Distributor();
                db.Distributors.InsertOnSubmit(distributor);
            }
            else
            {
                distributor = db.Distributors.FirstOrDefault(d => d.ID == newValue.ID);
            }
            
            distributor.FileAs = newValue.FileAs;
            distributor.IDDistributorType = newValue.IDDistributorType;
            distributor.Note = newValue.Note;
            distributor.ShowInImports = newValue.ShowInImports;
            distributor.ShowInReports = newValue.ShowInReports;

            db.SubmitChanges();
        }

    }
}
