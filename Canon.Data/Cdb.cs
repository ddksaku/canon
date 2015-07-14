using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Data
{
    public sealed class Cdb
    {
        static string _connection = string.Empty;

        Cdb()
        {
        }

        public static string ConnectionString
        {
            get
            {
                return Cdb._connection;
            }
            set
            {
                Cdb._connection = value;
            }
        }

        public static CanonDataContext Instance
        {
            get
            {
                if (string.IsNullOrEmpty(Cdb.ConnectionString))
                    return new CanonDataContext();
                else return new CanonDataContext(Cdb.ConnectionString);
            }
        }
    }
}
