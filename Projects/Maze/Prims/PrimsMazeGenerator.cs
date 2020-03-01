﻿using System;

namespace Prims
{
	public class PrimsMazeGenerator
	{
		public static Maze.Tile[,] Generate(
			int rows, int columns,
			int? start_row = null, int? start_column = null,
			int? end_row = null, int? end_column = null)
		{
			start_row ??= 0;
			start_column ??= 0;
			end_row ??= rows - 1;
			end_column ??= columns - 1;

			var grid = new Graph.Node[rows * columns];

			int Index(int row, int col) => row + rows * col;
			
			var random = new Random();

			for (int row = 0; row < rows; row++)
			{
				for (int col = 0; col < columns; col++)
				{
					var n = new Graph.Node(Index(row, col));
					if (row + 1 < rows)
					{
						n.Add(Index(row + 1, col), random.NextDouble());
					}
					if (row - 1 >= 0)
					{
						n.Add(Index(row - 1, col), random.NextDouble());
					}
					if (col + 1 < columns)
					{
						n.Add(Index(row, col + 1), random.NextDouble());
					}
					if (col - 1 >= 0)
					{
						n.Add(Index(row, col - 1), random.NextDouble());
					}
					grid[Index(row, col)] = n;
				}
			}

			var res = PrimsAlgorithm.SimplePrims(new Graph(grid));

			var tiles = new Maze.Tile[rows, columns];

			foreach (var node in res.Nodes)
			{
				(int, int) Unpack(int i) => (i % rows, i / rows);

				var (row, col) = Unpack(node.OwnIndex);

				// directional
				if (node.Connections.Contains(Index(row - 1, col)))
				{
					tiles[row, col] |= Maze.Tile.Up;
					tiles[row - 1, col] |= Maze.Tile.Down;
				}
				if (node.Connections.Contains(Index(row + 1, col)))
				{
					tiles[row, col] |= Maze.Tile.Down;
					tiles[row + 1, col] |= Maze.Tile.Up;
				}
				if (node.Connections.Contains(Index(row, col - 1)))
				{
					tiles[row, col] |= Maze.Tile.Left;
					tiles[row, col - 1] |= Maze.Tile.Right;
				}
				if (node.Connections.Contains(Index(row, col + 1)))
				{
					tiles[row, col] |= Maze.Tile.Right;
					tiles[row, col + 1] |= Maze.Tile.Left;
				}

				// start/end
				if (row == start_row.Value && col == start_column.Value)
				{
					tiles[row, col] |= Maze.Tile.Start;
				}
				if (row == end_row.Value && col == end_column.Value)
				{
					tiles[row, col] |= Maze.Tile.End;
				}
			}
			return tiles;
		}
	}
}