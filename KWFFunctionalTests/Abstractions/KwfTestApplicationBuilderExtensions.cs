namespace KWFFunctionalTests.Abstractions
{
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;

    public static class KwfTestApplicationBuilderExtensions
    {
        public static (WebApplicationFactory<TProgram>, HttpClient) CreateKwfTestApplicationClient<TProgram>(Action<IServiceCollection> addMocks)
            where TProgram : class
        {
            var applicationFactory = new WebApplicationFactory<TProgram>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        addMocks(services);
                    });
                });

            var client = applicationFactory.CreateClient();

            return(applicationFactory, client);
        }
    }
}
