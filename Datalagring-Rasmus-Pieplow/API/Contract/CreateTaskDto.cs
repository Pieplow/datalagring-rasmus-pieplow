namespace Datalagring_Rasmus_Pieplow.API.Contract;

    public record CreateTaskDto(
        string Title, 
        Guid ProjectId
        );
   
