namespace Datalagring_Rasmus_Pieplow.Domain.Entities
{
    public class Participant
    {
        
            public Guid Id { get; set; }

            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;

            public string Email { get; set; } = null!;

            public List<Registration> Registrations { get; set; } = new();
     
    }
}
