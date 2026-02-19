using Datalagring_Rasmus_Pieplow.Application.Services;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;
/// <summary>
/// 
/// </summary>
public static class RegistrationEndpoints
{
    public static void MapRegistrationEndpoints(this WebApplication app)
    {
        app.MapGet("/courseinstances/{instanceId:guid}/registrations",
            async (Guid instanceId, AppDbContext db) =>
            {
                var registrations = await db.Registrations
                    .Where(r => r.CourseInstanceId == instanceId)
                    .Include(r => r.Participant)
                    .AsNoTracking()
                    .ToListAsync();

                return registrations;
            });

        app.MapPost("/courseinstances/{instanceId:guid}/registrations",
            async (Guid instanceId, Guid participantId, RegistrationService service) =>
            {
                return await service.RegisterAsync(instanceId, participantId);
            });

        app.MapDelete("/registrations/{id:guid}",
            async (Guid id, RegistrationService service) =>
            {
                return await service.UnregisterAsync(id);
            });
    }
}
