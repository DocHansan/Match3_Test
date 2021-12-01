using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    class Grid
    {
        public GridCell[,] grid;

        public Grid(int Field_size, int Cell_size, int Types_of_cells)
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

        public void Swap(int x0, int y0, int x, int y)
        {
            GridCell p1 = grid[x0, y0];
            GridCell p2 = grid[x, y];
            GridCell p3 = p1;
            p1.column = p2.column;
            p2.column = p3.column;
            p1.row = p2.row;
            p2.row = p3.row;

            grid[p1.row, p1.column] = p1;
            grid[p2.row, p2.column] = p2;
        }

        public void Update_grid(bool isNot_moving)
        {
            if (isNot_moving)
            {
                int Field_size = Program.Field_size;
                int Types_of_cells = Program.Types_of_cells;
                int Cell_size = Program.Cell_size;

                for (int i = Field_size; i > 0; i--)
                    for (int j = 1; j <= Field_size; j++)
                        if (grid[i, j].match > 0)
                            for (int n = i; n > 0; n--)
                                if (grid[n, j].match == 0) { Swap(n, j, i, j); break; };

                for (int j = 1; j <= Field_size; j++)
                    for (int i = Field_size, n = 0; i > 0; i--)
                        if (grid[i, j].match > 0)
                        {
                            grid[i, j].kind = new Random().Next(Types_of_cells) + 1;
                            grid[i, j].y = -Cell_size * n++;
                            grid[i, j].match = 0;
                            grid[i, j].alpha = 255;
                        }
            }
        }

        public bool Compare_Elements_Kind(int i, int j, int i1, int j1)
        {
            bool isEqual = false;
            if (grid[i, j].kind == grid[i1, j1].kind)
                isEqual = true;
            return isEqual;
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
