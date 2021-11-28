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
        static void Swap(GridCell p1, GridCell p2, GridCell[,] grid)
        {
            GridCell p3 = p1;
            p1.column = p2.column;
            p2.column = p3.column;
            p1.row = p2.row;
            p2.row = p3.row;

            grid[p1.row, p1.column] = p1;
            grid[p2.row, p2.column] = p2;
        }

        static bool Bounds_Check(Vector2i Mouse_pos, Vector2i Start_bounds, Vector2i End_bounds)
        {
            bool isInBounds = false;
            //Console.Write(Start_bounds);
            //Console.Write(Mouse_pos);
            //Console.WriteLine(End_bounds);
            if (Start_bounds.X < Mouse_pos.X &&
                Mouse_pos.X < End_bounds.X &&
                Start_bounds.Y < Mouse_pos.Y &&
                Mouse_pos.Y < End_bounds.Y)
            {
                isInBounds = true;
            }
            return isInBounds;
        }

        static bool Compare_Elements_Kind(GridCell[,] grid, int i, int j, int i1, int j1)
        {
            bool isEqual = false;
            if (grid[i, j].kind == grid[i1, j1].kind)
                isEqual = true;
            return isEqual;
        }

        static RenderWindow app;
        private static bool isSwap;
        private static bool isMoving;
        private static int click = 0;
        private static readonly int Game_fps = 60;
        private static readonly int Game_time = 60;
        private static int Game_window = 0;
        private static int Game_score = 0;
        private static Vector2i pos;
        private static int score;
        private static readonly int Field_size = 8;
        private static readonly int Cell_size = 60;
        private static readonly int Types_of_cells = 5;
        private static readonly int Bomb_type = Types_of_cells + 2;

        public static int Bomb_score;

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

            Texture t1 = new Texture("./Content/images/1.png");
            Texture Play_button = new Texture("./Content/images/Play_button.png");
            Texture Ok_button = new Texture("./Content/images/Ok_button.png");
            Texture Game_over = new Texture("./Content/images/Game_over.png");
            Texture Discharge = new Texture("./Content/images/Discharge.png");
            Texture Bomb = new Texture("./Content/images/Bomb.png");

            Sprite texture = new Sprite(t1);
            Sprite Play_button_texture = new Sprite(Play_button);
            Sprite Ok_button_texture = new Sprite(Ok_button);
            Sprite Game_over_texture = new Sprite(Game_over);
            Sprite Discharge_texture = new Sprite(Discharge);
            Sprite Bomb_texture = new Sprite(Bomb);

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



            //Console.WriteLine(Play_button_texture.GetLocalBounds());

            Vector2i Button_pos = new Vector2i(150, 200);
            Vector2i Button_size = new Vector2i((int)Play_button_texture.GetLocalBounds().Width, (int)Play_button_texture.GetLocalBounds().Height);

            Grid Grid_main = new Grid();

            Stopwatch Time_Checker = new Stopwatch();



            //for (int i = 1; i <= Field_size; i++)
            //{
            //    for (int j = 1; j <= Field_size; j++)
            //    {
            //        Console.Write(grid[i, j].x);
            //        Console.Write(grid[i, j].y);
            //        Console.Write(grid[i, j].column);
            //        Console.Write(grid[i, j].row);
            //        Console.Write(grid[i, j].kind);
            //        Console.Write(grid[i, j].match);
            //        Console.Write(grid[i, j].alpha);
            //        Console.WriteLine("\n");
            //    }
            //    Console.WriteLine("\n");
            //}


            //for (int i = 1; i <= Field_size; i++)
            //{
            //    for (int j = 1; j <= Field_size; j++)
            //    {
            //        Console.Write(grid[i, j].x + " ");
            //        Console.Write(grid[i, j].y + " ");
            //        Console.Write(grid[i, j].column + " ");
            //        Console.Write(grid[i, j].row + " ");
            //        Console.Write(grid[i, j].kind + " ");
            //        Console.Write(grid[i, j].match + " ");
            //        Console.Write(grid[i, j].alpha + " ");
            //        Console.WriteLine("\n");
            //    }
            //    Console.WriteLine("\n");
            //}

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
                    Play_button_texture.Position = (Vector2f)Button_pos;
                    app.Draw(Play_button_texture);
                    //Console.WriteLine(Game_window);
                    if (click == 1)
                    {
                        //Console.WriteLine(Bounds_Check(pos, Button_pos, Button_size));
                        if (Bounds_Check(pos, Button_pos, Button_pos + Button_size))
                        {
                            Game_window = 1;
                            Game_score = 0;
                            TimerCallback Game_timer = new TimerCallback(End_game);
                            Timer timer = new Timer(Game_timer, null, Game_time * 1000, 0);
                            Grid_main.Create_Grid(Field_size, Cell_size, Types_of_cells);

                            Time_Checker.Restart();
                        }
                        click = 0;
                    }

                }
                if (Game_window == 2)
                {
                    Ok_button_texture.Position = (Vector2f)Button_pos;
                    Game_over_texture.Position = ((Vector2f)Button_pos) + new Vector2f(0, -80);
                    app.Draw(Ok_button_texture);
                    app.Draw(Game_over_texture);
                    if (click == 1)
                    {
                        //Console.WriteLine(Bounds_Check(pos, Button_pos, Button_size));
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
                        x0 = Program.pos.X / Cell_size;
                        y0 = Program.pos.Y / Cell_size;

                        Discharge_texture.Position = new Vector2f(x0 * Cell_size, y0 * Cell_size);
                        app.Draw(Discharge_texture);
                    }
                    if (click == 2)
                    {
                        x = Program.pos.X / Cell_size;
                        y = Program.pos.Y / Cell_size;
                        if (Math.Abs(x - x0) + Math.Abs(y - y0) == 1)
                        {
                            Swap(Grid_main[y0, x0], Grid_main[y, x], Grid_main.grid);
                            isSwap = true;
                            click = 0;
                        }
                        else click = 1;
                    }

                    //Match finding
                    if (!isMoving)
                    {
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                            {
                                if (Grid_main[i, j].kind == Grid_main[i + 1, j].kind)
                                    if (Grid_main[i, j].kind == Grid_main[i - 1, j].kind)
                                        for (int n = -1; n <= 1; n++)
                                        {
                                            Grid_main.grid[i + n, j].match++;
                                        }

                                if (Grid_main[i, j].kind == Grid_main[i, j + 1].kind)
                                    if (Grid_main[i, j].kind == Grid_main[i, j - 1].kind)
                                        for (int n = -1; n <= 1; n++)
                                        {
                                            Grid_main.grid[i, j + n].match++;
                                        }
                            }
                        
                        //Upping for bomb
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                            {
                                if (Grid_main[i, j].match == 2)
                                {
                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i + 1, j))
                                    {
                                        if (Grid_main[i + 1, j].match >= 2)
                                        {
                                            continue;
                                        }
                                    }

                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i - 1, j))
                                    {
                                        if (Grid_main[i - 1, j].match >= 2)
                                        {
                                            continue;
                                        }
                                    }

                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i, j + 1))
                                    {
                                        if (Grid_main[i, j + 1].match >= 2)
                                        {
                                            continue;
                                        }
                                    }

                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i, j - 1))
                                    {
                                        if (Grid_main[i, j - 1].match >= 2)
                                        {
                                            continue;
                                        }
                                    }
                                    Grid_main.grid[i, j].match++;
                                }
                            }
                        
                    }
                    /*
                    // Bomb filling
                    if (!isMoving)
                    {
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                            {
                                if (Grid_main[i, j].match >= 3)
                                {
                                    Grid_main.grid[i, j].match = 0;
                                    Game_score++;
                                    Grid_main.grid[i, j].kind = Bomb_type;
                                }
                            }
                    }
                    */
                    //Moving animation
                    isMoving = false;
                    for (int i = 1; i <= Field_size; i++)
                        for (int j = 1; j <= Field_size; j++)
                        {
                            ref GridCell p = ref Grid_main.grid[i, j];
                            int dx = 0,
                                dy = 0;
                            for (int n = 0; n < 4; n++)   // 4 - speed
                            {
                                dx = p.x - p.column * Cell_size;
                                dy = p.y - p.row * Cell_size;
                                if (dx != 0) p.x -= dx / Math.Abs(dx);
                                if (dy != 0) p.y -= dy / Math.Abs(dy);
                                //Console.WriteLine(dx + " " + dy);
                            }
                            if (dx != 0 || dy != 0) isMoving = true;
                        }

                    // Bomb filling
                    if (!isMoving)
                    {
                        Bomb_score = 0;
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                            {
                                if (Grid_main[i, j].match >= 3)
                                {
                                    Grid_main.grid[i, j].match = 0;
                                    Bomb_score++;
                                    Grid_main.grid[i, j].kind = Bomb_type;
                                }
                            }
                    }

                    //Deleting amimation
                    if (!isMoving)
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                                if (Grid_main[i, j].match > 0)
                                    if (Grid_main[i, j].alpha > 10)
                                    {
                                        Grid_main.grid[i, j].alpha -= 10; isMoving = true;
                                    }

                    //Get score
                    if (!isMoving)
                    {
                        score = 0;
                        for (int i = 1; i <= 8; i++)
                            for (int j = 1; j <= 8; j++)
                                if (Grid_main[i, j].match != 0)
                                {
                                    score++;
                                    Game_score++;
                                    Game_score += Bomb_score;
                                }
                    }

                    

                    //Second swap if no match
                    if (isSwap && !isMoving)
                    {
                        if (score == 0) Swap(Grid_main[y0, x0], Grid_main[y, x], Grid_main.grid);
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

                        isSwap = false;
                    }

                    //Update grid
                    if (!isMoving)
                    {
                        for (int i = Field_size; i > 0; i--)
                            for (int j = 1; j <= Field_size; j++)
                                if (Grid_main[i, j].match > 0)
                                    for (int n = i; n > 0; n--)
                                        if (Grid_main.grid[n, j].match == 0) { Swap(Grid_main[n, j], Grid_main[i, j], Grid_main.grid); break; };

                        for (int j = 1; j <= Field_size; j++)
                            for (int i = Field_size, n = 0; i > 0; i--)
                                if (Grid_main[i, j].match > 0)
                                {
                                    Grid_main.grid[i, j].kind = new Random().Next(Types_of_cells) + 1;
                                    Grid_main.grid[i, j].y = -Cell_size * n++;
                                    Grid_main.grid[i, j].match = 0;
                                    Grid_main.grid[i, j].alpha = 255;
                                }
                    }

                    // draw
                    for (int i = 1; i <= Field_size; i++)
                        for (int j = 1; j <= Field_size; j++)
                        {
                            GridCell p = new GridCell();
                            p = Grid_main[i, j];
                            if (p.kind <= Types_of_cells + 1)
                            {
                                texture.TextureRect = new IntRect((p.kind - 1) * Cell_size, 0, Cell_size, Cell_size);
                                texture.Position = new Vector2f(p.x, p.y);
                                texture.Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(texture);
                            }
                            if (p.kind == Bomb_type)
                            {
                                Bomb_texture.Position = new Vector2f(p.x, p.y);
                                Bomb_texture.Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(Bomb_texture);
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
                    if (!isSwap && !isMoving) click++;
                }
                else click = 0;
            }
        }

        private static void App_Closed(object sender, EventArgs e)
        {
            app.Close();
            //app = new RenderWindow(new VideoMode(60, 60), "Match-3 Game!");
            //app.DispatchEvents();
            //app.Closed += App_Closed;
            //app.Display();
        }
    }
}
