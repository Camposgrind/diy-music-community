using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Domain.Exceptions;

namespace DiyMusicCommunity.Domain.Tests;

public class BandClaimEntityTests
{
    private static readonly Guid ModeratorId = Guid.NewGuid();
    private static readonly DateTime ReviewedAt = DateTime.UtcNow;

    private static BandClaim CreatePendingClaim() =>
        new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ClaimType.CurrentMember, DateTime.UtcNow,
            message: "I am the drummer.", evidenceUrl: "https://evidence.example.com");

    // --- Construction ---

    [Fact]
    public void NewClaim_Should_HavePendingStatus()
    {
        var claim = CreatePendingClaim();

        Assert.Equal(ClaimStatus.Pending, claim.Status);
        Assert.Null(claim.ReviewedAt);
        Assert.Null(claim.ReviewedByUserId);
    }

    [Fact]
    public void NewClaim_WithEmptyBandId_Should_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new BandClaim(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), ClaimType.CurrentMember, DateTime.UtcNow));
    }

    [Fact]
    public void NewClaim_WithEmptyUserId_Should_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new BandClaim(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, ClaimType.CurrentMember, DateTime.UtcNow));
    }

    // --- Approve ---

    [Fact]
    public void ApprovePendingClaim_Should_SetApprovedStatusAndReviewer()
    {
        var claim = CreatePendingClaim();

        claim.Approve(ModeratorId, ReviewedAt);

        Assert.Equal(ClaimStatus.Approved, claim.Status);
        Assert.Equal(ModeratorId, claim.ReviewedByUserId);
        Assert.Equal(ReviewedAt, claim.ReviewedAt);
    }

    [Fact]
    public void ApproveAlreadyApprovedClaim_Should_ThrowDomainException()
    {
        var claim = CreatePendingClaim();
        claim.Approve(ModeratorId, ReviewedAt);

        Assert.Throws<DomainException>(() => claim.Approve(ModeratorId, ReviewedAt));
    }

    [Fact]
    public void ApproveRejectedClaim_Should_ThrowDomainException()
    {
        var claim = CreatePendingClaim();
        claim.Reject(ModeratorId, ReviewedAt, "No evidence.");

        Assert.Throws<DomainException>(() => claim.Approve(ModeratorId, ReviewedAt));
    }

    // --- Reject ---

    [Fact]
    public void RejectPendingClaim_Should_SetRejectedStatusAndReason()
    {
        var claim = CreatePendingClaim();

        claim.Reject(ModeratorId, ReviewedAt, "Insufficient evidence.");

        Assert.Equal(ClaimStatus.Rejected, claim.Status);
        Assert.Equal("Insufficient evidence.", claim.RejectionReason);
        Assert.Equal(ModeratorId, claim.ReviewedByUserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RejectPendingClaim_WithoutReason_Should_ThrowDomainException(string? reason)
    {
        var claim = CreatePendingClaim();

        Assert.Throws<DomainException>(() =>
            claim.Reject(ModeratorId, ReviewedAt, reason!));
    }

    [Fact]
    public void RejectAlreadyRejectedClaim_Should_ThrowDomainException()
    {
        var claim = CreatePendingClaim();
        claim.Reject(ModeratorId, ReviewedAt, "Reason.");

        Assert.Throws<DomainException>(() =>
            claim.Reject(ModeratorId, ReviewedAt, "Another reason."));
    }
}
