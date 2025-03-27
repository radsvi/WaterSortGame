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
        public SolutionSteps(LiquidColorNew[,] grid, SolutionSteps? previousStep = null)
        {
            Grid = GameState.CloneGrid(grid, true);
            PreviousStep = previousStep;
        }
    }
}
