using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public record RegistrationDto(
      Guid Id,
      Guid CourseInstanceId,
      Guid ParticipantId,
      string ParticipantName,
      string ParticipantEmail
  );
}
