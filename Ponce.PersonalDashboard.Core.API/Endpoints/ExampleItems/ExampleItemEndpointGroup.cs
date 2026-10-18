namespace Ponce.PersonalDashboard.Core.API.Endpoints.ExampleItems
{
    /// <summary>Example item endpoints.</summary>
    public static class ExampleItemEndpointGroup
    {
        public static RouteGroupBuilder MapExampleItemEndpoints(this RouteGroupBuilder appEndpoints)
        {
            var exampleItemEndpointGroup = appEndpoints.MapGroup("exampleitems").WithTags("ExampleItems");
            exampleItemEndpointGroup
                .MapGetExampleItems()
                .MapGetExampleItemById()
                .MapSaveExampleItem();
            return appEndpoints;
        }
    }
}
