using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class SolutionSteps
    {
        public SolutionSteps? PreviousStep { get; set; }
        public LiquidColorNew[,] Grid { get; set; }
        public ValidMove Move { get; set; }
        public SolutionSteps(LiquidColorNew[,] grid, ValidMove move, SolutionSteps? previousStep = null)
        {
            Grid = GameState.CloneGrid(grid, true);
            PreviousStep = previousStep;
            Move = move;
        }
    }
}
