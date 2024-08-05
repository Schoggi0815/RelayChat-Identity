namespace RelayChat_Identity.Models.Dtos;

public class LoginDto
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Displayname { get; set; }
}