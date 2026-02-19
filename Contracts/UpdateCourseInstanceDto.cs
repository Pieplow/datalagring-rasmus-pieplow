namespace Datalagring_Rasmus_Pieplow.API.Contract;

public record UpdateCourseInstanceDto(
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    Guid InstructorId
);
