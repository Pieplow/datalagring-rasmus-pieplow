using System;
using System.Threading.Tasks;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Datalagring_Rasmus_Pieplow.Tests;

public sealed class SqlServerTestDb : IAsyncDisposable
{
    private readonly string _dbName = $"DatalagringDb_Test_{Guid.NewGuid():N}";
    private readonly string _cs;

    public SqlServerTestDb()
    {
        _cs = $@"Server=(localdb)\MSSQLLocalDB;Database={_dbName};Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";
    }

    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_cs)
            .Options;

        return new AppDbContext(options);
    }

    public async Task ResetAsync()
    {
        await using var ctx = CreateContext();
        await ctx.Database.EnsureDeletedAsync();
        await ctx.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await using var ctx = CreateContext();
        await ctx.Database.EnsureDeletedAsync();
    }
}