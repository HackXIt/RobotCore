console.log("game.js loaded");

window.setFocus = (elementId) => {
    console.log("setFocus called with elementId:", elementId);
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
};

window.drawGame = (canvasId, game) => {
    console.log("drawGame called with canvasId:", canvasId, "and game:", game);
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext("2d");
    const gridSize = 20;
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Draw the food (displayed in red)
    if (game.currentFood) {
        ctx.fillStyle = "red";
        ctx.fillRect(game.currentFood.position.x * gridSize, game.currentFood.position.y * gridSize, gridSize, gridSize);
    }

    // Draw each player's snake using its unique color
    if (game.players) {
        game.players.forEach(player => {
            ctx.fillStyle = player.snake.color.toLowerCase();
            player.snake.body.forEach(segment => {
                ctx.fillRect(segment.x * gridSize, segment.y * gridSize, gridSize, gridSize);
            });
        });
    }
};
