using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Maze {
	/// <summary>
	/// Render a maze to a .png file.
	/// </summary>
	public static class RenderMaze {
		/// <summary>
		/// Render a maze to a .png file.
		/// </summary>
		/// <param name="maze">The maze</param>
		/// <param name="scale">The size of the cells in pixels</param>
		/// <param name="filename">The name of the file to produce</param>
		public static void Render(Maze maze, int scale, string filename) {
			Console.WriteLine("Writing file");
			var walls = (scale + 3) / 4;
			var total = scale + walls;
			using (var bmp = new Bitmap(maze.Width * total + walls, maze.Height * total + walls)) {
				using (var gfx = Graphics.FromImage(bmp)) {
					gfx.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);
				
					for (var y = 0; y < maze.Height; y++) {
						for (var x = 0; x < maze.Width; x++) {
							var cell = maze[x, y];
							
							var color = Brushes.White;
							if (cell.IsInactive()) {
								color = Brushes.Black;
							}
							if (maze.Start == cell) {
								color = Brushes.Green;
							}
							if (maze.End == cell) {
								color = Brushes.Red;
							}
							
							gfx.FillRectangle(color, x * total + walls, y * total + walls, scale , scale);
							if (!maze[x, y].HasWall(Direction.North)) {
								gfx.FillRectangle(color, x * total + walls, y * total, scale, walls);
							}
							if (!maze[x, y].HasWall(Direction.West)) {
								gfx.FillRectangle(color, x * total, y * total + walls, walls, scale);
							}
						}
					}
				}
				
				bmp.Save(filename, ImageFormat.Png);
			}
		}
	}
}
