namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class FinancesEndpointGroup
    {
        public static RouteGroupBuilder MapFinancesEndpoints(this RouteGroupBuilder appEndpoints)
        {
            var financesEndpointGroup = appEndpoints.MapGroup("finances").WithTags("Finances");
            financesEndpointGroup
                .MapGetTransactions()
                .MapGetTransactionById()
                .MapSaveTransaction()
                .MapUpdateTransaction()
                .MapDeleteTransaction()
                .MapGetMonthlySummary()
                .MapSetMonthlyBudget()
                .MapGetBalance()
                .MapSetInitialBalance();
            return appEndpoints;
        }
    }
}
