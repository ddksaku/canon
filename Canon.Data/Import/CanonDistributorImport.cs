using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Canon.Data.Business;
using System.Globalization;
using Memos.Framework.Logging;
using Memos.Import.Excel;
using System.Data;
using System.IO;

namespace Canon.Data.Import
{
    public class CanonDistributorImport<T> where T : CanonImportDistributorRecord
    {
        private CultureInfo culture = CultureInfo.CreateSpecificCulture(WebVariables.ServerLanguage);
        private string _filename = string.Empty;
        private string _OriginalFilename = string.Empty;
        private int DistributorId = -1;

        public List<ImportErrorMessage> ErrorMessages{get;set;}

        public CanonDistributorImport(string filename, int distributorId, string origFilename)
        {
            _OriginalFilename = origFilename;
            _filename = filename;
            DistributorId = distributorId;
            ErrorMessages = new List<ImportErrorMessage>();
            Logger.Log(string.Format("New excel import is starting: {0}", filename), LogLevel.Info);
        }

        public List<T> ImportFile(ref bool IsSucceeded)
        {
            IsSucceeded = true;
            List<T> records = new List<T>(100);
            XlsParser parser = new XlsParser(new System.IO.FileInfo(_filename), 1, 0);
            try
            {
                List<DataRow> parsed = parser.Parse();
                foreach (DataRow dr in parsed)
                {
                    T instance = this.ParseRow(dr);
                    if (instance == null)
                    {
                        IsSucceeded = false;
                        continue;
                    }
                    records.Add(instance);
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Add(new ImportErrorMessage("XLSFileIncorrectFormatError"));

                IsSucceeded = false;
                WebVariables.Logger.Fatal("Exception during parsing", ex);
                Logger.Log(string.Format("Inner exception: {0}", ex.InnerException), LogLevel.Fatal);
            }

            return records;
        }

        public T ParseRow(object row)
        {
            if (!(row is DataRow))
                throw new ArgumentException("Only DataRow object can be passed.");

            DataRow dr = row as DataRow;

            // Date
            if (dr[0] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Datum" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "Date"));
                return null;
            }

            string date = dr[0].ToString();
            date = date.Replace("/", ".");

            WebVariables.Logger.Error(date);
            DateTime dDate = DateTime.Now;

            if (DateTime.TryParse(date, culture, DateTimeStyles.None, out dDate) == false)
            {
                ErrorMessages.Add(new ImportErrorMessage("InvalidDateValueError", new string[] { "Datum" }));
                WebVariables.Logger.Error(string.Format("File {0}, error invalid date field {1}", _filename, "Date"));
                return null;
            }

            // Reseller Identification Number
            if (dr[1] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "IČO" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "IdentificationNumber"));
                return null;
            }
            string identificationNumber = dr[1].ToString();
            if (string.IsNullOrEmpty(identificationNumber))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "IČO" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "IdentificationNumber"));
                return null;
            }
            long lIDentificationNumber = 0;
            if (long.TryParse(identificationNumber, out lIDentificationNumber) == false)
            {
                ErrorMessages.Add(new ImportErrorMessage("CantParseNumber", new string[] { identificationNumber }));
                WebVariables.Logger.Error(string.Format("File {0}, error number parse {1}", _filename, identificationNumber));
                return null;
            }

            // Reseller Name
            if (dr[2] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Zákazník" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ResellerName"));
                return null;
            }
            string resellerName = dr[2].ToString();
            if (string.IsNullOrEmpty(resellerName))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Zákazník" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ResellerName"));
                return null;
            }

            // Product Code
            if (dr[3] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Kódu produktu" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductCode"));
                return null;
            }
            string productCode = dr[3].ToString();
            if (string.IsNullOrEmpty(productCode))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Kódu produktu" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductCode"));
                return null;
            }

            // Product Name
            if (dr[4] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Název produktu" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductName"));
                return null;
            }
            string productName = dr[4].ToString();
            if (string.IsNullOrEmpty(productName))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Název produktu" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductName"));
                return null;
            }

            // Count of sold products
            if (dr[5] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Prodaných kusů" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "Quantity"));
                return null;
            }
            string quantity = dr[5].ToString();
            if (string.IsNullOrEmpty(quantity))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Prodaných kusů" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "Quantity"));
                return null;
            }

            int iQuantity = 0;
            if (int.TryParse(quantity, out iQuantity) == false)
            {
                ErrorMessages.Add(new ImportErrorMessage("CantParseNumber", new string[] { quantity }));
                WebVariables.Logger.Error(string.Format("File {0}, error number parse {1}", _filename, quantity));
            }

            // Product Price
            if (dr[6] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Cena" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductPrice"));
                return null;
            }
            string unitPrice = dr[6].ToString();
            if (string.IsNullOrEmpty(unitPrice))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Cena" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductPrice"));
                return null;
            }

            double dUnitPrice = 0;
            unitPrice = unitPrice.Replace(".", ",");
            if (double.TryParse(unitPrice, NumberStyles.Number, culture, out dUnitPrice) == false)
            {
                ErrorMessages.Add(new ImportErrorMessage("CantParseNumber", new string[] { unitPrice }));
                WebVariables.Logger.Error(string.Format("File {0}, error number parse {1}", _filename, unitPrice));
            }


            T a = Activator.CreateInstance<T>();
            a.Date = dDate;
            a.ResellerIdentificationNumber = lIDentificationNumber;
            a.ResellerName = resellerName;
            a.ProductCode = productCode;
            a.ProductName = productName;
            a.Quantity = iQuantity;
            a.UnitPrice = (decimal)dUnitPrice;

            return a;
        }

        public bool ExportToDb()
        {
            ErrorMessages.Clear();

            try
            {
                bool IsSucceeded = false;
                List<T> list = this.ImportFile(ref IsSucceeded);
                if (IsSucceeded == false)
                {
                    return false;
                }

                CanonDataContext db = Cdb.Instance;

                // Create ImportDistributor
                ImportDistributor importDistributor = new ImportDistributor();
                importDistributor.IDDistributor = DistributorId;
                importDistributor.IDUser = WebVariables.LoggedUserId;
                importDistributor.FileName = _OriginalFilename;
                importDistributor.DateImported = DateTime.Now;
                importDistributor.DateFrom = DateTime.Now;
                importDistributor.DateTo = importDistributor.DateFrom.AddDays(30);
                importDistributor.ErrorMessage = string.Empty;
                importDistributor.Succeeded = true;
                importDistributor.IsDeleted = false;

                CanonImportDistributor.InsertImportDistributor(importDistributor);

                // import Distributor records
                foreach(T record in list)
                {
                    try
                    {
                        record.ImportDistributor = importDistributor;
                        record.InsertImportDistributorRecord();
                    }
                    catch (Exception ex)
                    {
                        //add message about error
                        ErrorMessages.Add(new ImportErrorMessage("GeneralRecordImportError",
                                                new string[] { "Reseller", record.ResellerIdentificationNumber + "_" + record.ResellerName }));
                        //into log
                        WebVariables.Logger.Error(string.Format("File {0}, general import error", _filename), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Add(new ImportErrorMessage("GeneralFileImportError"));
                //into log
                WebVariables.Logger.Error(string.Format("File {0} ", _filename), ex);
                return false;
            }

            return true;
        }
    }
}
