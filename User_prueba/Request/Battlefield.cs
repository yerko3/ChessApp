using chessEngine;

namespace User_prueba.Request
{
    public class Battlefield
    {
        public record Figure(
        int x,
        int y,
        ColorTypeFigure Color,
        FigureType Type
        );
        public Figure[] Figures { get; set; } = [];

    }
}
