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
    public class TrustTable : DBTable
    {
        public TrustTable(SQLiteConnection connection, string tableName = "Trust")
        {
            Connection = connection;
            TableName = tableName;
        }

        public int CreateIfNotExist()
        {
            if (TableExist())
                return 0;

            var  sql = "CREATE TABLE IF NOT EXISTS " + TableName + " " +
                "(" +
                "trustid BLOB NOT NULL PRIMARY KEY," +
                "version TEXT," +
                "script TEXT,"+
                "issuerid BLOB NOT NULL," +
                "issuersignature BLOB NOT NULL," +
                "serverid BLOB," +
                "serversignature BLOB,"+
                "timestamp TEXT"+
                ") WITHOUT ROWID";

            var result = ExecuteNonQuery(sql);
            ExecuteNonQuery("CREATE INDEX IF NOT EXISTS " + TableName + "TrustId ON " + TableName + " (trustid)");

            //using (var command = new SQLiteCommand("CREATE UNIQUE INDEX IF NOT EXISTS " + TableName + "IssuerId ON " + TableName + " (issuerid,issuersignature)", Connection))
            //{
            //    command.ExecuteNonQuery();
            //}
            //command = new SQLiteCommand("CREATE INDEX IF NOT EXISTS " + TableName + "IssuerSignature ON " + TableName + " (issuersignature)", Connection);
            //command.ExecuteNonQuery();
            return result;
            
        }

        public int Add(TrustModel trust)
        {
            using (var command = new SQLiteCommand("INSERT INTO " + TableName + " (trustid, version, script, issuerid, issuersignature, serverid, serversignature, timestamp) " +
                "VALUES (@trustid, @version, @script, @issuerid, @issuersignature, @serverid, @serversignature, @timestamp)", Connection))
            {
                command.Parameters.Add(new SQLiteParameter("@trustid", trust.TrustId));
                command.Parameters.Add(new SQLiteParameter("@version", trust.Head.Version));
                command.Parameters.Add(new SQLiteParameter("@script", trust.Head.Script));
                command.Parameters.Add(new SQLiteParameter("@issuerid", trust.Issuer.Id));
                command.Parameters.Add(new SQLiteParameter("@issuersignature", trust.Issuer.Signature));
                command.Parameters.Add(new SQLiteParameter("@serverid", (trust.Server != null) ? trust.Server.Id: new byte[] { }));
                command.Parameters.Add(new SQLiteParameter("@serversignature", (trust.Server != null) ? trust.Server.Signature : new byte[] { }));
                command.Parameters.Add(new SQLiteParameter("@timestamp", trust.Timestamp.SerializeObject()));
                return command.ExecuteNonQuery();
            }
        }

        public int Replace(TrustModel trust)
        {
            using (var command = new SQLiteCommand("REPLACE INTO " + TableName + " (trustid, version, script, issuerid, issuersignature, serverid, serversignature, timestamp) " +
                "VALUES (@trustid, @version, @script, @issuerid, @issuersignature, @serverid, @serversignature, @timestamp)", Connection))
            {
                command.Parameters.Add(new SQLiteParameter("@trustid", trust.TrustId));
                command.Parameters.Add(new SQLiteParameter("@version", trust.Head.Version));
                command.Parameters.Add(new SQLiteParameter("@script", trust.Head.Script));
                command.Parameters.Add(new SQLiteParameter("@issuerid", trust.Issuer.Id));
                command.Parameters.Add(new SQLiteParameter("@issuersignature", trust.Issuer.Signature));
                command.Parameters.Add(new SQLiteParameter("@serverid", (trust.Server != null) ? trust.Server.Id : new byte[] { }));
                command.Parameters.Add(new SQLiteParameter("@serversignature", (trust.Server != null) ? trust.Server.Signature : new byte[] { }));
                command.Parameters.Add(new SQLiteParameter("@timestamp", trust.Timestamp.SerializeObject()));
                return command.ExecuteNonQuery();
            }
        }



        public IEnumerable<TrustModel> Select()
        {
            using (var command = new SQLiteCommand("SELECT * FROM " + TableName, Connection))
            {
                return Query<TrustModel>(command, NewItem);
            }
        }

        public TrustModel SelectOne(byte[] trustid)
        {
            using (var command = new SQLiteCommand("SELECT * FROM " + TableName + " WHERE trustid = @trustid", Connection))
            {
                command.Parameters.Add(new SQLiteParameter("@trustid", trustid));

                return Query<TrustModel>(command, NewItem).FirstOrDefault();
            }
        }

        public IEnumerable<TrustModel> SelectServerUnsigned()
        {
            using (var command = new SQLiteCommand("SELECT * FROM " + TableName + " WHERE ifnull(length(serversignature), 0) = 0", Connection))
            {
                return Query<TrustModel>(command, NewItem);
            }
        }

        public int Delete(byte[] issuerId, byte[] signature)
        {
            using (var command = new SQLiteCommand("DELETE FROM " + TableName + " WHERE issuerid = @issuerid AND issuersignature = @issuersignature", Connection))
            {
                command.Parameters.Add(new SQLiteParameter("@issuerid", issuerId));
                command.Parameters.Add(new SQLiteParameter("@signature", signature));
                return command.ExecuteNonQuery();
            }
        }

        public TrustModel NewItem(SQLiteDataReader reader)
        {
            return new TrustModel
            {
                TrustId = reader.GetBytes("trustid"),
                Head = new HeadModel
                {
                    Version = reader.GetString("version"),
                    Script = reader.GetString("script")
                },
                Issuer = new IssuerModel
                {
                    Id = reader.GetBytes("issuerid"),
                    Signature = reader.GetBytes("issuersignature")
                },
                Server = new ServerModel
                {
                    Id = reader.GetBytes("serverid"),
                    Signature = reader.GetBytes("serversignature")
                },
                Timestamp = reader.GetString("timestamp").DeserializeObject<TimestampCollection>()
            };
        }
    }
}
