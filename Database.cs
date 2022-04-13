using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace mmorpg_server
{
    public static class Database
    {
        private const string userConnectionString = "Server=CHASE-PC\\SQLEXPRESS01;Database=mmorpg;Trusted_Connection=True;";

        public static byte[] GenerateHash(byte[] value) => new SHA256Managed().ComputeHash(value);

        public static byte[] GenerateSalt(int length)
        {
            RNGCryptoServiceProvider crypt = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[length];

            crypt.GetBytes(buffer);

            return buffer;
        }

        public static byte[] GenerateSaltedHash(byte[] value, byte[] salt)
        {
            byte[] saltedValue = new byte[value.Length + salt.Length];

            for (int i = 0; i < value.Length; i++)
                saltedValue[i] = value[i];

            for (int i = 0; i < salt.Length; i++)
                saltedValue[value.Length + i] = salt[i];

            return GenerateHash(saltedValue);
        }

        public static void RegisterUser(string username, byte[] hash, byte[] salt, Guid guid)
        {
            SqlConnection sql = new SqlConnection(userConnectionString);
            SqlCommand cmd = new SqlCommand(
                $"INSERT INTO registry (username, hash, salt, guid) VALUES (\'{username}\', \'{Convert.ToBase64String(hash)}\', \'{Convert.ToBase64String(salt)}\', \'{guid.ToString()}\');",
                sql);

            sql.Open();
            cmd.ExecuteNonQuery();
        }

        public static bool VerifyUsername(string username)
        {
            SqlConnection sql = new SqlConnection(userConnectionString);
            SqlCommand cmd = new SqlCommand($"SELECT username FROM registry WHERE username=\'{username}\';", sql);
            SqlDataAdapter data = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            data.Fill(table);

            if (table.Rows.Count > 0)
                return true;

            return false;
        }

        public static bool VerifyPassword(string username, string password)
        {
            SqlConnection sql = new SqlConnection(userConnectionString);
            sql.Open();

            SqlCommand cmd = new SqlCommand($"SELECT hash, salt FROM registry WHERE username=\'{username}\';", sql);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string hash = reader["hash"].ToString();
                byte[] hashBytes = Convert.FromBase64String(hash);

                string salt = reader["salt"].ToString();
                byte[] saltBytes = Convert.FromBase64String(salt);

                byte[] saltedHash = Database.GenerateSaltedHash(Encoding.ASCII.GetBytes(password), saltBytes);

                if (saltedHash.SequenceEqual(hashBytes))
                    return true;
            }

            return false;
        }

        public static bool VerifyCharacterGuid(string guid)
        {
            SqlConnection sql = new SqlConnection(userConnectionString);
            SqlCommand cmd = new SqlCommand($"SELECT guid FROM registry WHERE guid=\'{guid}\';", sql);
            SqlDataAdapter data = new SqlDataAdapter(cmd);

            DataTable table = new DataTable();
            data.Fill(table);

            if (table.Rows.Count > 0)
                return true;

            return false;
        }

        public static Guid GetCharacterGuid(string username)
        {
            SqlConnection sql = new SqlConnection(userConnectionString);
            sql.Open();

            SqlCommand cmd = new SqlCommand($"SELECT guid FROM registry WHERE username=\'{username}\';", sql);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string guid = reader["guid"].ToString();
                return Guid.Parse(guid);
            }

            return Guid.Empty;
        }
    }
}
