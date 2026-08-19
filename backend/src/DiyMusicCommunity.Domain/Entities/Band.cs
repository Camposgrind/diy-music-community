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
    public int? SplitUpYear { get; private set; }
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

    // Navigation properties — populated by EF Core when explicitly included
    private readonly List<Release> _releases = new();
    private readonly List<BandMember> _members = new();

    public IReadOnlyList<Release> Releases => _releases.AsReadOnly();
    public IReadOnlyList<BandMember> Members => _members.AsReadOnly();

    public Band(
        Guid id,
        string name,
        string country,
        Guid genreId,
        BandStatus status,
        DateTime createdAt,
        int? splitUpYear = null)
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
        SplitUpYear = GetSplitUpYear(status, splitUpYear);
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

    public void Update(string name, string country, Guid genreId, BandStatus status, int? splitUpYear)
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

        var previousStatus = Status;

        Name = name;
        Country = country;
        GenreId = genreId;
        Status = status;
        SplitUpYear = GetSplitUpYear(status, splitUpYear);
        UpdateMemberLineupForStatusTransition(previousStatus, status);
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdateMemberLineupForStatusTransition(BandStatus previousStatus, BandStatus newStatus)
    {
        if (previousStatus == BandStatus.SplitUp && newStatus != BandStatus.SplitUp)
        {
            foreach (var member in Members.Where(member => member.IsLastKnownLineup))
            {
                member.MoveToPastMembers();
            }

            return;
        }

        if (previousStatus != BandStatus.SplitUp && newStatus == BandStatus.SplitUp)
        {
            var currentMembers = Members.Where(member => member.IsCurrent).ToList();
            if (currentMembers.Count > 0)
            {
                foreach (var member in currentMembers)
                {
                    member.SetDeparted(SplitUpYear!.Value);
                    member.MarkAsLastKnownLineup();
                }

                return;
            }

            var mostRecentEndYear = Members
                .Where(member => !member.IsCurrent && !member.IsLastKnownLineup && member.EndYear.HasValue)
                .Select(member => member.EndYear)
                .Max();

            if (!mostRecentEndYear.HasValue)
            {
                return;
            }

            foreach (var member in Members.Where(member => !member.IsCurrent && !member.IsLastKnownLineup && member.EndYear == mostRecentEndYear))
            {
                member.MarkAsLastKnownLineup();
            }
        }
    }

    private static int? GetSplitUpYear(BandStatus status, int? splitUpYear)
    {
        if (status == BandStatus.SplitUp && !splitUpYear.HasValue)
        {
            throw new ArgumentException("Split-up year is required for a split-up band.", nameof(splitUpYear));
        }

        return status == BandStatus.SplitUp ? splitUpYear : null;
    }

    public void AddMember(BandMember member)
    {
        if (member.BandId != Id)
        {
            throw new ArgumentException("The member must belong to this band.", nameof(member));
        }

        _members.Add(member);
    }

    public void AddRelease(Release release)
    {
        if (release.BandId != Id)
        {
            throw new ArgumentException("The release must belong to this band.", nameof(release));
        }

        _releases.Add(release);
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
