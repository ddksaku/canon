using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Globalization;
using System.Threading;
using Memos.Import;
using Memos.Import.Excel;
using Memos.Framework.Logging;
using Canon.Data.Business;

namespace Canon.Data.Import
{
    public class CanonProductImport<T> : ICommonParser<T> where T : CanonProduct
    {
        private CultureInfo culture = CultureInfo.CreateSpecificCulture(WebVariables.ServerLanguage);
        private string _filename = string.Empty;

        /// <summary>
        /// Warnings and Errors during import
        /// </summary>
        public List<ImportErrorMessage> ErrorMessages { get; set; }

        public CanonProductImport(string filename)
        {
            _filename = filename;
            ErrorMessages = new List<ImportErrorMessage>();
            Logger.Log(string.Format("New excel import is starting: {0}", filename), LogLevel.Info);
        }

        /// <summary>
        /// Reads initial file and imports data
        /// </summary>
        /// <returns></returns>
        public List<T> ImportFile()
        {
            List<T> products = new List<T>(100);
            XlsParser parser = new XlsParser(new System.IO.FileInfo(_filename), 6, 0);
            try
            {
                List<DataRow> parsed = parser.Parse();
                //Go throw rows and validate each row
                foreach (DataRow dr in parsed)
                {
                    T instance = this.ParseRow(dr);
                    if (instance == null) continue;
                    products.Add(instance);
                }
            }
            catch (Exception ex)
            {
                WebVariables.Logger.Fatal("Exception during parsing", ex);
                Logger.Log(string.Format("Inner exception: {0}", ex.InnerException.Message), LogLevel.Fatal);
            }
            return products;
        }

        /// <summary>
        /// Parses a row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public T ParseRow(Object row)
        {
            if (!(row is DataRow))
                throw new ArgumentException("Only DataRow object can be passed.");

            DataRow dr = row as DataRow;
            //get, validate EAN code
            if (dr[0] == null) 
                return null;
            string ean = dr[0].ToString();
            if ((string.IsNullOrEmpty(ean)) || (ean.Length < 10) || (ean.Length > 13))
                return null;
            //get, validate product name
            if (dr[1] == null)
                return null;
            string name = dr[1].ToString();
            if (string.IsNullOrEmpty(name))
                return null;
            //get, validate recommended price
            if (dr[5] == null)
                return null;
            string price = dr[5].ToString();
            //synonims, stopwords
            string synonims = string.Empty;
            string stopwords = string.Empty;
            if (dr[13] != null)
                synonims = dr[13].ToString();
            if (dr[14] != null)
                stopwords = dr[14].ToString();
            //category
            string category = string.Empty;
            if (dr[12] != null)
                category = dr[12].ToString();
            double dPrice = 0;
            if (!string.IsNullOrEmpty(price))
            {
                try
                {
                    dPrice = double.Parse(price, culture.NumberFormat);
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add(new ImportErrorMessage("CantParseNumber", new string[]{ price.ToString() }));
                    WebVariables.Logger.Error(string.Format("File {0}, error number parse {1}", _filename, price), ex);
                    return null;
                }
            }
            //create new product
            CanonDataContext db = Cdb.Instance;
            T a = Activator.CreateInstance<T>();
            a.ProductCode = ean;
            a.ProductName = name;
            a.IsActive = true;
            a.CurrentPrice = (decimal)dPrice;

            if (dPrice > 0)
            {
                RecommendedPrice newPrice = new RecommendedPrice();
                newPrice.ChangeDate = DateTime.Now;
                newPrice.UserId = WebVariables.LoggedUserId;
                newPrice.Price = (decimal)dPrice;
                newPrice.Product = a;
                a.RecommendedPrices.Add(newPrice);
            }
            if (!string.IsNullOrEmpty(category))
            {
                Category newCat = new Category();
                newCat.CategoryName = category;
                newCat.InternalId = category;
                a.Category = newCat;
            }

            //update product relevance words
            string filteredName = name.Replace(",","").Replace(";","");
            string[] nameTokens = filteredName.Split(' ');
            string[] synTokens = synonims.Split(' ');
            int maxValue = this.CountMaxRelevance(2, nameTokens, synTokens);
            ProductsRelevance fromCode = new ProductsRelevance();
            fromCode.Word = ean;
            fromCode.Points = 2;
            fromCode.Max = maxValue;
            a.ProductsRelevances.Add(fromCode);

            //update relevance from name
            foreach (string nameToken in nameTokens)
            {
                if (nameToken.Trim().Length < 2) continue;
                ProductsRelevance fromName = new ProductsRelevance();
                fromName.Word = nameToken;
                fromName.Points = 2;
                fromName.Max = maxValue;
                a.ProductsRelevances.Add(fromName);
            }

            //update relevance from synonims
            foreach (string synToken in synTokens)
            {
                if (synToken.Trim().Length < 2) continue;
                ProductsRelevance fromSyn = new ProductsRelevance();
                fromSyn.Word = synToken;
                fromSyn.Points = 1;
                fromSyn.Max = maxValue;
                a.ProductsRelevances.Add(fromSyn);
            }

            //update relevance from stopwords
            string[] stopTokens = stopwords.Split(' ');
            foreach (string stopToken in stopTokens)
            {
                if (stopToken.Trim().Length < 2) continue;
                ProductsRelevance fromStop = new ProductsRelevance();
                fromStop.Word = stopToken;
                fromStop.Points = -1;
                fromStop.Max = maxValue;
                a.ProductsRelevances.Add(fromStop);
            }

            return a;
        }

        protected int CountMaxRelevance(int eanPoint, string[] name, string[] synonims)
        {
            int result = eanPoint;
            foreach (string nameToken in name)
            {
                if (nameToken.Trim().Length < 2) continue;
                result +=2;
            }
            foreach (string nameToken in synonims)
            {
                if (nameToken.Trim().Length < 2) continue;
                result++;
            }
            return result;
        }

        /// <summary>
        /// Exports data from file into database
        /// </summary>
        /// <returns></returns>
        public bool ExportToDb()
        {
            ErrorMessages.Clear();
            try
            {
                List<T> list = this.ImportFile();
                List<Product> active = CanonProduct.GetActiveProducts();
                //go throw current active products and mark them as inactive if they are not in list
                foreach (Product act in active)
                {
                    bool isFound = false;
                    foreach (T product in list)
                        if (act.ProductCode == product.ProductCode)
                        {
                            isFound = true;
                            break;
                        }
                    if (!isFound)
                        CanonProduct.ActivateProduct(act.ProductCode, false);
                }
                //import products from xls
                foreach (T product in list)
                {
                    try
                    {
                        List<ImportErrorMessage> results = product.InsertUpdateProductWithPrice();
                        foreach(ImportErrorMessage res in results)
                            ErrorMessages.Add(res);
                    }
                    catch (Exception ex)
                    {
                        //add message about error
                        ErrorMessages.Add(new ImportErrorMessage("GeneralRecordImportError", 
                                                new string[] { product.ProductName }));
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
