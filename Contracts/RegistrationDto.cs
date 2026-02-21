namespace Contracts;

public record RegistrationDto(
    Guid Id,
    Guid CourseInstanceId,
    Guid ParticipantId,
    string ParticipantName,
    string ParticipantEmail,
    string CourseName
);