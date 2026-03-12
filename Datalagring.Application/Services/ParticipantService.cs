using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;

namespace Datalagring.Application.Services;

public class ParticipantService
{
    private readonly IParticipantRepository _repo;

    // Pratar bara med Interfacet nu - ingen databas här!
    public ParticipantService(IParticipantRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ParticipantDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<ParticipantDto?> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task CreateAsync(CreateParticipantDto dto)
    {
        // Validering
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new Exception("E-post krävs.");

        // Affärsregel via Repot
        if (await _repo.EmailExistsAsync(dto.Email))
            throw new Exception("E-postadressen är redan upptagen.");

        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLower()
        };

        await _repo.AddAsync(participant);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, UpdateParticipantDto dto)
    {
        // Här kan du hämta entiteten först för att se om den finns
        // Men vi håller det enkelt: vi ber repot uppdatera
        var participant = new Participant
        {
            Id = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        _repo.Update(participant);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repo.RemoveAsync(id);
        await _repo.SaveChangesAsync();
    }
}