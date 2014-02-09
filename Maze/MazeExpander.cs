using System;
using System.Collections.Generic;

namespace Maze {
	/// <summary>
	/// Generate a perfect maze from another perfect maze.
	/// </summary>
	public class MazeExpander {
		private Maze _source;
		private int _factor;
		
		private MazeExpander(Maze source, int factor) {
			_source = source;
			_factor = factor;
		}
		
		/// <summary>
		/// Generate a perfect maze from another perfect maze.
		/// </summary>
		/// <param name="source">The input maze</param>
		/// <param name="factor">The scaling factor</param>
		/// <returns>The generated maze</returns>
		public static Maze Generate(Maze source, int factor) {
			return new MazeExpander(source, factor).Generate();
		}
		
		private Maze Generate() {
			var maze = new Maze(_source.Width * _factor, _source.Height * _factor);
			var gens = new List<DFSGenerator>();
			
			// Spawn the generators
			var offset = _factor / 2;
			var rnd = new Random();
			var mapping = new int[maze.Width * maze.Height];
			for (var y = 0; y < _source.Height; y++) {
				for (var x = 0; x < _source.Width; x++) {
					var cx = x * _factor + offset;
					var cy = y * _factor + offset;
					var x1 = (x - 1) * _factor + offset;
					var y1 = (y - 1) * _factor + offset;
					var x2 = (x + 1) * _factor + offset;
					var y2 = (y + 1) * _factor + offset;
					gens.Add(new DFSGenerator(gens.Count, maze, mapping, rnd, maze[cx, cy], x1, y1, x2, y2));
				}
			}
			
			// Generate!
			Console.WriteLine("Generating");
			while (true) {
				var hasMore = false;
				foreach (var gen in gens) {
					hasMore |= gen.Generate();
				}
				if (!hasMore) {
					break;
				}
			}
			
			// Connect...sometime later
			Console.WriteLine("Welding");
			for (var y = 0; y < _source.Height; y++) {
				for (var x = 0; x < _source.Width; x++) {
					var sourceCell = _source[x, y];
					
					foreach (var dir in Utils.ULDirs) {
						Cell otherCell;
						if (!sourceCell.HasWall(dir) && sourceCell.TryMove(dir, out otherCell)) {
							var id2 = (int)otherCell.Key;
							gens[(int)sourceCell.Key].Weld(id2);
						}
					}
				}
			}
			
			// Find the start and exit
			Console.WriteLine("Finding start/end");
			maze.Start = gens[(int)_source.Start.Key].FindStartExit();
			maze.End = gens[(int)_source.End.Key].FindStartExit();
			
			return maze;
		}
		
		private class DFSGenerator {
			private Random _rnd;
			private Maze _maze;
			private Stack<long> _stack;
			private readonly int _id;
			private int[] _mapping;
			public int _x1, _y1, _x2, _y2, _cx, _cy;
			
			public DFSGenerator(int id, Maze maze, int[] mapping, Random rnd, Cell cell, int x1, int y1, int x2, int y2) {
				_id = id;
				_rnd = rnd;
				_maze = maze;
				_mapping = mapping;
				_stack = new Stack<long>();
				_stack.Push(cell.Key);
				cell.GetPosition(out _cx, out _cy);
				_mapping[(int)cell.Key] = _id;
				
				_x1 = x1;
				_y1 = y1;
				_x2 = x2;
				_y2 = y2;
			}
			
			public bool Generate() {
				while (true) {
					if (_stack.Count == 0) {
						return false;
					}
					var cell = new Cell(_maze, _stack.Peek());
					
					var offset = _rnd.Next(4);
					for (var i = 0; i < 4; i++) {
						Cell tmp;
						if (cell.TryMove(Utils.Dirs[(offset + i) % 4], out tmp)) {
							if (InBounds(tmp) && tmp.IsInactive()) {
								cell.ClearMutualWall(tmp);
								_mapping[(int)tmp.Key] = _id;
								_stack.Push(tmp.Key);
								return true;
							}
						}
					}
					_stack.Pop();
				}
			}
			
			private bool InBounds(Cell cell) {
				int x, y;
				cell.GetPosition(out x, out y);
				return _x1 < x && x < _x2 && _y1 < y && y < _y2;
			}
			
			public void Weld(int id2) {
				var possible = new List<Tuple<Cell, Cell>>();
				for (var y = _y1 + 1; y < _y2; y++) {
					for (var x = _x1 + 1; x < _x2; x++) {
						if (!_maze.InBounds(x, y)) {
							continue;
						}
						
						var c1 = _maze[x, y];
						foreach (var dir in Utils.Dirs) {
							Cell c2;
							if (c1.TryMove(dir, out c2)) {
								var c1Id = _mapping[(int)c1.Key];
								var c2Id = _mapping[(int)c2.Key];
								if ((c1Id == _id && c2Id == id2)) {
									possible.Add(new Tuple<Cell, Cell>(c1, c2));
								}
							}
						}
					}
				}
				if (possible.Count == 0) {
					throw new InvalidOperationException("Cannot weld " + _id + " to " + id2);
				} else {
					var tuple = possible[_rnd.Next(possible.Count)];
					tuple.Item1.ClearMutualWall(tuple.Item2);
					return;
				}
			}
			
			public Cell FindStartExit() {
				for (var y = _y1 + 1; y < _y2; y++) {
					for (var x = _x1 + 1; x < _x2; x++) {
						if (!_maze.InBounds(x, y)) {
							continue;
						}
						var cell = _maze[x, y];
						if (_mapping[(int)cell.Key] == _id) {
							return cell;
						}
					}
				}
				throw new InvalidOperationException("Could not find suitable point");
			}
		}
	}
}
