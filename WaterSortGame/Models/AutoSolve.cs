using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class AutoSolve
    {
        MainWindowVM MainWindowVM;
        TreeNode<ValidMove> SolvingSteps;
        TreeNode<ValidMove> FirstStep;
        public bool ResumeRequest { get; set; }
        public AutoSolve(MainWindowVM mainWindowVM)
        {
            MainWindowVM = mainWindowVM;

        }
        public async void Start(LiquidColorNew[,] startingPosition)
        {
            SolvingSteps = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            FirstStep = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            FirstStep.Data.GameState = startingPosition;
            TreeNode<ValidMove> lastStep = FirstStep;
            //var gameState = startingPosition;
            while (true) // ## dodelat aby skoncilo kdyz nejsou zadny mozny nody s Visited == false
            {
                
                
                
                //if () // tady bude podminka kdyz se vracim o uroven vys na parent
                //{



                //    await WaitForContinueButton();
                //    continue;
                //}
                
                
                var movableLiquids = GetMovableLiquids(lastStep.Data.GameState);

                Debug.WriteLine("movableLiquids:");
                foreach (var liquid in movableLiquids)
                    Debug.WriteLine($"[{liquid.X},{liquid.Y}] {{{lastStep.Data.GameState[liquid.X, liquid.Y].Name}}} {{{liquid.AllIdenticalLiquids}}} {{{liquid.NumberOfRepeatingLiquids}}}");

                var emptySpots = GetEmptySpots(lastStep.Data.GameState, movableLiquids);
                var validMoves = GetValidMoves(lastStep.Data.GameState, movableLiquids, emptySpots);

                Debug.WriteLine("validMoves:");
                foreach (var move in validMoves)
                    Debug.WriteLine($"[{move.Source.X},{move.Source.Y}] => [{move.Target.X},{move.Target.Y}] {{{lastStep.Data.GameState[move.Source.X, move.Source.Y].Name}}} {{HowMany {move.Source.NumberOfRepeatingLiquids}}}");

                var mostFrequentColors = PickMostFrequentColor(movableLiquids);

                //RemoveUnoptimalMoves(validMoves, emptySpots);
                RemoveEqualColorMoves(validMoves); // ## oddelat movy ktery jen prehazujou treba z 2 modrych na 1 modrou.
                RemoveUselessMoves(validMoves);

                PickPreferentialMoves(lastStep.Data.GameState, validMoves);
                if (validMoves.Count == 0)
                {
                    MessageBox.Show("No valid move");
                    return;
                }

                // Pro kazdy validMove vytvorim sibling ve strome:
                TreeNode<ValidMove> nextNode;
                TreeNode<ValidMove> previousNode = lastStep;
                for (int i = 0; i < validMoves.Count; i++)
                {
                    nextNode = new TreeNode<ValidMove>(validMoves[i]);
                    if (i == 0)
                    {
                        previousNode.AddChild(nextNode);
                    }
                    else
                    {
                        previousNode.AddSibling(nextNode);
                    }
                    previousNode = nextNode;
                }

                // Projdu vsechny siblingy a vyberu ten s nejvetsi prioritou:
                TreeNode<ValidMove> currentNode = lastStep.FirstChild;
                if (currentNode == null)
                {
                    Debug.WriteLine("musim se vratit na parent"); // ## dodelat
                    break;
                }
                var highestPriorityNode = PickHighestPriority(currentNode);


                MakeAMove(highestPriorityNode, lastStep.Data.GameState);
                lastStep = highestPriorityNode;
                await WaitForContinueButton();
            }
        }

        private TreeNode<ValidMove> PickHighestPriority(TreeNode<ValidMove>? currentNode)
        {
            
            TreeNode<ValidMove> highestPriorityNode = currentNode;
            currentNode = currentNode.NextSibling;
            while (currentNode != null)
            {
                if (highestPriorityNode.Data.Priority < currentNode.Data.Priority)
                {
                    highestPriorityNode = currentNode;
                }
                currentNode = currentNode.NextSibling;
            }
            highestPriorityNode.Data.GameState = CloneGrid(highestPriorityNode.Parent.Data.GameState);
            highestPriorityNode.Data.SolutionValue = GetSolutionValue(highestPriorityNode.Parent.Data.GameState);
            //if (highestPriorityNode.Data.MaxSolutionValue < highestPriorityNode.Data.SolutionValue)
            //    highestPriorityNode.Data.MaxSolutionValue = highestPriorityNode.Data.SolutionValue;

            return highestPriorityNode;
        }

        private void MakeAMove(TreeNode<ValidMove> Node, LiquidColorNew[,] previousGameState)
        {
            Debug.WriteLine($"# [{Node.Data.Source.X},{Node.Data.Source.Y}] => [{Node.Data.Target.X},{Node.Data.Target.Y}] {{{Node.Data.GameState[Node.Data.Source.X, Node.Data.Source.Y].Name}}} {{HowMany {Node.Data.Source.NumberOfRepeatingLiquids}}}");

            //LiquidColorNew[,] newGameState = CloneGrid(previousGameState); // #### tady to zrusit, pridavam to uz driv. a dal uz pouzivat jen "Node.Data.GameState"
            var numberOfRepeatingLiquids = Node.Data.Source.NumberOfRepeatingLiquids;
            int i = 0;
            while (i < numberOfRepeatingLiquids && (Node.Data.Target.Y + i < Node.Data.GameState.GetLength(1))) // pocet stejnych barev na sobe source && uroven barvy v targetu
            {
                Node.Data.GameState[Node.Data.Target.X, Node.Data.Target.Y + i] = Node.Data.GameState[Node.Data.Source.X, Node.Data.Source.Y - i];
                Node.Data.GameState[Node.Data.Source.X, Node.Data.Source.Y - i] = null;
                i++;
            }
            
            //Node.Data.GameState = newGameState;
            Node.Visited = true;

            //var upcomingStep = new SolutionStepsOLD(newGameState, Node.Data);
            //SolvingStepsOLD.Add(upcomingStep);

            previousGameState = Node.Data.GameState; // tohle je gamestate kterej uchovavam jen uvnitr autosolvu
            MainWindowVM.GameState.SetGameState(Node.Data.GameState);

            MainWindowVM.DrawTubes();
            MainWindowVM.OnChangingGameState();
        }
        /// <summary>
        /// Determines how close we are to a solution. Higher value means closer to a solution
        /// </summary>
        private int GetSolutionValue(LiquidColorNew[,] gameState)
        {
            int solutionValue = 0;
            
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                LiquidColorNames? lastColor = null;
                for (int y = 0; y < gameState.GetLength(1); y++)
                {
                    if (gameState[x, y] is null)
                    {
                        continue;
                    }
                    
                    if (lastColor is null)
                    {
                        lastColor = gameState[x, y].Name;
                        continue;
                    }

                    if (lastColor == gameState[x, y].Name)
                    {
                        solutionValue++;
                    }
                    else
                    {
                        lastColor = gameState[x, y].Name;
                    }
                }
            }
            return solutionValue;
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
                    (bool allIdenticalLiquids, int numberOfRepeatingLiquids) = AreAllLayersIdentical(gameState, x, y);
                    currentItem.NumberOfRepeatingLiquids = numberOfRepeatingLiquids;
                    pointer.Add(currentItem);
                    if (allIdenticalLiquids == true)
                    {
                        currentItem.AllIdenticalLiquids = true;
                    }
                    break;
                }
            }
            return pointer;
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
                    if (liquid.X == emptySpot.X) // if source and target is the same tube
                        continue;

                    if (liquid.AllIdenticalLiquids == true && emptySpot.Y == 0) // skip moving already sorted tubes to empty spot, even when they are not full yet.
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
            do
            {
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
                    }
                    else
                    {
                        string indent = new String(' ', 14 - (grid[x, y].Name.ToString().Length)); ;
                        Debug.Write($"{grid[x, y].Name}{indent}\t");
                    }
                }
                Debug.WriteLine("");
            }
        }
        private LiquidColorNew[,] CloneGrid(LiquidColorNew[,] gameState)
        {
            return MainWindowVM.GameState.CloneGrid(gameState);
        }
        private (bool, int) AreAllLayersIdentical(LiquidColorNew[,] gameState, int x, int y)
        {
            if (y == 0 || y == 1) return (true, 1);

            //if (y == 1) return (true, 1);

            int numberOfRepeatingLiquids = 1;
            for (int internalY = y; internalY > 0; internalY--)
            {
                if (gameState[x, internalY].Name != gameState[x, internalY - 1].Name)
                {
                    return (false, numberOfRepeatingLiquids);
                }
                numberOfRepeatingLiquids++;
            }

            return (true, numberOfRepeatingLiquids);
        }
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
        /// <summary>
        /// If there are multiple moves for the same color, and in one of them the target is singleColor tube, always choose that one.
        /// </summary>
        //private void RemoveUnoptimalMoves(List<ValidMove> validMoves, List<PositionPointer> emptySpots)
        private void RemoveEqualColorMoves(List<ValidMove> validMoves)
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
        /// <summary>
        /// Removes moves that doesnt actually solve anything. For example 3 blue and 1 empty into another 3 blue and 1 empty.
        /// </summary>
        /// <param name="validMoves"></param>
        private void RemoveUselessMoves(List<ValidMove> validMoves)
        {
            for (int i = validMoves.Count() - 1; i >= 0; i--)
            {
                var move = validMoves[i];
                if (move.IsTargetSingleColor && move.Target.Y == 0)
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

        #region Controls
        private async Task WaitForContinueButton()
        {
            while (ResumeRequest is false)
            {
                await Task.Delay(100);
            }
            ResumeRequest = false;
        }
        public void CalculateNextStep(LiquidColorNew[,] gameState)
        {
            if (MainWindowVM.UIDisabled == false) // disable UI once starting the Auto Solve process
            {
                MainWindowVM.UIDisabled = true;
                Start(gameState);
                return;
            }
            
            //Vertices[1].CalculateGValue(Vertices.First());
            ResumeRequest = true;
        }
        #endregion
    }
}
