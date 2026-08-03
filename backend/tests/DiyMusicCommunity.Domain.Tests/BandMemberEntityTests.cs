using DiyMusicCommunity.Domain.Entities;

namespace DiyMusicCommunity.Domain.Tests;

public class BandMemberEntityTests
{
    private static readonly Guid BandId = Guid.NewGuid();

    private static BandMember CreateCurrentMember(string name = "Dave Reeves")
    {
        var member = new BandMember(Guid.NewGuid(), BandId, name, isCurrent: true);
        member.SetYears(1980, null);
        member.SetInstrument("Vocals");
        return member;
    }

    private static BandMember CreateMemberWithEndYear(int endYear)
    {
        var member = new BandMember(Guid.NewGuid(), BandId, "Dave Reeves", isCurrent: true);
        member.SetInstrument("Vocals");
        member.SetYears(1980, endYear);
        return member;
    }

    // --- Construction guards ---

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewBandMember_WithEmptyName_Should_ThrowArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() =>
            new BandMember(Guid.NewGuid(), BandId, name, isCurrent: true));
    }

    [Fact]
    public void NewBandMember_WithEmptyBandId_Should_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new BandMember(Guid.NewGuid(), Guid.Empty, "Dave", isCurrent: true));
    }

    // --- EndYear forces IsCurrent = false ---

    [Fact]
    public void NewBandMember_WithEndYear_Should_ForceIsCurrentToFalse()
    {
        var member = CreateMemberWithEndYear(1985);

        Assert.False(member.IsCurrent);
        Assert.Equal(1985, member.EndYear);
    }

    [Fact]
    public void NewBandMember_WithoutEndYear_And_IsCurrentTrue_Should_BeCurrent()
    {
        var member = CreateCurrentMember();

        Assert.True(member.IsCurrent);
        Assert.Null(member.EndYear);
    }

    // --- SetDeparted ---

    [Fact]
    public void SetDeparted_Should_SetEndYearAndIsCurrentFalse()
    {
        var member = CreateCurrentMember();

        member.SetDeparted(1990);

        Assert.Equal(1990, member.EndYear);
        Assert.False(member.IsCurrent);
    }

    [Fact]
    public void SetDeparted_WithEndYearBeforeStartYear_Should_ThrowArgumentException()
    {
        var member = CreateCurrentMember();

        Assert.Throws<ArgumentException>(() => member.SetDeparted(1975));
    }

    // --- Past vs current separation ---

    [Fact]
    public void Members_WithEndYear_Should_BeDistinguishableFromCurrentMembers()
    {
        var members = new[]
        {
            CreateCurrentMember("Alice"),
            CreateMemberWithEndYear(1990),
            CreateCurrentMember("Bob")
        };

        var current = members.Where(m => m.IsCurrent).ToList();
        var past    = members.Where(m => !m.IsCurrent).ToList();

        Assert.Equal(2, current.Count);
        Assert.Single(past);
    }
}

public class BandMemberOtherBandTests
{
    private static readonly Guid BandId = Guid.NewGuid();

    private static BandMember CreateMember()
    {
        var member = new BandMember(Guid.NewGuid(), BandId, "Dave Reeves", isCurrent: true);
        member.SetYears(1980, null);
        return member;
    }

    // --- OtherBands (replaces AlsoInBandsText) ---

    [Fact]
    public void NewBandMember_Should_HaveNoOtherBands()
    {
        var member = CreateMember();

        Assert.Empty(member.GetOtherBands());
    }

    [Fact]
    public void AddOtherBand_Should_AppendBandIdToMember()
    {
        var member = CreateMember();
        var otherBandId = Guid.NewGuid();

        member.AddOtherBand(otherBandId);

        Assert.Contains(otherBandId, member.GetOtherBands());
    }

    [Fact]
    public void AddOtherBand_Duplicate_Should_ThrowArgumentException()
    {
        var member = CreateMember();
        var otherBandId = Guid.NewGuid();
        member.AddOtherBand(otherBandId);

        Assert.Throws<ArgumentException>(() => member.AddOtherBand(otherBandId));
    }

    [Fact]
    public void AddOtherBand_EmptyGuid_Should_ThrowArgumentException()
    {
        var member = CreateMember();

        Assert.Throws<ArgumentException>(() => member.AddOtherBand(Guid.Empty));
    }

    [Fact]
    public void RemoveOtherBand_Existing_Should_RemoveFromMember()
    {
        var member = CreateMember();
        var otherBandId = Guid.NewGuid();
        member.AddOtherBand(otherBandId);

        member.RemoveOtherBand(otherBandId);

        Assert.DoesNotContain(otherBandId, member.GetOtherBands());
    }

    [Fact]
    public void RemoveOtherBand_NotPresent_Should_ThrowArgumentException()
    {
        var member = CreateMember();

        Assert.Throws<ArgumentException>(() => member.RemoveOtherBand(Guid.NewGuid()));
    }
}
