using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Domain.Exceptions;

namespace DiyMusicCommunity.Domain.Entities;

public sealed class Band : Entity
{
    public string Name { get; private set; }
    public string Country { get; private set; }
    public string? Location { get; private set; }
    public Guid GenreId { get; private set; }
    public BandStatus Status { get; private set; }
    public int? FormationYear { get; private set; }
    public string? Description { get; private set; }
    public TrustStatus TrustStatus { get; private set; }
    public bool IsClaimed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Band(
        Guid id,
        string name,
        string country,
        Guid genreId,
        BandStatus status,
        DateTime createdAt,
        string? location = null,
        int? formationYear = null,
        string? description = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Band name cannot be empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country cannot be empty.", nameof(country));
        }
        if (genreId == Guid.Empty)
        {
            throw new ArgumentException("GenreId cannot be empty.", nameof(genreId));
        }

        Name = name;
        Country = country;
        Location = location;
        GenreId = genreId;
        Status = status;
        FormationYear = formationYear;
        Description = description;
        TrustStatus = TrustStatus.CommunityCreated;
        IsClaimed = false;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public void MarkClaimPending()
    {
        if (TrustStatus == TrustStatus.Blocked)
        {
            throw new DomainException("A blocked band cannot receive claim requests.");
        }

        TrustStatus = TrustStatus.ClaimPending;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApproveClaim()
    {
        if (TrustStatus == TrustStatus.Blocked)
        {
            throw new DomainException("A blocked band cannot be claimed.");
        }

        TrustStatus = TrustStatus.Claimed;
        IsClaimed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RevertToUnclaimed()
    {
        TrustStatus = TrustStatus.CommunityCreated;
        IsClaimed = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Block()
    {
        TrustStatus = TrustStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        string country,
        string? location,
        Guid genreId,
        BandStatus status,
        int? formationYear,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Band name cannot be empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country cannot be empty.", nameof(country));
        }
        if (genreId == Guid.Empty)
        {
            throw new ArgumentException("GenreId cannot be empty.", nameof(genreId));
        }

        Name = name;
        Country = country;
        Location = location;
        GenreId = genreId;
        Status = status;
        FormationYear = formationYear;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
