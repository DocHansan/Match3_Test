using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    class Grid
    {
        public GridCell[,] grid;
        public void Create_Grid(int Field_size, int Cell_size, int Types_of_cells)
        {
            grid = new GridCell[Field_size + 2, Field_size + 2];
            for (int i = 1; i <= Field_size; i++)
                for (int j = 1; j <= Field_size; j++)
                {
                    grid[i, j].x = j * Cell_size;
                    grid[i, j].y = i * Cell_size;
                    grid[i, j].column = j;
                    grid[i, j].row = i;
                    grid[i, j].kind = new Random().Next(Types_of_cells) + 1;
                    grid[i, j].match = 0;
                    grid[i, j].alpha = 255;
                }
        }
        public GridCell this[int index1, int index2]
        {
            get
            {
                return grid[index1, index2];
            }
            set
            {
                grid[index1, index2] = value;
            }
        }

    }
}
