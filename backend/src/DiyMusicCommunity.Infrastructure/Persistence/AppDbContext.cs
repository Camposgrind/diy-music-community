using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiyMusicCommunity.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Band> Bands => Set<Band>();
    public DbSet<BandClaim> BandClaims => Set<BandClaim>();
    public DbSet<BandMember> BandMembers => Set<BandMember>();
    public DbSet<BandMemberOtherBand> BandMemberOtherBands => Set<BandMemberOtherBand>();
    public DbSet<BandProposal> BandProposals => Set<BandProposal>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<ModerationAction> ModerationActions => Set<ModerationAction>();
    public DbSet<Release> Releases => Set<Release>();
    public DbSet<ReleaseFormat> ReleaseFormats => Set<ReleaseFormat>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
