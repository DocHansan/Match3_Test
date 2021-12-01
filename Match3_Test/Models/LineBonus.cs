using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    static class LineBonus
    {
        public const int Line_horizontal_type = BombBonus.Bomb_type + 1;
        public const int Line_vertical_type = Line_horizontal_type + 1;

        public static void ActivateLine(Grid Grid_main, int x, int y)
        {
            Grid_main.grid[x, y].isNeed_bonus_activation = false;
            switch (Grid_main.grid[x, y].kind)
            {
                case Line_horizontal_type:
                    {
                        DestroyLine(Grid_main, x, y, 0, 1, y, Program.Field_size + 1);
                        DestroyLine(Grid_main, x, y, 0, -1, y, 0);
                        break;
                    }
                case Line_vertical_type:
                    {
                        DestroyLine(Grid_main, x, y, 1, 0, x, Program.Field_size + 1);
                        DestroyLine(Grid_main, x, y, -1, 0, x, 0);
                        break;
                    }
            }
        }

        public static void DestroyLine(Grid Grid_main, int x, int y, int x1, int y1, int start_index, int len)
        {
            for (int i = start_index; i != len; i += x1 + y1)
            {
                int x_new = (x1 == 0 ? x : i);
                int y_new = (y1 == 0 ? y : i);
                if (Grid_main.grid[x_new, y_new].match == 0)
                    Grid_main.grid[x_new, y_new].match++;
                if (Grid_main.grid[x_new, y_new].kind > Program.Types_of_cells)
                {
                    BonusActivator.ActivateBonus(Grid_main, x_new, y_new);
                }
            }
        }

        public static void CreateLine(Grid Grid_main, int i, int j, int line_bonus_type)
        {
            Grid_main.grid[i, j].match = 0;
            Grid_main.grid[i, j].alpha = 0;
            Grid_main.grid[i, j].isNeed_bonus_activation = true;
            ScoreCounter.Bonus_score++;
            Grid_main.grid[i, j].kind = line_bonus_type;
        }

        public static void FillLineOnClickPosition(Grid Main_grid, int x, int y)
        {
            CreateLineFromTwoCell(Main_grid, x, y, x - 1, y, LineBonus.Line_vertical_type);
            CreateLineFromTwoCell(Main_grid, x, y, x + 1, y, LineBonus.Line_vertical_type);
            CreateLineFromTwoCell(Main_grid, x, y, x, y - 1, LineBonus.Line_horizontal_type);
            CreateLineFromTwoCell(Main_grid, x, y, x, y + 1, LineBonus.Line_horizontal_type);
        }

        public static void FillLineBonus(Grid Main_grid, int i1, int j1, int line_bonus_type)
        {
            for (int i = 1; i <= Program.Field_size; i++)
                for (int j = 1; j <= Program.Field_size; j++)
                {
                    CreateLineFromTwoCell(Main_grid, i, j, i + i1, j + j1, line_bonus_type);
                }
        }

        static void CreateLineFromTwoCell(Grid Main_grid, int x, int y, int x1, int y1, int line_bonus_type)
        {
            if (Main_grid.CompareElementsKind(x, y, x1, y1))
                if (Main_grid.grid[x, y].match == 2 && Main_grid.grid[x1, y1].match == 2)
                    LineBonus.CreateLine(Main_grid, x, y, line_bonus_type);
        }
    }
}
