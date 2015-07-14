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
    public class CanonResellersImport<T> where T : CanonImportResellerRecord
    {
        private CultureInfo culture = CultureInfo.CreateSpecificCulture(WebVariables.ServerLanguage);
        private string _filename = string.Empty;
        private string _OriginalFilename = string.Empty;

        public List<ImportErrorMessage> ErrorMessages { get; set; }

        public CanonResellersImport(string filename, string origFilename)
        {
            _filename = filename;
            _OriginalFilename = origFilename;
            ErrorMessages = new List<ImportErrorMessage>();
            Logger.Log(string.Format("New excel import is starting: {0}", filename), LogLevel.Info);
        }

        public List<T> ImportFile(ref bool IsSucceeded)
        {
            IsSucceeded = true;
            List<T> resellers = new List<T>(100);
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
                    resellers.Add(instance);
                }
            }
            catch(Exception ex)
            {
                ErrorMessages.Add(new ImportErrorMessage("XLSFileIncorrectFormatError"));

                IsSucceeded = false;
                WebVariables.Logger.Fatal("Exception during parsing", ex);
                Logger.Log(string.Format("Inner exception: {0}", ex.InnerException), LogLevel.Fatal);
            }

            return resellers;
        }

        public T ParseRow(object row)
        {
            if (!(row is DataRow))
                throw new ArgumentException("Only DataRow object can be passed.");

            DataRow dr = row as DataRow;

            // Identification Number
            if (dr[0] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "IČO" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "IdentificationNumber"));
                return null;
            }
            string identificationNumber = dr[0].ToString();
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
            if (dr[1] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Název" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ResellerName"));
                return null;
            }
            string resellerName = dr[1].ToString();
            if (string.IsNullOrEmpty(resellerName))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Název" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ResellerName"));
                return null;
            }

            // Reseller Group Code
            if (dr[2] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Skupina reselerů" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ResellerGroup"));
                return null;
            }
            string resellerGroupCode = dr[2].ToString();
            if (string.IsNullOrEmpty(resellerGroupCode))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Skupina reselerů" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ResellerGroup"));
                return null;
            }

            // Country ID
            if (dr[3] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "ID Země" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "CountryID"));
                return null;
            }
            string countryID = dr[3].ToString();
            if (string.IsNullOrEmpty(countryID))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "ID Země" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "CountryID"));
                return null;
            }

            int iCountryID = 0;
            if (int.TryParse(countryID, out iCountryID) == false)
            {
                ErrorMessages.Add(new ImportErrorMessage("CantParseNumber", new string[] { countryID }));
                WebVariables.Logger.Error(string.Format("File {0}, error number parse {1}", _filename, countryID));
                return null;
            }

            if (iCountryID != 1 && iCountryID != 2)
            {
                ErrorMessages.Add(new ImportErrorMessage("InvalidCountryIDError", new string[] { countryID }));
                WebVariables.Logger.Error(string.Format("File {0}, error invalid countryId {1}", _filename, countryID));
                return null;
            }

            T a = Activator.CreateInstance<T>();
            a.IdentificationNumber = lIDentificationNumber;
            a.FileAs = resellerName;
            a.ResellerGroupCode = resellerGroupCode;
            a.CountryCode = countryID;

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

                // Create ImportReseller
                ImportReseller importReseller = new ImportReseller();
                importReseller.IDUser = WebVariables.LoggedUserId;
                importReseller.FileName = _OriginalFilename;
                importReseller.DateImported = DateTime.Now;
                importReseller.Succeeded = true;

                CanonImportReseller.InsertImportReseller(importReseller);

                // import resellers
                foreach (T record in list)
                {
                    try
                    {
                        record.ImportReseller = importReseller;
                        record.InsertImportResellerRecord();
                    }
                    catch (Exception ex)
                    {
                        //add message about error
                        ErrorMessages.Add(new ImportErrorMessage("GeneralRecordImportError",
                                                new string[] { "Reseler", record.FileAs }));
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
