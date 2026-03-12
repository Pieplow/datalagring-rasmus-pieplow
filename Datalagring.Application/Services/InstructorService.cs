using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;

namespace Datalagring.Application.Services;

public class InstructorService
{
    private readonly IInstructorRepository _repo;

    public InstructorService(IInstructorRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<InstructorDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<InstructorDto?> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task CreateAsync(InstructorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new Exception("E-post krävs.");

        var instructor = new Instructor
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLower()
        };

        await _repo.AddAsync(instructor);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        // Här kan vi lägga till logik om läraren får tas bort eller ej
        await _repo.RemoveAsync(id);
        await _repo.SaveChangesAsync();
    }
}