namespace User_prueba.Request
{
    public class MoveFigure
    {
        public record Request(string playerName, int sourceX,int sourceY, int destinationX, int destinationY);

        public record Response(bool Succeded);

    }
}
