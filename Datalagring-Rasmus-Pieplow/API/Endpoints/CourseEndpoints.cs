using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Contracts;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class CourseEndpoints
{
    private const string CoursesCacheKey = "courses_all_v1";

    public static void MapCourseEndpoints(this WebApplication app)
    {
        // GET all (cached + DTO)
        app.MapGet("/courses", async (AppDbContext db, IMemoryCache cache) =>
        {
            if (cache.TryGetValue(CoursesCacheKey, out List<CourseDto>? cached) && cached is not null)
                return Results.Ok(cached);

            var courses = await db.Courses
                .AsNoTracking()
                .Select(c => new CourseDto(c.Id, c.Name))
                .ToListAsync();

            cache.Set(CoursesCacheKey, courses, TimeSpan.FromMinutes(5));

            return Results.Ok(courses);
        });

        // GET by id
        app.MapGet("/courses/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var course = await db.Courses
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CourseDto(c.Id, c.Name))
                .FirstOrDefaultAsync();

            return course is null
                ? Results.NotFound()
                : Results.Ok(course);
        });

        // POST
        app.MapPost("/courses", async (CreateCourseDto dto, AppDbContext db, IMemoryCache cache) =>
        {
            var course = new Course
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim()
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();

            cache.Remove(CoursesCacheKey); // invalidate cache

            return Results.Created($"/courses/{course.Id}", new CourseDto(course.Id, course.Name));
        });

        // PUT
        app.MapPut("/courses/{id:guid}", async (Guid id, UpdateCourseDto dto, AppDbContext db, IMemoryCache cache) =>
        {
            var course = await db.Courses.FindAsync(id);
            if (course is null) return Results.NotFound();

            course.Name = dto.Name.Trim();
            await db.SaveChangesAsync();

            cache.Remove(CoursesCacheKey);

            return Results.NoContent();
        });

        // DELETE
        app.MapDelete("/courses/{id:guid}", async (Guid id, AppDbContext db, IMemoryCache cache) =>
        {
            var course = await db.Courses.FindAsync(id);
            if (course is null) return Results.NotFound();

            db.Courses.Remove(course);
            await db.SaveChangesAsync();

            cache.Remove(CoursesCacheKey);

            return Results.NoContent();
        });
    }
}