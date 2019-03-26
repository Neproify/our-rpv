using MySql.Data.MySqlClient;

namespace roleplay
{
    public class Database
    {
        public string adress;
        public string login;
        public string password;
        public string databaseName;

        public MySqlConnection connection;

        public bool Connect()
        {
            string connectionString = $"Server={adress}; database={databaseName}; UID={login}; password={password}";
            connection = new MySqlConnection(connectionString);
            connection.Open();
            if(connection.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            return false;
        }

        private static Database _instance;
        public static Database Instance()
        {
            return _instance ?? (_instance = new Database());
        }
    }
}
