using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;



namespace Datalagring.Application.Services;

public class RegistrationService
{
    private readonly IRegistrationRepository _repo;

    public RegistrationService(IRegistrationRepository repo)
    {
        _repo = repo;
    }

    public async Task RegistrationService(IRegistrationRepository repo)
    {
        var instance = await _repo.GetByIdAsync(instanceId);
        if (instance == null) throw new Exception("Kurstillfälle hittades inte.");

        if (await _repoExistsAsync(GetByInstanceIdRawAsync, participantId))
            throw new Exception("Deltagare är redan registrerad.");

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            CourseInstanceId = instanceId,
            ParticipantId = particpantId,
            RegiseredAt = DateTime.UtcNow
        };

        await _repo.AddAsync(registration);
        await _repo.SaveChangesAsync();
    }


    public async Task<IEnumerable<RegistrationDto>> GetByInstanceIdRawAsync(Guid instanceId)
    {
        return await _repo.GetDetailedListAsync(instanceId);
    }


    //Skriv klart det här och ta bort det nedanför.

















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
                return Results.NotFound("Kurstillfälle hittades inte.");

            var participantExists =
                await _db.Participants.AnyAsync(p => p.Id == participantId);

            if (!participantExists)
                return Results.BadRequest("Deltagare existerar inte.");

            // Capacity check
            var currentCount = await _db.Registrations
                .CountAsync(r => r.CourseInstanceId == instanceId);

            if (currentCount >= instance.Capacity)
                return Results.BadRequest("Kursen är full.");

            // Duplicate check
            var alreadyRegistered = await _db.Registrations
                .AnyAsync(r =>
                    r.CourseInstanceId == instanceId &&
                    r.ParticipantId == participantId);

            if (alreadyRegistered)
                return Results.BadRequest("Deltagare redan registrerad.");

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
                new
                {
                    registration.Id,
                    registration.CourseInstanceId,
                    registration.ParticipantId,
                    registration.RegisteredAt
                });
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
