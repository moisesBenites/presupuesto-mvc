using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;
using System.Data;

namespace Presupuesto.Services
{
    public interface IAccountRepository
    {
        Task Create(Account account);
        Task Delete(int id);
        Task<bool> Exists(string name, int userId);
        Task<IEnumerable<Account>> GetAccount(int UserId);
        Task<Account> GetAccountById(int id, int userId);
        Task Sort(IEnumerable<Account> sortedAccounts);
        Task Update(Account account);
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
            var id = await connection.QuerySingleAsync<int>("InsertAccount",
                                                             new { userId = account.userId, name = account.name },
                                                             commandType: CommandType.StoredProcedure);

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
            return await connection.QueryAsync<Account>("select id, name, orderId" +
                                                        " from Accounts" +
                                                        " where userId = @userId" +
                                                        " order by orderId", new { UserId });
        }

        public async Task Update(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("update Accounts set name = @name where id = @id", account);
        }

        public async Task<Account> GetAccountById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>("select *" +
                                                              " from Accounts" +
                                                              " where id = @id and userId = @userId", new { id, userId });
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete from Accounts where id = @id", new { id });
        }

        public async Task Sort(IEnumerable<Account> sortedAccounts)
        {
            string Query = "UPDATE Accounts set orderId = @orderId where id = @id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(Query, sortedAccounts);
        }
    }
}
