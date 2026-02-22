using Contracts;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Datalagring_Rasmus_Pieplow.Application.Services;

public class ParticipantService
{
    private readonly AppDbContext _db;

    public ParticipantService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> GetAllAsync()
    {
        var participants = await _db.Participants
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(participants);
    }

    public async Task<IResult> GetByIdAsync(Guid id)
    {
        var participant = await _db.Participants
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return participant is null
            ? Results.NotFound()
            : Results.Ok(participant);
    }

    public async Task<IResult> CreateAsync(CreateParticipantDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            return Results.BadRequest("First name required");

        if (string.IsNullOrWhiteSpace(dto.LastName))
            return Results.BadRequest("Last name required");

        if (string.IsNullOrWhiteSpace(dto.Email))
            return Results.BadRequest("Email required");

        var exists = await _db.Participants
            .AnyAsync(p => p.Email == dto.Email);

        if (exists)
            return Results.BadRequest("Email already exists");

        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim()
        };

        _db.Participants.Add(participant);
        await _db.SaveChangesAsync();

        return Results.Created($"/participants/{participant.Id}", participant);
    }

    public async Task<IResult> UpdateAsync(Guid id, UpdateParticipantDto dto)
    {
        var participant = await _db.Participants.FindAsync(id);
        if (participant is null)
            return Results.NotFound();

        participant.FirstName = dto.FirstName.Trim();
        participant.LastName = dto.LastName.Trim();
        participant.Email = dto.Email.Trim();

        await _db.SaveChangesAsync();

        return Results.NoContent();
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var participant = await _db.Participants.FindAsync(id);
        if (participant is null)
            return Results.NotFound();

        _db.Participants.Remove(participant);
        await _db.SaveChangesAsync();

        return Results.NoContent();
    }
}