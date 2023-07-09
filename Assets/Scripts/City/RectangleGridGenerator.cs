using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace City
{
    public class RectangleGridGenerator: MonoBehaviour
    {

        public int minWidth = 5;
        public int minHeight = 5;
        public int maxWidth = 20;
        public int maxHeight = 20;

        public int seed = 1;
        
        private List<Rectangle> rectangles;

        public List<Rectangle> Generate(int gridWidth, int gridHeight)
        {
            rectangles = new List<Rectangle>();

            Random.InitState(seed);

            SplitGrid(0, 0, gridWidth, gridHeight);

            // var grid = MakeGrid(gridWidth, gridHeight);
            return rectangles;
            
            // foreach (var rectangle in rectangles)
            // {
            //     Debug.Log(rectangle.X + " " + rectangle.Y + " " + rectangle.Width + " " + rectangle.Height);
            // }
        }
        
        // public int[,] MakeGrid(int gridWidth, int gridHeight)
        // {
        //     var grid = new int[gridWidth, gridHeight];
        //     foreach (var rectangle in rectangles)
        //     {
        //         for (int x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
        //         {
        //             for (int y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
        //             {
        //                 if (x == rectangle.X || x == rectangle.X + rectangle.Width - 1 ||
        //                     y == rectangle.Y || y == rectangle.Y + rectangle.Height - 1)
        //                 {
        //                     grid[x, y] = 0;
        //                 }
        //                 else
        //                 {
        //                     grid[x, y] = 1;
        //                 }
        //             }
        //         }
        //     }
        //
        //     return grid;
        // }
        
        
        void SplitGrid(int x, int y, int width, int height)
        {
            // Check if the rectangle can be split horizontally or vertically without violating the min/max constraints
            // bool canSplitHorizontally = (width >= 2 * minWidth) && (width <= 2 * maxWidth);
            // bool canSplitVertically = (height >= 2 * minHeight) && (height <= 2 * maxHeight);

            bool canSplitHorizontally = (width >= 2 * minWidth);
            bool canSplitVertically = (height >= 2 * minHeight);
            
            // If the rectangle can't be split, add it to the list and return
            if (!canSplitHorizontally && !canSplitVertically)
            {
                rectangles.Add(new Rectangle(x, y, width, height));
                return;
            }

            // Choose a split direction
            bool splitHorizontally;
            if (canSplitHorizontally && canSplitVertically)
            {
                splitHorizontally = (Random.value < 0.5);
            }
            else
            {
                splitHorizontally = canSplitHorizontally;
            }

            // Calculate the split position
            int splitPos;
            if (splitHorizontally)
            {
                splitPos = Random.Range(minWidth, width - minWidth + 1);
            }
            else
            {
                splitPos = Random.Range(minHeight, height - minHeight + 1);
            }

            // Recursively split the two smaller rectangles
            if (splitHorizontally)
            {
                SplitGrid(x, y, splitPos, height);
                SplitGrid(x + splitPos, y, width - splitPos, height);
            }
            else
            {
                SplitGrid(x, y, width, splitPos);
                SplitGrid(x, y + splitPos, width, height - splitPos);
            }
        }

        void FillGridWithRectangles(int x, int y, int width, int height)
        {
            if (width >= minWidth && height >= minHeight)
            {
                int rectWidth = Random.Range(minWidth, Mathf.Min(maxWidth, width + 1));
                int rectHeight = Random.Range(minHeight, Mathf.Min(maxHeight, height + 1));

                rectangles.Add(new Rectangle(x, y, rectWidth, rectHeight));

                // Split the remaining area into subgrids and recursively fill them with rectangles
                FillGridWithRectangles(x + rectWidth, y, width - rectWidth, height); // right subgrid
                FillGridWithRectangles(x, y + rectHeight, rectWidth, height - rectHeight); // top subgrid
            }
        }
    }
    
    public class Rectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

}