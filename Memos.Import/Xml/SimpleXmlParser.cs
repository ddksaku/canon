using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Memos.Import.Exceptions;

namespace Memos.Import.Xml
{
    public class SimpleXmlParser
    {
        private const string FileCannotBeOpen = "File with XML data cannot be opened";
        private const string BadXmlFormat = "File with XML data is in correct XML format";
        private const string StringIsIncorrect = "XML data is incorrect";
        private const string ExceptionWhileXmlParse = "Exception during XML file parsing";
        private StreamReader _fileContent;
        private XmlTextReader _xmlReader;
        private string _filename;

        /// <summary>
        /// Constructor by FileInfo
        /// </summary>
        /// <param name="fileInfo"></param>
        public SimpleXmlParser(FileInfo fileInfo)
        {
            try
            {
                _filename = fileInfo.FullName;
                _fileContent = new StreamReader(_filename);
            }
            catch (ParseException ex)
            {
                throw new ParseException(FileCannotBeOpen,ex);
            }
        }

        /// <summary>
        /// Parses xml file
        /// </summary>
        /// <returns>List of instanses as Dictionary, where key=NodeName and value=NodeValue</returns>
        public List<Dictionary<string, string>> Parse()
        {
            try
            {
                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>(100);
                XmlDocument doc = new XmlDocument();
                doc.Load(_filename);
                XmlNodeList nodes = doc.DocumentElement.ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    Dictionary<string, string> tmp = new Dictionary<string, string>();
                    foreach(XmlNode child in node.ChildNodes)
                        tmp.Add(child.Name.ToLower(), child.InnerText);
                    result.Add(tmp);
                }
                return result;
            }
            catch (ParseException ex)
            {
                throw new ParseException(ExceptionWhileXmlParse, ex);
            }
        }

    }
}
