using System;

namespace Maze {
	/// <summary>
	/// A cell in a maze.
	/// </summary>
	public struct Cell {
		private readonly Maze _maze;
		private readonly long _key;
		
		/// <summary>
		/// Create a cell corresponding to the given key in a given maze.
		/// </summary>
		/// <param name="maze">The maze</param>
		/// <param name="key">The key</param>
		public Cell(Maze maze, long key) {
			_maze = maze;
			_key = key;
		}
		
		/// <summary>
		/// Get the key.
		/// </summary>
		public long Key {
			get {
				return _key;
			}
		}
		
		/// <summary>
		/// Get the maze.
		/// </summary>
		public Maze Maze {
			get {
				return _maze;
			}
		}
		
		/// <summary>
		/// Get the position of the cell.
		/// </summary>
		/// <param name="x">The x position</param>
		/// <param name="y">The y position</param>
		public void GetPosition(out int x, out int y) {
			_maze.KeyToPosition(_key, out x, out y);
		}
		
		/// <summary>
		/// Is this cell inactive, i.e., has walls on all four sides?
		/// </summary>
		/// <returns>If the cell is inactive</returns>
		public bool IsInactive() {
			foreach (var dir in Utils.Dirs) {
				if (!HasWall(dir)) {
					return false;
				}
			}
			return true;
		}
		
		/// <summary>
		/// Is there a wall in the given direction?
		/// </summary>
		/// <param name="dir">The direction</param>
		/// <returns>If the cell has a wall on that side</returns>
		public bool HasWall(Direction dir) {
			long sKey;
			if (!_maze.TryRelativeKey(_key, dir, out sKey)) {
				return true;
			}
			switch (dir) {
				case Direction.North:
					return _maze._northWalls[_key];
				case Direction.West:
					return _maze._westWalls[_key];
				case Direction.South:
					return _maze._northWalls[sKey];
				case Direction.East:
					return _maze._westWalls[sKey];
				default:
					throw new ArgumentException();
			}
		}
		
		/// <summary>
		/// Set the state of the wall in the given direction.
		/// </summary>
		/// <param name="dir">The direction</param>
		/// <param name="state">The state of the wall</param>
		public void SetWall(Direction dir, bool state) {
			long sKey;
			if (!_maze.TryRelativeKey(_key, dir, out sKey)) {
				throw new ArgumentException("Fixed wall in this direction");
			}
			switch (dir) {
				case Direction.North:
					_maze._northWalls[_key] = state;
					break;
				case Direction.West:
					_maze._westWalls[_key] = state;
					break;
				case Direction.South:
					_maze._northWalls[sKey] = state;
					break;
				case Direction.East:
					_maze._westWalls[sKey] = state;
					break;
				default:
					throw new ArgumentException();
			}
		}
		
		/// <summary>
		/// Clear the wall in the given direction.
		/// </summary>
		/// <param name="dir">The direction</param>
		public void ClearWall(Direction dir) {
			SetWall(dir, false);
		}
		
		/// <summary>
		/// Try to move the cell one unit in the given direction.
		/// </summary>
		/// <param name="dir">The direction</param>
		/// <param name="newCell">The moved cell</param>
		/// <returns>If the move was successful</returns>
		public bool TryMove(Direction dir, out Cell newCell) {
			long sKey;
			if (_maze.TryRelativeKey(_key, dir, out sKey)) {
				newCell = new Cell(_maze, sKey);
				return true;
			} else {
				newCell = default(Cell);
				return false;
			}
		}
		
		/// <summary>
		/// Move the cell in the given direction.
		/// </summary>
		/// <param name="dir">The direction</param>
		/// <returns>The new cell</returns>
		public Cell Move(Direction dir) {
			Cell res;
			if (!TryMove(dir, out res)) {
				throw new ArgumentException();
			}
			return res;
		}
		
		/// <summary>
		/// Try to determine the direction an adjacent cell is relative to us.
		/// </summary>
		/// <param name="other">The other cell</param>
		/// <param name="dir">The direction</param>
		/// <returns>If the cell was really adjacent and so the direction is valid</returns>
		public bool TryGetAdjacentDirection(Cell other, out Direction dir) {
			if (_maze != other._maze) {
				throw new ArgumentException();
			}
			return _maze.TryGetAdjacentDirection(_key, other._key, out dir);
		}
		
		/// <summary>
		/// Set the state of the wall separating this cell and the other cell.
		/// </summary>
		/// <param name="other">The other cell</param>
		/// <param name="state">The state of the wall</param>
		public void SetMutualWall(Cell other, bool state) {
			Direction dir;
			if (TryGetAdjacentDirection(other, out dir)) {
				SetWall(dir, state);
			} else {
				throw new ArgumentException();
			}
		}
		
		/// <summary>
		/// Cell the wall separating this cell and the other cell.
		/// </summary>
		/// <param name="other">The other cell</param>
		public void ClearMutualWall(Cell other) {
			SetMutualWall(other, false);
		}
		
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				if (_maze != null)
					hashCode += 1000000007 * _maze.GetHashCode();
				hashCode += 1000000009 * _key.GetHashCode();
			}
			return hashCode;
		}
		
		public override bool Equals(object obj) {
			return (obj is Cell) && Equals((Cell)obj);
		}

		public bool Equals(Cell other) {
			return object.Equals(this._maze, other._maze) && this._key == other._key;
		}
		
		public static bool operator ==(Cell lhs, Cell rhs) {
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(Cell lhs, Cell rhs) {
			return !(lhs == rhs);
		}
	}
}
