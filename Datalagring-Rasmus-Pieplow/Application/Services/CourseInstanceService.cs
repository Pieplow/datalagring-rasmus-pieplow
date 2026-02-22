using Contracts;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;

namespace Datalagring_Rasmus_Pieplow.Application.Services;

public class CourseInstanceService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    private const string InstancesCacheKey = "instances_all_v1";

    public CourseInstanceService(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IResult> GetAllAsync()
    {
        if (_cache.TryGetValue(InstancesCacheKey, out List<CourseInstanceDto>? cached) && cached is not null)
            return Results.Ok(cached);

        var instances = await _db.CourseInstances
            .AsNoTracking()
            .Include(ci => ci.Course)
            .Include(ci => ci.Instructor)
            .Select(ci => new CourseInstanceDto(
                ci.Id,
                ci.CourseId,
                ci.Course.Name,
                ci.InstructorId,
                ci.Instructor.FirstName + " " + ci.Instructor.LastName,
                ci.StartDate,
                ci.EndDate,
                ci.Capacity
            ))
            .ToListAsync();

        _cache.Set(InstancesCacheKey, instances, TimeSpan.FromMinutes(3));

        return Results.Ok(instances);
    }

    public async Task<IResult> GetByCourseIdAsync(Guid courseId)
    {
        var instances = await _db.CourseInstances
            .AsNoTracking()
            .Where(ci => ci.CourseId == courseId)
            .Include(ci => ci.Instructor)
            .Select(ci => new CourseInstanceDto(
                ci.Id,
                ci.CourseId,
                ci.Course.Name,
                ci.InstructorId,
                ci.Instructor.FirstName + " " + ci.Instructor.LastName,
                ci.StartDate,
                ci.EndDate,
                ci.Capacity
            ))
            .ToListAsync();

        return Results.Ok(instances);
    }

    public async Task<IResult> GetByIdAsync(Guid id)
    {
        var instance = await _db.CourseInstances
            .AsNoTracking()
            .Include(ci => ci.Course)
            .Include(ci => ci.Instructor)
            .FirstOrDefaultAsync(ci => ci.Id == id);

        return instance is null ? Results.NotFound() : Results.Ok(instance);
    }

    public async Task<IResult> CreateAsync(Guid courseId, CreateCourseInstanceDto dto)
    {
        var courseExists = await _db.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists) return Results.NotFound("Course not found");

        var instructorExists = await _db.Instructors.AnyAsync(i => i.Id == dto.InstructorId);
        if (!instructorExists) return Results.BadRequest("Instructor not found");

        if (dto.EndDate <= dto.StartDate)
            return Results.BadRequest("EndDate must be after StartDate");

        if (dto.Capacity <= 0)
            return Results.BadRequest("Capacity must be > 0");

        var instance = new CourseInstance
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Capacity = dto.Capacity,
            InstructorId = dto.InstructorId
        };

        _db.CourseInstances.Add(instance);
        await _db.SaveChangesAsync();

        _cache.Remove(InstancesCacheKey);

        return Results.Created($"/courseinstances/{instance.Id}", instance);
    }

    public async Task<IResult> UpdateAsync(Guid id, UpdateCourseInstanceDto dto)
    {
        var instance = await _db.CourseInstances.FindAsync(id);
        if (instance is null) return Results.NotFound();

        if (dto.EndDate <= dto.StartDate)
            return Results.BadRequest("EndDate must be after StartDate");

        if (dto.Capacity <= 0)
            return Results.BadRequest("Capacity must be > 0");

        var instructorExists = await _db.Instructors.AnyAsync(i => i.Id == dto.InstructorId);
        if (!instructorExists)
            return Results.BadRequest("Instructor not found");

        instance.StartDate = dto.StartDate;
        instance.EndDate = dto.EndDate;
        instance.Capacity = dto.Capacity;
        instance.InstructorId = dto.InstructorId;

        await _db.SaveChangesAsync();
        _cache.Remove(InstancesCacheKey);

        return Results.NoContent();
    }
}