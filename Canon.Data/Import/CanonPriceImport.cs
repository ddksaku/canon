using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memos.Import;
using Canon.Data.Business;
using System.Globalization;
using Memos.Framework.Logging;
using Memos.Import.Excel;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Canon.Data.Import
{
    public class CanonPriceImport<T> where T : CanonImportPriceListRecord
    {
        private CultureInfo culture = CultureInfo.CreateSpecificCulture(WebVariables.ServerLanguage);
        private string _filename = string.Empty;
        private string _OriginalFilename = string.Empty;

        /// <summary>
        /// warnings and errors during import
        /// </summary>
        public List<ImportErrorMessage> ErrorMessages{get;set;}

        public CanonPriceImport(string filename, string origFilename)
        {
            _filename = filename;
            _OriginalFilename = origFilename;
            ErrorMessages = new List<ImportErrorMessage>();
            Logger.Log(string.Format("New excel import is starting: {0}", filename), LogLevel.Info);
        }

        /// <summary>
        /// Reads initial file and imports data
        /// </summary>
        /// <returns></returns>
        public List<T> ImportFile(ref bool IsSucceeded)
        {
            IsSucceeded = true;
            List<T> prices = new List<T>(100);
            XlsParser parser = new XlsParser(new System.IO.FileInfo(_filename), 2, 0);
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
                    prices.Add(instance);
                }
            }
            catch(Exception ex)
            {
                ErrorMessages.Add(new ImportErrorMessage("XLSFileIncorrectFormatError"));

                IsSucceeded = false;
                WebVariables.Logger.Fatal("Exception during parsing", ex);
                Logger.Log(string.Format("Inner exception: {0}", ex.InnerException), LogLevel.Fatal);
            }

            return prices;
        }
        
        public T ParseRow(object row)
        {
            if (!(row is DataRow))
                throw new ArgumentException("Only DataRow object can be passed.");

            DataRow dr = row as DataRow;

            // Product Code
            if (dr[4] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Kódu produktu" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductCode"));
                return null;
            }
            string productCode = dr[4].ToString();
            if (string.IsNullOrEmpty(productCode))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Kódu produktu" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductCode"));
                return null;
            }

            // Product Name
            string productName;
            if (dr[6] == null)
            {
                productName = string.Empty;
            }
            else
            {
                productName = dr[6].ToString();
            }

            if (string.IsNullOrEmpty(productName.Trim()))
            {
                productName = string.Format("{0} ({1})", productCode, "Chybí název");
                //ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Název" }));
                //WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductName"));
                //return null;
            }

            // Product Price
            string productPrice;
            if (dr[11] == null)
            {
                productPrice = string.Empty;
            }
            else
            {
                productPrice = dr[11].ToString();
            }

            if (string.IsNullOrEmpty(productPrice.Trim()))
            {
                productPrice = "0";
            }

            double dPrice = 0;
            productPrice = productPrice.Replace(".", ",");
            if (double.TryParse(productPrice, NumberStyles.AllowDecimalPoint, culture, out dPrice) == false)
            {
                ErrorMessages.Add(new ImportErrorMessage("CantParseNumber", new string[]{productPrice}));
                WebVariables.Logger.Error(string.Format("File {0}, error number parse {1}", _filename, productPrice));
                return null;
            }

            // Product Group
            if (dr[8] == null)
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Produktová skupina" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductGroup"));
                return null;
            }
            string productGroupCode = dr[8].ToString();
            if (string.IsNullOrEmpty(productGroupCode))
            {
                ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Produktová skupina" }));
                WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductGroup"));
                return null;
            }

            // Product Type
            string productType;
            if (dr[0] == null)
            {
                productType = string.Empty;
                //ErrorMessages.Add(new ImportErrorMessage("ColumnIsEmptyError", new string[] { "Typ produktu" }));
                //WebVariables.Logger.Error(string.Format("File {0}, error empty field {1}", _filename, "ProductType"));
                //return null;
            }
            else
            {
                productType = dr[0].ToString();
            }

            if (string.IsNullOrEmpty(productType.Trim()))
            {
                productType = "Other";
            }

            T a = Activator.CreateInstance<T>();
            a.ProductCode = productCode;
            a.ProductName = productName;
            a.ProductCategory = productGroupCode;
            a.ListPrice = (decimal)dPrice;
            a.ProductType = productType;

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

                // Create ImportPriceList
                ImportPriceList priceList = new ImportPriceList();
                priceList.IDUser = WebVariables.LoggedUserId;
                priceList.FileName = _OriginalFilename;
                priceList.DateImported = DateTime.Now;
                priceList.ErrorMessage = string.Empty;
                priceList.Succeeded = true;

                CanonImportPriceList.InsertImportPriceList(priceList);

                // import products
                foreach (T productPrice in list)
                {
                    try
                    {
                        productPrice.ImportPriceList = priceList;
                        productPrice.InsertImportPriceListRecord(db);
                    }
                    catch (Exception ex)
                    {
                        //add message about error
                        ErrorMessages.Add(new ImportErrorMessage("GeneralRecordImportError",
                                                new string[] { "Produkt", productPrice.ProductName }));
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
