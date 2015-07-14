using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Data;
using System.Globalization;
using System.Threading;
using log4net;
using Memos.Framework.Logging;
using Memos.Import;
using Memos.Import.Xml;
using Canon.Data.Business;
using Canon.Data.Enums;

namespace Canon.Data.Import
{
    public class CanonChannelImportXml<T> : CommonParser<T> where T : CanonChannelMonitor
    {
        private CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
        private string _url = string.Empty;
        private int _channelId = 0;
        private string _uploadDirectory = string.Empty;
        public string AdditionalCommand { get; set; }

        /// <summary>
        /// Warnings and Errors during import
        /// </summary>
        public List<ImportErrorMessage> ErrorMessages { get; set; }

        /// <summary>
        /// File which has been downloaded
        /// </summary>
        public string DownloadedFilename { get; set; }

        public CanonChannelImportXml(int channelId, string url, string uploadDirectory)
        {
            _url = url;
            _uploadDirectory = uploadDirectory;
            _channelId = channelId;
            this.AdditionalCommand = string.Empty;
            ErrorMessages = new List<ImportErrorMessage>();
        }

        public void DownloadXmlFile()
        {
            //make additional job before if necessary
            if (!string.IsNullOrEmpty(this.AdditionalCommand))
                this.MakeAdditionalJob();
            string url = _url;
            string todayStr = DateTime.Now.ToString("yyyy-MM-dd");
            //check if url is complete, if not add xml file name in format YYYY-MM-DD.xml
            if ((!url.ToLower().EndsWith(".xml"))&&
                (!url.ToLower().Contains(".asmx/"))&&
                (!url.ToLower().Contains(".asp")) &&
                (!url.ToLower().Contains(".aspx")))
                url = string.Format("{0}/{1}.xml", url, todayStr);
            //create local filename
            string localFilename = string.Format("{0}_{1}.xml", _channelId, todayStr);
            string localFullFilename = Path.Combine(_uploadDirectory, localFilename);
            //download file
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "Mozilla/4.0+");
            client.DownloadFile(url, localFullFilename);
            this.DownloadedFilename = localFullFilename;
        }

