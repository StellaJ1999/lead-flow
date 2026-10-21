using LeadFlow.Domain.Entities;
using LeadFlow.Domain.Enums;
using Xunit;

namespace LeadFlow.UnitTests.Domain;

public class LeadTests
{
    private static Lead CreateLead() =>
        new("Anna Andersson", "anna@example.com", null, "Would like more information.");

    [Fact]
    public void Constructor_StartsWithNewStatusAndNoOutcome()
    {
        var lead = CreateLead();

        Assert.Equal(LeadStatus.New, lead.Status);
        Assert.Null(lead.Outcome);
    }

    [Fact]
    public void MarkAsContacted_FromNew_ChangesStatusWithoutOutcome()
    {
        var lead = CreateLead();

        lead.MarkAsContacted();

        Assert.Equal(LeadStatus.Contacted, lead.Status);
        Assert.Null(lead.Outcome);
    }

    [Theory]
    [InlineData(LeadOutcome.Customer, false)]
    [InlineData(LeadOutcome.NotInterested, false)]
    [InlineData(LeadOutcome.Customer, true)]
    [InlineData(LeadOutcome.NotInterested, true)]
    public void Close_FromOpenLead_SetsStatusAndOutcome(
        LeadOutcome outcome,
        bool contacted)
    {
        var lead = CreateLead();

        if (contacted)
            lead.MarkAsContacted();

        lead.Close(outcome);

        Assert.Equal(LeadStatus.Closed, lead.Status);
        Assert.Equal<LeadOutcome?>(outcome, lead.Outcome);
    }

    [Fact]
    public void Close_WithInvalidOutcome_ThrowsAndKeepsLeadOpen()
    {
        var lead = CreateLead();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => lead.Close((LeadOutcome)999));

        Assert.Equal(LeadStatus.New, lead.Status);
        Assert.Null(lead.Outcome);
    }

    [Fact]
    public void Close_WhenAlreadyClosed_ThrowsAndPreservesOutcome()
    {
        var lead = CreateLead();
        lead.Close(LeadOutcome.Customer);

        Assert.Throws<InvalidOperationException>(
            () => lead.Close(LeadOutcome.NotInterested));

        Assert.Equal(LeadStatus.Closed, lead.Status);
        Assert.Equal<LeadOutcome?>(LeadOutcome.Customer, lead.Outcome);
    }

    [Theory]
    [InlineData(LeadOutcome.Customer)]
    [InlineData(LeadOutcome.NotInterested)]
    public void Reopen_FromClosed_ClearsOutcome(LeadOutcome outcome)
    {
        var lead = CreateLead();
        lead.Close(outcome);

        lead.Reopen();

        Assert.Equal(LeadStatus.Contacted, lead.Status);
        Assert.Null(lead.Outcome);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Reopen_FromOpenLead_Throws(bool contacted)
    {
        var lead = CreateLead();

        if (contacted)
            lead.MarkAsContacted();

        var originalStatus = lead.Status;

        Assert.Throws<InvalidOperationException>(() => lead.Reopen());

        Assert.Equal(originalStatus, lead.Status);
        Assert.Null(lead.Outcome);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MarkAsContacted_FromNonNewLead_Throws(bool closed)
    {
        var lead = CreateLead();
        lead.MarkAsContacted();

        if (closed)
            lead.Close(LeadOutcome.Customer);

        var originalStatus = lead.Status;
        var originalOutcome = lead.Outcome;

        Assert.Throws<InvalidOperationException>(
            () => lead.MarkAsContacted());

        Assert.Equal(originalStatus, lead.Status);
        Assert.Equal(originalOutcome, lead.Outcome);
    }

    [Fact]
    public void ReopenedLead_CanBeClosedWithAnotherOutcome()
    {
        var lead = CreateLead();
        lead.Close(LeadOutcome.NotInterested);
        lead.Reopen();

        lead.Close(LeadOutcome.Customer);

        Assert.Equal(LeadStatus.Closed, lead.Status);
        Assert.Equal<LeadOutcome?>(LeadOutcome.Customer, lead.Outcome);
    }

    [Fact]
    public void AddFollowUpNote_AllowsMultipleNotesWithAuthorAndTimestamp()
    {
        var lead = CreateLead();
        var before = DateTimeOffset.UtcNow;

        var first = lead.AddFollowUpNote("Called the customer.", "user-1");
        var second = lead.AddFollowUpNote("Scheduled a meeting.", "user-2");

        var after = DateTimeOffset.UtcNow;

        Assert.Equal(2, lead.FollowUpNotes.Count);

        Assert.Equal("Called the customer.", first.Text);
        Assert.Equal("user-1", first.UserId);
        Assert.Equal(lead.Id, first.LeadId);
        Assert.InRange(first.CreatedAt, before, after);

        Assert.Equal("Scheduled a meeting.", second.Text);
        Assert.Equal("user-2", second.UserId);
        Assert.Equal(lead.Id, second.LeadId);
        Assert.InRange(second.CreatedAt, before, after);
    }

    [Theory]
    [InlineData("", "user-1")]
    [InlineData("   ", "user-1")]
    [InlineData("Called the customer.", "")]
    public void AddFollowUpNote_WithMissingTextOrAuthor_Throws(
        string text,
        string userId)
    {
        var lead = CreateLead();

        Assert.Throws<ArgumentException>(
            () => lead.AddFollowUpNote(text, userId));

        Assert.Empty(lead.FollowUpNotes);
    }
}