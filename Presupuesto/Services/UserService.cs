namespace Presupuesto.Services
{
    public interface IUserService
    {
        int GetUserId();
    }
    public class UserService : IUserService
    {
        public int GetUserId()
        {
            return 1;
        }
    }
}
