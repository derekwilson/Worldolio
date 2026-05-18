using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public class SchemaRevisionAudit
    {
        [Column(Name = "sra_description")]
        public required string Description { get; set; }

        [Column(Name = "sra_timestamp")]
        public required DateTime Timestamp { get; set; }
    }
}
