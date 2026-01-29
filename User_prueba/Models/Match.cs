using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace User_prueba.Models
{
    [Table("matches")]
    public class Match : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("owner_id")]
        public long OwnerId { get; set; }

        [Column("opponent_id")]
        public long? OpponentId { get; set; }

        [Column("next_player_id")]
        public long NextPlayerId { get; set; }

        [Column("winner_id")]
        public long? WinnerId { get; set; }

        [Column("is_started")]
        public bool IsStarted { get; set; }

        [Column("is_completed")]
        public bool IsCompleted { get; set; }
    }
}
