namespace SolaBid.Domain.Models
{
    public static class Statics
    {
        public static string ConnectionString(string database = "SolaBid")
        {
            //return $"Server=localhost;Database={database};User Id=sa;Password=Pass1234;TrustServerCertificate=true";
            return $"Server=localhost;Database={database};User Id=solo;Password=cL8Kc&zW@V8pj9;Connection Timeout=60;TrustServerCertificate=true";
        }

        public static string API_SOCAR_BASE = "http://localhost:444/";

        public static string API_BASE = "http://116.203.90.202:444/";

        public static bool IsProductionEnvironment = true;
    }
}