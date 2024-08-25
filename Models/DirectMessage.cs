using System.ComponentModel.DataAnnotations;
using RelayChat_Identity.Models.Dtos;

namespace RelayChat_Identity.Models;

public class DirectMessage : IDtoAble<DirectMessageDto>
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public User? Sender { get; set; }
    public Guid ReceiverId { get; set; }
    public User? Receiver { get; set; }
    public DateTimeOffset SentAt { get; set; }
    [MaxLength(4096)]
    public required string Message { get; set; }
    public bool Read { get; set; }
    
    public DirectMessageDto ToDto() => new(this);
}
