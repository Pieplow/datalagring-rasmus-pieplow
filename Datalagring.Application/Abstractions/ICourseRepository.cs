using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;

namespace Datalagring.Application.Abstractions;

public interface ICourseRepository
{
    Task<CourseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CourseDto>> GetAllAsync();
    Task AddAsync(Course course);
    void Update(Course course);
    Task RemoveAsync(Guid id);
    Task SaveChangesAsync();
}