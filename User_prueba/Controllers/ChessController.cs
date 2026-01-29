using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace User_prueba.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChessController : ControllerBase
    {
        private IRepository _database;
        private readonly ILogger<UserController> _logger; // es un sistema de logging de ASP.NET Core

        public ChessController(IRepository database, ILogger<UserController> logger)
        {
            _database = database;
            _logger = logger; //Esto es de C#

        }
        [HttpPost("status")]
        public async Task<IActionResult> GetMatchStatus(Request.GetMatchStatus.Request request)
        {
            try
            {
                var status = await _database.GetMatchByNameAsync(request.MatchName);
                if (status == null)
                    return NotFound($"No se encontró ninguna partida con el nombre '{request.MatchName}'.");
                var response = new Request.GetMatchStatus.Response();
                response.Name = status.Name;
                response.ownerId = status.OwnerId;
                response.OpponentId = status.OpponentId;
                response.NextPlayerId = status.NextPlayerId;
                response.WinnerId = status.WinnerId;
                response.IsStarted = status.IsStarted;
                response.IsCompleted = status.IsCompleted;
                response.WinnerId = status.WinnerId;
                response.IsInCheck = await _database.IsInCheckAsync(request.MatchName, response.NextPlayerId);

                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el estado de la partida.");
                return BadRequest(new { message = "No se pudieron obtener informacion de las partidas. " + ex.Message });
            }
        }


        [HttpGet("getMatchInfo/{matchName}")]
        public async Task<IActionResult> GetMatchInfo(string matchName)
        {
            try
            {
                var match = await _database.GetMatchByNameAsync(matchName);
                if (match == null)
                    return NotFound(new { error = "Partida no encontrada." });

                var owner = await _database.GetUserByIdAsync((int)match.OwnerId);
                var opponent = match.OpponentId != null
                    ? await _database.GetUserByIdAsync((int)match.OpponentId)
                    : null;

                return Ok(new
                {
                    owner,
                    opponent
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error interno al obtener la información de la partida.",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost("createMatch")]
        public async Task<IActionResult> CreateMatch([FromBody] Request.CreateMatch.Request request)
        {
            try
            {
                var match = await _database.CreateMatchAsync(request.MatchName, request.ownerId);
                var response = new Request.CreateMatch.MatchResponse
                {
                    Name = match.Name,
                    ownerId = match.OwnerId,
                    OpponentId = match.OpponentId,
                    NextPlayerId = match.NextPlayerId,
                    WinnerId = match.WinnerId,
                    IsStarted = match.IsStarted,
                    IsCompleted = match.IsCompleted,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando la partida.");
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("startMatch")]
        public async Task<IActionResult> StartMatch([FromBody] Request.StartMatch.Request request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.MatchName))
                    return BadRequest("El nombre de la partida no puede estar vacío.");

                await _database.StartMatchAsync(request.MatchName);
                return Ok("Partida iniciada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error iniciando la partida.");
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("match")]
        public async Task<IActionResult> GetMatch([FromBody] Request.GetMatch.Request request)
        {
            try
            {
                var battlefield = await _database.GetBattleFieldAsync(request.MatchName);
                var figuresArray = new Request.GetMatch.Response.Figure[battlefield.Figures.Length];
                for (int i = 0; i < battlefield.Figures.Length; i++)
                {
                    var f = battlefield.Figures[i];
                    figuresArray[i] = new Request.GetMatch.Response.Figure(f.x, f.y, f.Color, f.Type);
                }

                var response = new Request.GetMatch.Response
                {
                    Figures = figuresArray
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el estado de la partida.");
                return StatusCode(500, new { message = ex.Message });
            }   
        }
        [HttpPost("deleteFigures")]
        public async Task<IActionResult> DeleteFigures([FromBody] Request.DeleteFiguresRequest.Request request)
        {
            try
            {
                var match = await _database.GetMatchByNameAsync(request.MatchName);
                if (match == null)
                    return NotFound(new { message = "Partida no encontrada." });

                var ok = await _database.DeleteFiguresByMatchIdAsync(match.Id);
                if (!ok)
                    return StatusCode(500, new { message = "Error al eliminar figuras." });

                return Ok(new { message = "Figuras eliminadas correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando figuras.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("move")]
        public async Task<IActionResult> MovePiece([FromBody] Request.MoveFigure.Request request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.playerName)) // <-- ojo con el nombre
                    return BadRequest("El nombre de la partida es obligatorio.");

                await _database.MovePieceAsync(
                    request.playerName, // ← matchName realmente
                    request.sourceX,
                    request.sourceY,
                    request.destinationX,
                    request.destinationY
                );

                return Ok(new Request.MoveFigure.Response(true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mover la pieza.");
                return BadRequest(new { succeded = false, error = ex.Message });
            }
        }

        [HttpPost("joinMatch")]
        public async Task<IActionResult> JoinMatch([FromBody] Request.JoinMatch.Request request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MatchName))
                    return BadRequest("MatchName es obligatorio.");

                var ok = await _database.JoinMatchAsync(request.MatchName, request.OpponentId);
                if (!ok)
                    return BadRequest(new { message = "No se pudo unir a la partida." });

                return Ok(new { message = "Jugador unido como oponente correctamente."});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en joinMatch");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("avpos")]
        public async Task<IActionResult> GetAvailblePosition([FromBody]  Request.AvailablePositions.Request request)
        {
            try
            {
                var response = new Request.AvailablePositions.Response();
                var positions = await _database.GetValidMovesForPiece(request.PlayerName, request.x, request.y);
                List<Request.AvailablePositions.Coords> list = new List<Request.AvailablePositions.Coords>();
                foreach (var coords in positions.ToArray())
                {
                    list.Add(new Request.AvailablePositions.Coords(coords.x, coords.y));
                }
                response.AvailableCoords = list.ToArray();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el estado de la partida.");
                return BadRequest(new Request.AvailablePositions.Response());
            }
        }
        [HttpPost("isCheckmate")]
        public async Task<IActionResult> IsCheckmate([FromBody] Request.IsCheckmate.Request request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MatchName))
                    return BadRequest("MatchName es obligatorio.");

                if (request.PlayerId <= 0)
                    return BadRequest("PlayerId inválido.");

                bool result = await _database.IsCheckmateAsync(request.MatchName, request.PlayerId);

                return Ok(new Request.IsCheckmate.Response(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar jaque mate.");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("moves/{matchName}")]
        public async Task<IActionResult> GetMoves(string matchName)
        {
            try
            {
                var match = await _database.GetMatchByNameAsync(matchName);
                if (match == null)
                    return NotFound(new { message = "Partida no encontrada." });

                var moves = await _database.GetMovesByMatchIdAsync(match.Id);
                return Ok(moves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo movimientos.");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("restart")]
        public async Task<IActionResult> Restart([FromBody] Request.Restart.Request request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.MatchName))
                    return BadRequest("MatchName es obligatorio.");
                await _database.RestartMatchAsync(request.MatchName);
                return Ok(new Request.Restart.Response(true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reiniciando la partida.");
                return BadRequest(new { succeded = false, error = ex.Message });
            }
        }
        [HttpGet("matches")]
        public async Task<IActionResult> GetAllMatches()
        {
            try
            {
                var matches = await _database.GetAllMatchesAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todas las partidas.");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("TwoUsers")]
        public async Task<IActionResult> GetTwoUsers()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var userId = JwtService.GetUserIdFromClaims(identity);
                if (userId == null)
                    return BadRequest("No se pudo obtener el ID del usuario desde los claims.");
                var TwoUsers = await _database.GetInfoTwoUserOfMatch(userId.Value);
                if (TwoUsers == null || TwoUsers.Owner == null || TwoUsers.Opponent == null)
                    return NotFound("No se encontraron dos usuarios en la partida.");
                var response = new
                {
                    You = new
                    {
                        MatchName = TwoUsers.MatchName,
                        Id = TwoUsers.Owner.Id,
                        Name = TwoUsers.Owner.Name,
                        Email = TwoUsers.Owner.Email,
                        Image = TwoUsers.Owner.Image
                    },
                    Opponent = new
                    {
                        MatchName = TwoUsers.MatchName,
                        Id = TwoUsers.Opponent.Id,
                        Name = TwoUsers.Opponent.Name,
                        Email = TwoUsers.Opponent.Email,
                        Image = TwoUsers.Opponent.Image
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo dos usuarios.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
