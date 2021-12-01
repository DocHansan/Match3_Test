using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Match3_Test.Models;
using SFML.Graphics;
using SFML.System;
using SFML.Window;



namespace Match3_Test
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static RenderWindow app;
        public const int Field_size = 8;
        public const int Cell_size = 60;
        public const int Types_of_cells = 5;
        private const int Game_fps = 60;
        private const int Game_time = 60;
        private static int click = 0;
        private static int Game_window = 0;
        private static Vector2i Mouse_position;

        static void Main(string[] args)
        {
            IntPtr h = GetConsoleWindow();
            ShowWindow(h, 0);

            uint width = 600,
                height = 800;

            int x0 = 0,
                y0 = 0,
                x = 0,
                y = 0;

            app = new RenderWindow(new VideoMode(width, height), "Match-3 Game");
            app.SetFramerateLimit((uint)Game_fps);

            app.Closed += App_Closed;
            app.MouseButtonPressed += App_MouseButtonPressed;
            app.Resized += App_Resized;

            SpriteStorage Sprite_storage = new SpriteStorage();
            TextInterfaceStorage Text_interface_storage = new TextInterfaceStorage();
            ScoreCounter Score_counter = new ScoreCounter();
            Grid Grid_main = new Grid(Field_size, Cell_size, Types_of_cells);

            Stopwatch Time_Checker = new Stopwatch();

            while (app.IsOpen)
            {
                app.DispatchEvents();
                app.Clear(Color.White);

                if (Game_window == 0)
                {
                    app.Draw(Sprite_storage.Play_button_sprite);
                    if (click == 1)
                    {
                        if (Bounds_Check(Mouse_position, Sprite_storage.Button_pos, Sprite_storage.Button_pos + Sprite_storage.Button_size))
                        {
                            Game_window = 1;
                            Score_counter.Game_score = 0;
                            TimerCallback Game_timer = new TimerCallback(End_game);
                            Timer timer = new Timer(Game_timer, null, Game_time * 1000, 0);
                            Grid_main = new Grid(Field_size, Cell_size, Types_of_cells);
                            Time_Checker.Restart();
                        }
                        click = 0;
                    }

                }
                if (Game_window == 2)
                {
                    Sprite_storage.Ok_button_sprite.Position = (Vector2f)Sprite_storage.Button_pos;
                    Sprite_storage.Game_over_sprite.Position = ((Vector2f)Sprite_storage.Button_pos) + new Vector2f(0, -80);
                    Text_interface_storage.Text_score.Position = Sprite_storage.Ok_button_sprite.Position + new Vector2f(0, Sprite_storage.Button_size.Y) + new Vector2f(0, Text_interface_storage.Font_size);
                    Text_interface_storage.Game_score_text.Position = Text_interface_storage.Text_score.Position + new Vector2f(0, Text_interface_storage.Font_size);
                    app.Draw(Sprite_storage.Ok_button_sprite);
                    app.Draw(Sprite_storage.Game_over_sprite);
                    app.Draw(Text_interface_storage.Text_score);
                    app.Draw(Text_interface_storage.Game_score_text);
                    if (click == 1)
                    {
                        if (Bounds_Check(Mouse_position, Sprite_storage.Button_pos, Sprite_storage.Button_pos + Sprite_storage.Button_size))
                            Game_window = 0;
                        click = 0;
                    }

                }

                if (Game_window == 1)
                {
                    Text_interface_storage.SetScoreInGamePosition();

                    // mouse click
                    if (click == 1)
                    {
                        x0 = Mouse_position.X / Cell_size;
                        y0 = Mouse_position.Y / Cell_size;

                        if (Grid_main[y0, x0].kind > Types_of_cells)
                            BonusActivator.isNeedBonusCheck = true;

                        Sprite_storage.Discharge_sprite.Position = new Vector2f(x0 * Cell_size, y0 * Cell_size);
                        app.Draw(Sprite_storage.Discharge_sprite);
                    }
                    if (click == 2)
                    {
                        x = Mouse_position.X / Cell_size;
                        y = Mouse_position.Y / Cell_size;
                        if (Math.Abs(x - x0) + Math.Abs(y - y0) == 1)
                        {
                            if (Grid_main[y, x].kind > Types_of_cells)
                                BonusActivator.isNeedBonusCheck = true;

                            Grid_main.Swap(y0, x0, y, x);
                            Animation.isSwap = true;
                            click = 0;
                        }
                        else click = 1;
                    }

                    //Match finding
                    if (!Animation.isMoving)
                        Grid_main.FindMatch();

                    //Moving animation
                    Animation.MoveCells(Grid_main);

                    //Create bonuses
                    if (!Animation.isMoving)
                        BonusCreator.CreateBonuses(Grid_main, x, y, x0, y0);

                    //Bonus activating
                    if (BonusActivator.isNeedBonusCheck && !Animation.isMoving && Animation.isSwap)
                        BonusActivator.ActivateBonusesAfterClick(Grid_main, x0, y0, x, y);

                    //Creating amimation
                    if (!Animation.isMoving)
                        Animation.CreateBonus(Grid_main);

                    //Deleting amimation
                    if (!Animation.isMoving)
                        Animation.DeleteCells(Grid_main);

                    //Get score
                    if (!Animation.isMoving)
                        Score_counter.CountPoints(Grid_main);

                    //Second swap if no match
                    if (Animation.isSwap && !Animation.isMoving)
                    {
                        if (Score_counter.score == 0)
                            Grid_main.Swap(y0, x0, y, x);
                        Animation.isSwap = false;
                    }

                    //Update grid
                    if (!Animation.isMoving)
                        Grid_main.UpdateGrid();

                    //Draw
                    for (int i = 1; i <= Field_size; i++)
                        for (int j = 1; j <= Field_size; j++)
                        {
                            GridCell p = new GridCell();
                            p = Grid_main[i, j];
                            if (p.alpha < 255 - Animation.Creating_animation_speed && p.alpha > 0 + Animation.Deleting_animation_speed)
                            {
                                Sprite_storage.Discharge_sprite.Position = new Vector2f(p.x, p.y);
                                app.Draw(Sprite_storage.Discharge_sprite);
                            }
                            if (p.kind <= Types_of_cells)
                            {
                                Sprite_storage.Cell_type_sprites[p.kind - 1].Position = new Vector2f(p.x, p.y);
                                Sprite_storage.Cell_type_sprites[p.kind - 1].Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(Sprite_storage.Cell_type_sprites[p.kind - 1]);
                            }
                            switch (p.kind)
                            {
                                case BombBonus.Bomb_type:
                                    {
                                        Sprite_storage.Bomb_sprite.Position = new Vector2f(p.x, p.y);
                                        Sprite_storage.Bomb_sprite.Color = new Color(255, 255, 255, p.alpha);
                                        app.Draw(Sprite_storage.Bomb_sprite);
                                        break;
                                    }
                                case LineBonus.Line_horizontal_type:
                                    {
                                        Sprite_storage.Line_bonus_horizontal_sprite.Position = new Vector2f(p.x, p.y);
                                        Sprite_storage.Line_bonus_horizontal_sprite.Color = new Color(255, 255, 255, p.alpha);
                                        app.Draw(Sprite_storage.Line_bonus_horizontal_sprite);
                                        break;
                                    }
                                case LineBonus.Line_vertical_type:
                                    {
                                        Sprite_storage.Line_bonus_vertical_sprite.Position = new Vector2f(p.x, p.y);
                                        Sprite_storage.Line_bonus_vertical_sprite.Color = new Color(255, 255, 255, p.alpha);
                                        app.Draw(Sprite_storage.Line_bonus_vertical_sprite);
                                        break;
                                    }
                            }
                        }
                    Text_interface_storage.Game_score_text.DisplayedString = Score_counter.Game_score.ToString();
                    Text_interface_storage.Time_remaining.DisplayedString = (Game_time - (int)Time_Checker.ElapsedMilliseconds / 1000).ToString();
                    app.Draw(Sprite_storage.White_rextangle);
                    app.Draw(Text_interface_storage.Game_score_text);
                    app.Draw(Text_interface_storage.Time_remaining);
                    app.Draw(Text_interface_storage.Text_score);
                    app.Draw(Text_interface_storage.Text_time_remaining);
                }

                app.Display();
            }
        }

        static bool Bounds_Check(Vector2i Mouse_pos, Vector2i Start_bounds, Vector2i End_bounds)
        {
            bool isInBounds = false;
            if (Start_bounds.X < Mouse_pos.X &&
                Mouse_pos.X < End_bounds.X &&
                Start_bounds.Y < Mouse_pos.Y &&
                Mouse_pos.Y < End_bounds.Y)
            {
                isInBounds = true;
            }
            return isInBounds;
        }

        private static void End_game(object state)
        {
            click = 0;
            Game_window = 2;
        }

        private static void App_Resized(object sender, SizeEventArgs e)
        {
            app.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private static void App_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                Mouse_position = Mouse.GetPosition(app);
                if (Cell_size < Mouse_position.X &&
                Mouse_position.X < Cell_size * (Field_size + 1) &&
                Cell_size < Mouse_position.Y &&
                Mouse_position.Y < Cell_size * (Field_size + 1))
                {
                    if (Game_window == 0 || Game_window == 2)
                    {
                        click++;
                        return;
                    }
                    if (!Animation.isSwap && !Animation.isMoving) click++;
                }
                else click = 0;
            }
        }

        private static void App_Closed(object sender, EventArgs e)
        {
            app.Close();
        }
    }
}
