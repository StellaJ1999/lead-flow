using LeadFlow.Application.Abstractions.Repositories;
using LeadFlow.Domain.Entities;
using LeadFlow.Infrastructure.Identity.Data;
using Microsoft.EntityFrameworkCore;
using LeadFlow.Application.DTOs.Leads;

namespace LeadFlow.Infrastructure.Persistence.Repositories;

public sealed class LeadRepository(AppIdentityDbContext context)
    : ILeadRepository
{
    public async Task AddAsync(
        Lead lead,
        CancellationToken cancellationToken)
    {
        context.Leads.Add(lead);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Lead>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Leads
            .AsNoTracking()
            .OrderByDescending(lead => lead.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Lead?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await context.Leads
            .AsNoTracking()
            .SingleOrDefaultAsync(
                lead => lead.Id == id,
                cancellationToken);
    }

    public async Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var deletedRows = await context.Leads
            .Where(lead => lead.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedRows > 0;
    }
    public async Task<LeadDetailsResponse?> GetDetailsByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var lead = await context.Leads
            .AsNoTracking()
            .SingleOrDefaultAsync(
                lead => lead.Id == id,
                cancellationToken);

        if (lead is null)
            return null;

        var notes = await (
            from note in context.FollowUpNotes.AsNoTracking()
            join author in context.Users.AsNoTracking()
                on note.UserId equals author.Id
            where note.LeadId == id
            orderby note.CreatedAt descending, note.Id descending
            select new FollowUpNoteResponse(
                note.Id,
                note.Text,
                author.UserName ?? author.Email ?? author.Id,
                note.CreatedAt)
        ).ToListAsync(cancellationToken);

        return new LeadDetailsResponse(
            lead.Id,
            lead.Name,
            lead.Email,
            lead.Company,
            lead.Message,
            lead.Status,
            lead.CreatedAt,
            lead.Outcome,
            notes);
    }
    public Task<Lead?> GetForUpdateAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return context.Leads
            .SingleOrDefaultAsync(
                lead => lead.Id == id,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<string?> GetAuthorDisplayNameAsync(
    string authorId,
    CancellationToken cancellationToken)
    {
        return context.Users
            .AsNoTracking()
            .Where(user => user.Id == authorId)
            .Select(user => user.UserName ?? user.Email ?? user.Id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void AddFollowUpNote(FollowUpNote note)
    {
        context.FollowUpNotes.Add(note);
    }
}