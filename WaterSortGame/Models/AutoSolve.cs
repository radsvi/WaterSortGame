using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class AutoSolve : ViewModelBase
    {
        MainWindowVM MainWindowVM;
        Notification Notification;
        //TreeNode<ValidMove> SolvingSteps;
        //TreeNode<ValidMove> FirstStep;
        private bool ResumeRequest { get; set; }
        [Obsolete]public int ResumeRequestCounterDebug { get; set; } = 0; // used only for debugging how many times I clicked the button and only triggering breakpoint upon certain number.
        //public List<ValidMove> CompleteSolution { get; private set; }
        private List<ValidMove> completeSolution;
        public List<ValidMove> CompleteSolution
        {
            get { return completeSolution; }
            set
            {
                if (value != completeSolution)
                {
                    completeSolution = value;
                    OnPropertyChanged();
                }
            }
        }
        public Action StepThrough;
        private int iterations = 0;
        public int Iterations
        {
            get { return iterations; }
            set
            {
                if (value != iterations)
                {
                    iterations = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DisplayIterations));
                }
            }
        }
        public Visibility DisplayIterations
        {
            get
            {
                if (iterations > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }
        public AutoSolve(MainWindowVM mainWindowVM)
        {
            MainWindowVM = mainWindowVM;
            Notification = mainWindowVM.Notification;
            StepThrough = StepThroughMethod;
        }
        private async void Start(LiquidColorNew[,] startingPosition)
        {
            bool debugVisualiseState = false; // ## smazat?
            //SolvingSteps = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            //FirstStep = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            var treeNode = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            treeNode.Data.StepNumber = -1000; // ## smazat. mam to tu jen abych oznacil prvni node pro debugovani
            //Dictionary<int, LinkedList<TreeNode<ValidMove>>> hashedSteps = new Dictionary<int, LinkedList<TreeNode<ValidMove>>>();
            CollisionDictionary<int, TreeNode<ValidMove>> hashedSteps = new CollisionDictionary<int, TreeNode<ValidMove>>();
            List<TreeNode<ValidMove>> debugList = new List<TreeNode<ValidMove>>(); // ## smazat
            //FirstStep.Data.GameState = startingPosition;
            //TreeNode<ValidMove> previousStep = FirstStep;
            //var gameState = startingPosition;
            //int Iterations = 0;
            while (true)
            {
                debugList.Add(treeNode);
                Iterations++;
                if (AskUserToContinue(treeNode, Iterations) == false) return;

                if (debugVisualiseState) await WaitForButtonPress();

                TreeNode<ValidMove> highestPriority_TreeNode = null;
                if (treeNode.Data.Closed == true)
                {
                    if (treeNode.Parent is null)
                        throw new Exception("treeNode.Parent is null for some reason");
                    treeNode = treeNode.Parent;
                    if (debugVisualiseState) MakeAMove(treeNode.Data);
                    Notification.Show($"{{{Iterations}}} Returning to previous move", MessageType.Debug);
                    if (debugVisualiseState) await WaitForButtonPress();

                    highestPriority_TreeNode = PickHighestPriorityNonVisitedNode(treeNode);

                    if (highestPriority_TreeNode.GetType() == typeof(NullTreeNode))
                    {
                        treeNode = highestPriority_TreeNode.Parent;
                        Notification.Show($"{{{Iterations}}} All siblings visited, returning to parent", MessageType.Debug);
                        continue;
                    }
                    else
                    {
                        treeNode = highestPriority_TreeNode;
                        Notification.Show($"{{{Iterations}}} Continuing with next child", MessageType.Debug);
                        if (debugVisualiseState) MakeAMove(treeNode.Data);
                        continue;
                    }
                }
                else
                {
                    var movableLiquids = GetMovableLiquids(treeNode.Data.GameState);

                    Debug.WriteLine("movableLiquids:");
                    foreach (var liquid in movableLiquids)
                        Debug.WriteLine($"[{liquid.X},{liquid.Y}] {{{treeNode.Data.GameState[liquid.X, liquid.Y].Name}}} {{{liquid.AllIdenticalLiquids}}} {{{liquid.NumberOfRepeatingLiquids}}}");

                    var emptySpots = GetEmptySpots(treeNode.Data.GameState, movableLiquids);
                    var validMoves = GetValidMoves(treeNode.Data.GameState, movableLiquids, emptySpots);

                    Debug.WriteLine("validMoves:");
                    foreach (var move in validMoves)
                        Debug.WriteLine($"[{move.Source.X},{move.Source.Y}] => [{move.Target.X},{move.Target.Y}] {{{treeNode.Data.GameState[move.Source.X, move.Source.Y].Name}}} {{HowMany {move.Source.NumberOfRepeatingLiquids}}}");

                    var mostFrequentColors = PickMostFrequentColor(movableLiquids); // ## tohle jsem jeste nezacal nikde pouzivat!

                    //RemoveUnoptimalMoves(validMoves, emptySpots);
                    RemoveEqualColorMoves(validMoves);
                    RemoveUselessMoves(validMoves);
                    //RemoveRepeatingMoves(validMoves, node);

                    RemoveSolvedTubesFromMoves(treeNode.Data.GameState, validMoves);

                    // Pro kazdy validMove vytvorim sibling ve strome:
                    CreateAllPossibleFutureStates(hashedSteps, treeNode, validMoves);

                    //if (validMoves.Count == 0)
                    if (UnvisitedChildrenExist(treeNode) == false)
                    {
                        if (MainWindowVM.GameState.IsLevelCompleted(treeNode.Data.GameState) is false)
                        {
                            treeNode.Data.Closed = true;
                            if (treeNode.Parent is not null)
                                treeNode.Parent.Data.Closed = true;

                            Notification.Show($"{{{Iterations}}} Reached a dead end.", MessageType.Debug);
                            continue;
                        }

                        break;
                    }

                    // Projdu vsechny siblingy a vyberu ten s nejvetsi prioritou:
                    highestPriority_TreeNode = PickHighestPriorityNonVisitedNode(treeNode.FirstChild);
                    treeNode = highestPriority_TreeNode;

                    if (treeNode.GetType() == typeof(NullTreeNode))
                    {
                        Notification.Show($"{{{Iterations}}} highestPriority_TreeNode is null, continuing.", MessageType.Debug);
                        continue;
                    }

                    if (debugVisualiseState) MakeAMove(treeNode.Data);
                }
            }
            BacktrackThroughAllSteps(treeNode!);
            if (Iterations >= 1000)
            {
                Notification.Show($"Reached {Iterations} steps. Interrupting", MessageType.Debug);
            }
            Notification.Show($"Total steps taken to generate: {Iterations}. Steps required to solve the puzzle {CompleteSolution.Count}", MessageType.Debug, 10000);
        }
        private bool AskUserToContinue(TreeNode<ValidMove> treeNode, int iterations)
        {
            if (iterations % 1000 == 0)
            {
                var msgResult = MessageBox.Show($"Already went through {iterations} iterations. Do you wish to continue?", "Do you wish to continue?", MessageBoxButton.YesNo);
                if (msgResult == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
        }
        private void BacktrackThroughAllSteps(TreeNode<ValidMove> treeNode)
        {
            CompleteSolution = new List<ValidMove>();

            while (treeNode.Parent is not null)
            {
                CompleteSolution.Add(treeNode.Data);
                treeNode = treeNode.Parent;
            }
            CompleteSolution.Reverse();
        }
        /// <summary>
        /// basically checks if there are any valid moves. If there is at least one children and it is unvisited, it returns true.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        private bool UnvisitedChildrenExist(TreeNode<ValidMove> parentNode)
        {
            if (parentNode.FirstChild is null)
            {
                return false;
            }

            var node = parentNode.FirstChild;
            while (node is not null)
            {
                if (node.Data.Closed is false && node.Data.Visited is false)
                {
                    return true;
                }

                node = node.NextSibling;
            }
            return false;
        }
        private void CreateAllPossibleFutureStates(CollisionDictionary<int, TreeNode<ValidMove>> hashedSteps, TreeNode<ValidMove> parentNode, List<ValidMove> validMoves)
        {
            var node = parentNode; // this is not a mistake. If I made "node" in the parameters then I would change the node on the outside!
            for (int i = 0; i < validMoves.Count; i++)
            {
                var nextNode = new TreeNode<ValidMove>(validMoves[i]);
                UpdateGameState(nextNode);

                if (GameStateAlreadyExists(hashedSteps, nextNode)) // excludes duplicate moves
                {
                    continue;
                }

                hashedSteps.Add(nextNode.Data.Hash, nextNode);

                if (parentNode.FirstChild is null)
                {
                    node.AddChild(nextNode);
                }
                else
                {
                    node.AddSibling(nextNode);
                }
                node = nextNode;
            }
        }
        private void UpdateGameState(TreeNode<ValidMove> node)
        {
            int j = 0;
            while (j < node.Data.Source.NumberOfRepeatingLiquids
                && node.Data.Target.Y + j < node.Data.GameState.GetLength(1)) // pocet stejnych barev na sobe source && uroven barvy v targetu
            {
                node.Data.GameState[node.Data.Target.X, node.Data.Target.Y + j] = node.Data.GameState[node.Data.Source.X, node.Data.Source.Y - j];
                node.Data.GameState[node.Data.Source.X, node.Data.Source.Y - j] = null;
                j++;
            }
            node.Data.UpdateHash();
        }
        private bool GameStateAlreadyExists(CollisionDictionary<int, TreeNode<ValidMove>> hashedSteps, TreeNode<ValidMove> nextNode)
        {
            if (hashedSteps.ContainsKey(nextNode.Data.Hash))
            {
                foreach (var hashItem in hashedSteps[nextNode.Data.Hash])
                {
                    if (hashItem.Data.Equals(nextNode.Data.GameState) && hashItem.Data.Visited == true) // pokud neni Visited == true tak jsem to jen vygeneroval jako dalsi krok, ale jeste nikdy neprozkoumal
                    {
                        Debug.WriteLine("Nasel jsem opakujici se stav!");
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        /// <summary>
        /// Checks siblings of provided node
        /// </summary>
        private TreeNode<ValidMove> PickHighestPriorityNonVisitedNode(TreeNode<ValidMove> node)
        {
            TreeNode<ValidMove> currentNode = node;
            TreeNode<ValidMove> resultNode = new NullTreeNode(node);
            while (currentNode != null)
            {
                if (currentNode.Data.Closed is false)
                {
                    resultNode = currentNode;
                    break; // i have got it sorted by highest priority, so first non-visited is fine
                }

                currentNode = currentNode.NextSibling;
            }

            if (resultNode.GetType() == typeof(NullTreeNode))
            {
                if (resultNode.Parent is not null) // null by mel byt jen v pripade ze jsme uplne na zacatku
                {
                    resultNode.Parent.Data.Closed = true;
                }
                resultNode.Data.SolutionValue = GetStepValue(resultNode.Data.GameState);
            }

            resultNode.Data.Visited = true; // have it here to prevent infinitely repeating gameStates like for example [1133],[-155] into - [-133],[1155]
            return resultNode;
        }
        private void MakeAMove(ValidMove node)
        {
            //Debug.WriteLine($"# [{node.Source.X},{node.Source.Y}] => [{node.Target.X},{node.Target.Y}] {{{node.Source.ColorName}}} {{HowMany {node.Source.NumberOfRepeatingLiquids}}}");
            
            //Node.Data.GameState = newGameState;

            //var upcomingStep = new SolutionStepsOLD(newGameState, Node.Data);
            //SolvingStepsOLD.Add(upcomingStep);

            //previousGameState = node.Data.GameState; // tohle je gamestate kterej uchovavam jen uvnitr autosolvu
            MainWindowVM.GameState.SetGameState(node.GameState);

            MainWindowVM.DrawTubes();
            MainWindowVM.OnChangingGameState();
        }
        /// <summary>
        /// Determines how close we are to a solution. Higher value means closer to a solution
        /// </summary>
        private int GetStepValue(LiquidColorNew[,] gameState)
        {
            int solutionValue = 0;
            
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                LiquidColorName? lastColor = null;
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

                        //if (IsThisRepeatingMove(gameState, move)) continue;
                        validMoves.Add(move);
                        continue;
                    }

                    if (gameState[liquid.X, liquid.Y].Name == gameState[emptySpot.X, emptySpot.Y - 1].Name) // if target is the same color
                    {
                        var move = new ValidMove(liquid, emptySpot, gameState, true);

                        //if (IsThisRepeatingMove(gameState, move)) continue;
                        validMoves.Add(move);
                    }
                }
            }

            return validMoves;
        }
        private void RemoveRepeatingMoves(List<ValidMove> validMoves, TreeNode<ValidMove> parentNode)
        {
            //bool first = true;
            //do
            //{
            //    if (first)
            //    {
            //        first = false;
            //    }
            //    else
            //    {
            //        parentOfParentNode = parentOfParentNode.Parent;
            //        if (parentOfParentNode is null) continue;
            //    }
            //    if (AreStatesIdentical(upcomingState, parentOfParentNode.Data.GameState)) return true;
            //} while (parentOfParentNode is not null);


            for (int i = validMoves.Count() - 1; i >= 0; i--)
            {
                //if (parent == null) return;

                //var upcomingState = CloneGrid(parentNode.Data.GameState);
                //upcomingState[parentNode.Data.Target.X, parentNode.Data.Target.Y] = upcomingState[parentNode.Data.Source.X, parentNode.Data.Source.Y];
                //upcomingState[parentNode.Data.Source.X, parentNode.Data.Source.Y] = null;

                var parentOfParentNode = parentNode.Parent;
                if (parentOfParentNode is null) return; // pokud nema parent znamena ze je prvni a tim padem se ani nemuze opakovat
                if (AreStatesIdentical(validMoves[i].GameState, parentOfParentNode.Data.GameState))
                {
                    validMoves.Remove(validMoves[i]);
                }
            }
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
        //private LiquidColorNew[,] CloneGrid(LiquidColorNew[,] gameState)
        //{
        //    return MainWindowVM.GameState.CloneGrid(gameState);
        //}
        private (bool, int) AreAllLayersIdentical(LiquidColorNew[,] gameState, int x, int y)
        {
            if (y == 0) return (true, 1); // jen jedna tekutina, takze dycky musi byt "vsechny"(jedna) stejny

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
        private List<KeyValuePair<LiquidColorName, int>> PickMostFrequentColor(List<PositionPointer> movableLiquids)
        {


            //Tuple<LiquidColorNames, int>[] colorCount = new Tuple<LiquidColorNames, int>[LiquidColorNew.ColorKeys.Count()];
            //for (int i = 0; i < LiquidColorNew.ColorKeys.Count; i++)
            //    colorCount[i] = new Tuple<LiquidColorNames, int>(LiquidColorNew.ColorKeys[i].Name, 0);

            Dictionary<LiquidColorName, int> colorCount = new Dictionary<LiquidColorName, int>();
            foreach (var colorItem in LiquidColorNew.ColorKeys)
            {
                //colorCount.Add(new KeyValuePair<LiquidColorNames, int>(colorItem.Name, 0));
                colorCount.Add(colorItem.Name, 0);
            }


            foreach (var liquid in movableLiquids)
            {
                colorCount[(LiquidColorName)liquid.ColorName]++;
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
        /// <summary>
        /// Removes moves that has, as as source, tubes that are already solved.
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="validMoves"></param>
        private void RemoveSolvedTubesFromMoves(LiquidColorNew[,] gameState, List<ValidMove> validMoves)
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
        private async Task WaitForButtonPress()
        {
            while (ResumeRequest is false)
            {
                await Task.Delay(100);
            }
            ResumeRequest = false;
        }
        public void CalculateNextStep(LiquidColorNew[,] gameState)
        {
            ResumeRequest = true; // provede se i pri prvnim spusteni, protoze je pauza na zacatku
            ResumeRequestCounterDebug++;
            if (MainWindowVM.UIEnabled == true) // disable UI once starting the Auto Solve process
            {
                MainWindowVM.UIEnabled = false;
                Start(gameState);
                return;
            }
            
            //Vertices[1].CalculateGValue(Vertices.First());
            
        }

        public async void StepThroughMethod()
        {
            ResumeRequest = false;
            StepThrough = () => ResumeRequest = true;
            foreach (var validMove in CompleteSolution)
            {
                MakeAMove(validMove);
                await WaitForButtonPress();
            }
        }
        #endregion
        #region debug
        private List<int> ListHashedSteps(CollisionDictionary<int, TreeNode<ValidMove>> hashedSteps)
        {
            List<int> result = [];
            foreach (var itemList in hashedSteps.DebugData)
            {
                foreach (var item in itemList.Value)
                {
                    result.Add(item.Data.StepNumber);
                }
            }
            return result;
        }
        #endregion
    }
}
