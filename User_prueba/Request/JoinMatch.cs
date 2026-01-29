namespace User_prueba.Request
{
    public class JoinMatch
    {
        public record Request(string MatchName, long OpponentId);
        public record Response(bool Succeded);
    }
}
