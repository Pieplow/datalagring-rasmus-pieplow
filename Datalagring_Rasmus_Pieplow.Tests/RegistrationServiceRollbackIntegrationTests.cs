using System;
using System.Threading;
using System.Threading.Tasks;
using Datalagring_Rasmus_Pieplow.Application.Services;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace Datalagring_Rasmus_Pieplow.Tests;

public class RegistrationServiceRollbackIntegrationTests
{
    private sealed class ThrowOnSaveInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Forced failure");
        }
    }

    [Fact]
    public async Task RegisterAsync_When_SaveFails_Should_Rollback()
    {
        await using var db = new SqlServerTestDb();
        await db.ResetAsync();

        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();
        var participantId = Guid.NewGuid();

        // Seed normal data
        await using (var seedCtx = db.CreateContext())
        {
            seedCtx.Courses.Add(new Course { Id = courseId, Name = "Rollback Course" });

            seedCtx.Instructors.Add(new Instructor
            {
                Id = instructorId,
                FirstName = "Test",
                LastName = "Instructor",
                Email = "inst@test.com"
            });

            seedCtx.CourseInstances.Add(new CourseInstance
            {
                Id = instanceId,
                Capacity = 10,
                CourseId = courseId,
                InstructorId = instructorId
            });

            seedCtx.Participants.Add(new Participant
            {
                Id = participantId,
                FirstName = "Rollback",
                LastName = "User",
                Email = "rollback@test.com"
            });

            await seedCtx.SaveChangesAsync();
        }

        // Create context WITH interceptor
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(db.ConnectionString)
            .AddInterceptors(new ThrowOnSaveInterceptor())
            .Options;

        await using (var ctx = new AppDbContext(options))
        {
            var service = new RegistrationService(ctx);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RegisterAsync(instanceId, participantId));
        }

        // Verify rollback (no registrations saved)
        await using (var verifyCtx = db.CreateContext())
        {
            var count = await verifyCtx.Registrations.CountAsync();
            Assert.Equal(0, count);
        }
    }
}