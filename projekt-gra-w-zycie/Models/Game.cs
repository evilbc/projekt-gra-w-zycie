using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraWZycie.Models
{
    public class Game
    {
        private const string Separator = ",";
        public bool[,] Cells { get; set; }
        public int GenerationCount { get; private set; }
        public int DeathCount => _deathCount;
        public int BirthCount => _birthCount;
        public int Rows { get; }
        public int Cols { get; }
        private ISet<int> NeighboursToStayAlive { get; set; }
        private ISet<int> NeighboursToBirth { get; set; }
        private string Rules { get; }
        private int _deathCount;
        private int _birthCount;



        public Game(int rows, int cols, string rules)
        {
            Cells = new bool[rows, cols];
            Rows = rows;
            Cols = cols;
            Rules = rules;
            ParseRules(rules);
        }

        public void CalculateNewGeneration()
        {
            bool[,] newGeneration = new bool[Rows, Cols];
            var tasks = new List<Task>();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    int row = i;
                    int col = j;
                    tasks.Add(Task.Run(() =>
                    {
                        bool newState = CalculateCellState(row, col);
                        newGeneration[row, col] = newState;
                        bool currentState = Cells[row, col];
                        if (currentState && !newState)
                            Interlocked.Increment(ref _deathCount);
                        else if (!currentState && newState)
                            Interlocked.Increment(ref _birthCount);
                    }));
                    
                }
            }

            Task.WhenAll(tasks).Wait();
            Cells = newGeneration;
            GenerationCount++;
        }

        private bool CalculateCellState(int row, int col)
        {
            int aliveNeighbours = CountAliveNeighbours(row, col);

            if (Cells[row, col])
                return NeighboursToStayAlive.Contains(aliveNeighbours);
            else
                return NeighboursToBirth.Contains(aliveNeighbours);

        }

        private int CountAliveNeighbours(int row, int col)
        {
            int counter = 0;

            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i != row || j != col)
                        counter += CountCell(i, j);
                }
            }

            return counter;
        }

        private int CountCell(int row, int col)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Cols) return 0;

            return Cells[row, col] ? 1 : 0;
        }

        private void ParseRules(string rules)
        {
            NeighboursToBirth = rules[1..rules.IndexOf('/')].Select(c => int.Parse(c.ToString())).ToHashSet();
            NeighboursToStayAlive = rules[(rules.IndexOf('S') + 1)..].Select(c => int.Parse(c.ToString())).ToHashSet();
        }

        public string CreateSaveState()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Rules);
            sb.Append(Separator).Append(Rows);
            sb.Append(Separator).Append(Cols);
            for (int row = 0; row<Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    sb.Append(Separator).Append(Cells[row, col]);
                }
            }

            return sb.ToString();
        }

        public static Game LoadSaveState(string save)
        {
            var arr = save.Split(Separator);
            string rules = arr[0];
            int rows = int.Parse(arr[1]);
            int cols = int.Parse(arr[2]);
            var game = new Game(rows, cols, rules);
            int index = 3;
            
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    game.Cells[row, col] = bool.Parse(arr[index++]);
                }
            }

            return game;
        }
    }
}
