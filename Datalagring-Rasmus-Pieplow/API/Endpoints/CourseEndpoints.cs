using Contracts;
using Datalagring_Rasmus_Pieplow.Application.Services;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        app.MapGet("/courses", (CourseService service) =>
            service.GetAllAsync());

        app.MapGet("/courses/{id:guid}", (Guid id, CourseService service) =>
            service.GetByIdAsync(id));

        app.MapPost("/courses", (CreateCourseDto dto, CourseService service) =>
            service.CreateAsync(dto));

        app.MapPut("/courses/{id:guid}", (Guid id, UpdateCourseDto dto, CourseService service) =>
            service.UpdateAsync(id, dto));

        app.MapDelete("/courses/{id:guid}", (Guid id, CourseService service) =>
            service.DeleteAsync(id));
    }
}