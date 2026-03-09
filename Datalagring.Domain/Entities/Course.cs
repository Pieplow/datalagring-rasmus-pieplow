using System;
using System.Collections.Generic;
using System.Text;

namespace Datalagring.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<CourseInstance> CourseInstances { get; set; } = [];

    }
}
