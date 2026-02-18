namespace Datalagring_Rasmus_Pieplow.API.Contract;

public record CreateInstructorDto(
    string FirstName,
    string LastName,
    string Email
);
