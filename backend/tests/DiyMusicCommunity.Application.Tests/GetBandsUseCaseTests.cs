using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Bands.GetBands;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Moq;

namespace DiyMusicCommunity.Application.Tests;

public class GetBandsUseCaseTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static Band MakeBand(
        string name = "Discharge",
        string country = "UK",
        BandStatus status = BandStatus.Active,
        TrustStatus trust = TrustStatus.CommunityCreated,
        Guid? genreId = null,
        int? formationYear = null)
    {
        var band = new Band(
            Guid.NewGuid(),
            name,
            country,
            genreId ?? Guid.NewGuid(),
            status,
            DateTime.UtcNow);

        if (formationYear.HasValue)
            band.SetFormationYear(formationYear);

        if (trust == TrustStatus.Blocked)
            band.Block();

        return band;
    }

    private static Genre MakeGenre(string name = "Grindcore")
        => new(Guid.NewGuid(), name);

    private static (GetBandsUseCase UseCase, Mock<IBandRepository> Repo) BuildSut()
    {
        var repo = new Mock<IBandRepository>();
        var validator = new GetBandsQueryValidator();
        var useCase = new GetBandsUseCase(repo.Object, validator);
        return (useCase, repo);
    }

    private static void SetupRepo(
        Mock<IBandRepository> repo,
        IReadOnlyList<Band> items,
        int? totalCount = null)
    {
        repo.Setup(r => r.SearchAsync(It.IsAny<BandSearchFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, totalCount ?? items.Count));
    }

    // -----------------------------------------------------------------------
    // Validation failures
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBands_WithPageLessThan1_Should_ReturnValidationFailure()
    {
        var (sut, _) = BuildSut();
        var query = new GetBandsQuery { Page = 0 };

        var result = await sut.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.InvalidFilter, result.Error!.Code);
    }

    [Fact]
    public async Task GetBands_WithPageSizeGreaterThan50_Should_ReturnValidationFailure()
    {
        var (sut, _) = BuildSut();
        var query = new GetBandsQuery { PageSize = 51 };

        var result = await sut.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.InvalidFilter, result.Error!.Code);
    }

    [Fact]
    public async Task GetBands_WithNameExceeding200Chars_Should_ReturnValidationFailure()
    {
        var (sut, _) = BuildSut();
        var query = new GetBandsQuery { Name = new string('x', 201) };

        var result = await sut.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.InvalidFilter, result.Error!.Code);
    }

    [Fact]
    public async Task GetBands_WithCountryExceeding100Chars_Should_ReturnValidationFailure()
    {
        var (sut, _) = BuildSut();
        var query = new GetBandsQuery { Country = new string('x', 101) };

        var result = await sut.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.InvalidFilter, result.Error!.Code);
    }

    // -----------------------------------------------------------------------
    // Too-many-results cap
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBands_WhenResultsExceed100_Should_ReturnTooManyResultsFailure()
    {
        var (sut, repo) = BuildSut();
        SetupRepo(repo, Array.Empty<Band>(), totalCount: 101);

        var result = await sut.Handle(new GetBandsQuery());

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.TooManyResults, result.Error!.Code);
    }

    [Fact]
    public async Task GetBands_WhenResultsAreExactly100_Should_Succeed()
    {
        var (sut, repo) = BuildSut();
        SetupRepo(repo, Array.Empty<Band>(), totalCount: 100);

        var result = await sut.Handle(new GetBandsQuery());

        Assert.True(result.IsSuccess);
    }

    // -----------------------------------------------------------------------
    // Filter pass-through
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBands_WithNoFilters_Should_ReturnPagedListExcludingBlocked()
    {
        var (sut, repo) = BuildSut();
        var bands = new List<Band> { MakeBand("Napalm Death", "UK") };
        SetupRepo(repo, bands);

        var result = await sut.Handle(new GetBandsQuery());

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Napalm Death", result.Value.Items[0].Name);
    }

    [Fact]
    public async Task GetBands_WithNameFilter_Should_PassNameFilterToRepository()
    {
        var (sut, repo) = BuildSut();
        SetupRepo(repo, new List<Band> { MakeBand("Discharge") });

        await sut.Handle(new GetBandsQuery { Name = "Discharge" });

        repo.Verify(r => r.SearchAsync(
            It.Is<BandSearchFilter>(f => f.Name == "Discharge"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBands_WithCountryFilter_Should_PassCountryFilterToRepository()
    {
        var (sut, repo) = BuildSut();
        SetupRepo(repo, new List<Band> { MakeBand(country: "UK") });

        await sut.Handle(new GetBandsQuery { Country = "UK" });

        repo.Verify(r => r.SearchAsync(
            It.Is<BandSearchFilter>(f => f.Country == "UK"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBands_WithGenreFilter_Should_PassGenreIdFilterToRepository()
    {
        var (sut, repo) = BuildSut();
        var genreId = Guid.NewGuid();
        SetupRepo(repo, new List<Band> { MakeBand(genreId: genreId) });

        await sut.Handle(new GetBandsQuery { GenreId = genreId });

        repo.Verify(r => r.SearchAsync(
            It.Is<BandSearchFilter>(f => f.GenreId == genreId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBands_WithCombinedFilters_Should_PassAllFiltersToRepository()
    {
        var (sut, repo) = BuildSut();
        var genreId = Guid.NewGuid();
        SetupRepo(repo, new List<Band> { MakeBand(country: "UK", genreId: genreId) });

        await sut.Handle(new GetBandsQuery { Name = "dis", Country = "UK", GenreId = genreId });

        repo.Verify(r => r.SearchAsync(
            It.Is<BandSearchFilter>(f =>
                f.Name == "dis" && f.Country == "UK" && f.GenreId == genreId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // -----------------------------------------------------------------------
    // DTO mapping
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBands_Should_MapBandFieldsToModel()
    {
        var (sut, repo) = BuildSut();
        var genreId = Guid.NewGuid();
        var band = MakeBand("Terrorizer", "USA", BandStatus.SplitUp, genreId: genreId, formationYear: 1986);

        // Simulate Genre navigation property being set by EF (use reflection)
        var genre = MakeGenre("Death-Grind");
        typeof(Band)
            .GetProperty("Genre")!
            .SetValue(band, genre);

        SetupRepo(repo, new List<Band> { band });

        var result = await sut.Handle(new GetBandsQuery());

        Assert.True(result.IsSuccess);
        var model = result.Value!.Items[0];
        Assert.Equal("Terrorizer", model.Name);
        Assert.Equal("USA", model.Country);
        Assert.Equal("Death-Grind", model.Genre);
        Assert.Equal("SplitUp", model.Status);
        Assert.Equal(1986, model.FormationYear);
    }

    // -----------------------------------------------------------------------
    // Pagination
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBands_WithValidPagination_Should_ReturnCorrectPageMetadata()
    {
        var (sut, repo) = BuildSut();
        var bands = Enumerable.Range(0, 5).Select(i => MakeBand($"Band {i}")).ToList();
        SetupRepo(repo, bands, totalCount: 5);

        var result = await sut.Handle(new GetBandsQuery { Page = 2, PageSize = 5 });

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Page);
        Assert.Equal(5, result.Value.PageSize);
        Assert.Equal(5, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetBands_WithBlockedBands_Should_ExcludeBlockedFromCount()
    {
        // Repository already filters out Blocked — use case honours totalCount returned by repo
        var (sut, repo) = BuildSut();
        var nonBlocked = new List<Band> { MakeBand("Extreme Noise Terror") };
        SetupRepo(repo, nonBlocked, totalCount: 1);

        var result = await sut.Handle(new GetBandsQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.TotalCount);
    }

    // -----------------------------------------------------------------------
    // Empty results
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBands_WhenNoMatchingBands_Should_Return200WithEmptyItems()
    {
        var (sut, repo) = BuildSut();
        SetupRepo(repo, Array.Empty<Band>(), totalCount: 0);

        var result = await sut.Handle(new GetBandsQuery { Name = "NonExistentBandXYZ" });

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
        Assert.Equal(0, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetBands_WhenNoMatchingBands_Should_StillReturnCorrectPaginationMetadata()
    {
        var (sut, repo) = BuildSut();
        SetupRepo(repo, Array.Empty<Band>(), totalCount: 0);

        var result = await sut.Handle(new GetBandsQuery { Page = 2, PageSize = 10 });

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
        Assert.Equal(2, result.Value.Page);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(0, result.Value.TotalCount);
    }

    [Fact]
    public async Task GetBands_WhenPageBeyondAvailableData_Should_ReturnEmptyItemsWithCorrectMetadata()
    {
        var (sut, repo) = BuildSut();
        // 5 total bands exist, but page 3 with pageSize 3 yields no items
        SetupRepo(repo, Array.Empty<Band>(), totalCount: 5);

        var result = await sut.Handle(new GetBandsQuery { Page = 3, PageSize = 3 });

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
        Assert.Equal(3, result.Value.Page);
        Assert.Equal(3, result.Value.PageSize);
        Assert.Equal(5, result.Value.TotalCount);
    }

    // -----------------------------------------------------------------------
    // Status filter
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBands_WithStatusFilter_Should_PassStatusFilterToRepository()
    {
        var (sut, repo) = BuildSut();
        SetupRepo(repo, new List<Band> { MakeBand(status: BandStatus.SplitUp) });

        await sut.Handle(new GetBandsQuery { Status = BandStatus.SplitUp });

        repo.Verify(r => r.SearchAsync(
            It.Is<BandSearchFilter>(f => f.Status == BandStatus.SplitUp),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBands_WhenStatusFilterMatchesNoBands_Should_ReturnEmptyItems()
    {
        var (sut, repo) = BuildSut();
        SetupRepo(repo, Array.Empty<Band>(), totalCount: 0);

        var result = await sut.Handle(new GetBandsQuery { Status = BandStatus.OnHold });

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
        Assert.Equal(0, result.Value.TotalCount);
    }

    // -----------------------------------------------------------------------
    // Null genre mapping
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetBands_WhenGenreNavigationPropertyIsNull_Should_MapGenreToEmptyString()
    {
        var (sut, repo) = BuildSut();
        // Genre nav property deliberately left null (not included by EF)
        var band = MakeBand("Repulsion", "USA");
        SetupRepo(repo, new List<Band> { band });

        var result = await sut.Handle(new GetBandsQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Value!.Items[0].Genre);
    }
}
