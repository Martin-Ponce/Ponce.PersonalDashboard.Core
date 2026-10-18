namespace Ponce.PersonalDashboard.Core.API.Endpoints.Goals
{
    /// <summary>Daily goals and their optional weekly goal. Weekly goals have no endpoints of their own, they're handled through the daily goal.</summary>
    public static class GoalEndpointGroup
    {
        public static RouteGroupBuilder MapGoalEndpoints(this RouteGroupBuilder appEndpoints)
        {
            var goalEndpointGroup = appEndpoints.MapGroup("goals").WithTags("Goals");
            goalEndpointGroup
                .MapGetGoals()
                .MapGetGoalById()
                .MapSaveGoal()
                .MapUpdateGoal()
                .MapDeleteGoal()
                .MapCheckGoal()
                .MapUncheckGoal();
            return appEndpoints;
        }
    }
}
