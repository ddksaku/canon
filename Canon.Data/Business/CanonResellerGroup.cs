using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Exceptions;

namespace Canon.Data.Business
{
    public class CanonResellerGroup : ResellerGroup
    {
        public static void DeleteResellerGroupById(int id)
        {
            CanonDataContext db = Cdb.Instance;
            ResellerGroup rg = db.ResellerGroups.FirstOrDefault(r => r.ID == id);

            if (rg.Resellers.Count > 0)
            {
                throw new ResellerAssignedException(rg.FileAs, rg.Resellers.Count);
            }

            db.ResellerGroups.DeleteOnSubmit(rg);
            db.SubmitChanges();
        }

        public static void InsertOrUpdateResellerGroup(CanonResellerGroup newValue)
        {
            CanonDataContext db = Cdb.Instance;
            ResellerGroup rg = null;

            if (newValue.ID == -1)
            {
                rg = new ResellerGroup();
                db.ResellerGroups.InsertOnSubmit(rg);
            }
            else
            {
                rg = db.ResellerGroups.FirstOrDefault(r => r.ID == newValue.ID);
            }

            rg.FileAs = newValue.FileAs;
            rg.Code = newValue.Code;

            db.SubmitChanges();
        }

        public static bool ExistAlreadyIdentifier(string identifier, int id)
        {
            CanonDataContext db = Cdb.Instance;

            if (db.ResellerGroups.Any(rg => rg.Code == identifier && rg.ID != id))
                return true;

            return false;
        }
    }
}
