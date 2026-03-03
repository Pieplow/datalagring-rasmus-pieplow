namespace Datalagring_Rasmus_Pieplow.Domain.Entities
{
    public class Registration
    {
      
            public Guid Id { get; set; }

            public Guid ParticipantId { get; set; }
            public Participant Participant { get; set; } = null!;

            public Guid CourseInstanceId { get; set; }
            public CourseInstance CourseInstance { get; set; } = null!;

            public DateTime RegisteredAt { get; set; }
   
    }
}
