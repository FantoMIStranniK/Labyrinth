using System;

namespace TheMaze
{
    class Program
    {
        static Random rand = new Random();

        static int width = 10;
        static int height = 13;

        static char[,] field;

        static int playerX;
        static int playerY;
        static int exitX;
        static int exitY;

        static int powerLeft = 3;

        static bool finishedLevel = false;

        static int countOfMoves = 1;
        static float score = 0;

        static void Main(string[] args)
        {
            Init();

            while(true)
            {
                while (!finishedLevel)
                {
                    DoGameStep();
                }

                ResetLevel();
            }
        }
        static void DoGameStep()
        {
            DrawMap();

            GetInput();

            DrawMap();

            CheckForPlayerPosition();

            Console.Clear();
        }
        static void Init()
        {
            field = GenerateMap();

            field = PopRandomPoints(field);

            PlacePlayer();

            PlaceExit();
        }
        static char[,] GenerateMap()
        {
            char[,] map = new char[height, width];

            for(int i = 0; i <  height; i++)
            { 
                for(int j = 0; j < width; j++)
                {
                    map[i, j] = rand.NextDouble() > 0.45 ? ' ' : '█';
                }
            }

            return map;
        }
        static char[,] PopRandomPoints(char[,] map)
        {
            for (int i = 0; i < height; i++)
            {
                int randomX = rand.Next(0, width);

                if(map[i, randomX] is '█')
                {
                    map[i, randomX] = rand.NextDouble() > 0.5 ? map[i, randomX] = ' ' : map[i, randomX] = '█';
                }
            }

            return map;
        }
        static void PlacePlayer()
        {
            playerX = rand.Next(0, height);
            playerY = rand.Next(0, width);

            field[playerX, playerY] = 'X';
        }
        static void PlaceExit()
        {
            (int, int) exitCords;

            while (true)
            {
                exitCords.Item1 = rand.Next(0, width);
                exitCords.Item2 = rand.Next(0, height);

                if(exitCords.Item1 != playerX && exitCords.Item2 != playerY)
                {
                    exitX = exitCords.Item1;
                    exitY = exitCords.Item2;

                    break;
                }
            }

            field[exitY, exitX] = '@';
        }
        static void DrawMap()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.WriteLine("Use WASD to move, use Shoot command to destroy nearby obstacles, if you stuck write GiveUp");

            Console.WriteLine($"YOUR SCORE: {score}");
            Console.WriteLine();

            Console.WriteLine($"POWER LEFT: {powerLeft}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    ConsoleColor color = field[i, j] switch
                    {
                        'X' => ConsoleColor.Red,
                        '@' => ConsoleColor.Cyan,
                        _ => ConsoleColor.White,
                    };

                    Console.ForegroundColor = color;

                    Console.Write(field[i, j]);
                }
                Console.WriteLine();
            }
        }
        static void GetInput()
        {
            string input = Console.ReadLine();

            InputLogic(input);
        }
        static void InputLogic(string input)
        {
            int Xadd = 0;
            int Yadd = 0;

            bool doShot = false;

            switch (input)
            {
                case "W" or "w":
                    Xadd = -1;
                    break;
                case "S" or "s":
                    Xadd = 1;
                    break;
                case "D" or "d":
                    Yadd = 1;
                    break;
                case "A" or "a":
                    Yadd = -1;
                    break;
                case "Shoot" or "shoot":
                    doShot = true;
                    break;
                case "GiveUp":
                    ResetLevel(true);
                    break;
            }

            if (Xadd is not 0 || Yadd is not 0) MovePlayer(Xadd, Yadd);

            if (doShot && powerLeft > 0) Shoot();
        }
        static void MovePlayer(int Xadd, int Yadd)
        {
            countOfMoves++;

            if (playerX + Xadd >= height || playerX + Xadd < 0) return;
            if (playerY + Yadd >= width || playerY + Yadd < 0) return;

            if (field[playerX + Xadd, playerY + Yadd] is not '█')
            {
                field[playerX, playerY] = ' ';

                playerX += Xadd;
                playerY += Yadd;

                field[playerX, playerY] = 'X';
            }
        }
        static void Shoot()
        {
            countOfMoves++;

            DeleteObstaclePositive(1, 0);
            DeleteObstaclePositive(0, 1);

            DeleteObstacleNegative(-1, 0);
            DeleteObstacleNegative(0, -1);

            powerLeft--;
        }
        static void DeleteObstaclePositive(int addX, int addY)
        {
            if (addX > 0 ? playerX + addX < height : true && addY > 0 ? playerY + addY < width : true)
            {
                if (IsNotExit(addX, addY)) return;

                DeleteCellNearPlayer(addX, addY);
            }
            else return;              
        }
        static void DeleteObstacleNegative(int takeX, int takeY)
        {
            if (takeX < 0 ? playerX + takeX > 0 : true && takeY < 0 ? playerY + takeY > 0 : true)
            {
                if (IsNotExit(takeX, takeY)) return;

                DeleteCellNearPlayer(takeX, takeY);
            }
            else return;
        }
        static bool IsNotExit(int X, int Y)
        {
            return field[playerX + X, playerY + Y] is not '@';
        }
        static void DeleteCellNearPlayer(int X, int Y)
        {
            field[playerX + X, playerY + Y] = ' ';
        }

        static void CheckForPlayerPosition()
        {
            if(playerX == exitY && playerY == exitX)
            {
                finishedLevel = true;
            }
        }
        static void ResetLevel(bool givedUp = false)
        {
            finishedLevel = false;

            score += !givedUp ? 20f / countOfMoves + rand.Next(0, 2) : -1f;

            countOfMoves = 1;

            width += rand.Next(0, 5);
            height += rand.Next(0, 3);

            powerLeft = 3 + (int)(score / 5);

            Init();
        }
    }
}
