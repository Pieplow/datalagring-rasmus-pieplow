using Datalagring_Rasmus_Pieplow.API.Contract;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class ParticipantEndpoints
{
    public static void MapParticipantEndpoints(this WebApplication app)
    {
        // GET: lista deltagare
        app.MapGet("/participants",
            async (AppDbContext db) =>
                await db.Participants
                    .AsNoTracking()
                    .ToListAsync());

        // GET: en deltagare
        app.MapGet("/participants/{id:guid}",
            async (Guid id, AppDbContext db) =>
            {
                var participant = await db.Participants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                return participant is null ? Results.NotFound() : Results.Ok(participant);
            });

        app.MapPost("/participants",
    async (CreateParticipantDto dto, AppDbContext db) =>
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            return Results.BadRequest("First name required");

        if (string.IsNullOrWhiteSpace(dto.LastName))
            return Results.BadRequest("Last name required");

        if (string.IsNullOrWhiteSpace(dto.Email))
            return Results.BadRequest("Email required");

        var exists = await db.Participants
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

        db.Participants.Add(participant);
        await db.SaveChangesAsync();

        return Results.Created($"/participants/{participant.Id}", participant);
    });


        app.MapPut("/participants/{id:guid}",
    async (Guid id, UpdateParticipantDto dto, AppDbContext db) =>
    {
        var participant = await db.Participants.FindAsync(id);
        if (participant is null)
            return Results.NotFound();

        participant.FirstName = dto.FirstName.Trim();
        participant.LastName = dto.LastName.Trim();
        participant.Email = dto.Email.Trim();

        await db.SaveChangesAsync();
        return Results.NoContent();
    });


        // DELETE: ta bort deltagare
        app.MapDelete("/participants/{id:guid}",
            async (Guid id, AppDbContext db) =>
            {
                var participant = await db.Participants.FindAsync(id);
                if (participant is null) return Results.NotFound();

                db.Participants.Remove(participant);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
    }
}
