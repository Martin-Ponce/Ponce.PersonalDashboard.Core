using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Helpers.UpcomingDate;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.UpcomingDate
{
    /// <summary>An event or appointment on a specific day, optionally at a set time.</summary>
    public class UpcomingDate : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public DateOnly Date { get; private set; }
        /// <summary>Null means all-day.</summary>
        public TimeOnly? Time { get; private set; }
        public string Title { get; private set; } = default!;
        public string? Description { get; private set; }
        public UpcomingDateStatus Status { get; private set; }

        private UpcomingDate() { } // Required by EF Core

        public UpcomingDate(Guid userId, DateOnly date, TimeOnly? time, string title, string? description, UpcomingDateStatus status)
        {
            Id = Guid.NewGuid();
            UserId = userId.ValidateUserId();
            SetDetails(date, time, title, description, status);
        }

        public void Update(DateOnly date, TimeOnly? time, string title, string? description, UpcomingDateStatus status) =>
            SetDetails(date, time, title, description, status);

        private void SetDetails(DateOnly date, TimeOnly? time, string title, string? description, UpcomingDateStatus status)
        {
            Date = date.ValidateUpcomingDate();
            Time = time;
            Title = title.ValidateUpcomingDateTitle();
            Description = description.ValidateUpcomingDateDescription();
            Status = status.ValidateStatus();
        }
    }
}
