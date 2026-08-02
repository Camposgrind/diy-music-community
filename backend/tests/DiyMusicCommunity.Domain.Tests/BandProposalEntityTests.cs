using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Domain.Exceptions;

namespace DiyMusicCommunity.Domain.Tests;

public class BandProposalEntityTests
{
    private static readonly Guid ModeratorId = Guid.NewGuid();
    private static readonly DateTime ReviewedAt = DateTime.UtcNow;

    private static BandProposal CreatePendingProposal()
    {
        return new BandProposal(Guid.NewGuid(), "Terrorizer", "US", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
    }

    // --- Construction ---

    [Fact]
    public void NewProposal_Should_HavePendingReviewStatus()
    {
        var proposal = CreatePendingProposal();

        Assert.Equal(ProposalStatus.Pending, proposal.ReviewStatus);
        Assert.Null(proposal.ReviewedAt);
        Assert.Null(proposal.ReviewedByUserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewProposal_WithEmptyName_Should_ThrowArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() =>
            new BandProposal(Guid.NewGuid(), name, "US", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewProposal_WithEmptyCountry_Should_ThrowArgumentException(string country)
    {
        Assert.Throws<ArgumentException>(() =>
            new BandProposal(Guid.NewGuid(), "Terrorizer", country, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow));
    }

    // --- Approve ---

    [Fact]
    public void ApprovePendingProposal_Should_SetApprovedStatusAndReviewer()
    {
        var proposal = CreatePendingProposal();

        proposal.Approve(ModeratorId, ReviewedAt);

        Assert.Equal(ProposalStatus.Approved, proposal.ReviewStatus);
        Assert.Equal(ModeratorId, proposal.ReviewedByUserId);
        Assert.Equal(ReviewedAt, proposal.ReviewedAt);
    }

    [Fact]
    public void ApproveAlreadyApprovedProposal_Should_ThrowInvalidProposalTransitionException()
    {
        var proposal = CreatePendingProposal();
        proposal.Approve(ModeratorId, ReviewedAt);

        Assert.Throws<InvalidProposalTransitionException>(() =>
            proposal.Approve(ModeratorId, ReviewedAt));
    }

    [Fact]
    public void ApproveRejectedProposal_Should_ThrowInvalidProposalTransitionException()
    {
        var proposal = CreatePendingProposal();
        proposal.Reject(ModeratorId, ReviewedAt, "Not enough info.");

        Assert.Throws<InvalidProposalTransitionException>(() =>
            proposal.Approve(ModeratorId, ReviewedAt));
    }

    // --- Reject ---

    [Fact]
    public void RejectPendingProposal_Should_SetRejectedStatusAndReason()
    {
        var proposal = CreatePendingProposal();

        proposal.Reject(ModeratorId, ReviewedAt, "Missing source URL.");

        Assert.Equal(ProposalStatus.Rejected, proposal.ReviewStatus);
        Assert.Equal("Missing source URL.", proposal.RejectionReason);
        Assert.Equal(ModeratorId, proposal.ReviewedByUserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RejectPendingProposal_WithoutReason_Should_ThrowDomainException(string? reason)
    {
        var proposal = CreatePendingProposal();

        Assert.Throws<DomainException>(() =>
            proposal.Reject(ModeratorId, ReviewedAt, reason!));
    }

    [Fact]
    public void RejectAlreadyRejectedProposal_Should_ThrowInvalidProposalTransitionException()
    {
        var proposal = CreatePendingProposal();
        proposal.Reject(ModeratorId, ReviewedAt, "Reason.");

        Assert.Throws<InvalidProposalTransitionException>(() =>
            proposal.Reject(ModeratorId, ReviewedAt, "Another reason."));
    }
}
