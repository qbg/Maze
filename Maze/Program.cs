using System;
using System.Collections.Generic;
using System.IO;

namespace Maze{
	class Program {
		private static IDictionary<string, Action> Commands;
		private static Stack<string> stack;
		private static Maze maze;
		
		static Program() {
			Commands = new Dictionary<string, Action>();
			Commands["seed"] = Seed;
			Commands["lab"] = Lab;
			Commands["gen"] = Gen;
			Commands["trim"] = Trim;
			Commands["render"] = Render;
			Commands["save"] = Save;
			Commands["load"] = Load;
		}
		
		public static void Main(string[] args) {
			stack = new Stack<string>();
			maze = null;
			
			var start = Environment.TickCount;
			foreach (var token in args) {
				Action action;
				if (Commands.TryGetValue(token, out action)) {
					action();
				} else {
					stack.Push(token);
				}
			}
			Console.WriteLine("Time: " + (Environment.TickCount - start) + " ms");
		}
		
		private static void Seed() {
			maze = MazeToLab.Generate(new Maze(1, 1));
		}
		
		private static void Lab() {
			maze = MazeToLab.Generate(maze);
		}
		
		private static void Gen() {
			var factor = int.Parse(stack.Pop());
			maze = MazeExpander.Generate(maze, factor);
		}
		
		private static void Trim() {
			maze = MazeTrimmer.Trim(maze);
		}
		
		private static void Render() {
			var scale = int.Parse(stack.Pop());
			var filename = stack.Pop();
			RenderMaze.Render(maze, scale, filename);
		}
		
		private static void Save() {
			var filename = stack.Pop();
			using (var stream = File.OpenWrite(filename)) {
				maze.ToStream(stream);
			}
		}
		
		private static void Load() {
			var filename = stack.Pop();
			using (var stream = File.OpenRead(filename)) {
				maze = Maze.FromStream(stream);
			}
		}
	}
}
