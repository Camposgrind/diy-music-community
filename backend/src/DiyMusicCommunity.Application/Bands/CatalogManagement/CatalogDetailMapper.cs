using DiyMusicCommunity.Application.Bands.GetBandDetail;
using DiyMusicCommunity.Application.Releases.GetReleaseDetail;
using DiyMusicCommunity.Domain.Entities;

namespace DiyMusicCommunity.Application.Bands.CatalogManagement;

internal static class CatalogDetailMapper
{
    public static BandDetailModel ToBandDetail(Band band)
    {
        return new BandDetailModel
        {
            Id = band.Id,
            Name = band.Name,
            Country = band.Country,
            Location = band.Location,
            Status = band.Status,
            Genre = band.Genre?.Name,
            FormationYear = band.FormationYear,
            SplitUpYear = band.SplitUpYear,
            Description = band.Description,
            LogoImageUrl = null,
            BandImageUrl = null,
            MusicUrlPortal = band.MusicUrlPortal,
            BandContact = band.BandContact,
            Releases = band.Releases
                .OrderBy(release => release.Year.HasValue ? 0 : 1)
                .ThenBy(release => release.Year)
                .ThenBy(release => release.ReleaseDate.HasValue ? 0 : 1)
                .ThenBy(release => release.ReleaseDate)
                .ThenBy(release => release.Title, StringComparer.OrdinalIgnoreCase)
                .Select(ToBandRelease)
                .ToList()
                .AsReadOnly(),
            Members = band.Members.Select(ToMember).ToList().AsReadOnly()
        };
    }

    public static BandMemberModel ToMember(BandMember member)
    {
        return new BandMemberModel
        {
            Id = member.Id,
            Name = member.Name,
            Instrument = member.Instrument,
            StartYear = member.StartYear,
            EndYear = member.EndYear,
            IsCurrent = member.IsCurrent,
            IsLastKnownLineup = member.IsLastKnownLineup,
            OtherBands = member.OtherBands.Select(otherBand => new BandMemberOtherBandModel
            {
                BandId = otherBand.OtherBandId,
                BandName = otherBand.OtherBand?.Name
            }).ToList().AsReadOnly()
        };
    }

    public static ReleaseDetailModel ToReleaseDetail(Release release)
    {
        return new ReleaseDetailModel
        {
            Id = release.Id,
            Title = release.Title,
            ReleaseType = release.ReleaseType,
            ReleaseDate = release.ReleaseDate,
            Year = release.Year,
            LabelText = release.LabelText,
            CoverImageUrl = null,
            Band = release.Band is null ? null : new ReleaseBandModel { BandId = release.Band.Id, Name = release.Band.Name },
            Formats = release.Formats.Select(format => format.Format).ToList().AsReadOnly(),
            Tracks = release.Tracks.OrderBy(track => track.TrackNumber).Select(track => new ReleaseTrackModel
            {
                ReleaseId = track.ReleaseId,
                Title = track.Title,
                TrackNumber = track.TrackNumber
            }).ToList().AsReadOnly()
        };
    }

    private static BandReleaseModel ToBandRelease(Release release)
    {
        return new BandReleaseModel { Id = release.Id, Title = release.Title, ReleaseType = release.ReleaseType, Year = release.Year };
    }
}
