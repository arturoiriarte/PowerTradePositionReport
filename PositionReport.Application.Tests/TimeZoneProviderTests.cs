using Microsoft.Extensions.Options;
using Moq;
using PositionReport.Application.Configuration;
using PositionReport.Application.TimeZoneProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Application.Tests
{
    public class TimeZoneProviderTests
    {
        [Fact]
        public void GetTimeZone_ShouldReturnCorrectTimeZoneInfo_WhenUsingBerlinTimeZoneProvider()
        {
            // Arrange
            var timeZoneProvider = new BerlinTimeZoneProvider();
            var expectedTimeZoneId = "Europe/Berlin";

            // Act
            var timeZone = timeZoneProvider.GetTimeZone();

            // Assert
            Assert.Equal(expectedTimeZoneId, timeZone.Id);
        }

        [Fact]
        public void GetTimeZone_ShouldReturnCorrectTimeZoneInfo_WhenUsingLondonTimeZoneProvider()
        {
            // Arrange
            var timeZoneProvider = new LondonTimeZoneProvider();
            var expectedTimeZoneId = "Europe/London";

            // Act
            var timeZoneInfo = timeZoneProvider.GetTimeZone();

            // Assert
            Assert.Equal(expectedTimeZoneId, timeZoneInfo.Id);
        }

        [Theory]
        [InlineData("Europe/Berlin")]
        [InlineData("Europe/London")]
        public void GetTimeZone_ShouldReturnCorrectTimeZoneInfo_WhenUsingConfigurableTimeZoneProvider(string expectedTimeZoneId)
        {
            // Arrange
            var timeZoneSettings = Options.Create(new TimeZoneSettings() { TimeZoneId = expectedTimeZoneId });

            var timeZoneProvider = new ConfigurableTimeZoneProvider(timeZoneSettings);

            // Act
            var timeZoneInfo = timeZoneProvider.GetTimeZone();

            // Assert
            Assert.Equal(expectedTimeZoneId, timeZoneInfo.Id);
        }

        [Theory]
        [InlineData("America/Berlin")]
        [InlineData("America/London")]
        public void GetTimeZone_ShouldThrowExceptionWhenZoneIdIsInvalid(string invalidTimeZoneId)
        {
            // Arrange
            var timeZoneSettings = Options.Create(new TimeZoneSettings() { TimeZoneId = invalidTimeZoneId });
            var timeZoneProvider = new ConfigurableTimeZoneProvider(timeZoneSettings);

            // Act & Assert
            Assert.Throws<TimeZoneNotFoundException>(() => timeZoneProvider.GetTimeZone());
        }
    }
}
