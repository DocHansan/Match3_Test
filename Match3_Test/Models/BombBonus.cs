using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    static class BombBonus
    {
        public const int Bomb_radius = 1;
        public const int Bomb_type = Program.Types_of_cells + 1;

        public static void ActivateBomb(Grid Grid_main, int x, int y)
        {
            Grid_main.grid[x, y].isNeed_bonus_activation = false;
            for (int i = x - Bomb_radius; i <= x + Bomb_radius; i++)
                for (int j = y - Bomb_radius; j <= y + Bomb_radius; j++)
                {
                    if (Grid_main.grid[i, j].match == 0)
                        Grid_main.grid[i, j].match++;
                    if (Grid_main.grid[i, j].kind > Program.Types_of_cells)
                    {
                        BonusActivator.ActivateBonus(Grid_main, i, j);
                    }
                }
        }

        public static void CreateBomb(Grid Grid_main)
        {
            for (int i = 1; i <= Program.Field_size; i++)
                for (int j = 1; j <= Program.Field_size; j++)
                {
                    if (Grid_main[i, j].match >= 3)
                    {
                        Grid_main.grid[i, j].match = 0;
                        Grid_main.grid[i, j].isNeed_bonus_activation = true;
                        Program.Bonus_score++;
                        Grid_main.grid[i, j].kind = BombBonus.Bomb_type;
                    }
                }
        }
    }
}
