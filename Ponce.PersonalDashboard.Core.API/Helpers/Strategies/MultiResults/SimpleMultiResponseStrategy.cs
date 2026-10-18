using Mapster;
using Ponce.PersonalDashboard.Core.API.Dtos.BaseResponses;
using Ponce.PersonalDashboard.Core.Application.Dtos.MultiResult;

namespace Ponce.PersonalDashboard.Core.API.Helpers.Strategies.MultiResults
{
    public class SimpleMultiResponseStrategy : IMultiResponseStrategy
    {
        public MultiResponse GetResponse(MultiResultHandler results)
        {
            return new MultiResponse
            {
                Processed = results.Processed,
                Failed = results.Failed,
                Errors = results.Errors.Adapt<List<FailedMultiResponseResult>>(),
                StatusCode = 206
            };
        }
    }
}
