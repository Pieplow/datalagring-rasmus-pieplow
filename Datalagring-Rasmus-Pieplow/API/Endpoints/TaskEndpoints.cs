using Datalagring_Rasmus_Pieplow.API.Contract;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            // GET: Tasks for a specific Course
            app.MapGet("/Courses/{CourseId:guid}/tasks",
                async (Guid CourseId, AppDbContext db) =>
                {
                    return await db.Tasks
                        .Where(t => t.CourseId == CourseId)
                        .ToListAsync();
                });

            // POST: Create task for a Course
            app.MapPost("/tasks", async (CreateTaskDto dto, AppDbContext db) =>
            {
                var CourseExists = await db.Courses.AnyAsync(p => p.Id == dto.CourseId);

                if (!CourseExists)
                    return Results.BadRequest("Course does not exist");

                var task = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title,
                    IsCompleted = false,
                    CourseId = dto.CourseId
                };

                db.Tasks.Add(task);
                await db.SaveChangesAsync();

                return Results.Created($"/tasks/{task.Id}", task);
            });


            // PUT: Update task
            app.MapPut("/tasks/{id:guid}", async (Guid id, UpdateTaskDto dto, AppDbContext db) =>
            {
                var task = await db.Tasks.FindAsync(id);

                if (task is null)
                    return Results.NotFound();

                task.Title = dto.Title;
                task.IsCompleted = dto.IsCompleted;

                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // DELETE: Remove task
            app.MapDelete("/tasks/{id:guid}", async (Guid id, AppDbContext db) =>
            {
                var task = await db.Tasks.FindAsync(id);

                if (task is null)
                    return Results.NotFound();

                db.Tasks.Remove(task);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}