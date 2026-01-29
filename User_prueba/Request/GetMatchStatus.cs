namespace User_prueba.Request
{
    public class GetMatchStatus
    {
        public record Request(string MatchName);

        public class Response
        {
            public string Name { get; set; } = string.Empty;
            public long? ownerId { get; set; }
            public long? OpponentId { get; set; }
            public long NextPlayerId { get; set; } 
            public long? WinnerId { get; set; }
            public bool IsStarted { get; set; } = false;
            public bool IsCompleted { get; set; } = false;
            public bool IsInCheck { get; set; } = false;

        }
    }
}
