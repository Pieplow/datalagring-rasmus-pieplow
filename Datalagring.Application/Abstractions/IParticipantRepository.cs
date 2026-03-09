using Datalagring.Domain.Entities;
using Datalagring.Application.Dto;


namespace Datalagring.Application.Abstractions
{
    public interface IParticipantRepository
    {
        Task<ParticipantDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<ParticipantDto>> GetAllAsync();

        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(Participant participant);

        void Update(Participant participant);
        Task RemoveAsync(Guid id);

        Task SaveChangesAsync();
    }
}
