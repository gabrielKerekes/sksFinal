using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib.Database
{
    public partial class SksSqlite
    {
        private const string DbFileName = "sks.db";

        private const string CreateDbScript = "CREATE TABLE \"User\"( " +
                                              "\"UserId\" integer primary key autoincrement not null , " +
                                              "\"Name\" varchar , " +
                                              "\"Ip\" varchar , " +
                                              "\"Key\" varchar , " +
                                              "\"Password\" varchar);" +

                                              "CREATE TABLE \"DbVersion\"( " +
                                              "\"Version\" integer primary key not null ); ";
                                              //"\"IsIgnored\" integer , " +

        private static object mainLock = new object();
        protected static SksSqlite me = null;

        private SQLiteConnection dbConnection;

        protected SksSqlite(string dbFileName)
        {
            lock (mainLock)
            {
                if (!File.Exists(dbFileName))
                    SQLiteConnection.CreateFile(DbFileName);

                dbConnection = new SQLiteConnection($"Data Source={dbFileName};Version=3;");

                me = this;
            }
        }

        public static void Init()
        {
            lock (mainLock)
            {
                me = new SksSqlite(DbFileName);
                me.dbConnection.Open();

                var version = GetDbVersion();
                if (version == -1)
                {
                    CreateDb();
                    version = 1;
                    me.UpdateDbVersion(version);
                }
            }
        }

        private static void CreateDb()
        {
            me.ExecuteNonQuery(CreateDbScript);
        }

        public static int GetDbVersion()
        {
            lock (mainLock)
            {
                if (!DoesVersionTableExist())
                {
                    return -1;
                }

                var version = me.ExecuteScalar("select Version AS Value from DbVersion") ?? 0;

                return version;
            }
        }

        private void UpdateDbVersion(int version)
        {
            ExecuteNonQuery("DELETE FROM DbVersion");
            // todo: prerobit na query s ?
            ExecuteNonQuery($"REPLACE INTO DbVersion (Version) VALUES ({version})");
            //Execute("REPLACE INTO DbVersion (Version) VALUES (?)", version);
        }
        
        private static bool DoesVersionTableExist()
        {
            var query = "SELECT count(name) as Value FROM sqlite_master WHERE type='table' AND name='Device';";
            var dbVersion = me.ExecuteScalar(query);

            return dbVersion != 0;
        }

        private int? ExecuteScalar(string query)
        {
            var command = new SQLiteCommand(query, dbConnection);

            var result = command.ExecuteScalar();
            if (result == null)
                return null;
            
            return (int) (long) command.ExecuteScalar();
        }

        private int ExecuteNonQuery(string query)
        {
            var command = new SQLiteCommand(query, dbConnection);
            return command.ExecuteNonQuery();
        }
    }
}
