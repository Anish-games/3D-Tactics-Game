🎮 Grid-Based Pathfinding Game in Unity
This Unity project features a 10x10 tile-based grid system with interactive tiles, custom obstacle placement, a movable player unit, and a simple enemy AI. Built using Unity’s GameObjects and C#, it focuses on core systems like pathfinding, custom editor tools, and object-oriented scripting.

🧱 Grid System
A 10x10 grid is generated using Unity cubes.

Each tile is a separate GameObject with a script storing its position and state.

A raycast from the mouse detects which tile is being hovered over.

The grid position of the hovered tile is shown through a simple UI element.

🚧 Obstacle Tool
A custom Unity Editor Tool was built to toggle obstacles on the grid.

Each tile can be toggled on/off using a 10x10 grid of buttons in the Editor window.

Obstacle data is saved using a ScriptableObject, making it easy to reuse or reset.

An Obstacle Manager reads the data and displays obstacles as red spheres on the blocked tiles.

🧭 Player Movement with Pathfinding
A player unit is spawned on the grid.

Click on a tile to move the player to that location.

The player uses a custom grid-based pathfinding algorithm (A* or similar).

The player avoids blocked tiles and moves smoothly between valid ones.

While the player is moving, input is disabled to avoid interruptions or queueing commands.

🤖 Enemy AI
An enemy unit is added that actively tracks and follows the player.

It uses the same pathfinding logic as the player.

The enemy moves toward one of the four adjacent tiles around the player.

Once it reaches a tile next to the player, it stops and waits for the player to move again.

Implemented using OOP principles, with an IAI interface for flexible AI logic.


![image](https://github.com/user-attachments/assets/a3b470ae-1c7c-4c34-90c3-44a9658ee77e)
