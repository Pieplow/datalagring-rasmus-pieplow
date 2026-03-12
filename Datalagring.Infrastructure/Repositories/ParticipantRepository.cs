using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;
using Datalagring.Infrastructure.Persistence;

namespace Datalagring.Infrastructure.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly AppDbContext _context;

        public ParticipantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ParticipantDto?> GetByIdAsync(Guid id)
        {
            return await _context.Participants
               .AsNoTracking()
               .Where(p => p.Id == id) // Ändrat till p.Id (vanlig standard)
               .Select(p => new ParticipantDto(p.Id, p.FirstName, p.LastName, p.Email))
               .FirstOrDefaultAsync();
        }

        // Glöm inte GetAllAsync om den finns i ditt Interface!
        public async Task<IEnumerable<ParticipantDto>> GetAllAsync()
        {
            return await _context.Participants
                .AsNoTracking()
                .Select(p => new ParticipantDto(p.Id, p.FirstName, p.LastName, p.Email))
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Participants.AnyAsync(p => p.Email == email);
        }

        public async Task AddAsync(Participant participant)
        {
            await _context.Participants.AddAsync(participant);
        }

        public void Update(Participant participant)
        {
            _context.Participants.Update(participant);
        }

        public async Task RemoveAsync(Guid id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant != null) _context.Participants.Remove(participant);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}