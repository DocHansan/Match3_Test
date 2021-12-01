using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    static class Animation
    {

        private const int Moving_animation_speed = 6;
        private const byte Deleting_animation_speed = 10;

        public static bool isMoving = false;
        public static bool isSwap = false;

        public static void DeleteCells(Grid Grid_main)
        {
            if (!isMoving)
                for (int i = 1; i <= Program.Field_size; i++)
                    for (int j = 1; j <= Program.Field_size; j++)
                        if (Grid_main[i, j].match > 0)
                            if (Grid_main[i, j].alpha > Deleting_animation_speed)
                            {
                                Grid_main.grid[i, j].alpha -= Deleting_animation_speed; isMoving = true;
                            }
        }

        public static void MoveCells(Grid Grid_main)
        {
            isMoving = false;
            for (int i = 1; i <= Program.Field_size; i++)
                for (int j = 1; j <= Program.Field_size; j++)
                {
                    ref GridCell p = ref Grid_main.grid[i, j];
                    int dx = 0,
                        dy = 0;
                    for (int n = 0; n < Moving_animation_speed; n++)
                    {
                        dx = p.x - p.column * Program.Cell_size;
                        dy = p.y - p.row * Program.Cell_size;
                        if (dx != 0) p.x -= dx / Math.Abs(dx);
                        if (dy != 0) p.y -= dy / Math.Abs(dy);
                    }
                    if (dx != 0 || dy != 0) isMoving = true;
                }
        }
}
}
