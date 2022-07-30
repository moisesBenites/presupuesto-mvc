using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;

namespace Presupuesto.Services
{
    public interface IAccountRepository
    {
        Task Create(Account account);
        Task<bool> Exists(string name, int userId);
        Task<IEnumerable<Account>> GetAccount(int UserId);
    }
    public class AccountRepository : IAccountRepository
    {
        public readonly string connectionString;
        public AccountRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>($"INSERT INTO Accounts (name, userId, orderId)" +
                                                 $"VALUES(@name, @userId, @orderId)" +
                                                 $"SELECT SCOPE_IDENTITY();", account);

            account.id = id;
        }

        public async Task<bool> Exists(string name, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            var Result = await connection.QueryFirstOrDefaultAsync("select 1 from Accounts where name = @name and userId = @userId;", new { name, userId });

            return Result != null;
        }


        public async Task<IEnumerable<Account>> GetAccount(int UserId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Account>("select id, name, orderId " +
                                                        "from Accounts " +
                                                        "where userId = @userId", new { UserId });
        }
    }
}
