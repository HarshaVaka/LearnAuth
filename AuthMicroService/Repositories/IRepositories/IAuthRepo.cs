using AuthMicroService.Entities;

namespace AuthMicroService.Repositories.IRepositories
{
    public interface IAuthRepo
    {
        Task<User> RegisterAsync(User user); 
        Task<User?> GetUserByUserIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUserId(Guid userId);
    }
}
