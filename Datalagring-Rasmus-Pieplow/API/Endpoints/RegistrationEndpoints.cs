using Contracts;
using Datalagring_Rasmus_Pieplow.Application.Services;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class RegistrationEndpoints
{
    public static void MapRegistrationEndpoints(this WebApplication app)
    {
        // GET: alla registreringar för ett kurstillfälle (VG: returnera DTO, inte EF entity)
        app.MapGet("/courseinstances/{instanceId:guid}/registrations",
            async (Guid instanceId, AppDbContext db) =>
            {
                var regs = await db.Registrations
                    .Where(r => r.CourseInstanceId == instanceId)
                    .Include(r => r.Participant)
                    .AsNoTracking()
                    .Select(r => new RegistrationDto(
                        r.Id,
                        r.CourseInstanceId,
                        r.ParticipantId,
                        r.Participant.FirstName + " " + r.Participant.LastName,
                        r.Participant.Email
                    ))
                    .ToListAsync();

                return Results.Ok(regs);
            });

        // POST: skapa registrering 
        app.MapPost("/courseinstances/{instanceId:guid}/registrations",
            async (Guid instanceId, CreateRegistrationDto dto, RegistrationService service) =>
            {
                return await service.RegisterAsync(instanceId, dto.ParticipantId);
            });

        app.MapDelete("/registrations/{id:guid}",
            async (Guid id, RegistrationService service) =>
            {
                return await service.UnregisterAsync(id);
            });
    }
}