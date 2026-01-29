using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace User_prueba.Models
{
    [Table("figures")]
    public class FigureModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("match_id")]
        public long MatchId { get; set; }

        [Column("type")]
        public short Type { get; set; }

        [Column("color")]
        public short Color { get; set; }

        [Column("x")]
        public int X { get; set; }

        [Column("y")]
        public int Y { get; set; }
    }
}
