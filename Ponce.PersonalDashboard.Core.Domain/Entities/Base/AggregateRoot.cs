namespace Ponce.PersonalDashboard.Core.Domain.Entities.Base
{
    /// <summary>Base class for aggregate roots. The derived class sets the Id.</summary>
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set; }

        protected AggregateRoot()
        {
            Id = Guid.Empty;
        }
    }
}
