using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraWZycie.Models
{
    public class Game
    {
        public bool[,] Cells { get; set; }
        public int GenerationCount { get; private set; } = 0;
        public int DeathCount { get; private set; } = 0;
        public int BirthCount { get; private set; } = 0;
        private int Rows { get; }
        private int Cols { get; }
        private ISet<int> NeighboursToStayAlive { get; set; }
        private ISet<int> NeighboursToBirth { get; set; }



        public Game(int rows, int cols, string rules)
        {
            Cells = new bool[rows, cols];
            Rows = rows;
            Cols = cols;
            ParseRules(rules);
        }

        public void CalculateNewGeneration()
        {
            bool[,] newGeneration = new bool[Rows, Cols];
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    bool newState = CalculateCellState(row, col);
                    newGeneration[row, col] = newState;
                    bool currentState = Cells[row, col];
                    if (currentState && !newState) DeathCount++;
                    else if (!currentState && newState) BirthCount++;
                }
            }

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
    }
}
