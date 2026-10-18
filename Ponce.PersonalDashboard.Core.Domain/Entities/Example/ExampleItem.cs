using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Example
{
    /// <summary>Sample aggregate root with a name and an optional description.</summary>
    public class ExampleItem : AggregateRoot
    {
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }

        private ExampleItem() { } // Required by EF Core

        public ExampleItem(string name, string? description)
        {
            Id = Guid.NewGuid();
            Name = name.ValidateName();
            Description = description.ValidateDescription();
        }
    }
}
