using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.City
{
    public struct Cell
    {
        public int x;
        public int y;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class MazeGenerator
    {
        private int[,] maze; // 2D Array
        private List<Vector2> walls = new List<Vector2>();
        private readonly int mazeWidth = 20;
        private readonly int mazeHeight = 20;
        private readonly int seed;

        public MazeGenerator(int mazeWidth, int mazeHeight, int seed)
        {
            this.mazeWidth = mazeWidth;
            this.mazeHeight = mazeHeight;
            this.seed = seed;
        }

        public int[,] generateMaze()
        {
            Random.InitState(seed);
            maze = new int[mazeWidth, mazeHeight];
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    maze[x, y] = 1;
                }
            }

            Cell startCell = new Cell(0, 0);
            maze[startCell.x, startCell.y] = 0;

            List<Cell> wallList = new List<Cell>();
            AddWalls(startCell, wallList);

            while (wallList.Count > 0)
            {
                Cell randomWall = wallList[Random.Range(0, wallList.Count)];
                wallList.Remove(randomWall);

                if (CellIsValid(randomWall))
                {
                    maze[randomWall.x, randomWall.y] = 0;
                    AddWalls(randomWall, wallList);
                }
            }

            return maze;
        }

        private void AddWalls(Cell cell, List<Cell> list) {
            if (cell.x > 0) {
                list.Add(new Cell(cell.x - 1, cell.y));
            }
            if (cell.y > 0) {
                list.Add(new Cell(cell.x, cell.y - 1));
            }
            if (cell.x < mazeWidth - 1) {
                list.Add(new Cell(cell.x + 1, cell.y));
            }
            if (cell.y < mazeHeight - 1) {
                list.Add(new Cell(cell.x, cell.y + 1));
            }
        }

        private bool CellIsValid(Cell cell) {
            int adjacentPathCells = 0;

            if (cell.x > 0) {
                if (maze[cell.x - 1, cell.y] == 0) {
                    adjacentPathCells++;
                }
            }
            if (cell.y > 0) {
                if (maze[cell.x, cell.y - 1] == 0) {
                    adjacentPathCells++;
                }
            }
            if (cell.x < mazeWidth - 1) {
                if (maze[cell.x + 1, cell.y] == 0) {
                    adjacentPathCells++;
                }
            }
            if (cell.y < mazeHeight - 1) {
                if (maze[cell.x, cell.y + 1] == 0) {
                    adjacentPathCells++;
                }
            }

            return adjacentPathCells == 1;
        }
    }
}