using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;
using Datalagring.Infrastructure.Persistence;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring.Infrastructure.Repositories;

public class CourseInstanceRepository : ICourseInstanceRepository
{
    private readonly AppDbContext _context;

    public CourseInstanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CourseInstanceDto?> GetByIdAsync(Guid id)
    {
        return await _context.CourseInstances
            .AsNoTracking()
            .Include(ci => ci.Course)
            .Include(ci => ci.Instructor)
            .Select(ci => new CourseInstanceDto(
                ci.Id,
                ci.CourseId,
                ci.Course.Name,
                ci.InstructorId,
                $"{ci.Instructor.FirstName} {ci.Instructor.LastName}",
                ci.StartDate))
            .FirstOrDefaultAsync(ci => ci.Id == id);
    }

    public async Task<IEnumerable<CourseInstanceDto>> GetAllAsync()
    {
        return await _context.CourseInstances
            .AsNoTracking()
            .Include(ci => ci.Course)
            .Select(ci => new CourseInstanceDto(
                ci.Id,
                ci.CourseId,
                ci.Course.Name,
                ci.InstructorId,
                "",
                ci.StartDate))
            .ToListAsync();
    }

    public async Task AddAsync(CourseInstance instance)
    {
        await _context.CourseInstances.AddAsync(instance);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}