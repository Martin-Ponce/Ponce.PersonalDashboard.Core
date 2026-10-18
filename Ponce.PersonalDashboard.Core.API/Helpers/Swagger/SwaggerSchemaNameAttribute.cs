namespace Ponce.PersonalDashboard.Core.API.Helpers.Swagger
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SwaggerSchemaNameAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}
