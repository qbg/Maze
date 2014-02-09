using System;
using System.IO;

namespace Maze {
	/// <summary>
	/// Various extension methods
	/// </summary>
	public static class Utils {
		/// <summary>
		/// All of the directions.
		/// </summary>
		public static Direction[] Dirs = new Direction[] { Direction.North, Direction.East, Direction.South, Direction.West };
		/// <summary>
		/// The North and West directions.
		/// </summary>
		public static Direction[] ULDirs = new Direction[] { Direction.North, Direction.West };
		
		/// <summary>
		/// Get the opposite direction.
		/// </summary>
		/// <param name="dir">The direction</param>
		/// <returns>The opposite direction</returns>
		public static Direction Opposite(this Direction dir) {
			switch (dir) {
				case Direction.North:
					return Direction.South;
				case Direction.East:
					return Direction.West;
				case Direction.South:
					return Direction.North;
				case Direction.West:
					return Direction.East;
				default:
					throw new ArgumentException();
			}
		}
	}
}
