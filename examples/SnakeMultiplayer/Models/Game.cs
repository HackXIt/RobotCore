using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace SnakeMultiplayer.Models
{
    public class Game
    {
        public int Width { get; }
        public int Height { get; }
        public List<Player> Players { get; set; } = new List<Player>();
        public Food CurrentFood { get; set; }
        private Random _random = new Random();

        public Game(int width, int height)
        {
            Width = width;
            Height = height;
            CurrentFood = SpawnFood();
        }

        public void Update()
        {
            // Move each snake
            foreach (var player in Players)
            {
                player.Snake.Move(Width, Height);
            }

            // Check for food consumption
            foreach (var player in Players)
            {
                var head = player.Snake.Head;
                if (head.X == CurrentFood.Position.X && head.Y == CurrentFood.Position.Y)
                {
                    player.Snake.Grow();
                    CurrentFood = SpawnFood();
                }
            }

            // Collision detection (e.g., snake vs. snake) can be added here.
        }

        private Food SpawnFood()
        {
            int x = _random.Next(0, Width);
            int y = _random.Next(0, Height);
            return new Food { Position = new Point(x, y) };
        }
    }
}
