using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace User_prueba.Models
{
    [Table("moves")]
    public class Move : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("match_id")]
        public long MatchId { get; set; }

        [Column("player_id")]
        public long? PlayerId { get; set; }

        [Column("from_x")]
        public int FromX { get; set; }

        [Column("from_y")]
        public int FromY { get; set; }

        [Column("to_x")]
        public int ToX { get; set; }

        [Column("to_y")]
        public int ToY { get; set; }

        [Column("moved_at")]
        public DateTime MovedAt { get; set; }
    }
}
