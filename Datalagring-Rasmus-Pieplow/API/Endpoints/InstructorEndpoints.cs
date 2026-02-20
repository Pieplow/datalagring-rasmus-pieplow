using Contracts;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class InstructorEndpoints
{
    public static void MapInstructorEndpoints(this WebApplication app)
    {
        // GET: Alla instruktörer
        app.MapGet("/instructors", async (AppDbContext db) =>
        {
            return await db.Instructors
                .AsNoTracking()
                .ToListAsync();
        });

        // GET: Specifik instruktör
        app.MapGet("/instructors/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var instructor = await db.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            return instructor is null ? Results.NotFound() : Results.Ok(instructor);
        });

        // POST: Skapa med validering
        app.MapPost("/instructors", async (CreateInstructorDto dto, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                return Results.BadRequest("First name and last name are required.");

            var instructor = new Instructor
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };

            db.Instructors.Add(instructor);
            await db.SaveChangesAsync();

            return Results.Created($"/instructors/{instructor.Id}", instructor);
        });

        // PUT: Uppdatera med validering
        app.MapPut("/instructors/{id:guid}", async (Guid id, UpdateInstructorDto dto, AppDbContext db) =>
        {
            var instructor = await db.Instructors.FindAsync(id);
            if (instructor is null) return Results.NotFound();

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                return Results.BadRequest("First name and last name are required.");

            instructor.FirstName = dto.FirstName;
            instructor.LastName = dto.LastName;
            instructor.Email = dto.Email;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE: Med relationskontroll
        app.MapDelete("/instructors/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var instructor = await db.Instructors.FindAsync(id);
            if (instructor is null) return Results.NotFound();

            var isAssigned = await db.CourseInstances.AnyAsync(ci => ci.InstructorId == id);

            if (isAssigned)
            {
                return Results.Conflict("Kan inte ta bort instruktören eftersom hen är kopplad till ett eller flera kurstillfällen.");
            }

            db.Instructors.Remove(instructor);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}