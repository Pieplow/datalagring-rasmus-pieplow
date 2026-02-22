using Contracts;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Datalagring_Rasmus_Pieplow.Application.Services;

public class InstructorService
{
    private readonly AppDbContext _db;

    public InstructorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> GetAllAsync()
    {
        var instructors = await _db.Instructors
            .AsNoTracking()
            .ToListAsync();

        return Results.Ok(instructors);
    }

    public async Task<IResult> GetByIdAsync(Guid id)
    {
        var instructor = await _db.Instructors
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return instructor is null
            ? Results.NotFound()
            : Results.Ok(instructor);
    }

    public async Task<IResult> CreateAsync(CreateInstructorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) ||
            string.IsNullOrWhiteSpace(dto.LastName))
            return Results.BadRequest("First name and last name are required.");

        var instructor = new Instructor
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email?.Trim()
        };

        _db.Instructors.Add(instructor);
        await _db.SaveChangesAsync();

        return Results.Created($"/instructors/{instructor.Id}", instructor);
    }

    public async Task<IResult> UpdateAsync(Guid id, UpdateInstructorDto dto)
    {
        var instructor = await _db.Instructors.FindAsync(id);
        if (instructor is null)
            return Results.NotFound();

        if (string.IsNullOrWhiteSpace(dto.FirstName) ||
            string.IsNullOrWhiteSpace(dto.LastName))
            return Results.BadRequest("First name and last name are required.");

        instructor.FirstName = dto.FirstName.Trim();
        instructor.LastName = dto.LastName.Trim();
        instructor.Email = dto.Email?.Trim();

        await _db.SaveChangesAsync();

        return Results.NoContent();
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var instructor = await _db.Instructors.FindAsync(id);
        if (instructor is null)
            return Results.NotFound();

        var isAssigned = await _db.CourseInstances
            .AnyAsync(ci => ci.InstructorId == id);

        if (isAssigned)
            return Results.Conflict("Instructor is assigned to one or more course instances.");

        _db.Instructors.Remove(instructor);
        await _db.SaveChangesAsync();

        return Results.NoContent();
    }
}