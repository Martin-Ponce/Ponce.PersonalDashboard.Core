using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Dtos.Pagination;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.UpcomingDates;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.UpcomingDate;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.UpcomingDates
{
    public static class GetUpcomingDates
    {
        public const string ENDPOINT_NAME = "Get upcoming dates";

        public static RouteGroupBuilder MapGetUpcomingDates(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("",
                async (
                    HttpContext context,
                    UpcomingDateService upcomingDateService,
                    [AsParameters] PaginationRequest request,
                    [FromQuery] UpcomingDateStatus? status,
                    [FromQuery] DateOnly? from,
                    [FromQuery] DateOnly? to) =>
                {
                    var upcomingDates = upcomingDateService.GetUpcomingDates(status, from, to);
                    var response = await Operation(upcomingDates, request);
                    context.AddPaginationHeader(response);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<IEnumerable<GetUpcomingDatesResponse>>(StatusCodes.Status200OK)
            .WithDescription("Gets the list of upcoming dates, optionally filtered by status and by an inclusive date range (from/to). Sorted by date and time (all-day events after the timed ones of their day) unless another sort is given.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }

        private static IReadOnlyDictionary<string, IOrderedQueryable<GetUpcomingDatesResponse>> GetUpcomingDatesSorts(IQueryable<GetUpcomingDatesResponse> query) => new Dictionary<string, IOrderedQueryable<GetUpcomingDatesResponse>>()
        {
            // Timed events first, then all-day ones (null time).
            { "date", query.OrderBy(a => a.Date).ThenBy(a => a.Time == null).ThenBy(a => a.Time).ThenBy(a => a.Title) },
            { "date_desc", query.OrderByDescending(a => a.Date).ThenByDescending(a => a.Time == null).ThenByDescending(a => a.Time).ThenByDescending(a => a.Title) },
            { "title", query.OrderBy(a => a.Title) },
            { "title_desc", query.OrderByDescending(a => a.Title) },
        };

        private static async Task<PagedList<GetUpcomingDatesResponse>> Operation(IQueryable<GetUpcomingDatesResponse> query, PaginationRequest paginationRequest)
        {
            if (!string.IsNullOrEmpty(paginationRequest.Filter))
            {
                var filter = paginationRequest.Filter.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(filter));
            }
            var sort = string.IsNullOrEmpty(paginationRequest.Sort) ? "date" : paginationRequest.Sort;
            if (!GetUpcomingDatesSorts(query).TryGetValue(sort, out var sortedQuery))
            {
                throw ValidationException.NotSupported("An unsupported sort parameter was received");
            }
            return await PagedList<GetUpcomingDatesResponse>.ToPagedList(sortedQuery, paginationRequest);
        }
    }
}
