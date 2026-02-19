using System;
using System.Linq;
using System.Threading.Tasks;
using Datalagring_Rasmus_Pieplow.Application.Services;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Datalagring_Rasmus_Pieplow.Tests;

public class RegistrationServiceTests
{
    [Fact]
    public async Task RegisterAsync_Should_Create_Registration()
    {
        // Arrange: skapa in-memory DbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
     .UseInMemoryDatabase(Guid.NewGuid().ToString())
     .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
     .Options;


        await using var db = new AppDbContext(options);

        // Lägg in testdata
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com"
        };

        var instance = new CourseInstance
        {
            Id = Guid.NewGuid(),
            Capacity = 10
        };

        db.Participants.Add(participant);
        db.CourseInstances.Add(instance);
        await db.SaveChangesAsync();

        var service = new RegistrationService(db);

        // Act: kör registrering
        var result = await service.RegisterAsync(instance.Id, participant.Id);

        // Assert: kontrollera att registration skapades
        Assert.Single(db.Registrations);
        var registration = db.Registrations.Single();
        Assert.Equal(instance.Id, registration.CourseInstanceId);
        Assert.Equal(participant.Id, registration.ParticipantId);
    }
}
