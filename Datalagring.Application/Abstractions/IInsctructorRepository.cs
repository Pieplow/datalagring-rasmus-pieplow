using Datalagring.Domain.Entities;
using Datalagring.Application.Dto;

namespace Datalagring.Application.Abstractions
{
    public interface IInsctructorRepository
    {
        Task<InstructorDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<InstructorDto>> GetAllAsync();
        Task AddAsync(Instructor instructor);
        Task SaveChangesAsync();
    }
}
