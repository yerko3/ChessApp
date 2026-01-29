using chessEngine;

namespace User_prueba.Request
{
    public class DeleteFiguresRequest
    {
        public record Request(string MatchName);
        public record Response(bool Succeded);

    }
}
