using AuthMicroService.Entities;

namespace AuthMicroService.Repositories.IRepositories
{
    public interface IAuthRepo
    {
        Task<User> RegisterAsync(User user); 
        Task<User?> GetUserByUserNameAsync(string userName);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUserId(Guid userId);
    }
}
