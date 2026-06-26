using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleSnake
{
    class Particle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double DX { get; set; }
        public double DY { get; set; }
        public int Age { get; set; }
        public int MaxAge { get; set; }
    }

    class Program
    {
        static int highScoreClassic = 0;
        static int highScoreNoWalls = 0;
        static int highScoreNeon = 0;

        static int maxComboClassic = 0;
        static int maxComboNoWalls = 0;
        static int maxComboNeon = 0;

        static void Main(string[] args)
        {
            int screenWidth = 65;
            int screenHeight = 25;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                try
                {
                    Console.SetWindowSize(screenWidth, screenHeight);
                    Console.SetBufferSize(screenWidth, screenHeight);
                }
                catch { }
            }
            Console.CursorVisible = false;

            while (true)
            {
                int gameMode = ShowMainMenu(screenWidth, screenHeight);
                bool normalGameOver = RunGame(screenWidth, screenHeight, gameMode);

                if (!normalGameOver) continue;

                ShowGameOverMenu(screenWidth, screenHeight, gameMode);
            }
        }

        static int ShowMainMenu(int screenWidth, int screenHeight)
        {
            int selectedOption = 1;
            while (true)
            {
                Console.ResetColor();
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(screenWidth / 2 - 9, screenHeight / 2 - 6);
                Console.WriteLine("=== SNAKE SYSTEM ===");

                Console.SetCursorPosition(screenWidth / 2 - 18, screenHeight / 2 - 2);
                if (selectedOption == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("> [1] Classic Mode (With Walls)");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("  [1] Classic Mode (With Walls)");
                }

                Console.SetCursorPosition(screenWidth / 2 - 18, screenHeight / 2);
                if (selectedOption == 2)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("> [2] Portal Mode (No Walls)");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("  [2] Portal Mode (No Walls)");
                }

                Console.SetCursorPosition(screenWidth / 2 - 18, screenHeight / 2 + 2);
                if (selectedOption == 3)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("> [3] Cyberpunk Mode (Neon Visuals)");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("  [3] Cyberpunk Mode (Neon Visuals)");
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(screenWidth / 2 - 19, screenHeight / 2 + 6);
                Console.WriteLine("Controls: Arrows/WASD | Select: Enter");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.SetCursorPosition(screenWidth / 2 - 5, screenHeight - 2);
                Console.WriteLine("By qbert4ik");

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                {
                    selectedOption--;
                    if (selectedOption < 1) selectedOption = 3;
                }
                else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                {
                    selectedOption++;
                    if (selectedOption > 3) selectedOption = 1;
                }
                else if (key.Key == ConsoleKey.Enter) return selectedOption;
            }
        }

        static void ShowGameOverMenu(int screenWidth, int screenHeight, int gameMode)
        {
            Console.ResetColor();
            Console.Clear();

            int boxWidth = 50;
            int boxHeight = 13;
            int startX = (screenWidth - boxWidth) / 2;
            int startY = (screenHeight - boxHeight) / 2;

            Console.ForegroundColor = ConsoleColor.Red;
            for (int x = startX; x < startX + boxWidth; x++)
            {
                Console.SetCursorPosition(x, startY); Console.Write("-");
                Console.SetCursorPosition(x, startY + boxHeight - 1); Console.Write("-");
            }
            for (int y = startY; y < startY + boxHeight; y++)
            {
                Console.SetCursorPosition(startX, y); Console.Write("|");
                Console.SetCursorPosition(startX + boxWidth - 1, y); Console.Write("|");
            }

            Console.SetCursorPosition(startX + (boxWidth / 2) - 5, startY + 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("GAME OVER!");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(startX + 4, startY + 3);
            string modeName = gameMode == 1 ? "Classic" : (gameMode == 2 ? "Portal" : "Cyberpunk");
            Console.Write($"Mode: {modeName}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(startX + 4, startY + 5);
            Console.Write($"Classic:   Score: {highScoreClassic,-4} | Max Combo: X{maxComboClassic}");

            Console.SetCursorPosition(startX + 4, startY + 7);
            Console.Write($"Portal:    Score: {highScoreNoWalls,-4} | Max Combo: X{maxComboNoWalls}");

            Console.SetCursorPosition(startX + 4, startY + 9);
            Console.Write($"Cyberpunk: Score: {highScoreNeon,-4} | Max Combo: X{maxComboNeon}");

            Console.SetCursorPosition(startX + 4, startY + boxHeight - 2);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Press 'R' for Menu or 'Q' to Quit");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.SetCursorPosition(screenWidth / 2 - 5, screenHeight - 2);
            Console.WriteLine("By qbert4ik");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo choice = Console.ReadKey(true);
                    if (choice.Key == ConsoleKey.R) return;
                    if (choice.Key == ConsoleKey.Q) Environment.Exit(0);
                }
            }
        }

        static bool RunGame(int screenWidth, int screenHeight, int gameMode)
        {
            int playWidth = 63;
            int playHeight = 24;

            Random random = new Random();
            int score = 0;
            bool gameOver = false;

            int comboMultiplier = 1;
            int currentMaxCombo = 1;
            int comboTimer = 0;

            int rainbowTimer = 0;
            ConsoleColor[] rainbowColors = {
                ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green,
                ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta
            };

            List<Particle> particles = new List<Particle>();
            List<int[]> snakeBody = new List<int[]>();
            snakeBody.Add(new int[] { playWidth / 2, playHeight / 2 });

            int foodX = random.Next(2, playWidth - 2);
            int foodY = random.Next(2, playHeight - 2);
            bool isGoldenFood = random.Next(0, 100) < 15;

            string direction = "RIGHT";

            int currentHighScore = gameMode == 1 ? highScoreClassic : (gameMode == 2 ? highScoreNoWalls : highScoreNeon);

            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            List<int[]> stars = new List<int[]>();
            for (int i = 0; i < 30; i++)
            {
                stars.Add(new int[] { random.Next(1, playWidth - 1), random.Next(1, playHeight - 1) });
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (int y = 1; y < playHeight - 1; y++)
            {
                Console.SetCursorPosition(1, y);
                Console.Write(new string('.', playWidth - 2));
            }

            char borderChar = gameMode == 2 ? '+' : '#';
            if (gameMode == 1) Console.ForegroundColor = ConsoleColor.DarkRed;
            else if (gameMode == 2) Console.ForegroundColor = ConsoleColor.DarkCyan;
            else if (gameMode == 3) Console.ForegroundColor = ConsoleColor.DarkMagenta;

            for (int i = 0; i < playWidth; i++)
            {
                Console.SetCursorPosition(i, 0); Console.Write(borderChar);
                Console.SetCursorPosition(i, playHeight - 1); Console.Write(borderChar);
            }
            for (int i = 0; i < playHeight; i++)
            {
                Console.SetCursorPosition(0, i); Console.Write(borderChar);
                Console.SetCursorPosition(playWidth - 1, i); Console.Write(borderChar);
            }

            int[] tailToClear = null;

            while (!gameOver)
            {
                if (rainbowTimer > 0) rainbowTimer--;

                if (comboTimer > 0)
                {
                    comboTimer--;
                    if (comboTimer == 0) comboMultiplier = 1; // reset combo multiplier when timer runs out
                }

                foreach (var p in particles)
                {
                    if (p.X > 0 && p.X < playWidth - 1 && p.Y > 0 && p.Y < playHeight - 1)
                    {
                        Console.SetCursorPosition(p.X, p.Y);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(".");
                    }
                }

                for (int i = particles.Count - 1; i >= 0; i--)
                {
                    var p = particles[i];
                    p.Age++;
                    if (p.Age >= p.MaxAge) particles.RemoveAt(i);
                    else
                    {
                        p.X += (int)Math.Round(p.DX);
                        p.Y += (int)Math.Round(p.DY);
                    }
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (var star in stars)
                {
                    if (star[0] > 0 && star[0] < playWidth - 1 && star[1] > 0 && star[1] < playHeight - 1)
                    {
                        Console.SetCursorPosition(star[0], star[1]);
                        Console.Write(".");
                    }

                    star[0]--;
                    if (star[0] <= 0)
                    {
                        star[0] = playWidth - 2;
                        star[1] = random.Next(1, playHeight - 1);
                    }
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape) return false;

                    if ((key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W) && direction != "DOWN") direction = "UP";
                    else if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S) && direction != "UP") direction = "DOWN";
                    else if ((key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A) && direction != "RIGHT") direction = "LEFT";
                    else if ((key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D) && direction != "LEFT") direction = "RIGHT";
                }

                int[] head = snakeBody[snakeBody.Count - 1];
                int newX = head[0];
                int newY = head[1];

                if (direction == "UP") newY--;
                if (direction == "DOWN") newY++;
                if (direction == "LEFT") newX--;
                if (direction == "RIGHT") newX++;

                if (gameMode == 1 || gameMode == 3)
                {
                    if (newX <= 0 || newX >= playWidth - 1 || newY <= 0 || newY >= playHeight - 1)
                        gameOver = true;
                }
                else
                {
                    if (newX <= 0) newX = playWidth - 2;
                    else if (newX >= playWidth - 1) newX = 1;

                    if (newY <= 0) newY = playHeight - 2;
                    else if (newY >= playHeight - 1) newY = 1;
                }

                foreach (var bodyPart in snakeBody)
                {
                    if (bodyPart[0] == newX && bodyPart[1] == newY)
                        gameOver = true;
                }

                if (gameOver) break;

                snakeBody.Add(new int[] { newX, newY });

                if (newX == foodX && newY == foodY)
                {
                    int basePoints = isGoldenFood ? 3 : 1;
                    score += basePoints * comboMultiplier;

                    if (isGoldenFood) rainbowTimer = 15; // turn on rainbow only when gold apple is eaten

                    if (comboTimer > 0) comboMultiplier++;
                    else comboMultiplier = 2;

                    if (comboMultiplier > currentMaxCombo) currentMaxCombo = comboMultiplier;

                    // let's spawn a new apple
                    foodX = random.Next(2, playWidth - 2);
                    foodY = random.Next(2, playHeight - 2);
                    isGoldenFood = random.Next(0, 100) < 15;

                    int distance = Math.Abs(newX - foodX) + Math.Abs(newY - foodY);
                    comboTimer = distance + 20;

                    int[] dxs = { -1, 0, 1, 1, 1, 0, -1, -1 };
                    int[] dys = { -1, -1, -1, 0, 1, 1, 1, 0 };
                    for (int k = 0; k < 8; k++)
                    {
                        particles.Add(new Particle
                        {
                            X = newX,
                            Y = newY,
                            DX = dxs[k],
                            DY = dys[k],
                            Age = 0,
                            MaxAge = random.Next(2, 5)
                        });
                    }

                    tailToClear = null;
                }
                else
                {
                    tailToClear = snakeBody[0];
                    snakeBody.RemoveAt(0);
                }

                if (tailToClear != null)
                {
                    Console.SetCursorPosition(tailToClear[0], tailToClear[1]);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(".");
                }

                Console.SetCursorPosition(foodX, foodY);
                if (isGoldenFood)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("$");
                }
                else
                {
                    Console.ForegroundColor = (gameMode == 3) ? ConsoleColor.Magenta : ConsoleColor.Red;
                    Console.Write("@");
                }

                foreach (var p in particles)
                {
                    if (p.X > 0 && p.X < playWidth - 1 && p.Y > 0 && p.Y < playHeight - 1)
                    {
                        Console.SetCursorPosition(p.X, p.Y);
                        Console.ForegroundColor = comboMultiplier > 2 ? ConsoleColor.Red : (gameMode == 3 ? ConsoleColor.Cyan : ConsoleColor.Green);
                        Console.Write(p.Age == 1 ? "*" : "·");
                    }
                }

                for (int i = 0; i < snakeBody.Count; i++)
                {
                    Console.SetCursorPosition(snakeBody[i][0], snakeBody[i][1]);

                    if (rainbowTimer > 0)
                    {
                        Console.ForegroundColor = rainbowColors[(i + rainbowTimer) % rainbowColors.Length];
                    }
                    else
                    {
                        if (gameMode == 3)
                        {
                            Console.ForegroundColor = (i == snakeBody.Count - 1) ? ConsoleColor.Cyan : ConsoleColor.Blue;
                        }
                        else
                        {
                            Console.ForegroundColor = (i == snakeBody.Count - 1) ? ConsoleColor.Green : ConsoleColor.DarkGreen;
                        }
                    }

                    Console.Write(i == snakeBody.Count - 1 ? "O" : "o");
                }

                Console.SetCursorPosition(1, 0);
                if (gameMode == 1) Console.BackgroundColor = ConsoleColor.DarkRed;
                else if (gameMode == 2) Console.BackgroundColor = ConsoleColor.DarkCyan;
                else if (gameMode == 3) Console.BackgroundColor = ConsoleColor.DarkBlue;

                Console.ForegroundColor = ConsoleColor.White;

                // create clean hud text that fits nicely on the top bar
                string infoText = $" S:{score} | B:{currentHighScore}";
                if (comboMultiplier > 1) infoText += $" | X{comboMultiplier}";
                if (rainbowTimer > 0) infoText += $" | RNBW";
                infoText += " | qbert4ik";

                if (infoText.Length > playWidth - 2) infoText = infoText.Substring(0, playWidth - 2);
                infoText = infoText.PadRight(playWidth - 2);

                Console.Write(infoText);
                Console.BackgroundColor = ConsoleColor.Black;

                if (direction == "UP" || direction == "DOWN") Thread.Sleep(100);
                else Thread.Sleep(65);
            }

            if (gameMode == 1)
            {
                if (score > highScoreClassic) highScoreClassic = score;
                if (currentMaxCombo > maxComboClassic) maxComboClassic = currentMaxCombo;
            }
            if (gameMode == 2)
            {
                if (score > highScoreNoWalls) highScoreNoWalls = score;
                if (currentMaxCombo > maxComboNoWalls) maxComboNoWalls = currentMaxCombo;
            }
            if (gameMode == 3)
            {
                if (score > highScoreNeon) highScoreNeon = score;
                if (currentMaxCombo > maxComboNeon) maxComboNeon = currentMaxCombo;
            }

            return true;
        }
    }
}