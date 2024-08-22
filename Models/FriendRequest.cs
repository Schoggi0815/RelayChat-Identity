namespace RelayChat_Identity.Models;

public class FriendRequest
{
    public Guid SenderId { get; set; }
    public User? Sender { get; set; }
    public Guid ReceiverId { get; set; }
    public User? Receiver { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool Accepted { get; set; }
    public DateTimeOffset AcceptedAt { get; set; }
}
