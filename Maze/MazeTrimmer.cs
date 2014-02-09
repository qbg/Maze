using System;
using System.Collections.Generic;

namespace Maze {
	/// <summary>
	/// Find the solution for a perfect maze.
	/// </summary>
	public static class MazeTrimmer {
		/// <summary>
		/// Find the solution for a perfect maze.
		/// </summary>
		/// <param name="source">The perfect maze</param>
		/// <returns>The solution maze</returns>
		public static Maze Trim(Maze source) {
			var maze = new Maze(source);
			var stack = new Stack<Cell>();
			
			Console.WriteLine("Trimming maze");
			// Phase 1: Queue all non-special tails
			for (var y = 0; y < maze.Height; y++) {
				for (var x = 0; x < maze.Width; x++) {
					var cell = maze[x, y];
					if (!IsSpecial(cell) && IsTail(cell)) {
						stack.Push(cell);
					}
				}
			}
			
			// Phase 2: Trim the tails
			while (stack.Count != 0) {
				var cell = stack.Pop();
				Direction dir;
				if (TryGetTailDir(cell, out dir)) {
					cell.SetWall(dir, true);
					var other = cell.Move(dir);
					if (IsTail(other) && !IsSpecial(other)) {
						stack.Push(other);
					}
				}
			}
			
			return maze;
		}
		
		private static bool TryGetTailDir(Cell cell, out Direction tailDir) {
			var missing = 0;
			tailDir = default(Direction);
			foreach (var dir in Utils.Dirs) {
				if (!cell.HasWall(dir)) {
					missing++;
					tailDir = dir;
				}
			}
			return missing == 1;
		}
		
		private static bool IsTail(Cell cell) {
			Direction tmp;
			return TryGetTailDir(cell, out tmp);
		}
		
		private static bool IsSpecial(Cell cell) {
			return cell.Maze.Start == cell || cell.Maze.End == cell;
		}
	}
}
