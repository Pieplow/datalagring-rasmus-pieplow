using Datalagring.Domain.Entities;

namespace Datalagring.Application.Abstractions;

public interface ICourseInstanceRepository
{
    Task<IEnumerable<CourseInstance>> GetAllAsync();
    Task<CourseInstance?> GetByIdAsync(Guid id);
    Task<bool> HasInstructorOverlapAsync(Guid instructorId, DateTime start, DateTime end, Guid? excludeId = null);
    Task AddAsync(CourseInstance instance);
    Task SaveChangesAsync();
}