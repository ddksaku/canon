using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Import;

namespace Canon.Data.Business
{
    public class CanonImportDistributorRecord : ImportDistributorRecord
    {
        public List<ImportErrorMessage> InsertImportDistributorRecord()
        {
            List<ImportErrorMessage> warnings = new List<ImportErrorMessage>();

            CanonDataContext db = Cdb.Instance;

            ImportDistributorRecord record = new ImportDistributorRecord();
            record.IDImportDistributor = this.ImportDistributor.ID;
            record.Date = this.Date;
            record.ResellerIdentificationNumber = this.ResellerIdentificationNumber;
            record.ResellerName = this.ResellerName;
            record.ProductCode = this.ProductCode;
            record.ProductName = this.ProductName;
            record.Quantity = this.Quantity;
            record.UnitPrice = this.UnitPrice;

            db.ImportDistributorRecords.InsertOnSubmit(record);
            db.SubmitChanges();

            return warnings;
        }
    }
}
