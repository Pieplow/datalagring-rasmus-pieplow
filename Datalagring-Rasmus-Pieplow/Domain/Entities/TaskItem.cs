using System;
using System.Collections.Generic;
using System.Text;

namespace Datalagring_Rasmus_Pieplow.Domain.Entities
{
    public class TaskItem
    {
            public Guid Id { get; set; }

            public string Title { get; set; }

            public bool IsCompleted { get; set; }

            public Guid ProjectId { get; set; }

            public Project? Project { get; set; }




    }
}
