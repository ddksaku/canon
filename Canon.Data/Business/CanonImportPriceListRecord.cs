using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Import;

namespace Canon.Data.Business
{
    public class CanonImportPriceListRecord : ImportPriceListRecord
    {
        public List<ImportErrorMessage> InsertImportPriceListRecord(CanonDataContext db)
        {
            List<ImportErrorMessage> warnings = new List<ImportErrorMessage>();

            // CanonDataContext db = Cdb.Instance;

            // Create Product Group if it doesn't exist yet
            ProductGroup productGroup = db.ProductGroups.FirstOrDefault(pg => pg.Code == this.ProductCategory);
            if (productGroup == null)
            {
                productGroup = new ProductGroup();
                productGroup.Code = this.ProductCategory;
                productGroup.FileAs = "Nová_" + this.ProductCategory;

                db.ProductGroups.InsertOnSubmit(productGroup);
            }

            // create Product type if it doesn't exist in DB yet
            ProductType pType = db.ProductTypes.FirstOrDefault(pt => pt.Type == this.ProductType);
            if (pType == null)
            {
                pType = new ProductType();
                pType.Type = this.ProductType;

                db.ProductTypes.InsertOnSubmit(pType);
            }

            // update product
            Product product = db.Products.FirstOrDefault(p => p.ProductCode == this.ProductCode);
            if (product == null)
            {
                product = new Product();
                product.IsActive = true;
                
                db.Products.InsertOnSubmit(product);
            }

            product.ProductCode = this.ProductCode;
            product.ProductName = this.ProductName;
            product.ProductGroup = productGroup;
            product.CurrentPrice = this.ListPrice;
            product.ProductType = pType;

            // insert ImportPriceListRecord
            ImportPriceListRecord record = new ImportPriceListRecord();
            record.IDImportPriceList = this.ImportPriceList.ID;
            record.ListPrice = this.ListPrice;
            record.ProductCode = this.ProductCode;
            record.ProductName = this.ProductName;
            record.ProductCategory = this.ProductCategory;
            record.ProductType = this.ProductType;
            
            db.ImportPriceListRecords.InsertOnSubmit(record);
            db.SubmitChanges();

            return warnings;
        }
    }
}
