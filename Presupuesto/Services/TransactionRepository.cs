using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;
using System.Data;

namespace Presupuesto.Services
{

    public interface ITransactionRepository
    {
        Task Create(Transactions transaction);
        Task Delete(int id);
        Task<IEnumerable<Transactions>> GetByAccountId(GetTransactionByAccount model);
        Task<Transactions> GetById(int id, int userId);
        Task<IEnumerable<GetByMonthResponse>> GetByMonthly(int userId, int year);
        Task<IEnumerable<Transactions>> GetByUserId(TransactionByUserParameter model);
		Task<IEnumerable<GetByWeeklyResponse>> GetByWeekly(TransactionByUserParameter model);
		Task Update(Transactions transaction, decimal lastAmount, int cuentaAnterior);
	}

    public class TransactionRepository : ITransactionRepository
    {
        private readonly string connectionString;
        public TransactionRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transactions transaction)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("InsertTransaction",
                new
                {
                    transaction.userId,
                    transaction.date,
                    transaction.amount,
                    transaction.categoryId,
                    transaction.cuentaId,
                    transaction.note
                },
                commandType: CommandType.StoredProcedure);

            transaction.id = id;
        }

        public async Task Update(Transactions transaction, decimal lastAmount, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("UpdateTransaction", new
            {
                transaction.id,
                transaction.date,
                transaction.amount,
                transaction.categoryId,
                transaction.cuentaId,
                transaction.note,
                lastAmount,
                cuentaAnteriorId
            },
            commandType: CommandType.StoredProcedure);
        }

        public async Task<Transactions> GetById(int id, int userId)
		{
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transactions>(@"SELECT t.*, cat.operationTypeId
                                                                             FROM Transactions t
                                                                             INNER JOIN Categories cat
                                                                             ON cat.id = t.categoryId
                                                                             WHERE t.id = @id AND t.userId = @userId",
                                                                             new
																			 {
                                                                                 id,
                                                                                 userId
																			 });
		}

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DeleteTransactions", new { id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transactions>> GetByAccountId(GetTransactionByAccount model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transactions>(@"select t.id, t.amount, t.date, c.name as category, 
                                                        cu.nombre as Cuenta, c.operationTypeId
                                                        from Transactions t
                                                        inner join Categories c
                                                        on c.id = t.categoryId
                                                        inner join Cuentas cu
                                                        on cu.id = t.cuentaId
                                                        where t.cuentaId = @cuentaId and t.userId = @userId
                                                        and date between @initialDate and @finalDate",
                                                        model);
        }
        public async Task<IEnumerable<Transactions>> GetByUserId(TransactionByUserParameter model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transactions>(@"select t.id, t.amount, t.date, c.name as category, 
                                                        cu.nombre as Cuenta, c.operationTypeId, t.note
                                                        from Transactions t
                                                        inner join Categories c
                                                        on c.id = t.categoryId
                                                        inner join Cuentas cu
                                                        on cu.id = t.cuentaId
                                                        where t.userId = @userId
                                                        and date between @initialDate and @finalDate
                                                        order by t.date DESC",
                                                        model);
        }  

        public async Task<IEnumerable<GetByWeeklyResponse>> GetByWeekly(TransactionByUserParameter model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<GetByWeeklyResponse>(@"select DATEDIFF(d, @initialDate, t.date) / 7 + 1 as week,
                                                                    sum(t.amount) as amount, cat.operationTypeId
                                                                    from Transactions as t
                                                                    inner join Categories as cat
                                                                    on cat.id = t.categoryId
                                                                    where t.userId = @userId and t.date between @initialDate and @finalDate
                                                                    group by DATEDIFF(d, @initialDate, t.date) / 7, cat.operationTypeId",
                                                               model);
        }   
        
        public async Task<IEnumerable<GetByMonthResponse>> GetByMonthly(int userId, int year)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<GetByMonthResponse>(@"select MONTH(t.date) as month,
                                                                      sum(t.amount) as amount, cat.operationTypeId
                                                                      from Transactions as t
                                                                      inner join Categories as cat
                                                                      on cat.id = t.categoryId
                                                                      where t.userId = @userId and YEAR(t.date) = @year
                                                                      group by MONTH(t.date), cat.operationTypeId",
                                                               new
                                                               {
                                                                   userId,
                                                                   year
                                                               });
        }


    }
}
