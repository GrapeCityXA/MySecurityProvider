using System;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace MySecurityProvider
{
    public sealed class Database
    {
        public static string ConnectionString;
        

        public static void SetConnectionString(string cnStr)
        {
            ConnectionString = cnStr;
        }

        static MySqlConnection GetDatabaseConnection()
        {
            MySqlConnection cn = new MySqlConnection();
            try
            {
                cn.ConnectionString = ConnectionString;
                cn.Open();
                return cn;
            }
            catch(Exception e)
            {
                WriteLogS("GetDatabaseConnection", e.ToString());
                return null;
            }
        }

        public static UserInfo GetUserInfo(string userName, string password)
        {
            try
            {
                using (MySqlConnection cn = GetDatabaseConnection())
                {
                    if (null == cn) return null;
                    using (var cmd = cn.CreateCommand())
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("SELECT * ");
                        sb.AppendLine("FROM blade_user ");
                        sb.AppendLine("WHERE account=@userName");
                        sb.AppendLine(" AND password=@password ");
                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.AddWithValue("@userName", userName);
                        cmd.Parameters.AddWithValue("@password", password);
                        using (var adp = new MySqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            adp.Fill(ds);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                WriteLogS("GetUserInfo", "No matched username and password.");
                                return null;
                            }
                            DataRow r = ds.Tables[0].Rows[0];

                            UserInfo u = new UserInfo();
                            u.username = (string)r["account"];
                            u.rolenames = "everyone";
                            u.title = (string)r["account"]; 
                            u.tenant_id= (string)r["tenant_id"];

                            return u;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                WriteLogS("GetUserInfo", e.ToString());
                return null;
            }
        }

        internal static UserInfo GetUserInfoFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            var userstr = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var userInfo = JsonConvert.DeserializeObject<UserInfo>(userstr);
            return userInfo;
        }

        public static void WriteLogS(string location, string logInfo)
        {
            try
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string s = time + " " + location + " " + logInfo + "\r\n";
                System.IO.File.AppendAllText("C:\\!Log\\Log_SecProv.txt", s);
            }
            catch
            {
                // Ignore errors
            }
        }
    }
}
