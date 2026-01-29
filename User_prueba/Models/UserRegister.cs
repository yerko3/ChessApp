using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace User_prueba.Models
{
    [Table("users")]
    public class UserRegister : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("username")]
        public string Name { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("image")]
        public string Image { get; set; }
    }
}