using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Canon.Data.Exceptions;

namespace Canon.Data.Business
{
    public class CanonCategory : Category
    {
        public static void DeleteCategoryById(int id)
        {
            CanonDataContext db = Cdb.Instance;
            Category category = db.Categories.First(u => u.CategoryId == id);
            int productsCount = db.Products.Count(p => p.CategoryId == category.CategoryId);
            if (productsCount > 0)
                throw new ProductAssignedException(category.CategoryName);
            db.Categories.DeleteOnSubmit(category);
            db.SubmitChanges();
        }

        public static void UpdateCategoryById(int id, CanonCategory newValues)
        {
            CanonDataContext db = Cdb.Instance;
            Category cat = db.Categories.First(u => u.CategoryId == id);
            cat.CategoryName = newValues.CategoryName;
            cat.InternalId = newValues.InternalId;
            db.SubmitChanges();
        }

        public static void InsertCategory(CanonCategory newValues)
        {
            CanonDataContext db = Cdb.Instance;
            Category cat = new Category();
            cat.CategoryName = newValues.CategoryName;
            cat.InternalId = newValues.InternalId;
            db.Categories.InsertOnSubmit(cat);
            db.SubmitChanges();
        }

        public static bool IsCategoryCodeExist(string code)
        {
            Category cat = Cdb.Instance.Categories.FirstOrDefault(c=> c.InternalId == code);
            if (cat != null)
                return true;
            return false;
        }

        public static bool IsCategoryNameExist(string name)
        {
            Category cat = Cdb.Instance.Categories.FirstOrDefault(c => c.CategoryName == name);
            if (cat != null)
                return true;
            return false;
        }
    }
}
