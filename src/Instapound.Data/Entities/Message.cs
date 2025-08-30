namespace Instapound.Data.Entities;

public class Message
{
    public Guid Id { get; set; }
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; set; }

    public Guid ToUserId { get; set; }
    public User? ToUser { get; set; }
    public Guid FromUserId { get; set; }
    public User? FromUser { get; set; }
}