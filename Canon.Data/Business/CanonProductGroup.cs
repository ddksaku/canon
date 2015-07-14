using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Exceptions;

namespace Canon.Data.Business
{
    public class CanonProductGroup : ProductGroup
    {
        public static void DeleteProductGroupById(int id)
        {
            CanonDataContext db = Cdb.Instance;
            ProductGroup pg = db.ProductGroups.FirstOrDefault(p => p.ID == id);

            if (pg.Products.Count > 0)
                throw new ProductAssignedException(pg.FileAs, pg.Products.Count);
            
            db.ProductGroups.DeleteOnSubmit(pg);
            db.SubmitChanges();
        }

        public static void InsertOrUpdateProductGroup(CanonProductGroup newValue)
        {
            CanonDataContext db = Cdb.Instance;

            ProductGroup pg = null;
            if (newValue.ID == -1)
            {
                pg = new ProductGroup();
                db.ProductGroups.InsertOnSubmit(pg);
            }
            else
            {
                pg = db.ProductGroups.FirstOrDefault(p => p.ID == newValue.ID);
            }

            pg.FileAs = newValue.FileAs;
            pg.Code = newValue.Code;

            db.SubmitChanges();
        }

        public static bool ExistAlreadyIdentifier(string identifier, int id)
        {
            CanonDataContext db = Cdb.Instance;

            if (db.ProductGroups.Any(pg => pg.Code == identifier && pg.ID != id))
                return true;

            return false;
        }
    }
}
