using System;
using System.IO;
using System.Text;

namespace Maze {
	/// <summary>
	/// A 2D maze.
	/// </summary>
	public class Maze {
		internal BitList _northWalls;
		internal BitList _westWalls;
		
		/// <summary>
		/// Create a 2D maze with all the walls set.
		/// </summary>
		/// <param name="width">The width of the maze</param>
		/// <param name="height">The height of the maze</param>
		public Maze(int width, int height) {
			Width = width;
			Height = height;
			
			var total = width * height;
			_northWalls = new BitList(total, true);
			_westWalls = new BitList(total, true);
		}
		
		/// <summary>
		/// Create a copy of another maze.
		/// </summary>
		/// <param name="other">The other maze</param>
		public Maze(Maze other) {
			Width = other.Width;
			Height = other.Height;
			Start = new Cell(this, other.Start.Key);
			End = new Cell(this, other.End.Key);
			_northWalls = new BitList(other._northWalls);
			_westWalls = new BitList(other._westWalls);
		}
		
		private Maze(int width, int height, BitList northWalls, BitList westWalls, long startKey, long endKey) {
			Width = width;
			Height = height;
			_northWalls = northWalls;
			_westWalls = westWalls;
			Start = new Cell(this, startKey);
			End = new Cell(this, endKey);
		}
		
		/// <summary>
		/// The width of the maze.
		/// </summary>
		public int Width { get; private set; }
		/// <summary>
		/// The height of the maze.
		/// </summary>
		public int Height { get; private set; }
		/// <summary>
		/// Get a cell in the maze.
		/// </summary>
		public Cell this[int x, int y] {
			get {
				return new Cell(this, PostionToKey(x, y));
			}
		}
		/// <summary>
		/// The starting cell.
		/// </summary>
		public Cell Start { get; set; }
		/// <summary>
		/// The ending cell.
		/// </summary>
		public Cell End { get; set; }
		
		/// <summary>
		/// Is the (x, y) point within the bounds of the maze?
		/// </summary>
		/// <param name="x">The x position</param>
		/// <param name="y">The y position</param>
		/// <returns>If it is within the bounds</returns>
		public bool InBounds(int x, int y) {
			if (x < 0 || y < 0 || x >= Width || y >= Height) {
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Try to get the key for the (x, y) position.
		/// </summary>
		/// <param name="x">The x position</param>
		/// <param name="y">The y position</param>
		/// <param name="key">The key</param>
		/// <returns>If the position was within the bounds</returns>
		public bool TryPositionToKey(int x, int y, out long key) {
			if (!InBounds(x, y)) {
				key = 0;
				return false;
			}
			key = y * Width + x;
			return true;
		}
		
		/// <summary>
		/// Get the key for the (x, y) position.
		/// </summary>
		/// <param name="x">The x position</param>
		/// <param name="y">The y position</param>
		/// <returns>The key for the position</returns>
		public long PostionToKey(int x, int y) {
			long key;
			if (TryPositionToKey(x, y, out key)) {
				return key;
			} else {
				throw new ArgumentOutOfRangeException();
			}
		}
		
		/// <summary>
		/// Get the (x, y) position for a key.
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="x">The x position</param>
		/// <param name="y">The y position</param>
		public void KeyToPosition(long key, out int x, out int y) {
			var rawY = key / Width;
			x = (int)(key - rawY * Width);
			y = (int)rawY;
		}
		
		/// <summary>
		/// Try to get a key that is relative to the given key by one cell in the given direction.
		/// </summary>
		/// <param name="key">The origin key</param>
		/// <param name="dir">The direction</param>
		/// <param name="newKey">The resulting key</param>
		/// <returns>True if the resulting key is valid, false otherwise</returns>
		public bool TryRelativeKey(long key, Direction dir, out long newKey) {
			switch (dir) {
				case Direction.North:
				case Direction.South:
					newKey = key + (dir == Direction.South ? Width : -Width);
					return newKey >= 0 && newKey < (Width * Height);
				case Direction.East:
				case Direction.West:
					int x, y;
					KeyToPosition(key, out x, out y);
					if (dir == Direction.West) {
						newKey = key - 1;
						return x > 0;
					} else {
						newKey = key + 1;
						return x < (Width - 1);
					}
				default:
					throw new ArgumentException();
			}
		}
		
		/// <summary>
		/// Get the relative offset of two keys.
		/// </summary>
		/// <param name="source">The source key</param>
		/// <param name="destination">The target key</param>
		/// <param name="dir">The direction target is from source</param>
		/// <returns>If the keys are adjacent and so the direction is valid</returns>
		public bool TryGetAdjacentDirection(long source, long destination, out Direction dir) {
			var diff = source - destination;
			if (diff == -Width || diff == Width) {
				dir = diff == -Width ? Direction.South : Direction.North;
				return true;
			}
			if (diff == -1 || diff == 1) {
				int x1, y1, x2, y2;
				KeyToPosition(source, out x1, out y1);
				KeyToPosition(destination, out x2, out y2);
				dir = diff == -1 ? Direction.East : Direction.West;
				return y1 == y2;
			} else {
				dir = default(Direction);
				return false;
			}
		}
		
		/// <summary>
		/// Save the maze to a stream.
		/// </summary>
		/// <param name="stream">The stream</param>
		public void ToStream(Stream stream) {
			using (var writer = new BinaryWriter(stream, Encoding.UTF8, true)) {
				writer.Write(Width);
				writer.Write(Height);
				writer.Write(Start.Key);
				writer.Write(End.Key);
			}
			_northWalls.ToStream(stream);
			_westWalls.ToStream(stream);
		}
		
		/// <summary>
		/// Create a maze from a stream.
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <returns>The maze</returns>
		public static Maze FromStream(Stream stream) {
			int width, height;
			long startKey, endKey;
			BitList northWalls, westWalls;
			using (var reader = new BinaryReader(stream, Encoding.UTF8, true)) {
				width = reader.ReadInt32();
				height = reader.ReadInt32();
				startKey = reader.ReadInt64();
				endKey = reader.ReadInt64();
			}
			northWalls = BitList.FromStream(stream);
			westWalls = BitList.FromStream(stream);
			return new Maze(width, height, northWalls, westWalls, startKey, endKey);
		}
	}
}
