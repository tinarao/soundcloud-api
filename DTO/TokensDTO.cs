namespace Sounds_New.DTO
{
    public class TokensDTO
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }

    public class RefreshTokensDTO
    {
        public required int UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}