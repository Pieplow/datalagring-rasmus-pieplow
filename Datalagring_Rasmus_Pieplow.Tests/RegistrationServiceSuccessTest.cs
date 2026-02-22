using System;
using System.Linq;
using System.Threading.Tasks;
using Datalagring_Rasmus_Pieplow.Application.Services;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Datalagring_Rasmus_Pieplow.Tests;

public class RegistrationService_SuccessTests
{
    private static DbContextOptions<AppDbContext> NewOptions()
        => new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

    [Fact]
    public async Task RegisterAsync_When_Valid_Should_Create_Registration_And_Return_Created()
    {
        await using var db = new AppDbContext(NewOptions());

        // Seed course
        var courseId = Guid.NewGuid();
        db.Courses.Add(new Course
        {
            Id = courseId,
            Name = "Valid Course"
        });

        // Seed course instance
        var instanceId = Guid.NewGuid();
        db.CourseInstances.Add(new CourseInstance
        {
            Id = instanceId,
            Capacity = 10,
            CourseId = courseId
        });

        // Seed participant
        var participantId = Guid.NewGuid();
        db.Participants.Add(new Participant
        {
            Id = participantId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com"
        });

        await db.SaveChangesAsync();

        var service = new RegistrationService(db);

        // Act
        var result = await service.RegisterAsync(instanceId, participantId);

        // Assert return type
        var created = Assert.IsType<Created<Registration>>(result);

        // Assert DB state
        var registrationInDb = db.Registrations.Single();
        Assert.Equal(instanceId, registrationInDb.CourseInstanceId);
        Assert.Equal(participantId, registrationInDb.ParticipantId);
    }
}