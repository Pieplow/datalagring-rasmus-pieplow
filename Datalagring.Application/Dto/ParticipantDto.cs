namespace Datalagring.Application.Dto;

public record ParticipantDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email
);
