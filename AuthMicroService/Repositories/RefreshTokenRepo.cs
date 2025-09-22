using AuthMicroService.Entities;
using AuthMicroService.Infrastructure;
using AuthMicroService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroService.Repositories
{
    public class RefreshTokenRepo(AuthServiceDBContext dbContext) : IRefreshTokenRepo
    {
        private readonly AuthServiceDBContext _dbContext = dbContext;

        public async Task<RefreshToken?> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
    }
}