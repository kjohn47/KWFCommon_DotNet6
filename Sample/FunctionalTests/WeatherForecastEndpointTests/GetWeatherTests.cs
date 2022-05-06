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

        [DataTestMethod]
        [DataRow("1", "Summary0")]
        [DataRow("2", "Summary1")]
        [DataRow("3", "Summary2")]
        [DataRow("4", "Summary3")]
        [DataRow("5", "Summary4")]
        public async Task ShouldReturnWeatherForId(string id, string summaryValue)
        {
            // Add service response mock
            _weatherSumariesServiceMock!
                .Setup(x => x.GetSumaries())
                .Returns(Task<string[]>.FromResult(new[] { summaryValue }));

            // Call service endpoint
            var (status, response) = await GetAsync<WeatherForecastQueryResponse>(id);

            // Assert result
            status.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();
            response?.ForecastResults.Should().HaveCount(1);
            response!.ForecastResults!.First().Summary.Should().Be(summaryValue);
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("6")]
        [DataRow("17")]
        [DataRow("100")]
        public async Task ShouldReturnErrorForOutOfRangeId(string id)
        {
            // Add service response mock
            _weatherSumariesServiceMock!
                .Setup(x => x.GetSumaries())
                .Returns(Task<string[]>.FromResult(new[] { weatherSummaryTest }));

            // Call service endpoint
            var (status, response) = await GetAsync<KWFCommon.Implementation.Models.ErrorResult>(id);

            // Assert result
            status.Should().Be(HttpStatusCode.BadRequest);
            response.Should().NotBeNull();
            response!.ErrorCode.Should().Be("INVID");
            response!.ErrorType.Should().Be(ErrorTypeEnum.Validation);
            response!.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
