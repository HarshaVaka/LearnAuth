namespace AuthMicroService.Repositories.IRepositories
{
    using AuthMicroService.Entities;

    public interface IRefreshTokenRepo
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task<RefreshToken?> AddRefreshTokenAsync(RefreshToken refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    }
}