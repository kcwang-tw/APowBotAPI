using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BotKernel.Data
{
    public class OracleDbContext
    {
        private readonly string _connectionString;

        public OracleDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")!;
        }

        public IDbConnection CreateConnection() => new OracleConnection(_connectionString);
    }
}
