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
    public class SubjectTable : DBTable
    {
        public SubjectTable(SQLiteConnection connection, string tableName = "Subject")
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
                "rowid INTEGER PRIMARY KEY AUTOINCREMENT,"+
                "issuerid BLOB," +
                "id BLOB," +
                "signature BLOB,"+
                "idtype TEXT,"+
                "claim TEXT," +
                "cost INTEGER," +
                "activate INTEGER," +
                "expire INTEGER," +
                "scope TEXT," +
                "trustid BLOB" +
                ")";

            ExecuteNonQuery(sql);
            ExecuteNonQuery("CREATE INDEX IF NOT EXISTS " + TableName + "TrustId ON " + TableName + " (trustid)");
            ExecuteNonQuery("CREATE INDEX IF NOT EXISTS " + TableName + "Id ON " + TableName + " (id)");
            ExecuteNonQuery("CREATE INDEX IF NOT EXISTS " + TableName + "IdType ON " + TableName + " (idtype)");
            ExecuteNonQuery("CREATE INDEX IF NOT EXISTS " + TableName + "Scope ON " + TableName + " (scope)");
        }

        public int Add(SubjectModel subject)
        {
            using (var command = new SQLiteCommand("REPLACE INTO " + TableName + " (issuerid, id, signature, idtype, claim, cost, activate, expire, scope, trustid) " +
                "VALUES (@issuerid, @id, @signature, @idtype, @claim, @cost, @activate, @expire, @scope, @trustid)", Connection))
            {

                command.Parameters.Add(new SQLiteParameter("@issuerid", subject.IssuerId));
                command.Parameters.Add(new SQLiteParameter("@id", subject.Id));
                command.Parameters.Add(new SQLiteParameter("@signature", subject.Signature));
                command.Parameters.Add(new SQLiteParameter("@idtype", subject.IdType));
                command.Parameters.Add(new SQLiteParameter("@claim", subject.Claim.SerializeObject()));
                command.Parameters.Add(new SQLiteParameter("@cost", subject.Cost));
                command.Parameters.Add(new SQLiteParameter("@activate", subject.Activate));
                command.Parameters.Add(new SQLiteParameter("@expire", subject.Expire));
                command.Parameters.Add(new SQLiteParameter("@scope", subject.Scope));
                command.Parameters.Add(new SQLiteParameter("@trustid", subject.TrustId));

                return command.ExecuteNonQuery();
            }
        }

        //public IEnumerable<SubjectModel> Select(byte[] issuerId)
        //{
        //    var command = new SQLiteCommand("SELECT * FROM " + TableName + " where issuerid = @issuerid", Connection);
        //    command.Parameters.Add(new SQLiteParameter("@issuerid", issuerId));

        //    return Query<SubjectModel>(command, NewItem);
        //}

        public IEnumerable<SubjectModel> Select(byte[] trustid)
        {
            using (var command = new SQLiteCommand("SELECT * FROM " + TableName + " WHERE trustid = @trustid ORDER BY rowid", Connection))
            {
                command.Parameters.Add(new SQLiteParameter("@trustid", trustid));

                return Query<SubjectModel>(command, NewItem);
            }
        }

        public SubjectModel NewItem(SQLiteDataReader reader)
        {
            return new SubjectModel
            {
                IssuerId = reader.GetBytes("issuerid"),
                Id = reader.GetBytes("id"),
                Signature = reader.GetBytes("signature"),
                IdType = reader.GetString("idtype"),
                Claim = reader.GetString("claim").DeserializeObject<JObject>(),
                Cost = reader.GetInt32(6),
                Activate = (uint)reader.GetInt32(7),
                Expire = (uint)reader.GetInt32(8),
                Scope = reader.GetString("scope"),
                TrustId = reader.GetBytes("trustid")
            };
        }



    }
   
}
