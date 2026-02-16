namespace Datalagring_Rasmus_Pieplow.API.Contract;

public record CreateCourseInstanceDto(
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    Guid InstructorId
);
