    namespace chessEngine
{
    public class Casilla
    {
        private ColorTypeSquare _color = ColorTypeSquare.NONE;
        private IFigure _figure = null;
        private FigureType _figureType = FigureType.NONE;
        private Coords _coords;
        private bool EstaEnCasilla = false;

        public ColorTypeSquare ColorSquare
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        public IFigure Figure
        {
            get
            {
                return _figure;
            }
        }
        public Coords Coords
        {
            get
            {
                return _coords;
            }
            set
            {
                _coords = value;
            }
        }
        public bool IsOcupada
        {
            get
            {
                return EstaEnCasilla;
            }
            set
            {
                EstaEnCasilla = value;
            }
        }
        internal void OcuparCasilla(IFigure figure)
        {
            if (figure == null)
                return;
            EstaEnCasilla = true;
            if (figure is Figure f)
            {
                f.Position = _coords;
                _figure = f;
                _figureType = figure.GetFigureType();

            }
        }
        public void LiberarCasilla()
        {
            _figure = null;
            EstaEnCasilla = false;
            _figureType = FigureType.NONE;
        }
    }
}
