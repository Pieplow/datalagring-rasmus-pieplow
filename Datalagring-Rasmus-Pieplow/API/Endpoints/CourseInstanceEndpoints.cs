using Contracts;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class CourseInstanceEndpoints
{
    public static void MapCourseInstanceEndpoints(this WebApplication app)
    {
        app.MapGet("/courseinstances",
   async (AppDbContext db) =>
   {
       var instances = await db.CourseInstances
           .AsNoTracking()
           .Include(ci => ci.Course)
           .Include(ci => ci.Instructor)
           .Select(ci => new CourseInstanceDto(
               ci.Id,
               ci.CourseId,
               ci.Course.Name,
               ci.InstructorId,
               ci.Instructor.FirstName + " " + ci.Instructor.LastName,
               ci.StartDate,
               ci.EndDate,
               ci.Capacity
           ))
           .ToListAsync();

       return Results.Ok(instances);
   });
        // GET (nested): alla kurstillfällen för en kurs
        app.MapGet("/courses/{courseId:guid}/instances",
            async (Guid courseId, AppDbContext db) =>
            {
                return await db.CourseInstances
                    .AsNoTracking()
                    .Where(ci => ci.CourseId == courseId)
                    .Include(ci => ci.Instructor) // visar relation
                    .ToListAsync();
            });

        // GET : ett specifikt kurstillfälle
        app.MapGet("/courseinstances/{id:guid}",
            async (Guid id, AppDbContext db) =>
            {
                var instance = await db.CourseInstances
                    .AsNoTracking()
                    .Include(ci => ci.Instructor)
                    .FirstOrDefaultAsync(ci => ci.Id == id);

                return instance is null ? Results.NotFound() : Results.Ok(instance);
            });

        // POST: skapa kurstillfälle under en kurs
        app.MapPost("/courses/{courseId:guid}/instances",
            async (Guid courseId, CreateCourseInstanceDto dto, AppDbContext db) =>
            {
                // FK-validering:
                // kursen måste finnas
                var courseExists = await db.Courses.AnyAsync(c => c.Id == courseId);
                if (!courseExists) return Results.NotFound("Course not found");

                // läraren måste finnas
                var instructorExists = await db.Instructors.AnyAsync(i => i.Id == dto.InstructorId);
                if (!instructorExists) return Results.BadRequest("Instructor not found");

                // enkel validering
                if (dto.EndDate <= dto.StartDate) return Results.BadRequest("EndDate must be after StartDate");
                if (dto.Capacity <= 0) return Results.BadRequest("Capacity must be > 0");

                var instance = new CourseInstance
                {
                    Id = Guid.NewGuid(),
                    CourseId = courseId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Capacity = dto.Capacity,
                    InstructorId = dto.InstructorId
                };

                db.CourseInstances.Add(instance);
                await db.SaveChangesAsync();

                return Results.Created($"/courseinstances/{instance.Id}", instance);
            });

        // PUT: uppdatera kurstillfälle
        app.MapPut("/courseinstances/{id:guid}",
            async (Guid id, UpdateCourseInstanceDto dto, AppDbContext db) =>
            {
                var instance = await db.CourseInstances.FindAsync(id);
                if (instance is null) return Results.NotFound();

                if (dto.EndDate <= dto.StartDate) return Results.BadRequest("EndDate must be after StartDate");
                if (dto.Capacity <= 0) return Results.BadRequest("Capacity must be > 0");

                // om man tillåter byte av instructor: kontrollera FK
                var instructorExists = await db.Instructors.AnyAsync(i => i.Id == dto.InstructorId);
                if (!instructorExists) return Results.BadRequest("Instructor not found");

                instance.StartDate = dto.StartDate;
                instance.EndDate = dto.EndDate;
                instance.Capacity = dto.Capacity;
                instance.InstructorId = dto.InstructorId;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

        app.MapDelete("/instructors/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var instructor = await db.Instructors.FindAsync(id);
            if (instructor is null) return Results.NotFound();

            // Kolla om instruktören har några kurstillfällen för att undvika potentiel krasch
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

