using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;
using Datalagring.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring.Infrastructure.Repositories;

public class InstructorRepository : IInstructorRepository
{
    private readonly AppDbContext _context;
    public InstructorRepository(AppDbContext context) => _context = context;

    public async Task<InstructorDto?> GetByIdAsync(Guid id)
    {
        return await _context.Instructors
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InstructorDto(i.Id, i.FirstName, i.LastName, i.Email))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<InstructorDto>> GetAllAsync()
    {
        return await _context.Instructors
            .AsNoTracking()
            .Select(i => new InstructorDto(i.Id, i.FirstName, i.LastName, i.Email))
            .ToListAsync();
    }

    public async Task AddAsync(Instructor instructor) => await _context.Instructors.AddAsync(instructor);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}