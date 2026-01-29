
using chessEngine;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Supabase.Gotrue;
using User_prueba.Models;
using static chessEngine.Utils;
using static User_prueba.IRepository;

namespace User_prueba
{
    public class SupabaseService : IRepository
    {
        private readonly Supabase.Client _repository;
        private readonly ICacheService _cache;

        public SupabaseService(IConfiguration configuration, ICacheService cache)
        {
            _cache = cache;
            var supabaseUrl = configuration["Supabase:Url"];
            var supabaseKey = configuration["Supabase:ApiKey"];

            _repository = new Supabase.Client(supabaseUrl, supabaseKey);
            _repository.InitializeAsync().Wait();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var result = await _repository.From<Models.UserRegister>()
                .Where(n => n.Id == id)
                .Get();

            var user = Utils.Utils.ToDto(result.Models.FirstOrDefault());
            if(user == null)
                throw new Exception("Usuario no encontrado.");
            await _cache.SetAsync($"user:{id}", user, TimeSpan.FromMinutes(5));
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var result = await _repository.From<Models.UserRegister>()
                .Where(n => n.Email == email)
                .Get();
            var user = Utils.Utils.ToDto(result.Models.FirstOrDefault());
            if (user == null)
                return null; // o lanzar una excepción si prefieres
            await _cache.SetAsync($"user:{email}", user, TimeSpan.FromMinutes(5));
            return user;
        }
        public async Task InsertUserAsync(User user)
        {
            var ExisteUser = await GetUserByEmailAsync(user.Email);
            if (ExisteUser != null)
                throw new Exception("Ya exite un usuario con ese Gmail."); 
            var NewUser = new Models.UserRegister
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = Utils.Utils.ComputeSha256Hash(user.Password),
                CreatedAt = DateTime.UtcNow,
                Image = user.Image
            };
                var result = await _repository.From<Models.UserRegister>().Insert(NewUser);
                // quizás podrías comprobar `result.Models` si quieres validar algo más
        }

        public async Task<MatchStatus> GetMatchByNameAsync(string matchName)
        {
            string key = $"match:{matchName}";
            if (string.IsNullOrEmpty(matchName))
                throw new ArgumentNullException($"No se encontró la partida con nombre {matchName}");
            var result = await _repository.From<Models.Match>()
                            .Where(m => m.Name == matchName)
                            .Get();

            var match = Utils.Utils.ToDtoMatch(result.Models.FirstOrDefault());
            if (match != null)
                await _cache.SetAsync(key, match, TimeSpan.FromMinutes(5));
            return match;
        }

        public async Task<MatchInfoUser> GetInfoTwoUserOfMatch(long UserId)
        {
            var result = await _repository
                .From<Models.Match>()
                .Where(m => m.IsCompleted == false)
                .Where(m => m.OwnerId == UserId || m.OpponentId == UserId)
                .Where(m => m.OpponentId != null)
                .Get();

            if (result.Models.Count == 0)
                throw new Exception("No se encontró ninguna partida para el usuario.");
            var match = result.Models.FirstOrDefault();
            if (match == null)
                throw new Exception("Partida no encontrada.");
            if (match.OwnerId == null)
                throw new Exception("La partida aún no tiene un propietario asignado.");
            if (match.OpponentId == null)
                throw new Exception("La partida aún no tiene un oponente asignado.");
            var owner = await GetUserByIdAsync((int)match.OwnerId);
            var opponent = await GetUserByIdAsync((int)(match.OpponentId));
            if(owner == null)
                throw new Exception("Propietario no encontrado.");
            if (opponent == null)
                throw new Exception("Oponente no encontrado.");
            
            return new MatchInfoUser
            {
                MatchName = match.Name,
                Owner = owner,
                Opponent = opponent
            };
        }

        public async Task<MatchStatus?> GetMatchByUserIdsAsync(int userId, int opponentId)
        {
            // (Opcional) Validar existencia de usuarios
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("Usuario no encontrado.");

            var opponent = await GetUserByIdAsync(opponentId);
            if (opponent == null)
                throw new Exception("Oponente no encontrado.");

            // Generar clave de caché
            var key = $"{user.Id}-{opponent.Id}";

            // Buscar partida entre ambos
            var result = await _repository.From<Models.Match>()
                .Where(m => (m.OwnerId == user.Id && m.OpponentId == opponent.Id) ||
                 (m.OwnerId == opponentId && m.OpponentId == userId))
                .Get();

            var matchModel = result.Models.FirstOrDefault();
            if (matchModel == null)
                return null; // o lanza excepción, según la lógica de negocio

            var match = Utils.Utils.ToDtoMatch(matchModel);

            // Guardar en caché
            await _cache.SetAsync(key, match, TimeSpan.FromMinutes(5));

            return match;
        }


