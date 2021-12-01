using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    class ScoreCounter
    {
        public int score = 0;
        public int Game_score = 0;
        public static int Bonus_score = 0;

        public void CountPoints(Grid Grid_main)
        {
            if (!Animation.isMoving)
            {
                score = 0;
                for (int i = 1; i <= Program.Field_size; i++)
                    for (int j = 1; j <= Program.Field_size; j++)
                        if (Grid_main[i, j].match != 0)
                        {
                            score++;
                            Game_score++;
                            Game_score += Bonus_score;
                            Bonus_score = 0;
                        }
            }
}
    }
}
