using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Match3_Test.Models
{
    class SpriteStorage
    {
        public Sprite TikTok_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/TikTok.png"));
        public Sprite Snapchat_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Snapchat.png"));
        public Sprite Twitter_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Twitter.png"));
        public Sprite WhatsApp_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/WhatsApp.png"));
        public Sprite YouTube_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/YouTube.png"));
        public Sprite Play_button_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Play_button.png"));
        public Sprite Ok_button_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Ok_button.png"));
        public Sprite Game_over_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Game_over.png"));
        public Sprite Discharge_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Discharge.png"));
        public Sprite Bomb_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Bomb.png"));

        public Sprite Line_bonus_horizontal_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Line_Bonus.png"));
        public Sprite Line_bonus_vertical_sprite = new Sprite(TextureLoader.LoadTexture("./Content/images/Line_Bonus.png"));

        public Sprite[] Cell_type_sprites;

        public Vector2i Button_pos = new Vector2i(150, 200);
        public Vector2i Button_size;

        public RectangleShape White_rextangle;

        public SpriteStorage()
        {
            Button_size = new Vector2i((int)Play_button_sprite.GetLocalBounds().Width, (int)Play_button_sprite.GetLocalBounds().Height);
            Play_button_sprite.Position = (Vector2f)Button_pos;

            White_rextangle = new RectangleShape(new Vector2f(Program.Cell_size * Program.Field_size, Program.Cell_size));
            White_rextangle.FillColor = new Color(Color.White);
            White_rextangle.Position = new Vector2f(Program.Cell_size, 0);

            Cell_type_sprites = new Sprite[] { TikTok_sprite, Snapchat_sprite, Twitter_sprite, WhatsApp_sprite, YouTube_sprite };
            RotateSprite(Line_bonus_vertical_sprite);
        }

        static void RotateSprite(Sprite Target)
        {
            Target.Origin = new Vector2f(0, Target.GetLocalBounds().Height);
            Target.Rotation = 90.0f;
        }
    }
}
