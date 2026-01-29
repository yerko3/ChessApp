namespace User_prueba.Request
{
    public class IsCheckmate
    {
        public record Request(string MatchName, long PlayerId);
        public record Response(bool IsCheckmate);
    }
}
