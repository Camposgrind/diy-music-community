using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Domain.Exceptions;

namespace DiyMusicCommunity.Domain.Entities;

public sealed class BandClaim : Entity
{
    public Guid BandId { get; private set; }
    public Guid UserId { get; private set; }
    public ClaimType ClaimType { get; private set; }
    public string? Message { get; private set; }
    public string? EvidenceUrl { get; private set; }
    public ClaimStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }
    public string? RejectionReason { get; private set; }

    public BandClaim(
        Guid id,
        Guid bandId,
        Guid userId,
        ClaimType claimType,
        DateTime createdAt,
        string? message = null,
        string? evidenceUrl = null)
        : base(id)
    {
        if (bandId == Guid.Empty)
        {
            throw new ArgumentException("BandId cannot be empty.", nameof(bandId));
        }
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }

        BandId = bandId;
        UserId = userId;
        ClaimType = claimType;
        Message = message;
        EvidenceUrl = evidenceUrl;
        Status = ClaimStatus.Pending;
        CreatedAt = createdAt;
    }

    public void Approve(Guid reviewedByUserId, DateTime reviewedAt)
    {
        EnsurePending();

        Status = ClaimStatus.Approved;
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

        Status = ClaimStatus.Rejected;
        ReviewedByUserId = reviewedByUserId;
        ReviewedAt = reviewedAt;
        RejectionReason = reason;
    }

    private void EnsurePending()
    {
        if (Status != ClaimStatus.Pending)
        {
            throw new DomainException(
                $"Cannot transition claim from '{Status}'. Only Pending claims can be reviewed.");
        }
    }
}
