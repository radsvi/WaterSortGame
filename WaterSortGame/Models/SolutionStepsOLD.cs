using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class SolutionStepsOLD
    {
        public SolutionStepsOLD? PreviousStep { get; set; }
        public LiquidColorNew[,] Grid { get; set; }
        public ValidMove Move { get; set; }
        public SolutionStepsOLD(LiquidColorNew[,] grid, ValidMove move, SolutionStepsOLD? previousStep = null)
        {
            Grid = GameState.CloneGridStatic(grid);
            Move = move;
            PreviousStep = previousStep;
        }
    }
}
