using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib.Database
{
    public partial class SksSqlite
    {
        // todo: to internal
        public static int AddNewUser(string name, string ip, string key, string password)
        {
            var query = $"INSERT INTO User VALUES(null, '{name}', '{ip}', '{key}', '{password}');";
            return me.ExecuteNonQuery(query);
        }

        //public static List<SksUser> GetAllUsers()
        //{
        //    var query = "SELECT * FROM User;";

        //}
    }
}
