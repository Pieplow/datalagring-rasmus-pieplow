using Contracts;
using Datalagring_Rasmus_Pieplow.Application.Services;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class InstructorEndpoints
{
    public static void MapInstructorEndpoints(this WebApplication app)
    {
        app.MapGet("/instructors",
            (InstructorService service) =>
                service.GetAllAsync());

        app.MapGet("/instructors/{id:guid}",
            (Guid id, InstructorService service) =>
                service.GetByIdAsync(id));

        app.MapPost("/instructors",
            (CreateInstructorDto dto, InstructorService service) =>
                service.CreateAsync(dto));

        app.MapPut("/instructors/{id:guid}",
            (Guid id, UpdateInstructorDto dto, InstructorService service) =>
                service.UpdateAsync(id, dto));

        app.MapDelete("/instructors/{id:guid}",
            (Guid id, InstructorService service) =>
                service.DeleteAsync(id));
    }
}