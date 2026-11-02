using LeadFlow.Domain.Entities;
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
        // Leadets eget ID skickas med automatiskt.
        var note = new FollowUpNote(Id, text, userId);

        // Anteckningen läggs till i leadets interna samling.
        _followUpNotes.Add(note);

        return note;
    }

    public void ChangeStatus(
    LeadStatus status,
    LeadOutcome? outcome)
    {
        // Kontrollera hela begäran innan något ändras.
        if (!Enum.IsDefined(status))
            throw new ArgumentException("Invalid lead status.");

        if (status == LeadStatus.Closed)
        {
            if (outcome is null || !Enum.IsDefined(outcome.Value))
            {
                throw new ArgumentException(
                    "Closing a lead requires Customer or Not interested.");
            }
        }
        else if (outcome is not null)
        {
            throw new ArgumentException(
                "An open lead cannot have a closing outcome.");
        }

        // Samma värden kan skickas igen utan att ge fel.
        if (status == Status && outcome == Outcome)
            return;

        switch (status)
        {
            case LeadStatus.Contacted when Status == LeadStatus.New:
                MarkAsContacted();
                break;

            case LeadStatus.Contacted when Status == LeadStatus.Closed:
                Reopen();
                break;

            case LeadStatus.Closed when Status != LeadStatus.Closed:
                Close(outcome!.Value);
                break;

            default:
                throw new InvalidOperationException(
                    "This status change is not allowed.");
        }
    }
}

