namespace RelayChat_Identity.Models.Dtos;

public class FriendRequestDto(FriendRequest friendRequest)
{
    public Guid SenderId { get; set; } = friendRequest.SenderId;
    public Guid ReceiverId { get; set; } = friendRequest.ReceiverId;
    public UnrelatedUserDto? Sender { get; set; } = friendRequest.Sender?.ToDto();
    public UnrelatedUserDto? Receiver { get; set; } = friendRequest.Receiver?.ToDto();
}
