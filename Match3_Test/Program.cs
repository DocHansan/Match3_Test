using System;
using System.Diagnostics;
using System.Threading;
using Match3_Test.Models;
using SFML.Graphics;
using SFML.System;
using SFML.Window;



namespace Match3_Test
{
    class Program
    {
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

        static void Check_Three_In_Line(GridCell[,] grid, int i, int j, int i1, int j1)
        {
            for (int n = -1; n <= 1; n++)
            {
                grid[i + i1 * n, j + j1 * n].match++;
            }
        }

        static RenderWindow app;
        private static int click = 0;
        private const int Game_fps = 60;
        private const int Game_time = 600;
        private static int Game_window = 0;
        private static int Game_score = 0;
        private static Vector2i pos;
        private static int score;
        public const int Field_size = 8;
        public const int Cell_size = 60;
        public const int Types_of_cells = 4;
        public static int Bonus_score = 0;

        static void Main(string[] args)
        {
            uint width = 600,
                height = 800,
                Font_size = 24,
                Font_size_inscription = 16;

            app = new RenderWindow(new VideoMode(width, height), "Match-3 Game");
            app.SetFramerateLimit((uint)Game_fps);

            app.Closed += App_Closed;
            app.MouseButtonPressed += App_MouseButtonPressed;
            app.Resized += App_Resized;

            ElementSpriteStorage Sprite_storage = new ElementSpriteStorage();


            Font Outfit_light_font = new Font("./Content/Fonts/Outfit-Light.ttf");

            Text Time_remaining = new Text("", Outfit_light_font, Font_size);
            Time_remaining.FillColor = new Color(Color.Black);
            Time_remaining.Position = new Vector2f(Field_size * Cell_size, Cell_size - 1.5f * Font_size);

            Text Text_time_remaining = new Text("Time remaining", Outfit_light_font, Font_size_inscription);
            Text_time_remaining.FillColor = new Color(Color.Black);
            Text_time_remaining.Position = new Vector2f(Field_size * Cell_size, Cell_size - 1.5f * Font_size - Font_size_inscription);

            Text Game_score_text = new Text("", Outfit_light_font, Font_size);
            Game_score_text.FillColor = new Color(Color.Black);
            Game_score_text.Position = new Vector2f(Cell_size, Cell_size - 1.5f * Font_size);

            Text Text_score = new Text("Score", Outfit_light_font, Font_size_inscription);
            Text_score.FillColor = new Color(Color.Black);
            Text_score.Position = new Vector2f(Cell_size, Cell_size - 1.5f * Font_size - Font_size_inscription);

            RectangleShape White_rextangle = new RectangleShape(new Vector2f(Cell_size * Field_size, Cell_size));
            White_rextangle.FillColor = new Color(Color.White);
            White_rextangle.Position = new Vector2f(Cell_size, 0);

            Vector2i Button_pos = new Vector2i(150, 200);
            Vector2i Button_size = new Vector2i((int)Sprite_storage.Play_button_sprite.GetLocalBounds().Width, (int)Sprite_storage.Play_button_sprite.GetLocalBounds().Height);

            Grid Grid_main = new Grid(Field_size, Cell_size, Types_of_cells);

            Stopwatch Time_Checker = new Stopwatch();

            int x0 = 0,
                y0 = 0,
                x = 0,
                y = 0;

            while (app.IsOpen)
            {
                app.DispatchEvents();

                app.Clear(Color.White);

                if (Game_window == 0)
                {
                    Sprite_storage.Play_button_sprite.Position = (Vector2f)Button_pos;
                    app.Draw(Sprite_storage.Play_button_sprite);
                    if (click == 1)
                    {
                        if (Bounds_Check(pos, Button_pos, Button_pos + Button_size))
                        {
                            Game_window = 1;
                            Game_score = 0;
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
                    Sprite_storage.Ok_button_sprite.Position = (Vector2f)Button_pos;
                    Sprite_storage.Game_over_sprite.Position = ((Vector2f)Button_pos) + new Vector2f(0, -80);
                    app.Draw(Sprite_storage.Ok_button_sprite);
                    app.Draw(Sprite_storage.Game_over_sprite);
                    if (click == 1)
                    {
                        if (Bounds_Check(pos, Button_pos, Button_pos + Button_size))
                        {
                            Game_window = 0;
                        };
                        click = 0;
                    }

                }

                if (Game_window == 1)
                {
                    // mouse click
                    if (click == 1)
                    {
                        x0 = pos.X / Cell_size;
                        y0 = pos.Y / Cell_size;

                        if (Grid_main[y0, x0].kind > Types_of_cells)
                            BonusActivator.isNeedBonusCheck = true;

                        Sprite_storage.Discharge_sprite.Position = new Vector2f(x0 * Cell_size, y0 * Cell_size);
                        app.Draw(Sprite_storage.Discharge_sprite);
                    }
                    if (click == 2)
                    {
                        x = pos.X / Cell_size;
                        y = pos.Y / Cell_size;
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
                    {
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                            {
                                if (Grid_main[i, j].kind <= Types_of_cells)
                                {
                                    if (Grid_main.Compare_Elements_Kind(i, j, i + 1, j))
                                        if (Grid_main.Compare_Elements_Kind(i, j, i - 1, j))
                                            Check_Three_In_Line(Grid_main.grid, i, j, 1, 0);

                                    if (Grid_main.Compare_Elements_Kind(i, j, i, j + 1))
                                        if (Grid_main.Compare_Elements_Kind(i, j, i, j - 1))
                                            Check_Three_In_Line(Grid_main.grid, i, j, 0, 1);
                                }
                            }

                        //Upping for bomb
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                            {
                                if (Grid_main[i, j].match == 2)
                                {
                                    //Upping for bomb
                                    if (Grid_main.Compare_Elements_Kind(i, j, i + 1, j))
                                        if (Grid_main[i + 1, j].match >= 2)
                                            continue;

                                    if (Grid_main.Compare_Elements_Kind(i, j, i - 1, j))
                                        if (Grid_main[i - 1, j].match >= 2)
                                            continue;

                                    if (Grid_main.Compare_Elements_Kind(i, j, i, j + 1))
                                        if (Grid_main[i, j + 1].match >= 2)
                                            continue;

                                    if (Grid_main.Compare_Elements_Kind(i, j, i, j - 1))
                                        if (Grid_main[i, j - 1].match >= 2)
                                            continue;
                                    Grid_main.grid[i, j].match++;
                                }
                            }

                    }

                    //Moving animation
                    Animation.MoveCells(Grid_main);

                    // Create bonuses
                    BonusCreator.CreateBonuses(Grid_main, x, y, x0, y0);

                    // Bonus activating
                    BonusActivator.ActivateBonusesAfterClick(Grid_main, x0, y0, x, y);

                    //Deleting amimation
                    Animation.DeleteCells(Grid_main);

                    //Get score
                    if (!Animation.isMoving)
                    {
                        score = 0;
                        for (int i = 1; i <= 8; i++)
                            for (int j = 1; j <= 8; j++)
                                if (Grid_main[i, j].match != 0)
                                {
                                    score++;
                                    Game_score++;
                                    Game_score += Bonus_score;
                                    Bonus_score = 0;
                                }
                    }

                    //Second swap if no match
                    if (Animation.isSwap && !Animation.isMoving)
                    {
                        if (score == 0)
                            Grid_main.Swap(y0, x0, y, x);
                        // Check match by console
                        /*
                        for (int i = 1; i <= Field_size; i++)
                        {
                            for (int j = 1; j <= Field_size; j++)
                            {
                                Console.Write(Grid_main[i, j].match);
                                Console.Write(" ");
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                        */
                        Animation.isSwap = false;
                    }

                    //Update grid
                    Grid_main.Update_grid(!Animation.isMoving);

                    // draw
                    for (int i = 1; i <= Field_size; i++)
                        for (int j = 1; j <= Field_size; j++)
                        {
                            GridCell p = new GridCell();
                            p = Grid_main[i, j];
                            if (p.kind <= Types_of_cells)
                            {
                                //texture.TextureRect = new IntRect((p.kind - 1) * Cell_size, 0, Cell_size, Cell_size);
                                Sprite_storage.Cell_type_sprites[p.kind - 1].Position = new Vector2f(p.x, p.y);
                                Sprite_storage.Cell_type_sprites[p.kind - 1].Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(Sprite_storage.Cell_type_sprites[p.kind - 1]);
                            }
                            if (p.kind == BombBonus.Bomb_type)
                            {
                                Sprite_storage.Bomb_sprite.Position = new Vector2f(p.x, p.y);
                                Sprite_storage.Bomb_sprite.Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(Sprite_storage.Bomb_sprite);
                            }
                            if (p.kind == LineBonus.Line_horizontal_type)
                            {
                                Sprite_storage.Line_bonus_horizontal_sprite.Position = new Vector2f(p.x, p.y);
                                Sprite_storage.Line_bonus_horizontal_sprite.Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(Sprite_storage.Line_bonus_horizontal_sprite);
                            }
                            if (p.kind == LineBonus.Line_vertical_type)
                            {
                                Sprite_storage.Line_bonus_vertical_sprite.Position = new Vector2f(p.x, p.y);
                                Sprite_storage.Line_bonus_vertical_sprite.Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(Sprite_storage.Line_bonus_vertical_sprite);
                            }
                        }
                    Game_score_text.DisplayedString = Game_score.ToString();
                    Time_remaining.DisplayedString = (Game_time - (int)Time_Checker.ElapsedMilliseconds / 1000).ToString();
                    app.Draw(White_rextangle);
                    app.Draw(Game_score_text);
                    app.Draw(Time_remaining);
                    app.Draw(Text_score);
                    app.Draw(Text_time_remaining);
                }

                app.Display();
            }
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
                pos = Mouse.GetPosition(app);
                if (Cell_size < pos.X &&
                pos.X < Cell_size * (Field_size + 1) &&
                Cell_size < pos.Y &&
                pos.Y < Cell_size * (Field_size + 1))
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
