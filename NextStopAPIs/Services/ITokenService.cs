using NextStopAPIs.DTOs;

namespace NextStopAPIs.Services
{
    public interface ITokenService
    {
        string GenerateToken(TokenDTO tokenDTO);
        Task SaveRefreshToken(string username, string token);
        Task<string> RetrieveEmailByRefreshToken(string refreshToken);
        Task<bool> RevokeRefreshToken(string refreshToken);
        string GenerateRefreshToken();
    }
}
