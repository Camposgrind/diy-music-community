namespace DiyMusicCommunity.Api.Telemetry;

public interface IApplicationTelemetry
{
    void TrackBusinessOperation(BusinessOperation operation);
}
