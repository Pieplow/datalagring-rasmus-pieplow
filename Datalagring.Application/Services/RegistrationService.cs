using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;

namespace Datalagring.Application.Services;

public class RegistrationService
{
    private readonly IRegistrationRepository _repo;

    public RegistrationService(IRegistrationRepository repo)
    {
        _repo = repo;
    }

    public async Task RegisterParticipantAsync(Guid instanceId, Guid participantId)
    {
        var instance = await _repo.GetByIdAsync(instanceId);
        if (instance is null) throw new Exception("Kurstillfälle hittades inte.");

        if (await _repo.ExistsAsync(instanceId, participantId))
            throw new Exception("Deltagare är redan registrerad.");

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            CourseInstanceId = instanceId,
            ParticipantId = participantId,
            RegisteredAt = DateTime.UtcNow
        };

        await _repo.AddAsync(registration);
        await _repo.SaveChangesAsync();
    }

    public async Task<IEnumerable<RegistrationDto>> GetByInstanceIdRawAsync(Guid instanceId)
    {
        return await _repo.GetDetailedListAsync(instanceId);
    }

    public async Task UnregisterAsync(Guid registrationId)
    {
        await _repo.RemoveAsync(registrationId);
        await _repo.SaveChangesAsync();
    }
}