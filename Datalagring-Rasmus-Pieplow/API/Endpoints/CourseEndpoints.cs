using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Contracts;




namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        // GET all
        app.MapGet("/courses", async (AppDbContext db) =>
            await db.Courses
                .AsNoTracking()
                .ToListAsync());

        // GET by id
        app.MapGet("/courses/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var course = await db.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return course is null
                ? Results.NotFound()
                : Results.Ok(course);
        });

        // POST
        app.MapPost("/courses", async (CreateCourseDto dto, AppDbContext db) =>
        {
            var course = new Course
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();

            return Results.Created($"/courses/{course.Id}", course);
        });

        // PUT
        app.MapPut("/courses/{id:guid}", async (Guid id, UpdateCourseDto dto, AppDbContext db) =>
        {
            var course = await db.Courses.FindAsync(id);
            if (course is null) return Results.NotFound();

            course.Name = dto.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE
        app.MapDelete("/courses/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var course = await db.Courses.FindAsync(id);
            if (course is null) return Results.NotFound();

            db.Courses.Remove(course);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
