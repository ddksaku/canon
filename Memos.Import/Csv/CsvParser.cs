using System.Collections.Generic;
using System.IO;
using System.Text;
using Memos.Import.Exceptions;

namespace Memos.Import.Csv
{
    public class CsvParser
    {
        private const string FileCannotBeOpen = "File with CSV data cannot be open";
        private const string StringIsIncorrect = "CSV data is incorrect";
        private CsvRow rowResult;
        private List<CsvRow> generalResult = new List<CsvRow>();
        private bool _hasHeaders;
        private TextReader _contentOfFile;

        //working with file
        public CsvParser(FileInfo fileInfo,bool hasHeaders)
        {
            try
            {
                _contentOfFile = new StreamReader(fileInfo.FullName);
            }
            catch (ParseException ex)
            {
                throw new ParseException(FileCannotBeOpen,ex);
            }
           
            _hasHeaders = hasHeaders;
           
        }
        //working with string
        public  CsvParser(string contentOfFile,bool hasHeaders)
        {
            try
            {
                _contentOfFile = new StringReader(contentOfFile);
            }
            catch (ParseException ex)
            {

                throw new ParseException(StringIsIncorrect, ex);
            }
          
         
            _hasHeaders = hasHeaders;

        }
        
        public List<CsvRow> Parse()
        {
            using (_contentOfFile)
            {
                string parseString = string.Empty;

                if (_hasHeaders)
                {
                    _contentOfFile.ReadLine();
                }

                while ((parseString = _contentOfFile.ReadLine()) != null)
                {
                    rowResult = ParseString(parseString);
                    generalResult.Add(rowResult);

                }

            }
            return generalResult;
        }
        
        private CsvRow ParseString(string strInputString)
        {
            // Create an instance of StreamReader to read from a file.


            int intCounter = 0, intLenght;
            StringBuilder strElem = new StringBuilder();
            CsvRow alParsedCsv = new CsvRow();
            intLenght = strInputString.Length;
            strElem = strElem.Append("");
            int intCurrState = 0;
            int[][] aActionDecider = new int[9][];

            //Build the state array
            aActionDecider[0] = new int[4] { 2, 0, 1, 5 };
            aActionDecider[1] = new int[4] { 6, 0, 1, 5 };
            aActionDecider[2] = new int[4] { 4, 3, 3, 6 };
            aActionDecider[3] = new int[4] { 4, 3, 3, 6 };
            aActionDecider[4] = new int[4] { 2, 8, 6, 7 };
            aActionDecider[5] = new int[4] { 5, 5, 5, 5 };
            aActionDecider[6] = new int[4] { 6, 6, 6, 6 };
            aActionDecider[7] = new int[4] { 5, 5, 5, 5 };
            aActionDecider[8] = new int[4] { 0, 0, 0, 0 };

            for (intCounter = 0; intCounter < intLenght; intCounter++)
            {
                intCurrState = aActionDecider[intCurrState][GetInputID(strInputString[intCounter])];
                //take the necessary action depending upon the state returned
                PerformAction(ref intCurrState, strInputString[intCounter], ref strElem, ref alParsedCsv);
            }
            intCurrState = aActionDecider[intCurrState][3];
            PerformAction(ref intCurrState, '\0', ref strElem, ref alParsedCsv);
            return alParsedCsv;
        }
        private static int GetInputID(char chrInput)
        {
            if (chrInput == '"')
            {
                return 0;
            }
            else if (chrInput == ',')
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static void PerformAction(ref int intCurrState, char chrInputChar, ref StringBuilder strElem, ref CsvRow alParsedCsv)
        {
            string strTemp = null;
            switch (intCurrState)
            {
                case 0:
                    //Seperate out value to array list
                    strTemp = strElem.ToString();
                    alParsedCsv.Add(strTemp);
                    strElem = new StringBuilder();
                    break;
                case 1:
                case 3:
                case 4:
                    //accumulate the character
                    strElem.Append(chrInputChar);
                    break;
                case 5:
                    //End of line reached. Seperate out value to array list
                    strTemp = strElem.ToString();
                    alParsedCsv.Add(strTemp);
                    break;
                case 6:
                    //Erroneous input. Reject line.
                    alParsedCsv.Clear();
                    break;
                case 7:
                    //wipe ending " and Seperate out value to array list
                    strElem.Remove(strElem.Length - 1, 1);
                    strTemp = strElem.ToString();
                    alParsedCsv.Add(strTemp);
                    strElem = new StringBuilder();
                    intCurrState = 5;
                    break;
                case 8:
                    //wipe ending " and Seperate out value to array list
                    strElem.Remove(strElem.Length - 1, 1);
                    strTemp = strElem.ToString();
                    alParsedCsv.Add(strTemp);
                    strElem = new StringBuilder();
                    //goto state 0
                    intCurrState = 0;
                    break;
            }
        }




    }
}
