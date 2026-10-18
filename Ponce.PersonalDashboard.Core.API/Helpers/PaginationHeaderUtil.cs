using Ponce.PersonalDashboard.Core.API.Dtos.Pagination;
using System.Text.Json;

namespace Ponce.PersonalDashboard.Core.API.Helpers
{
    public static class PaginationHeaderUtil
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

        public static HttpContext AddPaginationHeader<T>(this HttpContext context, PagedList<T> pagedList) where T : class
        {
            var responseHeaders = context.Response.Headers;
            const string headerKey = "X-Pagination";
            if (!responseHeaders.ContainsKey(headerKey))
            {
                responseHeaders.Append(headerKey, JsonSerializer.Serialize(new
                {
                    pagedList.TotalPages,
                    pagedList.CurrentPage,
                    pagedList.PageSize,
                    pagedList.TotalCount,
                    pagedList.HasPrevious,
                    pagedList.HasNext
                }, _jsonSerializerOptions));
            }
            return context;
        }
    }
}
