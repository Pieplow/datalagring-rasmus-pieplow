
using Datalagring.Application.Abstractions;
using Datalagring.Application.Dto;
using Datalagring.Domain.Entities;
using Datalagring.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context) => _context = context;

    public async Task<CourseDto?> GetByIdAsync(Guid id)
    {
        return await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseDto(c.Id, c.Name, c.Description))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        return await _context.Courses
            .AsNoTracking()
            .Select(c => new CourseDto(c.Id, c.Name, c.Description))
            .ToListAsync();
    }

    public async Task AddAsync(Course course) => await _context.Courses.AddAsync(course);

    public void Update(Course course) => _context.Courses.Update(course);

    public async Task RemoveAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null) _context.Courses.Remove(course);
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}