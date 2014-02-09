using System;

namespace Maze {
	/// <summary>
	/// Generate a labryinth from a maze.
	/// </summary>
	public static class MazeToLab {
		/// <summary>
		/// Generate a labryinth from a maze.
		/// </summary>
		/// <param name="source">The source maze</param>
		/// <returns>The labryinth</returns>
		public static Maze Generate(Maze source) {
			var maze = new Maze(source.Width * 2, source.Height * 2);
			
			Console.WriteLine("Converting to labyrinth");
			for (var y = 0; y < source.Height; y++) {
				for (var x = 0; x < source.Width; x++) {
					var sc = source[x, y];
					if (!sc.HasWall(Direction.North)) {
						maze[x * 2, y * 2].ClearWall(Direction.North);
						maze[x * 2 + 1, y * 2].ClearWall(Direction.North);
					} else {
						maze[x * 2, y * 2].ClearWall(Direction.East);
					}
					if (!sc.HasWall(Direction.East)) {
						maze[x * 2 + 1, y * 2].ClearWall(Direction.East);
						maze[x * 2 + 1, y * 2 + 1].ClearWall(Direction.East);
					} else {
						maze[x * 2 + 1, y * 2].ClearWall(Direction.South);
					}
					if (!sc.HasWall(Direction.South)) {
						maze[x * 2, y * 2 + 1].ClearWall(Direction.South);
						maze[x * 2 + 1, y * 2 + 1].ClearWall(Direction.South);
					} else {
						maze[x * 2, y * 2 + 1].ClearWall(Direction.East);
					}
					if (!sc.HasWall(Direction.West)) {
						maze[x * 2, y * 2].ClearWall(Direction.West);
						maze[x * 2, y * 2 + 1].ClearWall(Direction.West);
					} else {
						maze[x * 2, y * 2].ClearWall(Direction.South);
					}
				}
			}
			maze[0, 0].SetWall(Direction.East, true);
			maze.Start = maze[0, 0];
			maze.End = maze[1, 0];
			
			return maze;
		}
	}
}
