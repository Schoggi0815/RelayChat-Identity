namespace RelayChat_Identity.Models.Dtos;

public class UnrelatedUserDto(User user)
{
    public Guid Id { get; set; } = user.Id;
    public string DisplayName { get; set; } = user.DisplayName;
}
