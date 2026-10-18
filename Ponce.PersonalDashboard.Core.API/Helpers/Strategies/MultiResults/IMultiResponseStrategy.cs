using Ponce.PersonalDashboard.Core.API.Dtos.BaseResponses;
using Ponce.PersonalDashboard.Core.Application.Dtos.MultiResult;

namespace Ponce.PersonalDashboard.Core.API.Helpers.Strategies.MultiResults
{
    public interface IMultiResponseStrategy
    {
        MultiResponse GetResponse(MultiResultHandler results);
    }
}
