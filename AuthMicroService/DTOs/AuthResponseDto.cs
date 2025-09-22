namespace AuthMicroService.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(15);
    }

}
