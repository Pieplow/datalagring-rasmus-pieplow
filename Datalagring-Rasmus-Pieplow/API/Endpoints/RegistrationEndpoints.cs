using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;
/// <summary>
/// Provides extension methods for mapping registration-related API endpoints to a WebApplication instance.
/// </summary>
/// <remarks>This class defines endpoints for managing course registrations, including retrieving all
/// registrations for a course instance, registering a participant, and unregistering a participant. The endpoints are
/// intended to be used with minimal API configurations in ASP.NET Core applications.</remarks>
public static class RegistrationEndpoints
{
    public static void MapRegistrationEndpoints(this WebApplication app)
    {
        // GET: All registrations for a specific course instance
        app.MapGet("/courseinstances/{instanceId:guid}/registrations",
            async (Guid instanceId, AppDbContext db) =>
            {
                var registrations = await db.Registrations
                    .Where(r => r.CourseInstanceId == instanceId)
                    .Include(r => r.Participant)
                    .ToListAsync();

                return registrations;
            });

        // POST: Register participant to course instance (VG-safe)
        app.MapPost("/courseinstances/{instanceId:guid}/registrations",
            async (Guid instanceId, Guid participantId, AppDbContext db) =>
            {
                var instance = await db.CourseInstances
                    .Include(ci => ci.Registrations)
                    .FirstOrDefaultAsync(ci => ci.Id == instanceId);

                if (instance is null)
                    return Results.NotFound("Course instance not found");

                var participantExists =
                    await db.Participants.AnyAsync(p => p.Id == participantId);

                if (!participantExists)
                    return Results.BadRequest("Participant does not exist");
                //  capacity check
                if (instance.Registrations.Count >= instance.Capacity)
                    return Results.BadRequest("Course is full");

                // prevent duplicate registration
                var alreadyRegistered =
                    instance.Registrations.Any(r => r.ParticipantId == participantId);

                if (alreadyRegistered)
                    return Results.BadRequest("Participant already registered");

                var registration = new Registration
                {
                    Id = Guid.NewGuid(),
                    CourseInstanceId = instanceId,
                    ParticipantId = participantId,
                    RegisteredAt = DateTime.UtcNow
                };

                db.Registrations.Add(registration);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/courseinstances/{instanceId}/registrations/{registration.Id}",
                    registration);
            });

            // DELETE: Unregister participant
            app.MapDelete("/registrations/{id:guid}",
            async (Guid id, AppDbContext db) =>
            {
                var registration = await db.Registrations.FindAsync(id);

                if (registration is null)
                    return Results.NotFound();

                db.Registrations.Remove(registration);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
    }
}
