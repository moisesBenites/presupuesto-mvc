using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;

namespace Presupuesto.Services
{
    public interface IRepositorioCuentas
    {
        Task Create(Cuenta cuenta);
        Task Delete(int id);
        Task<IEnumerable<Cuenta>> Find(int userId);
        Task<Cuenta> GetById(int id, int userId);
        Task Update(CuentaCreacionViewModel cuenta);
    }

    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (nombre, tipoCuentaId, descripcion, balance)
                                                         VALUES (@nombre, @tipoCuentaId, @descripcion, @balance);
                                                         SELECT SCOPE_IDENTITY();", cuenta);

            cuenta.id = id;
        }

        public async Task<IEnumerable<Cuenta>> Find(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>("select c.id, c.nombre, c.balance, a.name as TipoCuenta " +
                                                       "from Cuentas as c " +
                                                       "inner join Accounts as a " +
                                                       "on c.tipoCuentaId = a.id " +
                                                       "where a.userId = @userId " +
                                                       "order by a.orderId", new { userId });
        }

        public async Task<Cuenta> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>("select c.id, c.nombre, c.balance, c.descripcion, a.name as TipoCuenta " +
                                                       "from Cuentas as c " +
                                                       "inner join Accounts as a " +
                                                       "on c.tipoCuentaId = a.id " +
                                                       "where a.userId = @userId and c.id = @id " +
                                                       "order by a.orderId", new { id, userId });
        }

        public async Task Update(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas
                                            SET nombre = @nombre, balance = @balance, 
                                            descripcion = @descripcion, tipoCuentaId = @tipoCuentaId
                                            WHERE id = @id", cuenta);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Cuentas WHERE id = @id", new { id });
        }
    }
}
