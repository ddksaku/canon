using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Import;

namespace Canon.Data.Business
{
    public class CanonImportResellerRecord : ImportResellerRecord
    {
        public List<ImportErrorMessage> InsertImportResellerRecord()
        {
            List<ImportErrorMessage> warnings = new List<ImportErrorMessage>();

            CanonDataContext db = Cdb.Instance;

            // create reseller group if it doesn't exist yet
            ResellerGroup resellerGroup = db.ResellerGroups.FirstOrDefault(rg => rg.Code == this.ResellerGroupCode);
            if (resellerGroup == null)
            {
                resellerGroup = new ResellerGroup();
                resellerGroup.Code = this.ResellerGroupCode;
                resellerGroup.FileAs = "Nová_" + this.ResellerGroupCode;

                db.ResellerGroups.InsertOnSubmit(resellerGroup);
            }

            // update Reseller
            Reseller reseller = db.Resellers.FirstOrDefault(r => r.IdentificationNumber == this.IdentificationNumber);
            if (reseller == null)
            {
                reseller = new Reseller();

                db.Resellers.InsertOnSubmit(reseller);
            }

            reseller.IdentificationNumber = this.IdentificationNumber;
            reseller.FileAs = this.FileAs;
            reseller.ResellerGroup = resellerGroup;
            reseller.IDCountry = int.Parse(this.CountryCode);
            reseller.Code = string.Empty;

            // insert ImportReseller
            ImportResellerRecord record = new ImportResellerRecord();
            record.CountryCode = this.CountryCode;
            record.FileAs = this.FileAs;
            record.IdentificationNumber = this.IdentificationNumber;
            record.IDImportReseller = this.ImportReseller.ID;
            record.ResellerGroupCode = this.ResellerGroupCode;

            db.ImportResellerRecords.InsertOnSubmit(record);
            db.SubmitChanges();

            return warnings;
        }
    }
}
