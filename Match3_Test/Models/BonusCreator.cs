using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    static class BonusCreator
    {
        public static void CreateBonuses(Grid Grid_main, int x, int y, int x0, int y0)
        {
            //Create bombs
            BombBonus.CreateBomb(Grid_main);

            //Create line bonuses for click positions
            if (Animation.isSwap)
            {
                LineBonus.FillLineOnClickPosition(Grid_main, y0, x0);
                LineBonus.FillLineOnClickPosition(Grid_main, y, x);
            }

            // Horizontal lines bonus filling
            LineBonus.FillLineBonus(Grid_main, 0, 1, LineBonus.Line_horizontal_type);

            // Vertical line bonus filling
            LineBonus.FillLineBonus(Grid_main, 1, 0, LineBonus.Line_vertical_type);
        }
    }
}
