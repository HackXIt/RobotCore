namespace SnakeMultiplayer.Models
{
    public class Player
    {
        public string Id { get; set; }
        public Snake Snake { get; set; }
        
        public Player(string id, Snake snake)
        {
            Id = id;
            Snake = snake;
        }
    }
}
