using Datalagring.Presentation.Endpoints;
using Datalagring.Application.Services;
using Datalagring.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        "Server=(localdb)\\MSSQLLocalDB;Database=DatalagringDb;Trusted_Connection=True;");
});
builder.Services.AddMemoryCache();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<CourseInstanceService>();
builder.Services.AddScoped<InstructorService>();
builder.Services.AddScoped<ParticipantService>();
builder.Services.AddScoped<RegistrationService>();




var app = builder.Build();



app.MapGet("/", () => "API is running");
app.MapCourseInstanceEndpoints();
app.MapParticipantEndpoints();
app.MapInstructorEndpoints();
app.MapCourseEndpoints();
app.MapRegistrationEndpoints();



app.Run();


