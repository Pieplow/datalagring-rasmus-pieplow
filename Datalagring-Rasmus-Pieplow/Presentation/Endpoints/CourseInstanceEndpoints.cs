using Contracts;
using Datalagring_Rasmus_Pieplow.Application.Services;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class CourseInstanceEndpoints
{
    public static void MapCourseInstanceEndpoints(this WebApplication app)
    {
        app.MapGet("/courseinstances",
            (CourseInstanceService service) =>
                service.GetAllAsync());

        app.MapGet("/courses/{courseId:guid}/instances",
            (Guid courseId, CourseInstanceService service) =>
                service.GetByCourseIdAsync(courseId));

        app.MapGet("/courseinstances/{id:guid}",
            (Guid id, CourseInstanceService service) =>
                service.GetByIdAsync(id));

        app.MapPost("/courses/{courseId:guid}/instances",
            (Guid courseId, CreateCourseInstanceDto dto, CourseInstanceService service) =>
                service.CreateAsync(courseId, dto));

        app.MapPut("/courseinstances/{id:guid}",
            (Guid id, UpdateCourseInstanceDto dto, CourseInstanceService service) =>
                service.UpdateAsync(id, dto));
    }
}