        private void MakeAdditionalJob()
        {
            try
            {
                if (this.AdditionalCommand.ToLower().StartsWith("url:"))
                {
                    WebClient client = new WebClient();
                    client.DownloadData(this.AdditionalCommand.ToLower().Replace("url:", ""));
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.Fatal(string.Format("Additional command failed {0}", this.AdditionalCommand), ex);
            }
        }

        /// <summary>
        /// Reads initial file and imports data
        /// </summary>
        /// <returns></returns>
        public override List<T> ImportFile()
        {
            if (string.IsNullOrEmpty(this.DownloadedFilename))
                throw new FileNotFoundException("There is no any file downloaded. Call DownloadXmlFile first.");

            List<T> products = new List<T>(100);
            try
            {
                SimpleXmlParser parser = new SimpleXmlParser(new System.IO.FileInfo(this.DownloadedFilename));
                List<Dictionary<string, string>> parsed = parser.Parse();
                //Go throw rows and validate each row
                foreach (Dictionary<string, string> dr in parsed)
                {
                    T instance = this.ParseRow(dr);
                    if (instance == null) continue;
                    products.Add(instance);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return products;
        }

        #region Parse
        /// <summary>
        /// Parses a row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public override T ParseRow(Object row)
        {
            if (!(row is Dictionary<string, string>))
                throw new ArgumentException("Only Dictionary<string, string> object can be passed.");

            string[] keys = new string[]{"product", "description", "url", "price", "price_vat", "vat"};
            Dictionary<string, string> dr = row as Dictionary<string, string>;
            //get product name
            if (!dr.ContainsKey(keys[0]))
                return null;
            string productName = System.Web.HttpUtility.HtmlDecode(dr[keys[0]]);

            //get description
            if (!dr.ContainsKey(keys[1]))
                return null;
            string productDesc = System.Web.HttpUtility.HtmlDecode(dr[keys[1]]);

            //get url
            if (!dr.ContainsKey(keys[2]))
                return null;
            string productUrl = dr[keys[2]];

            //get prices
            if ((!dr.ContainsKey(keys[3])) && (!dr.ContainsKey(keys[4])) && (!dr.ContainsKey(keys[5])))
                return null;
            double price = 0;
            double priceVat = 0;
            double vat = 0;
            if ((dr.ContainsKey(keys[3])) && (dr.ContainsKey(keys[4])))
            {
                //price and pricevat
                string strPrice = dr[keys[3]];
                string strPriceVat = dr[keys[4]];
                if ((string.IsNullOrEmpty(strPrice)) || (string.IsNullOrEmpty(strPriceVat)))
                    return null;
                try
                {
                    price = double.Parse(strPrice, culture.NumberFormat);
                    priceVat = double.Parse(strPriceVat, culture.NumberFormat);
                    if (price == 0)
                        return null;
                    vat = (int)Math.Round((priceVat / price - 1) * 100);
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add(new ImportErrorMessage("CantParseNumber", 
                                                            new string[]{strPrice, strPriceVat}));
                    if (Logger != null)
                        Logger.Error(string.Format("Url {0}, file {1}, exception {2}", _url, this.DownloadedFilename, 
                                            ex.ToString()));
                    return null;
                }
            }
            else if ((dr.ContainsKey(keys[3])) && (dr.ContainsKey(keys[5])))
            {
                //price and vat
                string strPrice = dr[keys[3]];
                string strVat = dr[keys[5]];
                if ((string.IsNullOrEmpty(strPrice)) || (string.IsNullOrEmpty(strVat)))
                    return null;
                try
                {
                    price = double.Parse(strPrice, culture.NumberFormat);
                    vat = double.Parse(strVat, culture.NumberFormat);
                    priceVat = price * ((double)vat / 100 + 1);
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add(new ImportErrorMessage("CantParseNumber",
                                                            new string[] { strPrice, strVat }));
                    if (Logger != null)
                        Logger.Error(string.Format("Url {0}, file {1}, exception {2}", _url, this.DownloadedFilename,
                                            ex.ToString()));
                    return null;
                }
            }
            else if ((dr.ContainsKey(keys[4])) && (dr.ContainsKey(keys[5])))
            {
                //price_vat and vat
                string strPriceVat = dr[keys[4]];
                string strVat = dr[keys[5]];
                if ((string.IsNullOrEmpty(strPriceVat)) || (string.IsNullOrEmpty(strVat)))
                    return null;
                try
                {
                    priceVat = double.Parse(strPriceVat, culture.NumberFormat);
                    vat = double.Parse(strVat, culture.NumberFormat);
                    price = priceVat / ((double)vat / 100 + 1);
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add(new ImportErrorMessage("CantParseNumber",
                                                            new string[] { strPriceVat, strVat }));
                    if (Logger != null)
                        Logger.Error(string.Format("Url {0}, file {1}, exception {2}", _url, this.DownloadedFilename,
                                            ex.ToString()));
                    return null;
                }
            }
            if ((price == 0) || (priceVat == 0) || (vat == 0))
                return null;
            
            //create new channel monitor
            T a = Activator.CreateInstance<T>();
            a.ProductName = productName;
            a.ProductDesc = productDesc;
            a.ProductUrl = productUrl;
            a.Price = (decimal) price;
            a.PriceVat = (decimal) priceVat;
            a.Vat = (decimal)vat;
            a.ChannelId = _channelId;
            a.ImportDate = DateTime.Now;

            return a;
        }
        #endregion

        /// <summary>
        /// Exports data from file into database
        /// </summary>
        /// <returns></returns>
        public override bool ExportToDb()
        {
            ErrorMessages.Clear();
            T tmp = Activator.CreateInstance<T>();
            tmp.CleanTodaysData(this._channelId);

            //download and save xml file
            try
            {
                this.DownloadXmlFile();

                if (Logger != null)
                    Logger.Debug("File download complete.");
            }
            catch (Exception ex)
            {
                //into db log
                int id = CanonProductsLog.AddImportLog(ChannelLogEnum.ChannelError, this._channelId, 0, 0);
                CanonProductsLog.AddImportErrorsLog(id, ChannelErrorEnum.ChannelIsNotAvailable, string.Empty);

                //into internal log
                ErrorMessages.Add(new ImportErrorMessage("XmlChannelDownloadError",
                                                        new string[] { _channelId.ToString() }));

                //into log
                if (Logger != null)
                    Logger.Error(string.Format("Url {0}, channel {1}, exception {2}", _url, _channelId, ex.ToString()));
                return false;
            }
            string resultFilename = this.DownloadedFilename;

            //parse downloaded file
            List<T> list = new List<T>();
            try
            {
                list = this.ImportFile();
                if (Logger != null)
                    Logger.Debug("File parsing complete.");
            }
            catch (Exception ex)
            {
                //into db log
                int id = CanonProductsLog.AddImportLog(ChannelLogEnum.ChannelError, this._channelId, 0, 0);
                CanonProductsLog.AddImportErrorsLog(id, ChannelErrorEnum.FeedFormatIsWrong, string.Empty);

                //into internal log
                ErrorMessages.Add(new ImportErrorMessage("XmlChannelParseError",
                                                        new string[] { _channelId.ToString() }));
                //into log
                if (Logger != null)
                    Logger.Error(string.Format("Url {0}, channel {1}, exception {2}", _url, _channelId, ex.ToString()));
                return false;
            }

            //export file into database
            if (Logger != null)
                Logger.Debug(string.Format("Parsed {0} products.", list.Count));
            this.TryedRecords = list.Count;
            int exportedCount = 0;
            foreach (T product in list)
            {
                try
                {
                    if (product.InsertNewRecord())
                        exportedCount++;
                }
                catch (Exception ex)
                {
                    //into db log
                    int id = CanonProductsLog.AddImportLog(ChannelLogEnum.ProductError, this._channelId, 0, 0);
                    CanonProductsLog.AddImportErrorsLog(id, ChannelErrorEnum.ProductParsingError, product.ProductName);

                    //add message about error
                    ErrorMessages.Add(new ImportErrorMessage("GeneralRecordImportError",
                                                            new string[] { product.ProductName }));
                    //into log
                    if (Logger != null)
                        Logger.Error(string.Format("File {0}, product {1} exception {2}", _url, product.ProductName, ex.ToString()));
                }
            }
            if (Logger != null)
                Logger.Debug(string.Format("Exported {0} products successfully.", exportedCount));

            this.SuccessRecords = exportedCount;
            //if exported files are empty return error
            if (exportedCount == 0)
            {
                //into db log
                int id = CanonProductsLog.AddImportLog(ChannelLogEnum.ChannelError, this._channelId, 0, 0);
                CanonProductsLog.AddImportErrorsLog(id, ChannelErrorEnum.ChannelIsEmpty, string.Empty);

                ErrorMessages.Add(new ImportErrorMessage("XmlChannelEmptyError"));
                //into log
                if (Logger != null)
                    Logger.Error(string.Format("File {0}, error {1}", _url, _channelId));
                return false;
            }
            return true;
        }
    }
}
