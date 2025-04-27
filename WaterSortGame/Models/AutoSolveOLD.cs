using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class AutoSolveOLD
    {
        MainWindowVM MainWindowVM;
        //LiquidColorNew[,] StartingPosition;
        //[Obsolete]List<SolutionStepsOLD> SolvingStepsOLD;
        TreeNode<ValidMove> SolvingSteps;
        TreeNode<ValidMove> CurrentNode;
        public AutoSolveOLD(MainWindowVM mainWindowVM, LiquidColorNew[,] startingPosition)
        {
            MainWindowVM = mainWindowVM;
            //StartingPosition = startingPosition;
            //SolvingStepsOLD = new List<SolutionStepsOLD>();

            SolvingSteps = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            CurrentNode = SolvingSteps;
        }
        public void Start(LiquidColorNew[,] gameState)
        {
            //// pokud je to uplny zacatek tak vytvorim TreeNode
            //if (SolvingSteps == null)
            //{
            //    SolvingSteps = new TreeNode<ValidMove>(new ValidMove(gameState));
            //}
            
            var movableLiquids = GetMovableLiquids(gameState);

            Debug.WriteLine("movableLiquids:");
            foreach (var liquid in movableLiquids)
                Debug.WriteLine($"[{liquid.X},{liquid.Y}] {{{gameState[liquid.X, liquid.Y].Name}}} {{{liquid.SingleColor}}}");
            
            var emptySpots = GetEmptySpots(gameState, movableLiquids);
            var validMoves = GetValidMoves(gameState, movableLiquids, emptySpots);

            Debug.WriteLine("validMoves:");
            foreach (var move in validMoves)
                Debug.WriteLine($"[{move.Source.X},{move.Source.Y}] => [{move.Target.X},{move.Target.Y}] {{{gameState[move.Source.X, move.Source.Y].Name}}}");

            var mostFrequentColors = PickMostFrequentColor(movableLiquids);

            //RemoveUnoptimalMoves(validMoves, emptySpots);
            RemoveUnoptimalMoves(validMoves);
            PickPreferentialMoves(gameState, validMoves);
            if (validMoves.Count == 0)
            {
                MessageBox.Show("No valid move");
                return;
            }
            //foreach (var move in validMoves)
            //    Debug.WriteLine($"[{move.Source.X},{move.Source.Y}] => [{move.Target.X},{move.Target.Y}] {{{gameState[move.Source.X, move.Source.Y].Name}}}");


            // Pro kazdy validMove vytvorim sibling ve strome:
            TreeNode<ValidMove> nextNode;
            for (int i = 0; i < validMoves.Count; i++)
            {
                nextNode = new TreeNode<ValidMove>(validMoves[i]);
                if (i == 0)
                {
                    SolvingSteps.AddChild(nextNode);
                }
                else
                {
                    SolvingSteps.AddSibling(nextNode);
                }
            }
            // Projdu vsechny siblingy a vyberu ten s nejvetsi prioritou:
            TreeNode<ValidMove> highestPriorityNode = CurrentNode;
            while (CurrentNode.NextSibling != null)
            {
                CurrentNode = CurrentNode.NextSibling;
                if (highestPriorityNode.Data.Priority < CurrentNode.Data.Priority)
                {
                    highestPriorityNode = CurrentNode;
                }
            }

            //var nextMove = validMoves.MaxBy(x => x.Priority); // tohle budu muset zmenit. je to jen docasne dokud nemam implementovany vraceni zpatky ve stromu
            MakeAMove(gameState, highestPriorityNode);
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

                    var currentItem = new PositionPointer(gameState, x, y);
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
        /// <summary>
        /// Used to later pick moves that are able to move most colors at once
        /// </summary>
        private List<KeyValuePair<LiquidColorNames, int>> PickMostFrequentColor(List<PositionPointer> movableLiquids)
        {
            

            //Tuple<LiquidColorNames, int>[] colorCount = new Tuple<LiquidColorNames, int>[LiquidColorNew.ColorKeys.Count()];
            //for (int i = 0; i < LiquidColorNew.ColorKeys.Count; i++)
            //    colorCount[i] = new Tuple<LiquidColorNames, int>(LiquidColorNew.ColorKeys[i].Name, 0);

            Dictionary<LiquidColorNames, int> colorCount = new Dictionary<LiquidColorNames, int>();
            foreach (var colorItem in LiquidColorNew.ColorKeys)
            {
                //colorCount.Add(new KeyValuePair<LiquidColorNames, int>(colorItem.Name, 0));
                colorCount.Add(colorItem.Name, 0);
            }


            foreach (var liquid in movableLiquids)
            {
                colorCount[(LiquidColorNames)liquid.ColorName]++;
            }
            //var colorCountSorted = (Dictionary<LiquidColorNames, int>)from entry in colorCount orderby entry.Value ascending select entry;
            //var colorCountSorted = from entry in colorCount orderby entry.Value ascending select entry;
            //var colorCountSorted = colorCount.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            var mostFrequentColors = colorCount.OrderByDescending(x => x.Value).ToList();

            


            //Tuple<LiquidColorNames, int>[] colorCountSortedTuple = new Tuple<LiquidColorNames, int>[LiquidColorNew.ColorKeys.Count()];


            //foreach (var liquid in movableLiquids)
            //{
            //    if (!mostFrequentColors.Exists(x => x.ColorName == liquid.ColorName))
            //    {
            //        mostFrequentColors.Add(liquid);
            //    }
            //}

            return mostFrequentColors;
        }
        private bool AreAllLayersIdentical(LiquidColorNew[,] gameState, int x, int y)
        {
            if (y == 0) return false;

            if (y == 1) return true;

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
                        emptySpots.Add(new PositionPointer(gameState, x, y));
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

                    if (emptySpot.Y == 0) // if target is empty tube
                    {
                        var move = new ValidMove(liquid, emptySpot, gameState);

                        if (IsThisRepeatingMove(gameState, move)) continue;
                        validMoves.Add(move);
                        continue;
                    }

                    if (gameState[liquid.X, liquid.Y].Name == gameState[emptySpot.X, emptySpot.Y - 1].Name) // if target is the same color
                    {
                        var move = new ValidMove(liquid, emptySpot, gameState, true);

                        if (IsThisRepeatingMove(gameState, move)) continue;
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


        /// <summary>
        /// If there are multiple moves for the same color, and in one of them the target is singleColor tube, always choose that one.
        /// </summary>
        //private void RemoveUnoptimalMoves(List<ValidMove> validMoves, List<PositionPointer> emptySpots)
        private void RemoveUnoptimalMoves(List<ValidMove> validMoves)
        {
            //var singleColorTargets = emptySpots.Exists((move) => move.SingleColor == true);
            //var colorsWithSingleColorTargets = validMoves.Exists((move) => move.IsTargetSingleColor == true);

            var colorsWithSingleColorTargets = validMoves.Where((move) => move.IsTargetSingleColor == true).ToList();
            //List<ValidMove> colorsWithSingleColorTargets = validMoves.Where((move) => move.IsTargetSingleColor == true) as List<ValidMove>;
            //if (colorsWithSingleColorTargets.Count() == 0) return;
            if (colorsWithSingleColorTargets is null) return;

            for (int i = validMoves.Count() - 1; i >= 0; i--)
            {
                var move = validMoves[i];
                // if for current liquid color is already a move that targets SingleColor tube eliminate all other possible moves
                if (colorsWithSingleColorTargets.Exists((liquid) => liquid.Liquid == move.Liquid)
                    && move.IsTargetSingleColor == false)
                {
                    validMoves.Remove(move);
                }
            }
        }
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
        private void MakeAMove(LiquidColorNew[,] gameState, TreeNode<ValidMove> highestPriorityNode)
        {
            var currentState = MainWindowVM.GameState.CloneGrid(gameState);

            gameState[highestPriorityNode.Data.Target.X, highestPriorityNode.Data.Target.Y] =
                gameState[highestPriorityNode.Data.Source.X, highestPriorityNode.Data.Source.Y];
            gameState[highestPriorityNode.Data.Source.X, highestPriorityNode.Data.Source.Y] = null;

            var upcomingStep = new SolutionStepsOLD(currentState, highestPriorityNode.Data);
            //SolvingStepsOLD.Add(upcomingStep);


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
        private bool IsThisRepeatingMove(LiquidColorNew[,] gameState, ValidMove move)
        {
            //if (SolvingStepsOLD.Count <= 1) return false;
            if (SolvingSteps.Parent == null) return false;

            var upcomingState = CloneGrid(gameState);
            upcomingState[move.Target.X, move.Target.Y] = upcomingState[move.Source.X, move.Source.Y];
            upcomingState[move.Source.X, move.Source.Y] = null;

            //var previousState = SolvingSteps.Last().PreviousStep.Grid;
            //var previousState = SolvingStepsOLD.Last();
            var previousState = SolvingSteps.Parent;
            bool first = true;
            do {
                if (first)
                {
                    first = false;
                }
                else
                {
                    previousState = previousState.Parent;
                    if (previousState is null) continue;
                }
                if (AreStatesIdentical(upcomingState, previousState.Data.GameState)) return true;
            } while (previousState is not null);
            
            return false;
        }
        private bool AreStatesIdentical(LiquidColorNew[,] first, LiquidColorNew[,] second)
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
            //for (int y = 0; y < grid.GetLength(1); y++)
            for (int y = grid.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = grid.GetLength(0) - 1; x >= 0; x--) 
                {
                    if (grid[x, y] == null)
                    {
                        Debug.Write($"____          \t");
                    } else
                    {
                        string indent = new String(' ', 14 - (grid[x, y].Name.ToString().Length)); ;
                        Debug.Write($"{grid[x, y].Name}{indent}\t");
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
