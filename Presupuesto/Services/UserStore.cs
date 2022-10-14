using Microsoft.AspNetCore.Identity;
using Presupuesto.Models;

namespace Presupuesto.Services
{
	public class UserStore : IUserStore<User>, IUserEmailStore<User>, IUserPasswordStore<User>
	{
		private readonly IUserRepository userRepository;

		public UserStore(IUserRepository userRepository)
		{
			this.userRepository = userRepository;
		}
		public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
		{
			user.id = await userRepository.CreateUser(user);
			return IdentityResult.Success;
		}

		public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			return await userRepository.FindUserByEmail(normalizedEmail);
		}

		public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			return await userRepository.FindUserByEmail(normalizedUserName);
		}

		public async Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
		{
			return await Task.FromResult(user.email);
		}

		public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
		{
			return await Task.FromResult(user.password);
		}

		public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.id.ToString());
		}

		public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
		{
			return await Task.FromResult(user.email);
		}

		public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
		{
			user.standardEmail = normalizedEmail;
			return Task.CompletedTask;
		}

		public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
		{
            user.password = passwordHash;
            return Task.CompletedTask;
        }

		public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
