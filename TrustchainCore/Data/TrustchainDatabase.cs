using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using TrustchainCore.Configuration;
using TrustchainCore.Extensions;
using TrustchainCore.Model;

namespace TrustchainCore.Data
{
    public class TrustchainDatabase : IDisposable
    {
        public static string MemoryConnectionString = "Data Source=:memory:;Version=3;";
        public static TrustchainDatabase MemoryDatabase;
        public static object lockObject = new object();

        public static IDisposable Open(object filename)
        {
            throw new NotImplementedException();
        }

        private static volatile bool created = true;

        public static volatile bool IsMemoryDatabase = false;

        public SQLiteConnection Connection;

        public string Name { get; set; }

        public KeyValueTable _keyValue = null;
        public KeyValueTable KeyValue
        {
            get
            {
                return _keyValue ?? (_keyValue = new KeyValueTable(Connection));
            }
        }

        public TrustTable _trust = null;
        public TrustTable Trust
        {
            get
            {
                return _trust ?? (_trust = new TrustTable(Connection));
            }
        }

        public SubjectTable _subject = null;
        public SubjectTable Subject
        {
            get
            {
                return _subject ?? (_subject = new SubjectTable(Connection));
            }
        }

        public virtual string GetDatabaseName()
        {
            return (!string.IsNullOrEmpty(App.Config["dbfilename"].ToStringValue())) ? App.Config["dbfilename"].ToStringValue() : "test.db";
        }

        public virtual void CreateIfNotExist()
        {
            if (!IsMemoryDatabase && !File.Exists(Connection.FileName))
                SQLiteConnection.CreateFile(Connection.FileName);

            KeyValue.CreateIfNotExist();
            Trust.CreateIfNotExist();
            Subject.CreateIfNotExist();
        }

        public virtual SQLiteConnection OpenConnection(string dbFilename = null)
        {
            if(IsMemoryDatabase)
            {
                Connection = new SQLiteConnection(MemoryConnectionString);
                Connection.Open();
                //Connection.EnableExtensions(true);
                //Connection.LoadExtension("SQLite.Interop.dll", "sqlite3_json_init");
                return Connection;
            }

            if(dbFilename == null && !string.IsNullOrEmpty(App.Config["dbconnectionstring"].ToStringValue()))
            {
                Connection = new SQLiteConnection(App.Config["dbconnectionstring"].ToStringValue());
                Connection.Open();
                return Connection;
            }


            if(string.IsNullOrEmpty(dbFilename))
               dbFilename= (!string.IsNullOrEmpty(App.Config["dbfilename"].ToStringValue())) ? App.Config["dbfilename"].ToStringValue() : Name;

            if (!string.IsNullOrEmpty(dbFilename))
            {
                var sb = new SQLiteConnectionStringBuilder();

                sb.DataSource = dbFilename;
                var dbObject = App.Config["database"];
                if (dbObject != null)
                {
                    sb.Flags = SQLiteConnectionFlags.UseConnectionPool;
                    //tt.NoSharedFlags = false;

                    sb.JournalMode = (SQLiteJournalModeEnum)dbObject["journalmode"].ToInteger((int)SQLiteJournalModeEnum.Default);
                    sb.Pooling = dbObject["pooling"].ToBoolean(true);
                    sb.ReadOnly = dbObject["readonly"].ToBoolean(false);
                    sb.Add("cache", dbObject["cache"].ToStringValue("shared"));
                    sb.Add("Compress", dbObject["compress"].ToStringValue("False"));
                    sb.SyncMode = (SynchronizationModes)dbObject["syncmode"].ToInteger((int)SynchronizationModes.Normal);

                    //sb.DefaultIsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
                    //tt.DefaultDbType = System.Data.DbType.
                    //var dd = new SQLiteConnection(;
                }
                else
                {
                    //sb.DefaultIsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
                    sb.Flags = SQLiteConnectionFlags.UseConnectionPool;
                    //tt.NoSharedFlags = false;

                    //sb.JournalMode = SQLiteJournalModeEnum.Default;
                    sb.Pooling = true;
                    sb.ReadOnly = false;
                    sb.Add("cache", "shared");
                    sb.Add("Compress", "False");
                    //sb.SyncMode = SynchronizationModes.Normal;

                }
                Connection = new SQLiteConnection(sb.ConnectionString);
                Connection.Open();
                return Connection;
            }

            throw new ApplicationException("No database connection found");
        }

        /// <summary>
        /// Opens a Connection to a database
        /// If App.Config["test"] is true, then the database will be in memory and ignore the dbname.
        /// </summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static TrustchainDatabase Open(string dbname = null)
        {
            if (App.Config["test"].ToBoolean())
                IsMemoryDatabase = true;

            if (IsMemoryDatabase)
            {
                if (MemoryDatabase == null)
                {
                    lock (lockObject)
                    {
                        if (MemoryDatabase == null)
                        {
                            MemoryDatabase = new TrustchainDatabase();
                            MemoryDatabase.OpenConnection();
                            MemoryDatabase.CreateIfNotExist();
                        }
                    }
                }
                return MemoryDatabase;
            }
            else
            {
                var db = new TrustchainDatabase();
                db.OpenConnection(dbname);
                return db;
            }
        }

        public int AddTrust(TrustModel trust)
        {
            var result = Trust.Add(trust);
            if (result < 1)
                return result;

            foreach (var subject in trust.Issuer.Subjects)
            {
                subject.IssuerId = trust.Issuer.Id;
                result = Subject.Add(subject);
                if (result < 1)
                    break;
            }
            return result;
        }

        public int Vacuum()
        {
            using (SQLiteCommand command = new SQLiteCommand("vacuum;", Connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        //public TrustModel GetTrust(byte[] issuerid, byte[] issuersignature)
        //{
        //    var result = Trust.Select(issuerid, issuersignature).FirstOrDefault();
        //    var subjects = Subject.Select(issuerid);
        //    result.Issuer.Subjects = subjects.ToArray();
        //    return result;
        //}

        public virtual void Dispose()
        {
            if (!IsMemoryDatabase)
            {
                Connection.Dispose();
                GC.Collect();
            }
        }
    }
}
