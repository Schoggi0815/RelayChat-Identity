namespace RelayChat_Identity.Models.Dtos;

public class TokenDto
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}