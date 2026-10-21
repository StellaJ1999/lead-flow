using LeadFlow.Domain.Entites;
using LeadFlow.Domain.Enums;

namespace LeadFlow.Domain.Entities;

public class Lead
{
    // Listan ändras endast genom leadets egna metoder.
    private readonly List<FollowUpNote> _followUpNotes = [];


    // Private set gör att andra lager kan läsa värdena,
    // men inte ändra dem direkt och kringgå domänreglerna.
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    // Företag är valfritt och får därför vara null.
    public string? Company { get; private set; }
    public string Message { get; private set; }

    public LeadStatus Status { get; private set; }

    // Ett öppet lead saknar avslutsutfall.
    public LeadOutcome? Outcome { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // Ger åtkomst till anteckningarna utan att tillåta
    // att anroparen lägger till eller tar bort dem direkt.
    public IReadOnlyCollection<FollowUpNote> FollowUpNotes =>
        _followUpNotes.AsReadOnly();

    public Lead(
        string name,
        string email,
        string? company,
        string message)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required.", nameof(message));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim();
        Company = string.IsNullOrWhiteSpace(company) ? null : company.Trim();
        Message = message.Trim();

        Status = LeadStatus.New;
        Outcome = null;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsContacted()
    {
        if (Status != LeadStatus.New)
        {
            throw new InvalidOperationException(
                "Only a new lead can be marked as contacted.");
        }

        Status = LeadStatus.Contacted;
    }

    public void Close(LeadOutcome outcome)
    {
        if (!Enum.IsDefined(outcome))
            throw new ArgumentOutOfRangeException(nameof(outcome));

        if (Status == LeadStatus.Closed)
        {
            throw new InvalidOperationException(
                "The lead is already closed.");
        }

        Status = LeadStatus.Closed;
        Outcome = outcome;
    }

    public void Reopen()
    {
        if (Status != LeadStatus.Closed)
        {
            throw new InvalidOperationException(
                "Only a closed lead can be reopened.");
        }

        Status = LeadStatus.Contacted;
        Outcome = null;
    }

    public FollowUpNote AddFollowUpNote(string text, string userId)
    {
        var note = new FollowUpNote(Id, text, userId);
        _followUpNotes.Add(note);

        return note;
    }
}

