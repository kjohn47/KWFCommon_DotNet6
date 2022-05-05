namespace Sample.FunctionalTests.WeatherForecastEndpointTests
{
    using FluentAssertions;

    using KWFCommon.Abstractions.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Sample.SampleApi.Queries.WeatherMemoryCache;

    using System.Net;
    using System.Threading.Tasks;

    [TestClass]
    public class GetWeatherTests : BaseFunctionalTests
    {
        private const string weatherSummaryTest = "TestSummary";
        protected override string BearerToken => string.Empty;

        protected override string BearerTokenHeader => string.Empty;

        protected override string UrlFormat => "WeatherForecastEndpoint/get-weather/{0}";

        [TestMethod]
        public async Task ShouldReturnWeather()
        {
            // Add service response mock
            _weatherSumariesServiceMock!
                .Setup(x => x.GetSumaries())
                .Returns(Task<string[]>.FromResult(new[] { weatherSummaryTest }));

            // Call service endpoint
            var (status, response) = await GetAsync<WeatherForecastQueryResponse>(string.Empty);

            // Assert result
            status.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response?.ForecastResults.Should().HaveCountGreaterThan(0);
            
            foreach (var item in response!.ForecastResults!)
            {
                item.Summary.Should().Be(weatherSummaryTest);
            }

        }

        [TestMethod]
        public async Task ShouldReturnWeatherForId()
        {
            // Add service response mock
            _weatherSumariesServiceMock!
                .Setup(x => x.GetSumaries())
                .Returns(Task<string[]>.FromResult(new[] { weatherSummaryTest }));

            // Call service endpoint
            var (status, response) = await GetAsync<WeatherForecastQueryResponse>("1");

            // Assert result
            status.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response?.ForecastResults.Should().HaveCount(1);
            response!.ForecastResults!.First().Summary.Should().Be(weatherSummaryTest);
        }

        [TestMethod]
        public async Task ShouldReturnErrorForOutOfRangeId()
        {
            // Add service response mock
            _weatherSumariesServiceMock!
                .Setup(x => x.GetSumaries())
                .Returns(Task<string[]>.FromResult(new[] { weatherSummaryTest }));

            // Call service endpoint
            var (status, response) = await GetAsync<TestErrorResult>("100");

            // Assert result
            status.Should().Be(HttpStatusCode.BadRequest);
            response.Should().NotBeNull();
            response!.ErrorCode.Should().Be("INVID");
            response!.ErrorType.Should().Be(ErrorTypeEnum.Validation);
            response!.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
