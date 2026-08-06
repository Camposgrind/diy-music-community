using System.Net;
using System.Net.Http.Json;
using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Bands.GetBands; // BandListItemModel
using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Infrastructure.Persistence;
using FluentAssertions;

namespace DiyMusicCommunity.Api.IntegrationTests;

// Each test gets its own factory → fresh in-memory SQLite database
public sealed class BandsControllerTests
{
    // -----------------------------------------------------------------------
    // Seeding helpers
    // -----------------------------------------------------------------------

    private static readonly Guid GrindcoreGenreId = new("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    private static readonly Guid CrustGenreId = new("b2c3d4e5-f6a7-8901-bcde-f12345678901");

    private static Band MakeBand(
        string name,
        string country,
        Guid genreId,
        BandStatus status = BandStatus.Active,
        TrustStatus trust = TrustStatus.CommunityCreated,
        int? formationYear = null)
    {
        var band = new Band(Guid.NewGuid(), name, country, genreId, status, DateTime.UtcNow);
        if (formationYear.HasValue) band.SetFormationYear(formationYear);
        if (trust == TrustStatus.Blocked) band.Block();
        return band;
    }

    private static (CustomWebApplicationFactory Factory, HttpClient Client) CreateClient(
        Action<AppDbContext>? seed = null)
    {
        var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClientWithDb(seed);
        return (factory, client);
    }

    // -----------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GET_Bands_NoFilters_Should_Return200WithPagedResult()
    {
        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(MakeBand("Napalm Death", "UK", GrindcoreGenreId));
            db.Bands.Add(MakeBand("Terrorizer", "USA", GrindcoreGenreId));
        });
        using var _ = factory;

        var response = await client.GetAsync("/api/bands");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body.Should().NotBeNull();
        // Seeded bands + any migration seed data; just assert at least our 2 visible ones
        body!.Items.Count.Should().BeGreaterThanOrEqualTo(2);
        body.Page.Should().Be(1);
        body.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GET_Bands_BlockedBand_Should_NotAppearInResponse()
    {
        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(MakeBand("VisibleBandXYZ", "UK", GrindcoreGenreId));
            db.Bands.Add(MakeBand("BlockedBandXYZ", "UK", GrindcoreGenreId, trust: TrustStatus.Blocked));
        });
        using var _ = factory;

        var response = await client.GetAsync("/api/bands");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body!.Items.Should().NotContain(i => i.Name == "BlockedBandXYZ");
        body.Items.Should().Contain(i => i.Name == "VisibleBandXYZ");
    }

    [Fact]
    public async Task GET_Bands_WithNameFilter_Should_Return200WithFilteredBands()
    {
        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(MakeBand("DischargeFilterTest", "UK", CrustGenreId));
            db.Bands.Add(MakeBand("ExtremeNoiseFilterTest", "UK", CrustGenreId));
        });
        using var _ = factory;

        var response = await client.GetAsync("/api/bands?name=DischargeFilter");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body!.Items.Should().OnlyContain(i => i.Name.Contains("DischargeFilter"));
    }

    [Fact]
    public async Task GET_Bands_WithCountryFilter_Should_ReturnOnlyMatchingCountry()
    {
        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(MakeBand("RepulsionCountryTest", "ZXCVB", GrindcoreGenreId));
            db.Bands.Add(MakeBand("BoltThrowerCountryTest", "QWERTY", GrindcoreGenreId));
        });
        using var _ = factory;

        var response = await client.GetAsync("/api/bands?country=ZXCVB");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body!.Items.Should().OnlyContain(i => i.Country.Equals("ZXCVB"));
    }

    [Fact]
    public async Task GET_Bands_InvalidPageSize_Should_Return400()
    {
        var (factory, client) = CreateClient();
        using var _ = factory;

        var response = await client.GetAsync("/api/bands?pageSize=51");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_Bands_InvalidPage_Should_Return400()
    {
        var (factory, client) = CreateClient();
        using var _ = factory;

        var response = await client.GetAsync("/api/bands?page=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_Bands_ResultsOver100_Should_Return422WithTooManyResultsError()
    {
        var (factory, client) = CreateClient(db =>
        {
            for (var i = 0; i < 101; i++)
            {
                db.Bands.Add(MakeBand($"TooManyBand{i:000}", "UNIQUE_COUNTRY_TM", GrindcoreGenreId));
            }
        });
        using var _ = factory;

        var response = await client.GetAsync("/api/bands?country=UNIQUE_COUNTRY_TM");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var error = await response.Content.ReadFromJsonAsync<Error>();
        error!.Code.Should().Be(BandErrors.Codes.TooManyResults);
    }

    [Fact]
    public async Task GET_Bands_Pagination_Should_ReturnCorrectPageAndSize()
    {
        var (factory, client) = CreateClient(db =>
        {
            for (var i = 0; i < 10; i++)
            {
                db.Bands.Add(MakeBand($"PaginatedBand{i:00}", "PAGTEST", GrindcoreGenreId));
            }
        });
        using var _ = factory;

        var response = await client.GetAsync("/api/bands?country=PAGTEST&page=2&pageSize=3");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body!.Page.Should().Be(2);
        body.PageSize.Should().Be(3);
        body.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task GET_Bands_WithFilterMatchingNoBands_Should_Return200WithEmptyItems()
    {
        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(MakeBand("Terrorizer", "USA", GrindcoreGenreId));
        });
        using var _ = factory;

        var response = await client.GetAsync("/api/bands?name=NONEXISTENT_BAND_ZZZ");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body.Should().NotBeNull();
        body!.Items.Should().BeEmpty();
        body.TotalCount.Should().Be(0);
        body.Page.Should().Be(1);
        body.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GET_Bands_WithStatusFilter_Should_ReturnOnlyMatchingStatus()
    {
        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(MakeBand("ActiveBandStatusTest", "DE", GrindcoreGenreId, BandStatus.Active));
            db.Bands.Add(MakeBand("SplitUpBandStatusTest", "DE", GrindcoreGenreId, BandStatus.SplitUp));
        });
        using var _ = factory;

        var response = await client.GetAsync("/api/bands?country=DE&status=SplitUp");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body!.Items.Should().OnlyContain(i => i.Status == nameof(BandStatus.SplitUp));
        body.Items.Should().NotContain(i => i.Name == "ActiveBandStatusTest");
    }

    [Fact]
    public async Task GET_Bands_WithGenreFilter_Should_ReturnOnlyMatchingGenre()
    {
        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(MakeBand("GrindBandGenreTest", "GENRETEST", GrindcoreGenreId));
            db.Bands.Add(MakeBand("CrustBandGenreTest", "GENRETEST", CrustGenreId));
        });
        using var _ = factory;

        var response = await client.GetAsync($"/api/bands?country=GENRETEST&genreId={GrindcoreGenreId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body!.Items.Should().OnlyContain(i => i.Name == "GrindBandGenreTest");
        body.Items.Should().NotContain(i => i.Name == "CrustBandGenreTest");
    }

    [Fact]
    public async Task GET_Bands_PageBeyondAvailableData_Should_Return200WithEmptyItems()
    {
        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(MakeBand("BeyondPageBand", "BEYONDTEST", GrindcoreGenreId));
        });
        using var _ = factory;

        // Only 1 band exists for this country; page 99 will have no items
        var response = await client.GetAsync("/api/bands?country=BEYONDTEST&page=99&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<BandListItemModel>>();
        body!.Items.Should().BeEmpty();
        body.TotalCount.Should().Be(1);
        body.Page.Should().Be(99);
    }
}
