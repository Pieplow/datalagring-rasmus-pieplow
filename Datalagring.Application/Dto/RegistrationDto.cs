namespace Datalagring.Application.Dto;

public record RegistrationDto(
    Guid Id,
    Guid CourseInstanceId,
    Guid ParticipantId,
    string ParticipantName,
    string ParticipantEmail,
    string CourseName
);