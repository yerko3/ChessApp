namespace User_prueba.Request
{
    public class LoginUser
    {
        public record Request(string Email, string Password);
        public class Response
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
