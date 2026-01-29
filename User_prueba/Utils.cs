using System.Security.Cryptography;
using System.Text;
using chessEngine;
using User_prueba.Models;

namespace User_prueba.Utils
{
    public static class Utils
    {
        public static string ComputeSha256Hash(string password)
        {
            // Verificar si la contraseña es nula o vacía
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("La contraseña no puede ser nula o vacía");
            }

            // Crear una instancia del algoritmo SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertir la contraseña a un array de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertir el array de bytes a string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // "x2" formatea cada byte como 2 dígitos hexadecimales
                }

                return builder.ToString();
            }

        }
        public static User ToDto(Models.UserRegister userRegister)
        {
            if (userRegister == null) return null;

            return new User
            {
                Id = userRegister.Id,
                Name = userRegister.Name,
                Email = userRegister.Email,
                Password = userRegister.Password,
                CreatedAt = userRegister.CreatedAt,
                Image = userRegister.Image
            };
        }

        public static MatchStatus ToDtoMatch(Models.Match match)
        {
            if (match == null) return null;

            return new MatchStatus
            {
                Id = match.Id,
                Name = match.Name,
                OwnerId = match.OwnerId,
                OpponentId = match.OpponentId,
                NextPlayerId = match.NextPlayerId,
                WinnerId = match.WinnerId,
                IsStarted = match.IsStarted,
                IsCompleted = match.IsCompleted
            };
        }
        public static List<Figure> ToDtoFigures(List<Models.FigureModel> models)
        {
            List<Figure> result = new List<Figure>();
            for (int i = 0; i < models.Count; i++)
            {
                var m = models[i];
                result.Add(new Figure
                {
                    Id = m.Id,
                    MatchId = m.MatchId,
                    Type = m.Type,
                    Color = m.Color,
                    X = m.X,
                    Y = m.Y
                });
            }
            return result;
        }

        public static List<Models.FigureModel> ExtractFiguresFromBoard(ChessBoard board, long matchId)
        {
            if (board == null)
                throw new Exception("Board nulo");
            List<Models.FigureModel> result = new();

            board.Visit(casilla =>
            {
                if (casilla.Figure != null)
                {
                    result.Add(new Models.FigureModel
                    {
                        Id = Guid.NewGuid(),
                        MatchId = matchId,
                        Type = (short)casilla.Figure.GetFigureType(),
                        Color = (short)casilla.Figure.ColorType,
                        X = casilla.Figure.Position.X,
                        Y = casilla.Figure.Position.Y
                    });
                }
            });
            return result;
        }
        public static void SwitchTurn(MatchStatus match)
        {
            if (match.OpponentId == null)
                throw new Exception("No hay oponente para cambiar el turno.");

            match.NextPlayerId = (match.NextPlayerId == match.OwnerId)
                ? match.OpponentId.Value
                : match.OwnerId;
        }

        public static List<Figure> ToFigureModels(long matchId, List<FigureDesc> descriptions)
        {
            List<Figure> result = new List<Figure>();
            for (int i = 0; i < descriptions.Count; i   ++)
            {
                var d = descriptions[i];
                result.Add(new Figure
                {
                    MatchId = matchId,
                    Type = (short)d.Type,
                    Color = (short)d.Color,
                    X = d.Position.X,
                    Y = d.Position.Y
                });
            }
            return result;
        }

        public static List<FigureDesc> ToDtoFiguresDescription(List<FigureFactory> updatedFigures)
        {
            throw new NotImplementedException();
        }
    }
}
