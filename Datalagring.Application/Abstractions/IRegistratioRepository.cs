
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;

namespace Datalagring.Application.Abstractions;

public interface IRegistrationRepository
{
    Task<CourseInstanceDto?> GetByIdAsync(Guid id);

    
    Task<bool> ExistsAsync(Guid instanceId, Guid participantId);

    
    Task AddAsync(Registration registration);

    Task SaveChangesAsync();

    
    Task<IEnumerable<RegistrationDto>> GetDetailedListAsync(Guid instanceId);
    Task RemoveAsync  (Guid instanceId);
}
