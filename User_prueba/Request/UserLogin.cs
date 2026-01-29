namespace User_prueba
{
    public class UserLogin
    {
        public record Request(string NickName);
        public record Response(string NickName, bool Succeded);
    }
}
