# Pong

A table-tennis themed Pong game made with Unity 6 and C#. Play it in your browser, no install needed.

**[▶ Play on itch.io](https://furkanbalpetek.itch.io/pong)**

Masa tenisi temali bir Pong oyunu. Unity 6 ve C# ile yaptigim ilk oyun. Tarayicidan oynayabilirsin.

## Features

- Single player against a bot with **10 difficulty levels**, a tick marks every level you have beaten (saved between sessions)
- Two player mode on one keyboard
- Table-tennis look: table, net, red and yellow paddles, ball trail
- Ball angle depends on where it hits the paddle, and it speeds up with every hit
- Scoreboard, end screen with PLAY AGAIN / MENU buttons
- Hit sparks, goal bursts, screen shake and win confetti
- Sound effects generated in code, no audio files
- First to 5 points wins

## Controls

| Player | Move up | Move down |
|--------|---------|-----------|
| Left   | W       | S         |
| Right  | Up Arrow | Down Arrow |

SPACE plays again after a match, ESC returns to the menu.

## How to run

Easiest: play it on [itch.io](https://furkanbalpetek.itch.io/pong).

To open the project:

1. Install Unity Hub and a Unity 6 editor.
2. Clone this repository: `git clone https://github.com/furkanbalpetek/Pong.git`
3. In Unity Hub choose **Add project from disk** and select the cloned folder.
4. Open the scene in `Assets/Scenes` and press Play.

## Code

- `Paddle.cs` moves a paddle (player input or AI) and keeps it inside the table.
- `Ball.cs` launches the ball, handles bounces, scoring and game over.
- `MenuManager.cs` builds the main menu and level select with code.
- `GameUI.cs` builds the scoreboard and end screen and keeps the playfield 16:9.
- `Juice.cs` adds particles, screen shake and confetti.
- `TableDecor.cs` draws the table and net. `SoundMaker.cs` generates sound effects. `GameSettings.cs` stores the mode and beaten levels.

## Built with

- Unity 6 (2D, Universal Render Pipeline), WebGL build
- C# in Visual Studio Code
- TextMeshPro for the UI
- Git and GitHub, itch.io for hosting

## Next ideas

- Custom arcade font
- Pause menu and serve countdown
- Next project: Herber
