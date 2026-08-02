using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Domain.Exceptions;

namespace DiyMusicCommunity.Domain.Tests;

public class BandEntityTests
{
    private static Band CreateBand(
        string name = "Discharge",
        string country = "UK",
        BandStatus status = BandStatus.Active)
    {
        return new Band(Guid.NewGuid(), name, country, Guid.NewGuid(), status, DateTime.UtcNow);
    }

    // --- Construction guards ---

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewBand_WithEmptyName_Should_ThrowArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() => CreateBand(name: name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewBand_WithEmptyCountry_Should_ThrowArgumentException(string country)
    {
        Assert.Throws<ArgumentException>(() => CreateBand(country: country));
    }

    [Fact]
    public void NewBand_WithEmptyGenreId_Should_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Band(Guid.NewGuid(), "Napalm Death", "UK", Guid.Empty, BandStatus.Active, DateTime.UtcNow));
    }

    // --- Default trust state ---

    [Fact]
    public void NewBand_Should_HaveCommunityCreatedTrustStatus()
    {
        var band = CreateBand();

        Assert.Equal(TrustStatus.CommunityCreated, band.TrustStatus);
        Assert.False(band.IsClaimed);
    }

    // --- Optional setters ---

    [Fact]
    public void SetLocation_Should_UpdateLocation()
    {
        var band = CreateBand();

        band.SetLocation("Birmingham");

        Assert.Equal("Birmingham", band.Location);
    }

    [Fact]
    public void SetFormationYear_Should_UpdateFormationYear()
    {
        var band = CreateBand();

        band.SetFormationYear(1977);

        Assert.Equal(1977, band.FormationYear);
    }

    [Fact]
    public void SetDescription_Should_UpdateDescription()
    {
        var band = CreateBand();

        band.SetDescription("UK hardcore punk band.");

        Assert.Equal("UK hardcore punk band.", band.Description);
    }

    [Fact]
    public void SetImages_Should_UpdateLogoAndBandImageUrl()
    {
        var band = CreateBand();

        band.SetImages("https://example.com/logo.png", "https://example.com/band.jpg");

        Assert.Equal("https://example.com/logo.png", band.LogoImageUrl);
        Assert.Equal("https://example.com/band.jpg", band.BandImageUrl);
    }

    // --- MarkClaimPending ---

    [Fact]
    public void MarkClaimPending_Should_SetTrustStatusToClaimPending()
    {
        var band = CreateBand();

        band.MarkClaimPending();

        Assert.Equal(TrustStatus.ClaimPending, band.TrustStatus);
    }

    [Fact]
    public void MarkClaimPending_OnBlockedBand_Should_ThrowDomainException()
    {
        var band = CreateBand();
        band.Block();

        Assert.Throws<DomainException>(() => band.MarkClaimPending());
    }

    // --- ApproveClaim ---

    [Fact]
    public void ApproveClaim_Should_SetClaimedStatusAndIsClaimedTrue()
    {
        var band = CreateBand();
        band.MarkClaimPending();

        band.ApproveClaim();

        Assert.Equal(TrustStatus.Claimed, band.TrustStatus);
        Assert.True(band.IsClaimed);
    }

    [Fact]
    public void ApproveClaim_OnBlockedBand_Should_ThrowDomainException()
    {
        var band = CreateBand();
        band.Block();

        Assert.Throws<DomainException>(() => band.ApproveClaim());
    }

    // --- RevertToUnclaimed ---

    [Fact]
    public void RevertToUnclaimed_Should_ResetTrustStatusAndIsClaimed()
    {
        var band = CreateBand();
        band.MarkClaimPending();
        band.ApproveClaim();

        band.RevertToUnclaimed();

        Assert.Equal(TrustStatus.CommunityCreated, band.TrustStatus);
        Assert.False(band.IsClaimed);
    }

    // --- Block ---

    [Fact]
    public void Block_Should_SetTrustStatusToBlocked()
    {
        var band = CreateBand();

        band.Block();

        Assert.Equal(TrustStatus.Blocked, band.TrustStatus);
    }

    // --- Update ---

    [Fact]
    public void Update_Should_ChangeRequiredFields()
    {
        var band = CreateBand();
        var newGenreId = Guid.NewGuid();

        band.Update("Amebix", "UK", newGenreId, BandStatus.SplitUp);

        Assert.Equal("Amebix", band.Name);
        Assert.Equal("UK", band.Country);
        Assert.Equal(newGenreId, band.GenreId);
        Assert.Equal(BandStatus.SplitUp, band.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithEmptyName_Should_ThrowArgumentException(string name)
    {
        var band = CreateBand();

        Assert.Throws<ArgumentException>(() =>
            band.Update(name, "UK", Guid.NewGuid(), BandStatus.Active));
    }
}
