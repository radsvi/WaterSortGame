using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    [Obsolete]
    internal class SolutionStepsOld
    {
        public SolutionStepsOld? PreviousStep { get; set; }
        public LiquidColorNew[,] Grid { get; set; }
        public SolutionStep Move { get; set; }
        public SolutionStepsOld(LiquidColorNew[,] grid, SolutionStep move, SolutionStepsOld? previousStep = null)
        {
            Grid = GameState.CloneGrid(grid, true);
            PreviousStep = previousStep;
            Move = move;
        }
    }
}
