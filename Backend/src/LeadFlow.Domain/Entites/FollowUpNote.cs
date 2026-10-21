
namespace LeadFlow.Domain.Entites;

public class FollowUpNote
{
    public Guid Id { get; set; }
    public Guid LeadId { get; set; }
    public string UserId { get; private set; }
    public string Text { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    internal FollowUpNote(Guid leadId, string text, string userId)
    {
        if (leadId == Guid.Empty)
            throw new ArgumentNullException("Lead ID is required.", nameof(leadId));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Note text is required.", nameof(text));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Author ID is required.", nameof(userId));

        Id = Guid.NewGuid();
        LeadId = leadId;
        Text = text.Trim();
        UserId = userId;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
