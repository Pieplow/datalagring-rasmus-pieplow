namespace Contracts;

public record CourseInstanceDto(
    Guid Id,
    Guid CourseId,
    string CourseName,
    Guid InstructorId,
    string InstructorName,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity
);