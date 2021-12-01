using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    class TextInterfaceStorage
    {
        public uint Font_size = 24;
        public uint Font_size_inscription = 16;

        Font Outfit_light_font = new Font("./Content/Fonts/Outfit-Light.ttf");

        public Text Time_remaining;

        public Text Text_time_remaining;

        public Text Game_score_text;

        public Text Text_score;

        public TextInterfaceStorage()
        {
            Time_remaining = new Text("", Outfit_light_font, Font_size);
            Time_remaining.FillColor = new Color(Color.Black);
            Time_remaining.Position = new Vector2f(Program.Field_size * Program.Cell_size, Program.Cell_size - 1.5f * Font_size);

            Text_time_remaining = new Text("Time remaining", Outfit_light_font, Font_size_inscription);
            Text_time_remaining.FillColor = new Color(Color.Black);
            Text_time_remaining.Position = new Vector2f(Program.Field_size * Program.Cell_size, Program.Cell_size - 1.5f * Font_size - Font_size_inscription);

            Game_score_text = new Text("", Outfit_light_font, Font_size);
            Game_score_text.FillColor = new Color(Color.Black);
            Text_score = new Text("Score", Outfit_light_font, Font_size_inscription);
            Text_score.FillColor = new Color(Color.Black);
            SetScoreInGamePosition();
        }

        public void SetScoreInGamePosition()
        {
            Game_score_text.Position = new Vector2f(Program.Cell_size, Program.Cell_size - 1.5f * Font_size);
            Text_score.Position = new Vector2f(Program.Cell_size, Program.Cell_size - 1.5f * Font_size - Font_size_inscription);
        }
    }
}
