using Datalagring.Application.Dto;

namespace Datalagring.Application.Abstractions;

public interface ICourseInstanceRepository
{
    Task<CourseInstanceDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CourseInstanceDto>> GetAllAsync();
    Task AddAsync(Datalagring.Domain.Entities.CourseInstance instance);
    Task SaveChangesAsync();
}