using Npgsql;

namespace DentalClinic.Database
{
    public static class DbConnection
    {
        private static string? _dbPassword;
        private const string BaseConnectionString = "Host=localhost;Port=5432;Database=dental_office;Username=postgres";
        
        public static void SetDbPassword(string password)
        {
            _dbPassword = password;
        }
        
        private static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_dbPassword))
                {
                    return BaseConnectionString + ";Password=12345"; // Значение по умолчанию
                }
                return BaseConnectionString + $";Password={_dbPassword}";
            }
        }
        
        public static NpgsqlConnection CreateConnection()
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
