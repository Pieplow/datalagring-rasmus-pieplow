using Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;



namespace Datalagring_Rasmus_Pieplow.Application.Services;

public class RegistrationService
{
    private readonly AppDbContext _db;

    public RegistrationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> RegisterAsync(Guid instanceId, Guid participantId)
    {
        // Start transaction 
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var instance = await _db.CourseInstances
                .AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.Id == instanceId);

            if (instance is null)
                return Results.NotFound("Course instance not found");

            var participantExists =
                await _db.Participants.AnyAsync(p => p.Id == participantId);

            if (!participantExists)
                return Results.BadRequest("Participant does not exist");

            // Capacity check
            var currentCount = await _db.Registrations
                .CountAsync(r => r.CourseInstanceId == instanceId);

            if (currentCount >= instance.Capacity)
                return Results.BadRequest("Course is full");

            // Duplicate check
            var alreadyRegistered = await _db.Registrations
                .AnyAsync(r =>
                    r.CourseInstanceId == instanceId &&
                    r.ParticipantId == participantId);

            if (alreadyRegistered)
                return Results.BadRequest("Participant already registered");

            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                CourseInstanceId = instanceId,
                ParticipantId = participantId,
                RegisteredAt = DateTime.UtcNow
            };

            _db.Registrations.Add(registration);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return Results.Created(
                $"/courseinstances/{instanceId}/registrations/{registration.Id}",
                registration);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IResult> UnregisterAsync(Guid registrationId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var registration = await _db.Registrations
                .FirstOrDefaultAsync(r => r.Id == registrationId);

            if (registration is null)
                return Results.NotFound();

            _db.Registrations.Remove(registration);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return Results.NoContent();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IResult> GetByInstanceIdRawAsync(Guid instanceId)
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

        var regs = await _db.Set<RegistrationDto>()
            .FromSqlRaw(sql, new SqlParameter("@instanceId", instanceId))
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(regs);
    }

}