        public async Task<Match> CreateMatchAsync(string name, long ownerId)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException($"No se encontró la partida con nombre {name}");

            if (ownerId == null)
                throw new ArgumentNullException("Owner nulo");

            // Verificar si el usuario ya tiene una partida activa
            //var existingActiveMatchResponse = await _repository
            //    .From<Models.Match>()
            //    .Where(x => (x.OwnerId == ownerId || x.OpponentId == ownerId) && !x.IsCompleted).Get(); // Obtener partidas activas del usuario

            //var existingActiveMatch = existingActiveMatchResponse.Models.FirstOrDefault();

            //if (existingActiveMatch != null)
            //    throw new InvalidOperationException("Ya tienes una partida activa. No puedes crear otra.");

            var existingMatch = await GetMatchByNameAsync(name);
            if (existingMatch != null && existingMatch.Id > 0)
                throw new ArgumentException($"Ya existe una partida con el nombre {name}");
            var match = new Match
            {
                Name = name,
                OwnerId = ownerId,
                OpponentId = null,
                NextPlayerId = ownerId,
                WinnerId = null,
                IsStarted = false,
                IsCompleted = false
            };
            var insertResult = await _repository.From<Match>().Insert(match);
            var insertedMatch = insertResult.Models.FirstOrDefault();

