namespace User_prueba.Request
{
    public class StartMatch
    {
        public record Request(string MatchName);
        public class Response
        {
            public string MatchName { get; set; }
            public string Status { get; set; }
            public DateTime StartTime { get; set; }
        }
    }
}
