using Datalagring_Rasmus_Pieplow.API.Endpoints;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        "Server=(localdb)\\MSSQLLocalDB;Database=DatalagringDb;Trusted_Connection=True;");
});

var app = builder.Build();

app.MapGet("/", () => "API is running");
app.MapProjectEndpoints();

app.Run();



