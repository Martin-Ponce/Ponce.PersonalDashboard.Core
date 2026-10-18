namespace Ponce.PersonalDashboard.Core.API.Dtos.BaseResponses
{
    public class ExtendedMultiResponse : MultiResponse
    {
        public IEnumerable<ProcessedResponseResult> Success { get; set; } = [];
    }
}
