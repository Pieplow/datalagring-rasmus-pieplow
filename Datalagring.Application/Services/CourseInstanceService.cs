using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;
namespace Datalagring.Application.Services;

public class CourseInstanceService
{
    private readonly ICourseInstanceRepository _repo;

    public CourseInstanceService(ICourseInstanceRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<CourseInstanceDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<CourseInstanceDto?> GetByIdAsync(Guid id)
    {
        var instance = await _repo.GetByIdAsync(id);
        if (instance == null) throw new Exception("Kurstillfället hittades inte.");

        return instance;
    }

    public async Task CreateAsync(Guid courseId, Guid instructorId, DateTime startDate)
    {
        var instance = new CourseInstance
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            InstructorId = instructorId,
            StartDate = startDate
        };

        await _repo.AddAsync(instance);
        await _repo.SaveChangesAsync();
    }
}