using Ponce.PersonalDashboard.Core.API.Dtos.JobApplications;
using Ponce.PersonalDashboard.Core.API.Dtos.Pagination;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.JobApplications
{
    public static class GetJobApplications
    {
        public const string ENDPOINT_NAME = "Get job applications";

        public static RouteGroupBuilder MapGetJobApplications(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("",
                async (
                    HttpContext context,
                    JobApplicationService jobApplicationService,
                    [AsParameters] PaginationRequest request,
                    [AsParameters] JobApplicationFiltersRequest filters) =>
                {
                    var jobApplications = jobApplicationService.GetJobApplications(filters.Year, filters.Month);
                    var matching = ApplyFilters(jobApplications, request.Filter, filters);
                    var response = await Operation(matching, request);
                    context.AddPaginationHeader(response);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<IEnumerable<GetJobApplicationsResponse>>(StatusCodes.Status200OK)
            .WithDescription("Gets the list of job applications, optionally only the ones sent in a month (year and month) and filtered by company/job title, statuses, work modes and cities. Newest first unless a sort is given.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }

        // Ties broken so pages stay stable.
        private static IReadOnlyDictionary<string, IOrderedQueryable<GetJobApplicationsResponse>> GetJobApplicationsSorts(IQueryable<GetJobApplicationsResponse> query) => new Dictionary<string, IOrderedQueryable<GetJobApplicationsResponse>>()
        {
            { "company", query.OrderBy(a => a.CompanyName).ThenBy(a => a.DateApplied) },
            { "company_desc", query.OrderByDescending(a => a.CompanyName).ThenByDescending(a => a.DateApplied) },
            { "jobtitle", query.OrderBy(a => a.JobTitle).ThenBy(a => a.CompanyName) },
            { "jobtitle_desc", query.OrderByDescending(a => a.JobTitle).ThenByDescending(a => a.CompanyName) },
            { "dateapplied", query.OrderBy(a => a.DateApplied).ThenBy(a => a.UpdatedAtUtc) },
            { "dateapplied_desc", query.OrderByDescending(a => a.DateApplied).ThenByDescending(a => a.UpdatedAtUtc) },
            { "updatedat", query.OrderBy(a => a.UpdatedAtUtc) },
            { "updatedat_desc", query.OrderByDescending(a => a.UpdatedAtUtc) },
            // Statuses are stored in pipeline order, newest first within each one.
            { "status", query.OrderBy(a => a.Status).ThenByDescending(a => a.DateApplied) },
            { "status_desc", query.OrderByDescending(a => a.Status).ThenBy(a => a.DateApplied) },
        };

        private static IQueryable<GetJobApplicationsResponse> ApplyFilters(IQueryable<GetJobApplicationsResponse> query, string? searchFilter, JobApplicationFiltersRequest filters)
        {
            if (!string.IsNullOrEmpty(searchFilter))
            {
                var filter = searchFilter.ToLower();
                query = query.Where(a => a.CompanyName.ToLower().Contains(filter) || a.JobTitle.ToLower().Contains(filter));
            }
            if (filters.Statuses is { Length: > 0 } statuses)
                query = query.Where(a => statuses.Contains(a.Status));
            if (filters.WorkModes is { Length: > 0 } workModes)
                query = query.Where(a => workModes.Contains(a.WorkMode));
            if (filters.Cities is { Length: > 0 } cities)
            {
                var includeRemote = cities.Contains(JobApplicationService.REMOTE_CITY);
                query = query.Where(a =>
                    a.WorkMode != WorkMode.Remote && a.City != null && cities.Contains(a.City) ||
                    includeRemote && (a.WorkMode == WorkMode.Remote || a.City == null));
            }
            return query;
        }

        private static async Task<PagedList<GetJobApplicationsResponse>> Operation(IQueryable<GetJobApplicationsResponse> query, PaginationRequest paginationRequest)
        {
            var sort = string.IsNullOrEmpty(paginationRequest.Sort) ? "dateapplied_desc" : paginationRequest.Sort;
            if (!GetJobApplicationsSorts(query).TryGetValue(sort, out var sortedQuery))
            {
                throw ValidationException.NotSupported("An unsupported sort parameter was received");
            }
            return await PagedList<GetJobApplicationsResponse>.ToPagedList(sortedQuery, paginationRequest);
        }
    }
}
