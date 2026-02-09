namespace Datalagring_Rasmus_Pieplow.Domain.Entities
{
    public class CourseInstance
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public Guid InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

        public int Capacity { get; set; }
        public List<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
