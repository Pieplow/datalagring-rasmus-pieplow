using Contracts;
using Datalagring_Rasmus_Pieplow.Application.Services;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class ParticipantEndpoints
{
    public static void MapParticipantEndpoints(this WebApplication app)
    {
        app.MapGet("/participants",
            (ParticipantService service) =>
                service.GetAllAsync());

        app.MapGet("/participants/{id:guid}",
            (Guid id, ParticipantService service) =>
                service.GetByIdAsync(id));

        app.MapPost("/participants",
            (CreateParticipantDto dto, ParticipantService service) =>
                service.CreateAsync(dto));

        app.MapPut("/participants/{id:guid}",
            (Guid id, UpdateParticipantDto dto, ParticipantService service) =>
                service.UpdateAsync(id, dto));

        app.MapDelete("/participants/{id:guid}",
            (Guid id, ParticipantService service) =>
                service.DeleteAsync(id));
    }
}