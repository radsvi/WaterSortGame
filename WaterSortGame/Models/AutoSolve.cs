using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public void Start(LiquidColorNew[,] gameState)
        {
            var movableLiquids = GetMovableLiquids(gameState);
            
            foreach (var liquid in movableLiquids)
                Debug.WriteLine($"[{liquid.X},{liquid.Y}] {{{gameState[liquid.X, liquid.Y].Name}}} {{{liquid.SingleColor}}}");
            
            var emptySpots = GetEmptySpots(gameState, movableLiquids);
            var validMoves = GetValidMoves(gameState, movableLiquids, emptySpots);

            Debug.WriteLine("validMoves:");
            foreach (var move in validMoves)
                Debug.WriteLine($"[{move.Source.X},{move.Source.Y}] => [{move.Target.X},{move.Target.Y}] {{{gameState[move.Source.X, move.Source.Y].Name}}}");

            PickPreferentialMoves(gameState, validMoves);

            if (validMoves.Count == 0)
            {
                MessageBox.Show("No valid move");
                return;
            }


            MakeAMove(gameState, validMoves[0], (SolvingSteps.Count > 0) ? SolvingSteps.Last() : null);
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

            for (int internalY = 0; internalY < y; internalY++)
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
                        var move = new ValidMove(liquid, emptySpot, gameState);
                        //if (IsThisRepeatingMove(move)) continue;
                        
                        var upcomingState = CloneGrid(gameState);
                        upcomingState[move.Target.X, move.Target.Y] = upcomingState[move.Source.X, move.Source.Y];
                        upcomingState[move.Source.X, move.Source.Y] = null;

                        if (IsThisRepeatingMove(upcomingState)) continue;
                        validMoves.Add(move);
                        continue;
                    }

                    if (gameState[liquid.X, liquid.Y].Name == gameState[emptySpot.X, emptySpot.Y - 1].Name)
                    {
                        var move = new ValidMove(liquid, emptySpot, gameState);
                        //if (IsThisRepeatingMove(move)) continue;
                        if (IsThisRepeatingMove(gameState)) continue;
                        validMoves.Add(move);
                    }
                }
            }

            return validMoves;
        }
        /// <summary>
        /// If we already have a tube with 3 colors and there is a possibility to add 4th one, pick that choice
        /// </summary>
        //private List<ValidMove> PickPreferentialMoves(LiquidColorNew[,] gameState, List<ValidMove> validMoves)
        private void PickPreferentialMoves(LiquidColorNew[,] gameState, List<ValidMove> validMoves)
        {
            //var preferentialMoves = new List<ValidMove>();
            
            List<int> tubeNumbers = new List<int>();
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                if (gameState[x, 0] == null || gameState[x, 1] == null || gameState[x, 2] == null || gameState[x, 3] == null) continue;

                if (gameState[x, 0].Name == gameState[x, 1].Name && gameState[x, 0].Name == gameState[x, 2].Name && gameState[x, 0].Name == gameState[x, 3].Name)
                {
                    tubeNumbers.Add(x);
                }
            }
            if (tubeNumbers.Count == 0) return;

            for (int i = validMoves.Count() - 1; i >= 0; i--)
            {
                if (tubeNumbers.Contains(validMoves[i].Source.X))
                {
                    validMoves.Remove(validMoves[i]);
                }
            }


            //return preferentialMoves;
        }
        private void MakeAMove(LiquidColorNew[,] gameState, ValidMove move, SolutionSteps previousStepReferer = null)
        {
            var currentState = MainWindowVM.GameState.CloneGrid(gameState);

            gameState[move.Target.X, move.Target.Y] = gameState[move.Source.X, move.Source.Y];
            gameState[move.Source.X, move.Source.Y] = null;

            var upcomingStep = new SolutionSteps(currentState, move, previousStepReferer);
            SolvingSteps.Add(upcomingStep);


            MainWindowVM.GameState.SetGameState(gameState);

            MainWindowVM.DrawTubes();
            MainWindowVM.OnChangingGameState();
        }
        private LiquidColorNew[,] CloneGrid(LiquidColorNew[,] gameState)
        {
            return MainWindowVM.GameState.CloneGrid(gameState);
        }
        //private bool IsThisRepeatingMove(LiquidColorNew[,] gameState, SolutionSteps previousStepReferer)
        //{
        //    if (gameState == previousStepReferer.PreviousStep.Grid)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //private bool IsThisRepeatingMove(ValidMove move)
        //{
        //    if (SolvingSteps.Count <= 1) return false;

        //    var lastMove = SolvingSteps.Last().PreviousStep.Move;
        //    if (lastMove == move) return true;

        //    return false;
        //}
        private bool IsThisRepeatingMove(LiquidColorNew[,] gameState)
        {
            if (SolvingSteps.Count <= 1) return false;

            //var previousState = SolvingSteps.Last().PreviousStep.Grid;
            var previousState = SolvingSteps.Last();
            bool first = true;
            do {
                if (first)
                {
                    first = false;
                }
                else
                {
                    previousState = previousState.PreviousStep;
                    if (previousState is null) continue;
                }
                if (AreStatesSame(gameState, previousState.Grid)) return true;
            } while (previousState is not null);
            
            return false;
        }
        private bool AreStatesSame(LiquidColorNew[,] first, LiquidColorNew[,] second)
        {
            //DebugGrid(first, "currentGamestate");
            //DebugGrid(second, "previousStep");



            for (int x = 0; x < first.GetLength(0); x++)
            {
                for (int y = 0; y < first.GetLength(1); y++)
                {
                    if (first[x, y] == null || second[x, y] == null)
                    {
                        if (first[x, y] == second[x, y])
                            continue;

                        return false;
                    }

                    if (first[x, y].Name == second[x, y].Name)
                    {
                        continue;
                    }
                    return false;

                    //if (first[x, y] == second[x, y]) return true;
                }
            }
            return true;
        }
        private void DebugGrid(LiquidColorNew[,] grid, string header)
        {
            Debug.WriteLine("=====================================================");
            Debug.WriteLine(header);
            Debug.WriteLine("-----------------------------------------------------");
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++) 
                {
                    if (grid[x, y] == null)
                    {
                        Debug.Write($"____\t");
                    } else
                    {
                        Debug.Write($"{grid[x, y].Name}\t");
                    }
                }
                Debug.WriteLine("");
            }
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
