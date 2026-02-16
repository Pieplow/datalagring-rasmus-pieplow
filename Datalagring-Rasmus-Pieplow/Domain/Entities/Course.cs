using System;
using System.Collections.Generic;
using System.Text;

namespace Datalagring_Rasmus_Pieplow.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<CourseInstance> CourseInstances { get; set; } = new List<CourseInstance>();

    }
}
