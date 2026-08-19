using DiyMusicCommunity.Application.Bands.CatalogManagement;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DiyMusicCommunity.Api.IntegrationTests;

public sealed class BandStatusTransitionIntegrationTests
{
    [Fact]
    public async Task UpdateBand_SplitUpToActive_Should_PersistLastKnownLineupAsPastMembers()
    {
        var genreId = Guid.NewGuid();
        var bandId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var factory = new CustomWebApplicationFactory();
        using var _ = factory;
        factory.CreateClientWithDb(db =>
        {
            db.Genres.Add(new Genre(genreId, "Punk"));
            db.Bands.Add(new Band(bandId, "Discharge", "UK", genreId, BandStatus.SplitUp, DateTime.UtcNow, 1986));
            var member = new BandMember(memberId, bandId, "Bones", false);
            member.Update("Bones", "Bass", 1980, 1986, false, true);
            db.BandMembers.Add(member);
        });

        using (var scope = factory.Services.CreateScope())
        {
            var useCase = scope.ServiceProvider.GetRequiredService<CatalogManagementUseCase>();
            var result = await useCase.UpdateBand(bandId, new BandWriteRequest
            {
                Name = "Discharge",
                Country = "UK",
                GenreId = genreId,
                Status = BandStatus.Active
            });

            Assert.True(result.IsSuccess);
        }

        using var assertionScope = factory.Services.CreateScope();
        var database = assertionScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var savedMember = await database.BandMembers.FindAsync(memberId);
        Assert.NotNull(savedMember);
        Assert.False(savedMember.IsLastKnownLineup);
        Assert.False(savedMember.IsCurrent);
    }
}
