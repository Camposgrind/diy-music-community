using System.ComponentModel;

namespace DiyMusicCommunity.Domain.Enums;

public enum Format
{
    [Description("7\" Vinyl")]
    Vinyl7,

    [Description("10\" Vinyl")]
    Vinyl10,

    [Description("12\" Vinyl")]
    Vinyl12,

    [Description("Vinyl Lathe Cut")]
    VinylLatheCut,

    [Description("Vinyl (Other)")]
    VinylOther,

    [Description("CD")]
    CD,

    [Description("CD-R")]
    CDR,

    [Description("DVD")]
    DVD,

    [Description("Cassette")]
    Cassette,

    [Description("Digital")]
    Digital
}
