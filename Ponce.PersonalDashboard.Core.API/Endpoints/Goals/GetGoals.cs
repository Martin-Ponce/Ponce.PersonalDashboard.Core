using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Dtos.Pagination;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Goals;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Goals
{
    public static class GetGoals
    {
        public const string ENDPOINT_NAME = "Get goals";

        public static RouteGroupBuilder MapGetGoals(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("",
                async (
                    HttpContext context,
                    GoalService goalService,
                    [AsParameters] PaginationRequest request,
                    [FromQuery] DateOnly? date) =>
                {
                    var goals = goalService.GetGoals(date ?? DateUtil.TodayUtc());
                    var response = await Operation(goals, request);
                    context.AddPaginationHeader(response);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<IEnumerable<GoalResponse>>(StatusCodes.Status200OK)
            .WithDescription("Gets the daily goals with whether they were checked on the given date (default: today in UTC) and, for those with a weekly goal, the progress of that date's week. Each goal includes the days it was checked in that week. Sorted by title unless another sort is given (title, createdat).")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }

        private static IReadOnlyDictionary<string, IOrderedQueryable<GoalResponse>> GetGoalsSorts(IQueryable<GoalResponse> query) => new Dictionary<string, IOrderedQueryable<GoalResponse>>()
        {
            { "title", query.OrderBy(a => a.Title) },
            { "title_desc", query.OrderByDescending(a => a.Title) },
            { "createdat", query.OrderBy(a => a.CreatedAtUtc) },
            { "createdat_desc", query.OrderByDescending(a => a.CreatedAtUtc) },
        };

        private static async Task<PagedList<GoalResponse>> Operation(IQueryable<GoalResponse> query, PaginationRequest paginationRequest)
        {
            if (!string.IsNullOrEmpty(paginationRequest.Filter))
            {
                var filter = paginationRequest.Filter.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(filter));
            }
            var sort = string.IsNullOrEmpty(paginationRequest.Sort) ? "title" : paginationRequest.Sort;
            if (!GetGoalsSorts(query).TryGetValue(sort, out var sortedQuery))
            {
                throw ValidationException.NotSupported("An unsupported sort parameter was received");
            }
            return await PagedList<GoalResponse>.ToPagedList(sortedQuery, paginationRequest);
        }
    }
}
