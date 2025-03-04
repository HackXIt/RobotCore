using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SnakeMultiplayer.Models;

namespace SnakeMultiplayer.Services
{
    public class GameService : BackgroundService
    {
        public static Game CurrentGame { get; private set; } = new Game(20, 20);
        private readonly object _lock = new object();

        public GameService()
        {
            // CurrentGame is already initialized
        }

        public GameService(Game game)
        {
            CurrentGame = game;
        }

        public GameService(int width, int height)
        {
            CurrentGame = new Game(width, height);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                lock (_lock)
                {
                    // Log a small message each tick
                    Console.WriteLine($"Game Update Tick: #Players={CurrentGame.Players.Count}, Food=({CurrentGame.CurrentFood.Position.X},{CurrentGame.CurrentFood.Position.Y})");
                    CurrentGame.Update();
                }
                await Task.Delay(200, stoppingToken);
            }
        }
    }
}
