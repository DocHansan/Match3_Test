using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    static class BonusActivator
    {
        public static bool isNeedBonusCheck = false;

        public static void ActivateBonusesAfterClick(Grid Grid_main, int x0, int y0, int x, int y)
        {
            if (isNeedBonusCheck && !Animation.isMoving && Animation.isSwap)
            {
                ActivateBonus(Grid_main, y0, x0);
                ActivateBonus(Grid_main, y, x);
                isNeedBonusCheck = false;
            }
        }

        public static void ActivateBonus(Grid Grid_main, int x, int y)
        {
            if (!Grid_main.grid[x, y].isNeed_bonus_activation)
                return;
            int Cell_type = Grid_main.grid[x, y].kind;
            if (Cell_type == BombBonus.Bomb_type)
                BombBonus.ActivateBomb(Grid_main, x, y);

            else if (Cell_type == LineBonus.Line_horizontal_type || Cell_type == LineBonus.Line_vertical_type)
                LineBonus.ActivateLine(Grid_main, x, y);
        }
    }
}
