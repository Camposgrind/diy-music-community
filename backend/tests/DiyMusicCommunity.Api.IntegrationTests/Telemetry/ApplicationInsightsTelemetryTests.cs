using System.Collections.Concurrent;
using DiyMusicCommunity.Api.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace DiyMusicCommunity.Api.IntegrationTests.Telemetry;

public sealed class ApplicationInsightsTelemetryTests
{
    [Fact]
    public void TrackBusinessOperation_UserRegistered_Should_SendEventAndMetricWithoutProperties()
    {
        var channel = new CapturingTelemetryChannel();
        var configuration = new TelemetryConfiguration
        {
            TelemetryChannel = channel
        };
        var telemetryClient = new TelemetryClient(configuration);
        var telemetry = new ApplicationInsightsTelemetry(telemetryClient);

        telemetry.TrackBusinessOperation(BusinessOperation.UserRegistered);

        var businessEvent = Assert.Single(channel.Items.OfType<EventTelemetry>());
        var metric = Assert.Single(channel.Items.OfType<MetricTelemetry>());
        Assert.Equal("UserRegistered", businessEvent.Name);
        Assert.Empty(businessEvent.Properties);
        Assert.Equal("UsersRegistered", metric.Name);
        Assert.Equal(1, metric.Sum);
    }

    [Fact]
    public void TrackBusinessOperation_UnsupportedOperation_Should_Throw()
    {
        var channel = new CapturingTelemetryChannel();
        var configuration = new TelemetryConfiguration
        {
            TelemetryChannel = channel
        };
        var telemetry = new ApplicationInsightsTelemetry(new TelemetryClient(configuration));

        Assert.Throws<ArgumentOutOfRangeException>(() => telemetry.TrackBusinessOperation((BusinessOperation)999));
    }

    private sealed class CapturingTelemetryChannel : ITelemetryChannel
    {
        private readonly ConcurrentQueue<ITelemetry> _items = new();

        public bool? DeveloperMode { get; set; }

        public string? EndpointAddress { get; set; }

        public bool ThrowOnTransmission { get; set; }

        public IReadOnlyCollection<ITelemetry> Items
        {
            get
            {
                return _items.ToArray();
            }
        }

        public void Send(ITelemetry item)
        {
            _items.Enqueue(item);
        }

        public void Flush()
        {
        }

        public void Dispose()
        {
        }
    }
}
