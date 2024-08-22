namespace RelayChat_Identity.Models.Dtos;

public class UnrelatedUserDto(User user)
{
    public string DisplayName { get; set; } = user.DisplayName;
}
