using System;
using System.Threading.Tasks;
using Datalagring_Rasmus_Pieplow.Application.Services;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Datalagring_Rasmus_Pieplow.Tests;

public class RegistrationServiceUnitTests
{
    private static DbContextOptions<AppDbContext> NewOptions()
        => new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

    private static async Task<Guid> SeedCourseAsync(AppDbContext db)
    {
        var courseId = Guid.NewGuid();
        db.Courses.Add(new Course
        {
            Id = courseId,
            Name = "Test Course"
        });
        await db.SaveChangesAsync();
        return courseId;
    }

    [Fact]
    public async Task RegisterAsync_When_Participant_Missing_Should_Return_400()
    {
        await using var db = new AppDbContext(NewOptions());
        var courseId = await SeedCourseAsync(db);

        var instanceId = Guid.NewGuid();
        db.CourseInstances.Add(new CourseInstance
        {
            Id = instanceId,
            Capacity = 10,
            CourseId = courseId
        });
        await db.SaveChangesAsync();

        var service = new RegistrationService(db);

        var res = await service.RegisterAsync(instanceId, Guid.NewGuid());

        Assert.IsType<BadRequest<string>>(res);
    }

    [Fact]
    public async Task RegisterAsync_When_Course_Full_Should_Return_400()
    {
        await using var db = new AppDbContext(NewOptions());
        var courseId = await SeedCourseAsync(db);

        var instanceId = Guid.NewGuid();
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();

        db.CourseInstances.Add(new CourseInstance
        {
            Id = instanceId,
            Capacity = 1,
            CourseId = courseId
        });

        db.Participants.AddRange(
            new Participant { Id = p1, FirstName = "A", LastName = "A", Email = "a@test.com" },
            new Participant { Id = p2, FirstName = "B", LastName = "B", Email = "b@test.com" }
        );

        // Fyll kursen med en registrering
        db.Registrations.Add(new Registration
        {
            Id = Guid.NewGuid(),
            CourseInstanceId = instanceId,
            ParticipantId = p1,
            RegisteredAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var service = new RegistrationService(db);

        var res = await service.RegisterAsync(instanceId, p2);

        Assert.IsType<BadRequest<string>>(res);
    }

    [Fact]
    public async Task RegisterAsync_When_Duplicate_Should_Return_400()
    {
        await using var db = new AppDbContext(NewOptions());
        var courseId = await SeedCourseAsync(db);

        var instanceId = Guid.NewGuid();
        var participantId = Guid.NewGuid();

        db.CourseInstances.Add(new CourseInstance
        {
            Id = instanceId,
            Capacity = 10,
            CourseId = courseId
        });

        db.Participants.Add(new Participant
        {
            Id = participantId,
            FirstName = "T",
            LastName = "U",
            Email = "t@test.com"
        });

        // Lägg in en befintlig registrering
        db.Registrations.Add(new Registration
        {
            Id = Guid.NewGuid(),
            CourseInstanceId = instanceId,
            ParticipantId = participantId,
            RegisteredAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var service = new RegistrationService(db);

        var res = await service.RegisterAsync(instanceId, participantId);

        Assert.IsType<BadRequest<string>>(res);
    }
}