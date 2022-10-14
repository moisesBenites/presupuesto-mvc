using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;

namespace Presupuesto.Services
{
	public interface IUserRepository
	{
		Task<User> FindUserByEmail(string email);
		Task<int> CreateUser(User user);
	}

	public class UserRepository : IUserRepository
	{
		private readonly string connectionString;
		public UserRepository(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async Task<int> CreateUser(User user)
		{
			using var connection = new SqlConnection(connectionString);
			var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Users 
                                                              (email, standardEmail, password)
                                                              VALUES (@email, @standardEmail, @password)", user);

			return id;
		}

        public async Task<User> FindUserByEmail(string email)
        {
            using var connection = new SqlConnection(connectionString);
           return await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE standardEmail = @standardEmail", new { email });
        }
    }
}
