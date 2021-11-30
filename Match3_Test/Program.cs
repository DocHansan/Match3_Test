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

        static void Activate_Bonus(GridCell[,] grid, int x, int y)
        {
            Console.WriteLine(y.ToString() + " " + x.ToString());
            Console.WriteLine();
            if (grid[x, y].kind == Bomb_type)
            {
                if (grid[x, y].isNeed_bonus_activation)
                {
                    grid[x, y].isNeed_bonus_activation = false;
                    //grid[x, y].kind = new Random().Next(Types_of_cells) + 1;
                    for (int i = x - Bomb_radius; i <= x + Bomb_radius; i++)
                        for (int j = y - Bomb_radius; j <= y + Bomb_radius; j++)
                        {
                            if (grid[i, j].match == 0)
                                grid[i, j].match++;
                            if (grid[i, j].kind > Types_of_cells + 1)
                            {
                                Activate_Bonus(grid, i, j);
                            }

                        }
                }
            }
            if (grid[x, y].kind == Line_horizontal_type)
            {
                if (grid[x, y].isNeed_bonus_activation)
                {
                    grid[x, y].isNeed_bonus_activation = false;
                    //grid[x, y].kind = new Random().Next(Types_of_cells) + 1;
                    Destroy_Line_Bonus(grid, x, y, 0, 1, y, Field_size + 1);
                    Destroy_Line_Bonus(grid, x, y, 0, -1, y, 0);
                }
            }
            if (grid[x, y].kind == Line_vertical_type)
            {
                if (grid[x, y].isNeed_bonus_activation)
                {
                    grid[x, y].isNeed_bonus_activation = false;
                    //grid[x, y].kind = new Random().Next(Types_of_cells) + 1;
                    Destroy_Line_Bonus(grid, x, y, 1, 0, x, Field_size + 1);
                    Destroy_Line_Bonus(grid, x, y, -1, 0, x, 0);
                }
            }
        }

        static void Fill_Line_Bonus(GridCell[,] grid, int i1, int j1, int line_bonus_type)
        {
            for (int i = 1; i <= Field_size; i++)
                for (int j = 1; j <= Field_size; j++)
                {
                    if (Compare_Elements_Kind(grid, i, j, i + i1, j + j1))
                        if (grid[i, j].match == 2 && grid[i + i1, j + j1].match == 2)
                        {
                            Create_Line_Bonus(grid, i, j, line_bonus_type);
                            /*
                            grid[i, j].match = 0;
                            Bonus_score++;
                            grid[i, j].kind = line_bonus_type;
                            */
                        }
                }
        }

        static void Create_Line_Bonus(GridCell[,] grid, int i, int j, int line_bonus_type)
        {
            grid[i, j].match = 0;
            grid[i, j].isNeed_bonus_activation = true;
            Bonus_score++;
            grid[i, j].kind = line_bonus_type;
        }

        static void Fill_Line_Bonus_On_Click_Position(GridCell[,] grid, int x, int y)
        {
            if (Compare_Elements_Kind(grid, x, y, x - 1, y))
                if (grid[x, y].match == 2 && grid[x - 1, y].match == 2)
                    Create_Line_Bonus(grid, x, y, Line_vertical_type);

            if (Compare_Elements_Kind(grid, x, y, x + 1, y))
                if (grid[x, y].match == 2 && grid[x + 1, y].match == 2)
                    Create_Line_Bonus(grid, x, y, Line_vertical_type);

            if (Compare_Elements_Kind(grid, x, y, x, y - 1))
                if (grid[x, y].match == 2 && grid[x, y - 1].match == 2)
                    Create_Line_Bonus(grid, x, y, Line_horizontal_type);

            if (Compare_Elements_Kind(grid, x, y, x, y + 1))
                if (grid[x, y].match == 2 && grid[x, y + 1].match == 2)
                    Create_Line_Bonus(grid, x, y, Line_horizontal_type);
        }

        static void Check_Three_In_Line(GridCell[,] grid, int i, int j, int i1, int j1)
        {
            for (int n = -1; n <= 1; n++)
            {
                grid[i + i1 * n, j + j1 * n].match++;
            }
        }

        static void Destroy_Line_Bonus(GridCell[,] grid, int x, int y, int x1, int y1, int start_index, int len)
        {
            for (int i = start_index; i != len; i += x1 + y1)
            {
                if (grid[x1 == 0 ? x : i, y1 == 0 ? y : i].match == 0)
                    grid[x1 == 0 ? x : i, y1 == 0 ? y : i].match++;
                if (grid[x1 == 0 ? x : i, y1 == 0 ? y : i].kind > Types_of_cells + 1)
                {
                    Activate_Bonus(grid, x1 == 0 ? x : i, y1 == 0 ? y : i);
                }
            }
        }

        static RenderWindow app;
        private static bool isSwap;
        private static bool isMoving;
        private static bool isNeedBonusCheck;
        private static int click = 0;
        private static readonly int Game_fps = 60;
        private static readonly int Game_time = 600;
        private static int Game_window = 0;
        private static int Game_score = 0;
        private static Vector2i pos;
        private static int score;
        private static readonly int Field_size = 8;
        private static readonly int Moving_animation_speed = 6;
        private static readonly byte Deleting_animation_speed = 5;
        private static readonly int Cell_size = 60;
        private static readonly int Types_of_cells = 5;
        private static readonly int Bomb_type = Types_of_cells + 2;
        private static readonly int Bomb_radius = 1;
        private static readonly int Line_horizontal_type = Bomb_type + 1;
        private static readonly int Line_vertical_type = Line_horizontal_type + 1;

        public static int Bonus_score;

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
            Texture Line_bonus = new Texture("./Content/images/Line_Bonus.png");

            Sprite texture = new Sprite(t1);
            Sprite Play_button_texture = new Sprite(Play_button);
            Sprite Ok_button_texture = new Sprite(Ok_button);
            Sprite Game_over_texture = new Sprite(Game_over);
            Sprite Discharge_texture = new Sprite(Discharge);
            Sprite Bomb_texture = new Sprite(Bomb);
            Sprite Line_bonus_horizontal_texture = new Sprite(Line_bonus);
            Sprite Line_bonus_vertical_texture = new Sprite(Line_bonus);

            Line_bonus_vertical_texture.Origin = new Vector2f(0, Line_bonus_vertical_texture.GetLocalBounds().Height);
            Line_bonus_vertical_texture.Rotation = 90.0f;
            

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

                        if (Grid_main[y0, x0].kind > Types_of_cells + 1)
                            isNeedBonusCheck = true;

                        Discharge_texture.Position = new Vector2f(x0 * Cell_size, y0 * Cell_size);
                        app.Draw(Discharge_texture);
                    }
                    if (click == 2)
                    {
                        x = Program.pos.X / Cell_size;
                        y = Program.pos.Y / Cell_size;
                        if (Math.Abs(x - x0) + Math.Abs(y - y0) == 1)
                        {
                            if (Grid_main[y, x].kind > Types_of_cells + 1)
                                isNeedBonusCheck = true;

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
                                if (Grid_main[i, j].kind <= Types_of_cells + 1)
                                {
                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i + 1, j))
                                        if (Compare_Elements_Kind(Grid_main.grid, i, j, i - 1, j))
                                            Check_Three_In_Line(Grid_main.grid, i, j, 1, 0);
                                    /*
                                            for (int n = -1; n <= 1; n++)
                                            {
                                                Grid_main.grid[i + n, j].match++;
                                            }*/

                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i, j + 1))
                                        if (Compare_Elements_Kind(Grid_main.grid, i, j, i, j - 1))
                                            Check_Three_In_Line(Grid_main.grid, i, j, 0, 1);
                                                /*
                                            for (int n = -1; n <= 1; n++)
                                            {
                                                Grid_main.grid[i, j + n].match++;
                                            }*/
                                }
                            }

                        //Upping for bomb
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                            {
                                if (Grid_main[i, j].match == 2)
                                {
                                    //Upping for bomb
                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i + 1, j))
                                        if (Grid_main[i + 1, j].match >= 2)
                                            continue;

                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i - 1, j))
                                        if (Grid_main[i - 1, j].match >= 2)
                                            continue;

                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i, j + 1))
                                        if (Grid_main[i, j + 1].match >= 2)
                                            continue;

                                    if (Compare_Elements_Kind(Grid_main.grid, i, j, i, j - 1))
                                        if (Grid_main[i, j - 1].match >= 2)
                                            continue;
                                    Grid_main.grid[i, j].match++;
                                }
                            }

                    }

                    //Moving animation
                    isMoving = false;
                    for (int i = 1; i <= Field_size; i++)
                        for (int j = 1; j <= Field_size; j++)
                        {
                            ref GridCell p = ref Grid_main.grid[i, j];
                            int dx = 0,
                                dy = 0;
                            for (int n = 0; n < Moving_animation_speed; n++)
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
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                            {
                                if (Grid_main[i, j].match >= 3)
                                {
                                    Grid_main.grid[i, j].match = 0;
                                    Grid_main.grid[i, j].isNeed_bonus_activation = true;
                                    Bonus_score++;
                                    Grid_main.grid[i, j].kind = Bomb_type;
                                }
                            }
                    }
                    
                    // Line bonus filling
                    if (!isMoving)
                    {
                        //Create line bonuses for click positions
                        if (isSwap)
                        {
                            Fill_Line_Bonus_On_Click_Position(Grid_main.grid, y0, x0);
                            Fill_Line_Bonus_On_Click_Position(Grid_main.grid, y, x);
                        }

                        // Horizontal line bonus filling
                        Fill_Line_Bonus(Grid_main.grid, 0, 1, Line_horizontal_type);

                        // Vertical line bonus filling
                        Fill_Line_Bonus(Grid_main.grid, 1, 0, Line_vertical_type);
                    }

                    // Bonus activating
                    if (isNeedBonusCheck && !isMoving && isSwap)
                    {
                        Console.WriteLine(y0.ToString() + " " + x0.ToString());
                        Console.WriteLine(y.ToString() + " " + x.ToString());
                        Console.WriteLine();
                        Activate_Bonus(Grid_main.grid, y0, x0);
                        Activate_Bonus(Grid_main.grid, y, x);
                        isNeedBonusCheck = false;
                    }

                    //Deleting amimation
                    if (!isMoving)
                        for (int i = 1; i <= Field_size; i++)
                            for (int j = 1; j <= Field_size; j++)
                                if (Grid_main[i, j].match > 0)
                                    if (Grid_main[i, j].alpha > Deleting_animation_speed)
                                    {
                                        Grid_main.grid[i, j].alpha -= Deleting_animation_speed; isMoving = true;
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
                                    Game_score += Bonus_score;
                                    Bonus_score = 0;
                                }
                    }

                    //Second swap if no match
                    if (isSwap && !isMoving)
                    {
                        if (score == 0)
                            Swap(Grid_main[y0, x0], Grid_main[y, x], Grid_main.grid);
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
                            if (p.kind == Line_horizontal_type)
                            {
                                Line_bonus_horizontal_texture.Position = new Vector2f(p.x, p.y);
                                Line_bonus_horizontal_texture.Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(Line_bonus_horizontal_texture);
                            }
                            if (p.kind == Line_vertical_type)
                            {
                                Line_bonus_vertical_texture.Position = new Vector2f(p.x, p.y);
                                Line_bonus_vertical_texture.Color = new Color(255, 255, 255, p.alpha);
                                app.Draw(Line_bonus_vertical_texture);
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
