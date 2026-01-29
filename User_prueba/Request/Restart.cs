namespace User_prueba.Request
{
    public class Restart
    {
        public record Request(string MatchName);
        public record Response(bool Succeded);
    }
}
