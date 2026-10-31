# Mahjong2

A classic Mahjong solitaire game written in C# using Windows Presentation Foundation (WPF).

The objective is to clear the board by matching pairs of tiles. The game includes a hint feature, a counter showing the number of selectable pairs, and a timer that records completed games.

## Features

- **Match pairs of tiles** to clear them from the board.
- **Special tiles:** Flower and bird tiles can be matched with other tiles of the same special category, regardless of their individual design.
- **Hint:** Enable the `Hint` checkbox to highlight all currently available pairs.
- **Selectable pairs counter:** Displays the number of pairs that can currently be selected, for example `5 Selectable pairs`.
- **Game timer:** Timing starts when the first tile is selected.
- **Game results log:** Elapsed times and game results are written to a log file in the same directory as the executable.
- **Reload:** Start a new tile layout at any time.
- **Automatic initial layout:** A new layout is displayed when the game starts.

## Game results

The game records elapsed times and completion results in a log file. Example entries:

```text
0 Selectable pairs. Time used:00:10:30.6138168
0 Selectable pairs. Time used:00:06:32.1618423
The winner, time used:00:11:09.2922789
The winner, time used:00:13:46.4588273
```

The log provides a simple history of game results and completion times.

## Getting started

1. Clone or download this repository.
2. Open `Mahjong2.sln` in Visual Studio.
3. Build the solution.
4. Run the application.

The project was developed using Visual Studio 2019. Check the project settings for the required .NET Framework or .NET version.

## Project structure

- `Mahjong2.sln` — Visual Studio solution.
- `Mahjong2/` — Application source code and project resources.
- `Mahjong2/Images/` — Mahjong tile images.

## License

The source code is provided under the MIT License; see the `LICENSE` file.

Third-party images and other assets may be subject to their own license terms. Please check the relevant asset documentation before reusing them.
