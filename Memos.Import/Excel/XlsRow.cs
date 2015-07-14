using System.Collections.Generic;
using System.Text;

namespace Memos.Import.Excel
{
    public class XlsRow : List<string>
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
