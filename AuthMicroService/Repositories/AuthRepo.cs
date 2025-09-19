using AuthMicroService.Entities;
using AuthMicroService.Infrastructure;
using AuthMicroService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroService.Repositories
{
    public class AuthRepo(AuthServiceDBContext dbContext) : IAuthRepo
    {
        private readonly AuthServiceDBContext _dbContext = dbContext;

        public async Task<User> RegisterAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _dbContext.Users
                  .Include(u => u.UserRoles)
                  .ThenInclude(ur => ur.Role)
                  .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User?> GetUserByEmailAsync(string mail)
        {
            return await _dbContext.Users
                  .Include(u => u.UserRoles)
                  .ThenInclude(ur => ur.Role)
                  .FirstOrDefaultAsync(u => u.Email == mail);
        }
        public async Task<User?> GetUserByUserId(Guid userId)
        {
            return await _dbContext.Users
                .Include(urm => urm.UserRoles)
                .ThenInclude(r =>r.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
    }
}
