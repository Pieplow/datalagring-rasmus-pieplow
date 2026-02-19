namespace Contracts;

public record CreateCourseInstanceDto(
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    Guid InstructorId
);
