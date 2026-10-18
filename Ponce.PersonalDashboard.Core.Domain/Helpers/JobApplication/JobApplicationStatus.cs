namespace Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication
{
    /// <summary>Stage of a job application. The chars are alphabetical in pipeline order, so sorting by status follows the pipeline.</summary>
    public enum JobApplicationStatus
    {
        Applied = 'A',
        Interview = 'I',
        Offer = 'O',
        Rejected = 'R'
    }
}
