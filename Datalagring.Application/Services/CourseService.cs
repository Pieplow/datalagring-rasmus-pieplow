using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;

namespace Datalagring.Application.Services;

public class CourseService
{
    private readonly ICourseRepository _repo;

    public CourseService(ICourseRepository repo) => _repo = repo;

    public async Task<IEnumerable<CourseDto>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task CreateCourseAsync(CourseDto dto)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        await _repo.AddAsync(course);
        await _repo.SaveChangesAsync();
    }
}