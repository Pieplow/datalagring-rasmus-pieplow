using Contracts;
using Datalagring_Rasmus_Pieplow.Application.Services;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.API.Endpoints;

public static class RegistrationEndpoints
{
    public static void MapRegistrationEndpoints(this WebApplication app)
    {
        // GET: alla registreringar för ett kurstillfälle (VG: returnera DTO, inte EF entity)
        app.MapGet("/courseinstances/{instanceId:guid}/registrations",
     async (Guid instanceId, AppDbContext db) =>
     {
         var sql = @"
            SELECT
                r.Id,
                r.CourseInstanceId,
                r.ParticipantId,
                (p.FirstName + ' ' + p.LastName) AS ParticipantName,
                p.Email AS ParticipantEmail,
                c.Name AS CourseName
            FROM Registrations r
            INNER JOIN Participants p ON p.Id = r.ParticipantId
            INNER JOIN CourseInstances ci ON ci.Id = r.CourseInstanceId
            INNER JOIN Courses c ON c.Id = ci.CourseId
            WHERE r.CourseInstanceId = @instanceId
            ";

         var regs = await db.Set<RegistrationDto>()
        .FromSqlRaw(sql, new SqlParameter("@instanceId", instanceId))
        .AsNoTracking()
        .ToListAsync();

         return Results.Ok(regs);
     });

        // POST: skapa registrering 
        app.MapPost("/courseinstances/{instanceId:guid}/registrations",
            async (Guid instanceId, CreateRegistrationDto dto, RegistrationService service) =>
            {
                return await service.RegisterAsync(instanceId, dto.ParticipantId);
            });

        app.MapDelete("/registrations/{id:guid}",
            async (Guid id, RegistrationService service) =>
            {
                return await service.UnregisterAsync(id);
            });
    }
}