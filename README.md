# Pong

A two-player table-tennis style Pong game made with Unity 6 and C#. This is my first finished game project as I start building a game development portfolio.

Iki kisilik, masa tenisi temali bir Pong oyunu. Unity 6 ve C# ile yaptigim ilk oyun.

## Features

- Two paddles, one ball, table-tennis look
- Ball speeds up with every paddle hit
- Score shown at the top, first player to the win score wins
- Press SPACE to restart after a match
- Win score is adjustable in the Inspector (`Win Score` on the Ball object)

## Controls

| Player | Move up | Move down |
|--------|---------|-----------|
| Left   | W       | S         |
| Right  | Up Arrow | Down Arrow |

SPACE restarts the game after it ends.

## How to run

1. Install Unity Hub and a Unity 6 editor.
2. Clone this repository: `git clone https://github.com/furkanbalpetek/Pong.git`
3. In Unity Hub choose **Add project from disk** and select the cloned folder.
4. Open the scene in `Assets/Scenes` and press Play.

## Code

- `Paddle.cs` moves a paddle with the new Input System and keeps it inside the table.
- `Ball.cs` launches the ball, speeds it up on paddle hits, tracks the score and handles game over and restart.

## Built with

- Unity 6 (2D, Universal Render Pipeline)
- C# in Visual Studio Code
- TextMeshPro for the UI
- Git and GitHub

## Next ideas

- Sound effects
- Single player mode with an AI opponent
- Main menu
