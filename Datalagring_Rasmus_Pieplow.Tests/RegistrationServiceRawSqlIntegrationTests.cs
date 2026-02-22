using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
using Datalagring_Rasmus_Pieplow.Application.Services;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace Datalagring_Rasmus_Pieplow.Tests;

public class RegistrationServiceRawSqlIntegrationTests
{
    [Fact]
    public async Task GetByInstanceIdRawAsync_Should_Return_Dto_With_Name_Email_And_Course()
    {
        await using var db = new SqlServerTestDb();
        await db.ResetAsync();

        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var instanceId = Guid.NewGuid();
        var participantId = Guid.NewGuid();

        // Seed
        await using (var ctx = db.CreateContext())
        {
            ctx.Courses.Add(new Course
            {
                Id = courseId,
                Name = "SQL Course"
            });

            ctx.Instructors.Add(new Instructor
            {
                Id = instructorId,
                FirstName = "Test",
                LastName = "Instructor",
                Email = "instructor@test.com"
            });

            ctx.CourseInstances.Add(new CourseInstance
            {
                Id = instanceId,
                Capacity = 10,
                CourseId = courseId,
                InstructorId = instructorId
            });

            ctx.Participants.Add(new Participant
            {
                Id = participantId,
                FirstName = "Ada",
                LastName = "Lovelace",
                Email = "ada@test.com"
            });

            ctx.Registrations.Add(new Registration
            {
                Id = Guid.NewGuid(),
                CourseInstanceId = instanceId,
                ParticipantId = participantId,
                RegisteredAt = DateTime.UtcNow
            });

            await ctx.SaveChangesAsync();
        }

        // Act + Assert
        await using (var ctx = db.CreateContext())
        {
            var service = new RegistrationService(ctx);
            var res = await service.GetByInstanceIdRawAsync(instanceId);

            var ok = Assert.IsType<Ok<List<RegistrationDto>>>(res);
            Assert.Single(ok.Value);

            var dto = ok.Value[0];
            Assert.Equal(instanceId, dto.CourseInstanceId);
            Assert.Equal(participantId, dto.ParticipantId);
            Assert.Equal("Ada Lovelace", dto.ParticipantName);
            Assert.Equal("ada@test.com", dto.ParticipantEmail);
            Assert.Equal("SQL Course", dto.CourseName);
        }
    }
}