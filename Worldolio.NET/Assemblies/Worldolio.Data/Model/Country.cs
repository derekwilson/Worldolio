using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public class Country
    {
        [Column(Name = "cnt_iso2name")]
        public required string Iso2Name { get; set; }

        [Column(Name = "cnt_iso3name")]
        public required string Iso3Name { get; set; }

        [Column(Name = "cnt_displayname")]
        public required string DisplayName { get; set; }

        [Column(Name = "cnt_peoplename")]
        public required string PeopleName { get; set; }

        [Column(Name = "cnt_isonumber")]
        public long IsoNumber { get; set; }

        [Column(Name = "cnt_intdialcode")]
        public string? InternationalDialCode { get; set; }

        [Column(Name = "cnt_intaccesscode")]
        public string? InternationalAccessCode { get; set; }

        [Column(Name = "cnt_areacodeprefix")]
        public string? AreaCodePrefix { get; set; }

        public DriveSide DriveSide { get; set; } = default!;

        public ICollection<City> Cities { get; set; } = [];
    }
}
