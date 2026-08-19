using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DiyMusicCommunity.Api.IntegrationTests;

public sealed class CatalogDeletionRepositoryTests
{
    private static readonly Guid GenreId = new("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    [Fact]
    public async Task DeleteBand_WithAllDependencies_Should_RemoveBandAndDependentRecords()
    {
        var bandId = Guid.NewGuid();
        var otherBandId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var otherMemberId = Guid.NewGuid();
        var releaseId = Guid.NewGuid();
        var factory = new CustomWebApplicationFactory();
        using var _ = factory;
        factory.CreateClientWithDb(db =>
        {
            db.Bands.AddRange(
                new Band(bandId, "Deleted Band", "UK", GenreId, BandStatus.Active, DateTime.UtcNow),
                new Band(otherBandId, "Remaining Band", "UK", GenreId, BandStatus.Active, DateTime.UtcNow));
            db.BandMembers.AddRange(
                new BandMember(memberId, bandId, "Deleted Member", true),
                new BandMember(otherMemberId, otherBandId, "Remaining Member", true));
            db.BandMemberOtherBands.AddRange(
                new BandMemberOtherBand(Guid.NewGuid(), memberId, otherBandId),
                new BandMemberOtherBand(Guid.NewGuid(), otherMemberId, bandId));
            var release = new Release(releaseId, bandId, "Deleted Release", ReleaseType.Album);
            release.AddFormat(Format.CD);
            db.Releases.Add(release);
            db.Tracks.Add(new Track(Guid.NewGuid(), releaseId, "Deleted Track", 1));
        });

        var deleted = await Execute(factory, repository => repository.DeleteBandAsync(bandId));

        deleted.Should().BeTrue();
        await AssertDatabase(factory, db =>
        {
            db.Bands.Should().NotContain(item => item.Id == bandId);
            db.BandMembers.Should().NotContain(item => item.BandId == bandId);
            db.Releases.Should().NotContain(item => item.BandId == bandId);
            db.Tracks.Should().NotContain(item => item.ReleaseId == releaseId);
            db.ReleaseFormats.Should().NotContain(item => item.ReleaseId == releaseId);
            db.BandMemberOtherBands.Should().NotContain(item => item.OtherBandId == bandId || item.BandMemberId == memberId);
        });
    }

    [Fact]
    public async Task DeleteMember_InAnyLineup_Should_RemoveItsOtherBandLinks()
    {
        var bandId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var factory = new CustomWebApplicationFactory();
        using var _ = factory;
        factory.CreateClientWithDb(db =>
        {
            db.Bands.Add(new Band(bandId, "Split Up", "UK", GenreId, BandStatus.SplitUp, DateTime.UtcNow));
            var member = new BandMember(memberId, bandId, "Last Known", false);
            member.Update("Last Known", null, 1990, 2000, false, true);
            db.BandMembers.Add(member);
            db.BandMemberOtherBands.Add(new BandMemberOtherBand(Guid.NewGuid(), memberId, bandId));
        });

        var deleted = await Execute(factory, repository => repository.DeleteMemberAsync(bandId, memberId));

        deleted.Should().BeTrue();
        await AssertDatabase(factory, db =>
        {
            db.BandMembers.Should().NotContain(item => item.Id == memberId);
            db.BandMemberOtherBands.Should().NotContain(item => item.BandMemberId == memberId);
        });
    }

    [Fact]
    public async Task DeleteRelease_WithTracksAndFormats_Should_RemoveAllReleaseChildren()
    {
        var bandId = Guid.NewGuid();
        var releaseId = Guid.NewGuid();
        var factory = new CustomWebApplicationFactory();
        using var _ = factory;
        factory.CreateClientWithDb(db =>
        {
            db.Bands.Add(new Band(bandId, "Release Owner", "UK", GenreId, BandStatus.Active, DateTime.UtcNow));
            var release = new Release(releaseId, bandId, "Release", ReleaseType.Album);
            release.AddFormat(Format.CD);
            db.Releases.Add(release);
            db.Tracks.AddRange(new Track(Guid.NewGuid(), releaseId, "One", 1), new Track(Guid.NewGuid(), releaseId, "Two", 2));
        });

        var deleted = await Execute(factory, repository => repository.DeleteReleaseAsync(releaseId));

        deleted.Should().BeTrue();
        await AssertDatabase(factory, db =>
        {
            db.Releases.Should().NotContain(item => item.Id == releaseId);
            db.Tracks.Should().NotContain(item => item.ReleaseId == releaseId);
            db.ReleaseFormats.Should().NotContain(item => item.ReleaseId == releaseId);
        });
    }

    [Fact]
    public async Task DeleteTrack_FromMiddle_Should_RenumberRemainingTracks()
    {
        var bandId = Guid.NewGuid();
        var releaseId = Guid.NewGuid();
        var removedTrackId = Guid.NewGuid();
        var factory = new CustomWebApplicationFactory();
        using var _ = factory;
        factory.CreateClientWithDb(db =>
        {
            db.Bands.Add(new Band(bandId, "Track Owner", "UK", GenreId, BandStatus.Active, DateTime.UtcNow));
            db.Releases.Add(new Release(releaseId, bandId, "Release", ReleaseType.Album));
            db.Tracks.AddRange(
                new Track(Guid.NewGuid(), releaseId, "One", 1),
                new Track(removedTrackId, releaseId, "Two", 2),
                new Track(Guid.NewGuid(), releaseId, "Three", 4));
        });

        var deleted = await Execute(factory, repository => repository.DeleteTrackAsync(releaseId, removedTrackId));

        deleted.Should().BeTrue();
        await AssertDatabase(factory, db =>
        {
            var tracks = db.Tracks.Where(item => item.ReleaseId == releaseId).OrderBy(item => item.TrackNumber).ToList();
            tracks.Select(item => item.Title).Should().Equal("One", "Three");
            tracks.Select(item => item.TrackNumber).Should().Equal(1, 2);
        });
    }

    private static async Task<bool> Execute(CustomWebApplicationFactory factory, Func<ICatalogDeletionRepository, Task<bool>> action)
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICatalogDeletionRepository>();
        return await action(repository);
    }

    private static async Task AssertDatabase(CustomWebApplicationFactory factory, Action<AppDbContext> assertion)
    {
        using var scope = factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await database.Database.EnsureCreatedAsync();
        assertion(database);
    }
}
