using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustchainCore.Extensions
{
    public static class SQLiteDataReaderExtensions
    {
        public static byte[] GetBytes(this SQLiteDataReader reader, string name)
        {
            var obj = reader[name];
            if (obj == DBNull.Value)
                return new byte[0];

            return (byte[])obj;
        }

        public static string GetString(this SQLiteDataReader reader, string name)
        {
            var obj = reader[name];
            if (obj == DBNull.Value)
                return null;

            return (string)obj;
        }

    }
}
