using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Abstractions;

public sealed class BandSearchFilter
{
    public string? Name { get; init; }
    public string? Country { get; init; }
    public Guid? GenreId { get; init; }
    public BandStatus? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
