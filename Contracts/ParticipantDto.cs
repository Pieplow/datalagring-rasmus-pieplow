namespace Contracts;

public record ParticipantDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email
);
