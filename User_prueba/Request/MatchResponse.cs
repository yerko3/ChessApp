namespace User_prueba.Request
{
    public class CreateMatch
    {
        public record Request(string MatchName, long ownerId);
        public class MatchResponse
        {
            public string Name { get; set; } = string.Empty;
            public long ownerId { get; set; }
            public long? OpponentId { get; set; }
            public long NextPlayerId { get; set; }
            public long? WinnerId { get; set; }
            public bool IsStarted { get; set; } = false;
            public bool IsCompleted { get; set; } = false;
        }
    }
}
