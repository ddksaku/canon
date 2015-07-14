using System.Collections.Generic;
using System.Text;

namespace Memos.Import.Csv
{
   public class CsvRow : List<string>
    {
       public override string ToString()
       {
           StringBuilder row = new StringBuilder(256);

           foreach (string cell in this)
           {
               row.Append(cell + ",");
           }

           return row.ToString();
       }
    }
}
