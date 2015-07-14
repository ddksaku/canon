using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Memos.Import
{
    public interface ICommonParser<T>
    {
        /// <summary>
        /// Imports/Parses a file and returns list of parsed objects
        /// </summary>
        /// <returns>List of parsed objects</returns>
        List<T> ImportFile();

        /// <summary>
        /// Parses one row and creates necessary instance
        /// </summary>
        /// <param name="row"></param>
        /// <returns>Result of parsing OR null</returns>
        T ParseRow(Object row);
    }
}
