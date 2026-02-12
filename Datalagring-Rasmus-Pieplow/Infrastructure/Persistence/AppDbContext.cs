using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<CourseInstance> CourseInstances { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Registration> Registrations { get; set; }


    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
