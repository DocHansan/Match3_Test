using System;
using System.IO;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


struct Game_Field
{
    public int x,
        y,
        column,
        row,
        kind,
        match;
    public byte alpha;
}



namespace Match3_Test
{
    class Program
    {
        static void Swap(Game_Field p1, Game_Field p2, Game_Field[,] grid)
        {
            Game_Field p3 = p1;
            p1.column = p2.column;
            p2.column = p3.column;
            p1.row = p2.row;
            p2.row = p3.row;

            grid[p1.row, p1.column] = p1;
            grid[p2.row, p2.column] = p2;
        }

        static RenderWindow app;
        private static bool isSwap;
        private static bool isMoving;
        private static int click = 0;
        private static Vector2i pos;

        static void Main(string[] args)
        {
            uint width = 900,
                height = 900;

            app = new RenderWindow(new VideoMode(width, height), "Match-3 Game!");
            app.SetFramerateLimit(60);

            app.Closed += App_Closed;
            app.MouseButtonPressed += App_MouseButtonPressed;

            Texture t1 = new Texture("./Content/images/1.png");

            Sprite texture = new Sprite(t1);

            int Field_size = 8,
                Cell_size = 60,
                Types_of_cells = 5;

            Game_Field[,] grid = new Game_Field[Field_size + 2, Field_size + 2];


            for (int i = 1; i <= Field_size; i++)
            {
                for (int j = 1; j <= Field_size; j++)
                {
                    Console.Write(grid[i, j].x);
                    Console.Write(grid[i, j].y);
                    Console.Write(grid[i, j].column);
                    Console.Write(grid[i, j].row);
                    Console.Write(grid[i, j].kind);
                    Console.Write(grid[i, j].match);
                    Console.Write(grid[i, j].alpha);
                    Console.WriteLine("\n");
                }
                Console.WriteLine("\n");
            }

            for (int i = 1; i <= Field_size; i++)
                for (int j = 1; j <= Field_size; j++)
                {
                    grid[i, j].x = j * Cell_size;
                    grid[i, j].y = i * Cell_size;
                    grid[i,j].column = j;
                    grid[i,j].row = i;
                    grid[i, j].kind = new Random().Next(Types_of_cells) + 1;
                    grid[i, j].match = 0;
                    grid[i, j].alpha = 255;
                }

            for (int i = 1; i <= Field_size; i++)
            {
                for (int j = 1; j <= Field_size; j++)
                {
                    Console.Write(grid[i, j].x + " ");
                    Console.Write(grid[i, j].y + " ");
                    Console.Write(grid[i, j].column + " ");
                    Console.Write(grid[i, j].row + " ");
                    Console.Write(grid[i, j].kind + " ");
                    Console.Write(grid[i, j].match + " ");
                    Console.Write(grid[i, j].alpha + " ");
                    Console.WriteLine("\n");
                }
                Console.WriteLine("\n");
            }

            int x0 = 0,
                y0 = 0,
                x = 0,
                y = 0;

            Vector2i pos;

            

            while (app.IsOpen)
            {
                app.DispatchEvents();

                app.Clear(Color.White);

                // mouse click
                if (click == 1)
                {
                    x0 = Program.pos.X / Cell_size;
                    y0 = Program.pos.Y / Cell_size;
                }
                if (click == 2)
                {
                    x = Program.pos.X / Cell_size;
                    y = Program.pos.Y / Cell_size;
                    if (Math.Abs(x - x0) + Math.Abs(y - y0) == 1)
                    {
                        Swap(grid[y0, x0], grid[y, x], grid); isSwap = true; click = 0;
                    }
                    else click = 1;
                }

                //Match finding
                for (int i = 1; i <= Field_size; i++)
                    for (int j = 1; j <= Field_size; j++)
                    {
                        if (grid[i, j].kind == grid[i + 1, j].kind)
                            if (grid[i, j].kind == grid[i - 1, j].kind)
                                for (int n = -1; n <= 1; n++) grid[i + n, j].match++;

                        if (grid[i, j].kind == grid[i, j + 1].kind)
                            if (grid[i, j].kind == grid[i, j - 1].kind)
                                for (int n = -1; n <= 1; n++) grid[i, j + n].match++;
                    }

                //Moving animation
                isMoving = false;
                for (int i = 1; i <= Field_size; i++)
                    for (int j = 1; j <= Field_size; j++)
                    {
                        ref Game_Field p = ref grid[i, j];
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
                
                //Deleting amimation
                if (!isMoving)
                    for (int i = 1; i <= Field_size; i++)
                        for (int j = 1; j <= Field_size; j++)
                            if (grid[i,j].match > 0) if (grid[i, j].alpha > 10) { grid[i, j].alpha -= 10; isMoving = true; }

                //Get score
                int score = 0;
                for (int i = 1; i <= 8; i++)
                    for (int j = 1; j <= 8; j++)
                        if (grid[i, j].match != 0) score++;

        //Second swap if no match
        if (isSwap && !isMoving)
                {
                    if (score == 0) Swap(grid[y0, x0], grid[y,x], grid); isSwap = false;
                }

                //Update grid
                if (!isMoving)
                {
                    for (int i = Field_size; i > 0; i--)
                        for (int j = 1; j <= Field_size; j++)
                            if (grid[i, j].match > 0)
                                for (int n = i; n > 0; n--)
                                    if (grid[n, j].match == 0) { Swap(grid[n, j], grid[i, j], grid); break; };

                    for (int j = 1; j <= Field_size; j++)
                        for (int i = Field_size, n = 0; i > 0; i--)
                            if (grid[i, j].match > 0)
                            {
                                grid[i, j].kind = new Random().Next(Types_of_cells) + 1;
                                grid[i, j].y = -Cell_size * n++;
                                grid[i, j].match = 0;
                                grid[i, j].alpha = 255;
                            }
                }

                // draw
                for (int i = 1; i <= Field_size; i++)
                    for (int j = 1; j <= Field_size; j++)
                    {
                        Game_Field p = new Game_Field();
                        p = grid[i, j];
                        texture.TextureRect = new IntRect((p.kind - 1) * Cell_size, 0, Cell_size, Cell_size);
                        texture.Color = new Color(255, 255, 255, p.alpha);
                        texture.Position = new Vector2f(p.x, p.y);
                        app.Draw(texture);
                    }
                app.Display();
            }
        }

        private static void App_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if (!isSwap && !isMoving) click++;
                pos = Mouse.GetPosition(app);
            }
        }

        private static void App_Closed(object sender, EventArgs e)
        {
            app.Close();
            app = new RenderWindow(new VideoMode(60, 60), "Match-3 Game!");
            app.DispatchEvents();
            app.Closed += App_Closed;
            app.Display();
        }
    }
}
