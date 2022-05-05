namespace Sample.FunctionalTests
{
    using KWFFunctionalTests.Abstractions;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using Sample.SampleApi;
    using Sample.SampleApi.Services;

    using System;

    public abstract class BaseFunctionalTests : KwfBaseFunctionalTests<Program>
    {
        protected Mock<IWeatherForecastServices>? _weatherSumariesServiceMock;
        protected override void AddMockServices(IServiceCollection services)
        {
            _weatherSumariesServiceMock = new Mock<IWeatherForecastServices>();
            services.AddSingleton<IWeatherForecastServices>(_weatherSumariesServiceMock.Object);
        }
    }
}
