namespace RelayChat_Identity.Models.Dtos;

public class DirectMessageDto(DirectMessage directMessage)
{
    public Guid Id { get; set; } = directMessage.Id;
    public Guid SenderId { get; set; } = directMessage.SenderId;
    public User? Sender { get; set; } = directMessage.Sender;
    public Guid ReceiverId { get; set; } = directMessage.ReceiverId;
    public User? Receiver { get; set; } = directMessage.Receiver;
    public DateTimeOffset SentAt { get; set; } = directMessage.SentAt;
    public string Message { get; set; } = directMessage.Message;
    public bool Read { get; set; } = directMessage.Read;
}
