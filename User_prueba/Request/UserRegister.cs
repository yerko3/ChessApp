namespace User_prueba.Request
{
    public class UserRegister
    {
        public record Request(string Name, string Password);

        public class Response
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
    
}
