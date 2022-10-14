using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;

namespace Presupuesto.Services
{
    public interface ICategoryRepository
    {
        Task Create(Category category);
        Task Delete(int id);
        Task<Category> GetById(int id, int userId);
        Task<IEnumerable<Category>> GetCategories(int userId);
		Task<IEnumerable<Category>> GetCategories(int userId, OperationTypeEnum operationType);
		Task Update(Category category);
    }

    public class CategoryRepository : ICategoryRepository
    {

        private readonly string connectionString;
        public CategoryRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categories (name, operationTypeId, userId)
                                                       VALUES(@name, @operationTypeId, @userId)
                                                       SELECT SCOPE_IDENTITY();", category);


            category.id = id;
        }

        public async Task<IEnumerable<Category>> GetCategories(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@"SELECT * FROM Categories WHERE userId = @userId", new { userId });
        }

        public async Task<IEnumerable<Category>> GetCategories(int userId, OperationTypeEnum operationTypeId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@"SELECT * FROM Categories 
                                                          WHERE userId = @userId and operationTypeId = @operationTypeId", 
                                                          new { userId, operationTypeId });
        }

        public async Task<Category> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
           return await connection.QueryFirstOrDefaultAsync<Category>(@"SELECT * FROM Categories 
                                                                           WHERE id = @id and userId = @userId", 
                                                                           new { id, userId });
        }

        public async Task Update(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categories
                                            SET name = @name, operationTypeId = @operationTypeId
                                            WHERE id = @id",
                                            category);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM Categories WHERE id = @id", new { id });
        }
    }
}
