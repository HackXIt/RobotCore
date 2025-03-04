using Microsoft.AspNetCore.Mvc;
using SnakeMultiplayer.Models;
using SnakeMultiplayer.Services;
using System.Collections.Generic;

namespace SnakeMultiplayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]  // This disables antiforgery for all endpoints in this controller.
    public class GameController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Game> GetGameState()
        {
            // Log each time the game state is requested
            Console.WriteLine("GET /api/game called.");
            return Ok(GameService.CurrentGame);
        }

        [HttpPost("player/join")]
        public ActionResult<string> JoinGame([FromBody] string playerId)
        {
            Console.WriteLine($"POST /api/game/player/join called with playerId: {playerId}");

            var game = GameService.CurrentGame;

            // Debug: List existing players
            Console.WriteLine($"Currently {game.Players.Count} players in the game.");

            var availableColors = new List<string> { "Red", "Green", "Blue", "Yellow", "Purple", "Orange" };
            foreach (var p in game.Players)
            {
                availableColors.Remove(p.Snake.Color);
            }
            if (availableColors.Count == 0)
            {
                Console.WriteLine("No available colors left. Rejecting join request.");
                return BadRequest("No available colors for new players.");
            }

            var color = availableColors[0];
            var rand = new System.Random();
            int startX = rand.Next(0, game.Width);
            int startY = rand.Next(0, game.Height);

            var snake = new Snake(new Point(startX, startY), Direction.Right, color);
            var player = new Player(playerId, snake);
            game.Players.Add(player);

            Console.WriteLine($"Player {playerId} joined. Snake color: {color}, Starting Position: ({startX},{startY}).");

            return Ok(playerId);
        }

        [HttpPost("player/{playerId}/move")]
        public IActionResult ChangeDirection(string playerId, [FromBody] Direction newDirection)
        {
            Console.WriteLine($"POST /api/game/player/{playerId}/move called. New direction: {newDirection}");
            var game = GameService.CurrentGame;
            var player = game.Players.Find(p => p.Id == playerId);
            if (player == null)
            {
                Console.WriteLine($"Player {playerId} not found.");
                return NotFound();
            }

            player.Snake.CurrentDirection = newDirection;
            Console.WriteLine($"Player {playerId}'s direction set to {newDirection}.");
            return Ok();
        }
    }
}
