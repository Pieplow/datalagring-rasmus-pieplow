using Datalagring_Rasmus_Pieplow.API.Contract;
using Datalagring_Rasmus_Pieplow.Domain.Entities;
using Datalagring_Rasmus_Pieplow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;




namespace Datalagring_Rasmus_Pieplow.API.Endpoints
{
    public static class TaskEndpoints
    {

            public static void MapTaskEndpoints(this WebApplication app)
        {
            //GET
            app.MapGet("/projects", async {})


    }
}
