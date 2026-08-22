using Microsoft.ApplicationInsights;

namespace DiyMusicCommunity.Api.Telemetry;

public sealed class ApplicationInsightsTelemetry : IApplicationTelemetry
{
    private readonly TelemetryClient _telemetryClient;

    public ApplicationInsightsTelemetry(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }

    public void TrackBusinessOperation(BusinessOperation operation)
    {
        var (eventName, metricName) = operation switch
        {
            BusinessOperation.UserRegistered => ("UserRegistered", "UsersRegistered"),
            BusinessOperation.UserLoginSucceeded => ("UserLoginSucceeded", "SuccessfulLogins"),
            BusinessOperation.BandCreated => ("BandCreated", "BandsCreated"),
            BusinessOperation.BandUpdated => ("BandUpdated", "BandsUpdated"),
            BusinessOperation.BandDeleted => ("BandDeleted", "BandsDeleted"),
            BusinessOperation.MemberCreated => ("MemberCreated", "MembersCreated"),
            BusinessOperation.MemberUpdated => ("MemberUpdated", "MembersUpdated"),
            BusinessOperation.MemberDeleted => ("MemberDeleted", "MembersDeleted"),
            BusinessOperation.ReleaseCreated => ("ReleaseCreated", "ReleasesCreated"),
            BusinessOperation.ReleaseUpdated => ("ReleaseUpdated", "ReleasesUpdated"),
            BusinessOperation.ReleaseTracksUpdated => ("ReleaseTracksUpdated", "ReleaseTrackListsUpdated"),
            BusinessOperation.BandImageConfirmed => ("BandImageConfirmed", "BandImagesConfirmed"),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, "Unsupported business operation.")
        };

        _telemetryClient.TrackEvent(eventName);
        _telemetryClient.TrackMetric(metricName, 1);
    }
}
