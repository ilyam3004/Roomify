using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChatApp.Infrastructure.Config;

public class AppDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("SqlConnection")!;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}