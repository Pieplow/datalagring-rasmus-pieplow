namespace Datalagring.Application.Dto;

public record CreateCourseInstanceDto(
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    Guid InstructorId
);
