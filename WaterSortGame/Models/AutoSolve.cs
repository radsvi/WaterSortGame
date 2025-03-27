using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class AutoSolve
    {
        MainWindowVM MainWindowVM;
        //LiquidColorNew[,] StartingPosition;
        List<SolutionSteps> SolvingSteps;

        public AutoSolve(MainWindowVM mainWindowVM, LiquidColorNew[,] startingPosition)
        {
            MainWindowVM = mainWindowVM;
            //StartingPosition = startingPosition;
            SolvingSteps = new List<SolutionSteps>();
        }
        public void Start(LiquidColorNew[,] gameState, LiquidColorNew[,] previousStep)
        {
            var movableLiquids = GetMovableLiquids(gameState);
            //foreach (var liquid in movableLiquids)
            //    Debug.WriteLine($"[{liquid.X},{liquid.Y}] {{{gameState[liquid.X, liquid.Y].Name}}} {{{liquid.SingleColor}}}");
            var emptySpots = GetEmptySpots(gameState, movableLiquids);
            var validMoves = GetValidMoves(gameState, movableLiquids, emptySpots);

            //foreach (var move in validMoves)
            //    Debug.WriteLine($"[{move.Source.X},{move.Source.Y}] => [{move.Target.X},{move.Target.Y}] {{{gameState[move.Source.X, move.Source.Y].Name}}}");

            MakeAMove(gameState, validMoves[0]);
        }
        /// <summary>
        /// Picks topmost liquid from each tube, but excludes tubes that are already solved
        /// </summary>
        private List<PositionPointer> GetMovableLiquids(LiquidColorNew[,] gameState)
        {
            //var pointer = new List<Tuple<int, int>>();
            var pointer = new List<PositionPointer>();
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                for (int y = gameState.GetLength(1) - 1; y >= 0; y--)
                {
                    if (gameState[x, y] == null) continue;

                    var currentItem = new PositionPointer(x, y);
                    if (AreAllLayersIdentical(gameState, x, y) == true)
                    {
                        if (y == gameState.GetLength(1) - 1)
                            break;

                        currentItem.SingleColor = true;
                    }
                    
                    pointer.Add(currentItem);
                    break;
                }
            }
            return pointer;
        }
        private bool AreAllLayersIdentical(LiquidColorNew[,] gameState, int x, int y)
        {
            if (y == 0) return false;

            for (int internalY = 0; internalY < y - 1; internalY++)
            {
                if (gameState[x, internalY].Name != gameState[x, internalY + 1].Name) return false;
            }

            return true;
        }
        private List<PositionPointer> GetEmptySpots(LiquidColorNew[,] gameState, List<PositionPointer> movableLiquids)
        {
            var emptySpots = new List<PositionPointer>();
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                for (int y = 0; y < gameState.GetLength(1); y++)
                {
                    if (gameState[x, y] == null)
                    {
                        emptySpots.Add(new PositionPointer(x, y));
                        break;
                    }
                }
            }
            return emptySpots;
        }
        private List<ValidMove> GetValidMoves(LiquidColorNew[,] gameState, List<PositionPointer> movableLiquids, List<PositionPointer> emptySpots)
        {
            var validMoves = new List<ValidMove>();
            foreach (var liquid in movableLiquids)
            {
                foreach (var emptySpot in emptySpots)
                {
                    if (liquid.X == emptySpot.X)
                        continue;

                    if (liquid.SingleColor == true && emptySpot.Y == 0) // skip moving already sorted tubes to empty spot, even when they are not full yet.
                        continue;

                    if (emptySpot.Y == 0)
                    {
                        validMoves.Add(new ValidMove(liquid, emptySpot));
                        continue;
                    }

                    if (gameState[liquid.X, liquid.Y].Name == gameState[emptySpot.X, emptySpot.Y - 1].Name)
                        validMoves.Add(new ValidMove(liquid, emptySpot));
                }
            }

            return validMoves;
        }
        private void MakeAMove(LiquidColorNew[,] gameState, ValidMove move)
        {
            var previousStep = MainWindowVM.GameState.CloneGrid(gameState);
            gameState[move.Target.X, move.Target.Y] = gameState[move.Source.X, move.Source.Y];
            gameState[move.Source.X, move.Source.Y] = null;
            var currentStep = new SolutionSteps(gameState, previousStep);

            SolvingSteps.Add(currentStep);

            MainWindowVM.GameState.SetGameState(gameState);

            MainWindowVM.DrawTubes();
        }
        //private void xxx(LiquidColorNew[,] gameState)
        //{
        //    for (int x = 0; x < gameState.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < gameState.GetLength(1); y++)
        //        {

        //        }
        //    }
        //}
    }
}
