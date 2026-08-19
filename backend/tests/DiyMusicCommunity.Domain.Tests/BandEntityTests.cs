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
    public void Update_SplitUpWithYear_Should_SetSplitUpYear()
    {
        var band = CreateBand();

        band.Update("Discharge", "UK", Guid.NewGuid(), BandStatus.SplitUp, 1986);

        Assert.Equal(1986, band.SplitUpYear);
    }

    [Fact]
    public void Update_SplitUpWithoutYear_Should_ThrowArgumentException()
    {
        var band = CreateBand();

        Assert.Throws<ArgumentException>(() => band.Update("Discharge", "UK", Guid.NewGuid(), BandStatus.SplitUp, null));
    }

    [Fact]
    public void Update_NonSplitUp_Should_ClearSplitUpYear()
    {
        var band = CreateBand();
        band.Update("Discharge", "UK", Guid.NewGuid(), BandStatus.SplitUp, 1986);

        band.Update("Discharge", "UK", Guid.NewGuid(), BandStatus.Active, 1986);

        Assert.Null(band.SplitUpYear);
    }

    [Fact]
    public void Update_SplitUpToActive_Should_MoveLastKnownLineupToPastMembers()
    {
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", Guid.NewGuid(), BandStatus.SplitUp, DateTime.UtcNow, 1986);
        var member = new BandMember(Guid.NewGuid(), band.Id, "Bones", false);
        member.Update("Bones", "Bass", 1980, 1986, false, true);
        band.AddMember(member);

        band.Update("Discharge", "UK", band.GenreId, BandStatus.Active, null);

        Assert.False(member.IsLastKnownLineup);
        Assert.False(member.IsCurrent);
    }

    [Fact]
    public void Update_ActiveToSplitUp_Should_PromoteMostRecentPastLineup()
    {
        var band = CreateBand();
        var formerMember = new BandMember(Guid.NewGuid(), band.Id, "Older", false);
        formerMember.Update("Older", "Guitar", 1980, 1985, false, false);
        var lastMemberOne = new BandMember(Guid.NewGuid(), band.Id, "Latest One", false);
        lastMemberOne.Update("Latest One", "Vocals", 1981, 1990, false, false);
        var lastMemberTwo = new BandMember(Guid.NewGuid(), band.Id, "Latest Two", false);
        lastMemberTwo.Update("Latest Two", "Drums", 1982, 1990, false, false);
        band.AddMember(formerMember);
        band.AddMember(lastMemberOne);
        band.AddMember(lastMemberTwo);

        band.Update("Discharge", "UK", band.GenreId, BandStatus.SplitUp, 1990);

        Assert.False(formerMember.IsLastKnownLineup);
        Assert.True(lastMemberOne.IsLastKnownLineup);
        Assert.True(lastMemberTwo.IsLastKnownLineup);
    }

    [Fact]
    public void Update_ActiveToSplitUp_WithCurrentMembers_Should_MakeThemLastKnownWithSplitUpYear()
    {
        var band = CreateBand();
        var currentMember = new BandMember(Guid.NewGuid(), band.Id, "Rat", true);
        currentMember.Update("Rat", "Drums", 1988, null, true, false);
        band.AddMember(currentMember);

        band.Update("Discharge", "UK", band.GenreId, BandStatus.SplitUp, 1991);

        Assert.False(currentMember.IsCurrent);
        Assert.True(currentMember.IsLastKnownLineup);
        Assert.Equal(1991, currentMember.EndYear);
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

    // --- SetMusicUrl ---

    [Fact]
    public void SetMusicUrlPortal_Should_UpdateMusicUrlPortal()
    {
        var band = CreateBand();

        band.SetMusicUrlPortal("https://bandcamp.com/discharge");

        Assert.Equal("https://bandcamp.com/discharge", band.MusicUrlPortal);
    }

    [Fact]
    public void SetMusicUrlPortal_WithNull_Should_ClearMusicUrlPortal()
    {
        var band = CreateBand();
        band.SetMusicUrlPortal("https://bandcamp.com/discharge");

        band.SetMusicUrlPortal(null);

        Assert.Null(band.MusicUrlPortal);
    }

    [Fact]
    public void SetMusicUrlPortal_Should_UpdateUpdatedAt()
    {
        var band = CreateBand();
        var before = band.UpdatedAt;

        band.SetMusicUrlPortal("https://bandcamp.com/discharge");

        Assert.True(band.UpdatedAt >= before);
    }

    // --- SetBandContact ---

    [Fact]
    public void SetBandContact_Should_UpdateBandContact()
    {
        var band = CreateBand();

        band.SetBandContact("contact@discharge.com");

        Assert.Equal("contact@discharge.com", band.BandContact);
    }

    [Fact]
    public void SetBandContact_WithNull_Should_ClearBandContact()
    {
        var band = CreateBand();
        band.SetBandContact("contact@discharge.com");

        band.SetBandContact(null);

        Assert.Null(band.BandContact);
    }

    [Fact]
    public void SetBandContact_Should_UpdateUpdatedAt()
    {
        var band = CreateBand();
        var before = band.UpdatedAt;

        band.SetBandContact("contact@discharge.com");

        Assert.True(band.UpdatedAt >= before);
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

        band.Update("Amebix", "UK", newGenreId, BandStatus.SplitUp, 1993);

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
            band.Update(name, "UK", Guid.NewGuid(), BandStatus.Active, null));
    }
}
