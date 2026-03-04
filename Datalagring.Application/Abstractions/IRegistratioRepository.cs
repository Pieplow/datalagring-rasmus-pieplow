
using Datalagring.application.Dto;
using Datalagring.Domain.Entities;

namespace Datalagring.Application.Abstractions;

public interface IRegistrationRepository
{
    Task<CourseInstanceDto?> GetByIdAsync(Guid id);

    // Notera kommatecknet här:
    Task<bool> ExistsAsync(Guid instanceId, Guid participantId);

    // Vi använder entiteten för att spara (DDD-principen)
    Task AddAsync(Registration registration);

    Task SaveChangesAsync();

    // Vi använder DTO för att visa data (Presentation)
    Task<IEnumerable<RegistrationDto>> GetDetailedListAsync(Guid instanceId);
}
