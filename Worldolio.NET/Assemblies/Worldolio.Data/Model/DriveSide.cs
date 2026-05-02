using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public class DriveSide
    {
        [Column(Name = "dsi_id")]
        public long Id { get; set; }

        [Column(Name = "dsi_description")]
        public required string Description { get; set; }
    }

}
