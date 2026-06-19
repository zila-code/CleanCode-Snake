using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame
{
    // Používám Enum místo textových řetězců ("UP", "DOWN").
    // Zabraňuje to překlepům a kompilátor nás upozorní na chyby.
    public enum Direction { Up, Down, Left, Right }

    // Třída s jedinou zodpovědností.
    // Slouží pouze jako datový nosič pro souřadnice v prostoru. Neřeší barvy ani konzoli.
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // JÁDRO HRY
    // Decoupling: Tato třída je zcela nezávislá na konzoli.
    // Neobsahuje jediné volání System.Console. Řeší jen čistou matematiku a pravidla hry.
    public class SnakeEngine
    {
        // Vlastnosti (Properties) jsou zapouzdřené. Zvenku je lze jen číst (private set),
        // takže nám stav hry nemůže přepsat nikdo zvenčí.
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }

        public List<Position> Body { get; private set; }
        public Position Food { get; private set; }
        public Direction CurrentDirection { get; set; }

        private Random _random;

        public SnakeEngine(int width, int height)
        {
            Width = width;
            Height = height;
            Score = 5;
            IsGameOver = false;
            CurrentDirection = Direction.Right;
            _random = new Random();

            // Inicializace hada uprostřed hracího pole
            Body = new List<Position> { new Position(width / 2, height / 2) };
            SpawnFood();
        }

        // Metoda, která posune hru o jeden "krok" dopředu.
        public void UpdateGameStep()
        {
            if (IsGameOver) return;

            Position head = Body.Last();
            int nextX = head.X;
            int nextY = head.Y;

            // Výpočet nové pozice hlavy podle aktuálního směru
            switch (CurrentDirection)
            {
                case Direction.Up: nextY--; break;
                case Direction.Down: nextY++; break;
                case Direction.Left: nextX--; break;
                case Direction.Right: nextX++; break;
            }

            Position newHead = new Position(nextX, nextY);

            // Kontrola kolize se zdí (hrací pole je od 0 do Width-1 / Height-1)
            if (newHead.X <= 0 || newHead.X >= Width - 1 || newHead.Y <= 0 || newHead.Y >= Height - 1)
            {
                IsGameOver = true;
                return;
            }

            // Kontrola kolize s vlastním tělem (zda nová hlava leží na existujícím článku)
            if (Body.Any(part => part.X == newHead.X && part.Y == newHead.Y))
            {
                IsGameOver = true;
                return;
            }

            // Posun hada: přidáme novou hlavu do seznamu
            Body.Add(newHead);

            // Zjištění, zda hlava narazila na jídlo
            if (newHead.X == Food.X && newHead.Y == Food.Y)
            {
                Score++;
                SpawnFood(); // Vygenerujeme nové jídlo, ocas neumažeme (had vyroste)
            }
            else
            {
                // Pokud had nic nesnědl, musíme smazat poslední článek ocasu,
                // aby se zachovala jeho aktuální délka.
                if (Body.Count > Score)
                {
                    Body.RemoveAt(0);
                }
            }
        }

        // Clean Code: Extrakce logiky do menší, privátní metody.
        // Udržuje kód přehlednější.
        private void SpawnFood()
        {
            // Vygeneruje jídlo náhodně, ale tak, aby neleželo ve zdi
            int x = _random.Next(1, Width - 2);
            int y = _random.Next(1, Height - 2);
            Food = new Position(x, y);
        }
    }

    // PREZENTAČNÍ VRSTVA
    // Stará se pouze o to, jak se hra zobrazí uživateli a jak se ovládá.
    class Program
    {
        static void Main()
        {
            // Clean Code: Odstranění "Magic Numbers" (tvrdě zakódovaných čísel v kódu).
            // Rozměry si uložíme do proměnných hned na začátku.
            int width = 32;
            int height = 16;

            // Nastavení okna konzole
            Console.WindowWidth = width;
            Console.WindowHeight = height;
            Console.CursorVisible = false; // Skryje blikající kurzor pro lepší vzhled

            // Vytvoření instance herní logiky
            SnakeEngine game = new SnakeEngine(width, height);

            // HERNÍ SMYČKA 
            // Rozdělená do tří jasných kroků: Vstup, Zpracování, Vykreslení.
            while (!game.IsGameOver)
            {
                HandleInput(game);
                game.UpdateGameStep();
                Render(game);

                // Kontroluje rychlost hry (čím menší číslo, tím rychlejší had)
                Thread.Sleep(350);
            }

            // Kód pro ukončení hry po nárazu
            Console.Clear();
            Console.SetCursorPosition(width / 5, height / 2);
            Console.WriteLine($"Game over, Score: {game.Score}");

            // OPRAVA: Původně zde bylo jen "height", což vyvolalo výjimku, 
            // protože maximální index řádku je výška okna minus 1.
            Console.SetCursorPosition(0, height - 1);
        }

        // Zpracování vstupu z klávesnice
        static void HandleInput(SnakeEngine game)
        {
            // Pokud nebyla stisknuta žádná klávesa, nic nedělej
            if (!Console.KeyAvailable) return;

            var key = Console.ReadKey(true).Key;

            // Změna směru. Podmínky zajišťují, že had nemůže nacouvat sám do sebe
            // (např. pokud jede nahoru, nemůže se otočit rovnou dolů).
            if (key == ConsoleKey.UpArrow && game.CurrentDirection != Direction.Down)
                game.CurrentDirection = Direction.Up;
            else if (key == ConsoleKey.DownArrow && game.CurrentDirection != Direction.Up)
                game.CurrentDirection = Direction.Down;
            else if (key == ConsoleKey.LeftArrow && game.CurrentDirection != Direction.Right)
                game.CurrentDirection = Direction.Left;
            else if (key == ConsoleKey.RightArrow && game.CurrentDirection != Direction.Left)
                game.CurrentDirection = Direction.Right;
        }

        // Vykreslení aktuálního stavu hry do konzole
        static void Render(SnakeEngine game)
        {
            // Vymaže předchozí snímek
            Console.Clear();

            // Vykreslení horní a dolní zdi
            Console.ForegroundColor = ConsoleColor.White;
            for (int x = 0; x < game.Width; x++)
            {
                DrawPixel(x, 0, "■");
                DrawPixel(x, game.Height - 1, "■");
            }

            // Vykreslení levé a pravé zdi
            for (int y = 0; y < game.Height; y++)
            {
                DrawPixel(0, y, "■");
                DrawPixel(game.Width - 1, y, "■");
            }

            // Vykreslení jídla
            Console.ForegroundColor = ConsoleColor.Cyan;
            DrawPixel(game.Food.X, game.Food.Y, "■");

            // Vykreslení těla hada (všechno kromě posledního prvku = hlavy)
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < game.Body.Count - 1; i++)
            {
                DrawPixel(game.Body[i].X, game.Body[i].Y, "■");
            }

            // Vykreslení hlavy hada (zvýrazněno červeně pro lepší orientaci)
            Position head = game.Body.Last();
            Console.ForegroundColor = ConsoleColor.Red;
            DrawPixel(head.X, head.Y, "■");
        }

        // Pomocná metoda pro čistší zápis vykreslování znaku na konkrétní pozici
        static void DrawPixel(int x, int y, string symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(symbol);
        }
    }
}
