namespace Ponce.PersonalDashboard.Core.Domain.Entities.Base
{
    /// <summary>Base class for child entities. Gets a new Id when created.</summary>
    public abstract class Entity
    {
        public Guid Id { get; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}
