namespace UOC.SharedContracts
{
    public class PostgresqlConfigOptions
    {
        public string host { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string database { get; set; }
        public string SslMode { get; set; }
        public string Connection => $"Server={host};Port={port};Database={database};User Id={username};Password={password};Maximum Pool Size=1000;";
    }
}