            if (insertedMatch == null)
                throw new Exception("No se pudo insertar la partida en Supabase.");
            return insertedMatch;
        }


        public async Task StartMatchAsync(string matchName)
        {
            var match = await GetMatchByNameAsync(matchName);
            if (match == null)
                throw new Exception("Partida no encontrada.");

            if (match.OpponentId == null)
                throw new Exception("No puedes iniciar la partida sin oponente.");

            var existing = await GetFiguresByMatchIdAsync(match.Id);
            if (existing.Count > 0)
                throw new Exception("La partida ya fue iniciada.");

            var board = new ChessBoard();
            var figuras = Utils.Utils.ExtractFiguresFromBoard(board, match.Id);
            await _repository.From<Models.FigureModel>().Insert(figuras);
        }



        public async Task<bool> JoinMatchAsync(string matchName, long opponentId)
        {

                var match = await GetMatchByNameAsync(matchName);
                if (match == null)
                    throw new Exception("Partida no encontrada.");

                if (match.OpponentId != null && match.OpponentId != 0)
                    throw new Exception("La partida ya tiene un oponente.");

                if (match.OwnerId == opponentId)
                throw new Exception("El propietario no puede unirse como oponente.");

                match.OpponentId = opponentId;
                await UpdateMatchAsync(match);
                return true;
        }
        public async Task<bool> UpdateMatchAsync(MatchStatus match)
        {

                // Buscar el match actual desde la base de datos
                var result = await _repository.From<Models.Match>()
                    .Where(m => m.Id == match.Id)
                    .Get();

                var matchToUpdate = result.Models.FirstOrDefault();
                if (matchToUpdate == null)
                    throw new Exception("Partida no encontrada en Supabase.");

                // Actualizar campos
                matchToUpdate.NextPlayerId = match.NextPlayerId;
                matchToUpdate.WinnerId = match.WinnerId;
                matchToUpdate.IsCompleted = match.IsCompleted;
                matchToUpdate.IsStarted = match.IsStarted;
                matchToUpdate.OpponentId = match.OpponentId;

                await _repository.From<Models.Match>().Update(matchToUpdate);

                return true;
        }


        public async Task<List<Figure>> GetFiguresByMatchIdAsync(long matchId) // GetAllFifures
        {
            var result = await _repository.From<Models.FigureModel>()
                .Where(f => f.MatchId == matchId)
                .Get();
            var figures = Utils.Utils.ToDtoFigures(result.Models);
            return figures;
        }



        public async Task<bool> DeleteFiguresByMatchIdAsync(long matchId)
        {
            try
            {
                await _repository
                    .From<Models.FigureModel>()
                    .Where(f => f.MatchId == matchId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando figuras: {ex.Message}");
                return false;
            }
        }


        public async Task InsertMoveAsync(Move move)
        {
                if (move == null)
                    throw new ArgumentNullException(nameof(move), "El movimiento no puede ser nulo.");

                await _repository
                    .From<Models.Move>()
                    .Insert(move);
        }

        public async Task<List<Move>> GetMovesByMatchIdAsync(long matchId)
        {
            var result = await _repository
                .From<Models.Move>()
                .Where(m => m.MatchId == matchId)
                .Order(x => x.MovedAt, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();
            if (result.Models == null || result.Models.Count == 0)
                return new List<Move>();
            await _cache.SetAsync($"moves:{matchId}", result.Models, TimeSpan.FromMinutes(5));
            return result.Models;
        }
        public async Task<AllAvailablePositions.Coords[]> GetValidMovesForPiece(string matchName, int x, int y)
        {
            if (string.IsNullOrEmpty(matchName))
                throw new Exception("El nombre de la partida no puede estar vacio o ser nulo");
            var result = await GetMatchByNameAsync(matchName);
            if (result == null)
                throw new Exception("Partida no encontrada.");
            var board = await ReconstructBoard(result.Id);
            if (x < 0 || x >= board.Width || y < 0 || y >= board.Height)
                throw new Exception("Coordenadas fuera del tablero.");
            var figure = board.GetFigureAt(x, y);
            if (figure == null)
                throw new Exception("No hay figura en la posición indicada.");
            var availableMoves = figure.GetAvailablePositions(board);

            var resultCoords = new AllAvailablePositions.Coords[availableMoves.Count];
            for (int i = 0; i < availableMoves.Count; i++)
            {
                resultCoords[i] = new AllAvailablePositions.Coords
                {
                    x = availableMoves[i].X,
                    y = availableMoves[i].Y
                };
            }
            return resultCoords;
        }



        //Obtener el estado de una partida de la bd 
        public async Task<BattleField> GetBattleFieldAsync(string matchName)
        {
            if(string.IsNullOrEmpty(matchName))
                throw new ArgumentNullException($"No se encontró la partida con nombre {matchName}");
            var match = await GetMatchByNameAsync(matchName);
            if (match == null)
                throw new Exception("Partida no encontrada.");
            var figures = await GetFiguresByMatchIdAsync(match.Id);
            var battlefieldFigures = new BattleField.Figure[figures.Count];
            for (int i = 0; i < figures.Count; i++)
            {
                var figure = figures[i];
                battlefieldFigures[i] = new BattleField.Figure(
                    figure.X,
                    figure.Y,
                    (ColorTypeFigure)figure.Color,
                    (FigureType)figure.Type);
            }
            return new BattleField
            {
                Status = new MatchStatus
                {
                    Id = match.Id,
                    Name = match.Name,
                    OwnerId = match.OwnerId,
                    OpponentId = match.OpponentId,
                    NextPlayerId = match.NextPlayerId,
                    WinnerId = match.WinnerId,
                    IsStarted = match.IsStarted,
                    IsCompleted = match.IsCompleted
                },
                Figures = battlefieldFigures
            };
        }
        private async Task RecordMoveAsync(MatchStatus match, IFigure figura, int fromX, int fromY, int toX, int toY)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "El match no puede ser nulo.");
            if (figura == null)
                throw new ArgumentNullException(nameof(figura), "La figura no puede ser nula.");
            await InsertMoveAsync(new Models.Move
            {
                Id = Guid.NewGuid(),
                MatchId = match.Id,
                PlayerId = figura.ColorType == ColorTypeFigure.WHITE ? match.OwnerId : match.OpponentId,
                FromX = fromX,
                FromY = fromY,
                ToX = toX,
                ToY = toY,
                MovedAt = DateTime.UtcNow
            });
        }
        private async Task<ChessBoard> ReconstructBoard(long matchId)
        {
            // 1. Leer modelo BD
            var dbFigures = await _repository
                .From<Models.FigureModel>()
                .Where(f => f.MatchId == matchId)
                .Get();

            // 2. Convertir FigureModel → FigureInit
            List<FigureInit> initFigures = new();
            for (int i = 0; i < dbFigures.Models.Count; i++)
            {
                var m = dbFigures.Models[i];
                initFigures.Add(new FigureInit
                {
                    Type = (FigureType)m.Type,
                    Color = (ColorTypeFigure)m.Color,
                    Position = new Coords(m.X, m.Y)
                });
            }

            // 3. Llenar tablero
            var board = new ChessBoard();
            board.SetFigures(initFigures);
            return board;
        }

        public async Task MovePieceAsync(string matchName, int fromX, int fromY, int toX, int toY)
        {
            var match = await GetMatchByNameAsync(matchName);
            if (match == null) throw new Exception("Partida no encontrada.");

            // 1. Reconstruir tablero
            var board = await ReconstructBoard(match.Id);
            board.SetTurn(match.NextPlayerId == match.OwnerId
                          ? ColorTypeFigure.WHITE
                          : ColorTypeFigure.BLACK);

            // 2. Validar y ejecutar
            var piece = board.GetFigureAt(fromX, fromY);
            if (piece == null) throw new Exception("No hay pieza en origen.");
            if (!board.IsValidMoveFigure(piece, toX, toY))
                throw new Exception("Movimiento no válido.");
            board.MoveFigure(new Coords(fromX, fromY), new Coords(toX, toY));

            // 3. Persistir nuevas figuras
            var updated = board.GetFiguresDescription();            // devuelve List<FigureInit>
            // convertir FigureInit → FigureDesc
            List<FigureDesc> descs = new();
            for (int i = 0; i < updated.Count; i++)
            {
                var f = updated[i];
                descs.Add(new FigureDesc
                {
                    Type = f.Type,
                    Color = f.Color,
                    Position = f.Position
                });
            }
            await SaveFigures(match.Id, descs);

            // 4. Registrar movimiento
            await RecordMoveAsync(match, piece, fromX, fromY, toX, toY);

            // 5. Comprobar jaque mate
            if (board.IsCheckmate())
            {
                match.IsCompleted = true;
                match.WinnerId = match.NextPlayerId == match.OwnerId
                                 ? match.OpponentId
                                 : match.OwnerId;
            }

            // 6. Cambiar turno y actualizar match
            Utils.Utils.SwitchTurn(match);
            await UpdateMatchAsync(match);
        }


        public async Task<bool> IsInCheckAsync(string matchName, long playerId)
        {
            // 1. Obtener partida
            var match = await GetMatchByNameAsync(matchName);
            if (match == null)
                throw new Exception("Partida no encontrada.");
            var board = await ReconstructBoard(match.Id);
            // 2. Obtener color del jugador
            var color = playerId == match.OwnerId
                ? ColorTypeFigure.WHITE
                : ColorTypeFigure.BLACK;
            // 4. Verificar si está en jaque
            return board.IsInCheck2(color);
        }



        public async Task<bool> IsCheckmateAsync(string matchName, long playerId)
        {
            var match = await GetMatchByNameAsync(matchName);
            if (match == null)
                throw new Exception("Partida no encontrada.");
            var board = await ReconstructBoard(match.Id);
            return board.IsCheckmate();
        }
        public async Task RestartMatchAsync(string matchName)
        {
            var match = await GetMatchByNameAsync(matchName);
            if (match == null)
                throw new Exception("Partida no encontrada.");

            if (match.OpponentId == null)
                throw new Exception("No puedes reiniciar la partida sin oponente.");

            // 1. Eliminar figuras actuales y borro sus movientos registrados
            await DeleteFiguresByMatchIdAsync(match.Id);
            await DeleteMovesByMatchIdAsync(match.Id); // ← Agregado


            // 2. Crear nuevas figuras
            var board = new ChessBoard();
            var figuras = Utils.Utils.ExtractFiguresFromBoard(board, match.Id);
            await _repository.From<Models.FigureModel>().Insert(figuras);

            // 3. Reiniciar estado del match
            match.WinnerId = null;
            match.IsCompleted = false;
            match.NextPlayerId = match.OwnerId;

            await UpdateMatchAsync(match);
        }
        private async Task<bool> DeleteMovesByMatchIdAsync(long matchId)
        {
            try
            {
                await _repository
                    .From<Models.Move>()
                    .Where(m => m.MatchId == matchId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando movimientos: {ex.Message}");
                return false;
            }
        }

        public async Task<MatchStatus[]> GetAllMatchesAsync()
        {
            var result = await _repository.From<Models.Match>().Get();
            List<MatchStatus> matches = new();
            foreach (var match in result.Models)
            {
                matches.Add(new MatchStatus
                {
                    Id = match.Id,
                    Name = match.Name,
                    OwnerId = match.OwnerId,
                    OpponentId = match.OpponentId,
                    NextPlayerId = match.NextPlayerId,
                    WinnerId = match.WinnerId,
                    IsStarted = match.IsStarted,
                    IsCompleted = match.IsCompleted
                });
            }
            await _cache.SetAsync($"ListaMatch", matches.ToArray(), TimeSpan.FromMinutes(1));
            return matches.ToArray();
        }

        // Guarda en BD a partir de FigureDesc
        public async Task SaveFigures(long matchId, List<FigureDesc> figures)
        {
            // 1. Borrar viejas
            await DeleteFiguresByMatchIdAsync(matchId);
            // 2. Crear lista BD
            List<Models.FigureModel> toInsert = new();
            for (int i = 0; i < figures.Count; i++)
            {
                var f = figures[i];
                toInsert.Add(new Models.FigureModel
                {
                    MatchId = matchId,
                    Type = (short)f.Type,
                    Color = (short)f.Color,
                    X = f.Position.X,
                    Y = f.Position.Y
                });
            }

            // 3. Insertar nuevas
            await _repository.From<Models.FigureModel>().Insert(toInsert);
        }
    }
}
