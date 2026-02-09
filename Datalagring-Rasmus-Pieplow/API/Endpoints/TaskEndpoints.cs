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
            // GET: Tasks for a specific project
            app.MapGet("/projects/{projectId:guid}/tasks",
                async (Guid projectId, AppDbContext db) =>
                {
                    return await db.Tasks
                        .Where(t => t.ProjectId == projectId)
                        .ToListAsync();
                });

            // POST: Create task for a project
            app.MapPost("/tasks", async (CreateTaskDto dto, AppDbContext db) =>
            {
                var projectExists = await db.Projects.AnyAsync(p => p.Id == dto.ProjectId);

                if (!projectExists)
                    return Results.BadRequest("Project does not exist");

                var task = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title,
                    IsCompleted = false,
                    ProjectId = dto.ProjectId
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