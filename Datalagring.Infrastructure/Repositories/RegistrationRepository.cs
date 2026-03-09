using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Datalagring.Infrastructure.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly AppDbContext _context;

    public RegistrationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CourseInstanceDto?> GetByIdAsync(Guid id)
    {
        // Vi hämtar och mappar direkt till en DTO för att vara effektiva
        return await _context.CourseInstances
            .AsNoTracking()
            .Where(ci => ci.Id == id)
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
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(Guid instanceId, Guid participantId)
    {
        return await _context.Registrations
            .AnyAsync(r => r.CourseInstanceId == instanceId && r.ParticipantId == participantId);
    }

    public async Task AddAsync(Registration registration)
    {
        await _context.Registrations.AddAsync(registration);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RegistrationDto>> GetDetailedListAsync(Guid instanceId)
    {
        // Här bor din råa SQL nu – helt dold för Application-lagret!
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
            WHERE r.CourseInstanceId = @instanceId";

        return await _context.Set<RegistrationDto>()
            .FromSqlRaw(sql, new SqlParameter("@instanceId", instanceId))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        // Vi letar upp den specifika registreringen i databasen
        var registration = await _context.Registrations.FindAsync(id);

        // Om den finns (inte är null), så tar vi bort den
        if (registration != null)
        {
            _context.Registrations.Remove(registration);
        }
    }
}