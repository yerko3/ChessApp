namespace User_prueba.Request
{
    public class AvailablePositions
    {
        public record Coords(
        int X, 
        int Y);

        public class Response
        {
            public Coords[] AvailableCoords { get; set; } = [];
        }
        public record Request(string PlayerName, int x, int y);
    }
}
