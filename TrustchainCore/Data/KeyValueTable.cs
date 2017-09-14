using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustchainCore.Extensions;
using TrustchainCore.Model;

namespace TrustchainCore.Data
{
    public class KeyValueTable : DBTable
    {
        public KeyValueTable(SQLiteConnection connection, string tableName = "KeyValue")
        {
            Connection = connection;
            TableName = tableName;
        }

        public void CreateIfNotExist()
        {
            if (TableExist())
                return;

            var  sql = "CREATE TABLE IF NOT EXISTS " + TableName + " " +
                "(" +
                "key TEXT NOT NULL," +
                "value TEXT"+
                ")";
            using (var command = new SQLiteCommand(sql, Connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SQLiteCommand("CREATE UNIQUE INDEX IF NOT EXISTS " + TableName + "Key ON " + TableName + " (key)", Connection))
            { 
                command.ExecuteNonQuery();
            }
        }

        public int Put(string key, string value)
        {
            using (var command = new SQLiteCommand("REPLACE INTO " + TableName + " (key, value) VALUES (@key, @value)", Connection))
            {
                command.Parameters.Add(new SQLiteParameter("@key", key));
                command.Parameters.Add(new SQLiteParameter("@value", value));
                return command.ExecuteNonQuery();
            }
        }

        public string Get(string key)
        {
            using (var command = new SQLiteCommand("SELECT * FROM " + TableName + " WHERE key = @key", Connection))
            {
                command.Parameters.Add(new SQLiteParameter("@key", key));

                return Query<string>(command, (reader) => reader.GetString("value")).FirstOrDefault();
            }
        }

        public int Delete(string key)
        {
            using (var command = new SQLiteCommand("DELETE FROM " + TableName + " WHERE key = @key", Connection))
            {
                command.Parameters.Add(new SQLiteParameter("@key", key));
                return command.ExecuteNonQuery();
            }
        }

    }
}
