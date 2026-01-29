using chessEngine;

namespace User_prueba.Request
{
    public class GetMatch
    {
        public record Request(string MatchName);
        public class Response
        {
            public record Figure(
            int x,
            int y,
            ColorTypeFigure FigureColor,
            FigureType FigureType
            );
            public Figure[] Figures { get; set; } = [];

        }
    }
}
