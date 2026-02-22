using Contracts;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;


namespace Datalagring_Rasmus_Pieplow.Application.Services;

public class CourseService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    private const string CoursesCacheKey = "courses_all_v1";

    public CourseService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IResult> GetAllAsync()
    {
        if (_cache.TryGetValue(CoursesCacheKey, out List<CourseDto>? cached) && cached is not null)
            return Results.Ok(cached);

        var courses = await _db.Courses
            .AsNoTracking()
            .Select(c => new CourseDto(c.Id, c.Name))
            .ToListAsync();

        _cache.Set(CoursesCacheKey, courses, TimeSpan.FromMinutes(5));

        return Results.Ok(courses);
    }

    public async Task<IResult> GetByIdAsync(Guid id)
    {
        var course = await _db.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseDto(c.Id, c.Name))
            .FirstOrDefaultAsync();

        return course is null
            ? Results.NotFound()
            : Results.Ok(course);
    }

    public async Task<IResult> CreateAsync(CreateCourseDto dto)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim()
        };

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();

        _cache.Remove(CoursesCacheKey);

        return Results.Created($"/courses/{course.Id}",
            new CourseDto(course.Id, course.Name));
    }

    public async Task<IResult> UpdateAsync(Guid id, UpdateCourseDto dto)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course is null) return Results.NotFound();

        course.Name = dto.Name.Trim();
        await _db.SaveChangesAsync();

        _cache.Remove(CoursesCacheKey);

        return Results.NoContent();
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course is null) return Results.NotFound();

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();

        _cache.Remove(CoursesCacheKey);

        return Results.NoContent();
    }
}