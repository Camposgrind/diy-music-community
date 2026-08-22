namespace DiyMusicCommunity.Api.Telemetry;

public sealed class NullApplicationTelemetry : IApplicationTelemetry
{
    public void TrackBusinessOperation(BusinessOperation operation)
    {
    }
}
