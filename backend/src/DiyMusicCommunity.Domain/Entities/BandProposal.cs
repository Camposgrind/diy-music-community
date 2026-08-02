using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Domain.Exceptions;

namespace DiyMusicCommunity.Domain.Entities;

public sealed class BandProposal : Entity
{
    public string Name { get; private set; }
    public string Country { get; private set; }
    public string? Location { get; private set; }
    public Guid GenreId { get; private set; }
    public int? FormationYear { get; private set; }
    public string? Description { get; private set; }
    public string? SourceUrl { get; private set; }
    public Guid SubmittedByUserId { get; private set; }
    public ProposalStatus ReviewStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }
    public string? RejectionReason { get; private set; }

    public BandProposal(
        Guid id,
        string name,
        string country,
        Guid genreId,
        Guid submittedByUserId,
        DateTime createdAt,
        string? location = null,
        int? formationYear = null,
        string? description = null,
        string? sourceUrl = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Proposal name cannot be empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country cannot be empty.", nameof(country));
        }
        if (genreId == Guid.Empty)
        {
            throw new ArgumentException("GenreId cannot be empty.", nameof(genreId));
        }
        if (submittedByUserId == Guid.Empty)
        {
            throw new ArgumentException("SubmittedByUserId cannot be empty.", nameof(submittedByUserId));
        }

        Name = name;
        Country = country;
        Location = location;
        GenreId = genreId;
        FormationYear = formationYear;
        Description = description;
        SourceUrl = sourceUrl;
        SubmittedByUserId = submittedByUserId;
        ReviewStatus = ProposalStatus.Pending;
        CreatedAt = createdAt;
    }

    public void Approve(Guid reviewedByUserId, DateTime reviewedAt)
    {
        EnsurePending();

        ReviewStatus = ProposalStatus.Approved;
        ReviewedByUserId = reviewedByUserId;
        ReviewedAt = reviewedAt;
    }

    public void Reject(Guid reviewedByUserId, DateTime reviewedAt, string reason)
    {
        EnsurePending();

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("A rejection reason is required.");
        }

        ReviewStatus = ProposalStatus.Rejected;
        ReviewedByUserId = reviewedByUserId;
        ReviewedAt = reviewedAt;
        RejectionReason = reason;
    }

    private void EnsurePending()
    {
        if (ReviewStatus != ProposalStatus.Pending)
        {
            throw new InvalidProposalTransitionException(
                $"Cannot transition proposal from '{ReviewStatus}'. Only Pending proposals can be reviewed.");
        }
    }
}
