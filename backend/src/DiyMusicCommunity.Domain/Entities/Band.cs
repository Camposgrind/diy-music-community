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
    public string? LogoImageUrl { get; private set; }
    public string? BandImageUrl { get; private set; }
    public string? MusicUrlPortal { get; private set; }
    public string? BandContact { get; private set; }
    public TrustStatus TrustStatus { get; private set; }
    public bool IsClaimed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation property — populated by EF Core when explicitly included
    public Genre? Genre { get; private set; }

    public Band(
        Guid id,
        string name,
        string country,
        Guid genreId,
        BandStatus status,
        DateTime createdAt)
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
        GenreId = genreId;
        Status = status;
        TrustStatus = TrustStatus.CommunityCreated;
        IsClaimed = false;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    // --- Optional field setters ---

    public void SetLocation(string? location)
    {
        Location = location;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFormationYear(int? formationYear)
    {
        FormationYear = formationYear;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetImages(string? logoImageUrl, string? bandImageUrl)
    {
        LogoImageUrl = logoImageUrl;
        BandImageUrl = bandImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMusicUrlPortal(string? musicUrlPortal)
    {
        MusicUrlPortal = musicUrlPortal;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBandContact(string? bandContact)
    {
        BandContact = bandContact;
        UpdatedAt = DateTime.UtcNow;
    }

    // --- Required field update ---

    public void Update(string name, string country, Guid genreId, BandStatus status)
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
        GenreId = genreId;
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    // --- Trust state transitions ---

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
}
