namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.Users
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public string? DisplayName { get; set; }
        public string Name => DisplayName ?? UserName;
        public DateTime CreatedAtUtc { get; set; }
    }
}
