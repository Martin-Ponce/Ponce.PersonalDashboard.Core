using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Goals;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Goals
{
    /// <summary>A goal you do every day, like "gym for an hour". Can have a weekly goal on top that fills up from its check-ins.</summary>
    public class DailyGoal : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = default!;
        /// <summary>What one check-in is worth in Unit, e.g. 30 minutes.</summary>
        public decimal Amount { get; private set; }
        public GoalUnit Unit { get; private set; }
        /// <summary>Just a reminder: any day can be checked, but a planned day left unchecked counts as missed.</summary>
        public DayOfWeek[] PlannedDays { get; private set; } = [];
        public DateTime CreatedAtUtc { get; private set; }
        public WeeklyGoal? WeeklyGoal { get; private set; }

        private readonly List<GoalCheckIn> _checkIns = [];
        /// <summary>Only for queries. Check-ins go through the repository and aren't loaded with the goal.</summary>
        public IReadOnlyCollection<GoalCheckIn> CheckIns => _checkIns;

        private DailyGoal() { } // Required by EF Core

        public DailyGoal(Guid userId, string title, decimal amount, GoalUnit unit, IEnumerable<DayOfWeek> plannedDays)
        {
            Id = Guid.NewGuid();
            UserId = userId.ValidateUserId();
            CreatedAtUtc = DateTime.UtcNow;
            Update(title, amount, unit, plannedDays);
        }

        public void Update(string title, decimal amount, GoalUnit unit, IEnumerable<DayOfWeek> plannedDays)
        {
            Title = title.ValidateGoalTitle();
            Amount = amount.ValidateGoalAmount();
            Unit = unit.ValidateGoalUnit();
            PlannedDays = plannedDays.ValidatePlannedDays();
        }

        public void SetWeeklyGoal(string? title, int targetCount)
        {
            if (WeeklyGoal is null)
                WeeklyGoal = new WeeklyGoal(Id, title, targetCount);
            else
                WeeklyGoal.Update(title, targetCount);
        }

        public void RemoveWeeklyGoal() => WeeklyGoal = null;
    }
}
