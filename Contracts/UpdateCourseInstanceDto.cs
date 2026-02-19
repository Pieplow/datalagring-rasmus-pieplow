namespace Contracts;

public record UpdateCourseInstanceDto(
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    Guid InstructorId
);
