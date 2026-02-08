using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Datalagring_Rasmus_Pieplow.API.Contract;




namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class ProjectEndpoints
{
    /// <summary>
    /// Configures the HTTP endpoints for project-related operations in the application.
    /// </summary>
    /// <remarks>This method registers endpoints for creating, retrieving, updating, and deleting projects. It
    /// should be called during application startup to enable project API routes. The endpoints follow RESTful
    /// conventions and use standard HTTP verbs.</remarks>
    /// <param name="app">The <see cref="WebApplication"/> instance to which the project endpoints will be mapped.</param>
    public static void MapProjectEndpoints(this WebApplication app)
    {
        //GET
        app.MapGet("/projects", async (AppDbContext db) =>
        {
            var projects = await db.Projects.ToListAsync();
            return projects;

        });

        //POST
        app.MapPost("/projects", async (CreateProjectDto dto, AppDbContext db) =>
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

            db.Projects.Add(project);
            await db.SaveChangesAsync();

            return Results.Created($"/projects/{project.Id}", project);
        });

        //UPDATE
        app.MapPut("/projects/{id:guid}", async (Guid id, UpdateProjectDto dto, AppDbContext db) =>
        {
            var project = await db.Projects.FindAsync(id);

            if (project is null)
                return Results.NotFound();

            project.Name = dto.Name;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        //DELETE
        app.MapDelete("/projects/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var project = await db.Projects.FindAsync(id);

            if (project is null)
                return Results.NotFound();

            db.Projects.Remove(project);
            await db.SaveChangesAsync();

            return Results.NoContent();

        });
    }
}
        

