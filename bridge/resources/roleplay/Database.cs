using MySql.Data;
using MySql.Data.MySqlClient;

namespace roleplay
{
    public class Database
    {
        public string Address;
        public string Login;
        public string Password;
        public string DatabaseName;

        public MySqlConnection Connection;

        public bool Connect()
        {
            string connectionString = string.Format("Server={0}; database={1}; UID={2}; password={3}", Address, DatabaseName, Login, Password);
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
            if(Connection.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            return false;
        }

        private static Database _instance;
        public static Database Instance()
        {
            if (_instance == null)
                _instance = new Database();
            return _instance;
        }
    }
}
