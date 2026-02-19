namespace Contracts;

public record CourseInstanceDto(
    Guid Id,
    Guid CourseId,
    int Capacity
);
