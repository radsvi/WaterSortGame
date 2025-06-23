using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using WaterSortGame.ViewModels;
using WaterSortGame.Views.UserControls;

namespace WaterSortGame.Models
{
    internal class AutoSolve : ViewModelBase
    {
        MainWindowVM mainWindowVM;
        Notification notification;
        readonly string exportLogFilename;
        //TreeNode<ValidMove> SolvingSteps;
        //TreeNode<ValidMove> FirstStep;
        private bool ResumeRequest { get; set; }
        //[Obsolete]public int ResumeRequestCounterDebug { get; set; } = 0; // used only for debugging how many times I clicked the button and only triggering breakpoint upon certain number.
        //public List<ValidMove> CompleteSolution { get; private set; }
        private ObservableCollection<ValidMove> completeSolution = new ObservableCollection<ValidMove>();
        public ObservableCollection<ValidMove> CompleteSolution
        {
            get { return completeSolution; }
            set
            {
                if (value != completeSolution)
                {
                    completeSolution = value;
                    
                }
            }
        }
        private Action stepThrough;
        public Action StepThrough { get => stepThrough; set => stepThrough = value; }
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
        private int currentSolutionStep = 0;
        public int CurrentSolutionStep { get => currentSolutionStep; set { currentSolutionStep = value; OnPropertyChanged(); } }
        public bool LimitToOneStep { get; set; } = false; // When true - makes the AutoSolve generate only one step for each press of the button and visualises the changes. When false - generates whole solution
        public AutoSolve(MainWindowVM mainWindowVM)
        {
            this.mainWindowVM = mainWindowVM;
            notification = mainWindowVM.Notification;
            StepThrough += StepThroughMethod;
            exportLogFilename = this.mainWindowVM.logFolderName + "/Export-AutoSolve-" + DateTime.Now.ToString("MMddyyyy-HH.mm.ss") + ".log";
        }
        private async void Start(LiquidColor[,] startingPosition)
        {
            //var notificationType = MessageType.Debug;
            var notificationType = MessageType.Hidden;
            var startTime = DateTime.Now;
            //SolvingSteps = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            //FirstStep = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            var treeNode = new TreeNode<ValidMove>(new ValidMove(startingPosition));
            treeNode.Data.StepNumber = -1;
            treeNode.Data.UpdateHash();
            //Dictionary<int, LinkedList<TreeNode<ValidMove>>> hashedSteps = new Dictionary<int, LinkedList<TreeNode<ValidMove>>>();
            CollisionDictionary<int, TreeNode<ValidMove>> hashedSteps = new CollisionDictionary<int, TreeNode<ValidMove>>();
#if DEBUG            
            List<TreeNode<ValidMove>> debugList = new List<TreeNode<ValidMove>>();
#endif
            //FirstStep.Data.GameState = startingPosition;
            //TreeNode<ValidMove> previousStep = FirstStep;
            //var gameState = startingPosition;
            //Iterations = 0;

            while (true)
            {
                await Task.Delay(1);
#if DEBUG
                debugList.Add(treeNode);
#endif
                Debug_WriteToFileAutoSolveSteps(treeNode, $"[{hashedSteps.DebugData.Count}] Visiting node");
                Iterations++;
                
#if DEBUG
                if (LimitToOneStep) { MakeAMove(treeNode.Data); await WaitForButtonPress(); }
#endif

                TreeNode<ValidMove> highestPriority_TreeNode = null;
                if (treeNode.Data.FullyVisited == true)
                {
                    if (treeNode.Parent is null)
                    {
                        notification.Show("Tried all branches, and didn't find a solution!", notificationType, 60000); // this should rarely happen.
                        break;
                    }
                        
                    treeNode = treeNode.Parent;
                    
                    notification.Show($"{{{Iterations}}} Returning to previous move", notificationType);
                }
                else if (treeNode.Data.FullyVisited == false && treeNode.Data.Visited == true)
                {
                    highestPriority_TreeNode = PickHighestPriorityNonVisitedChild(treeNode);

                    if (highestPriority_TreeNode.GetType() == typeof(NullTreeNode))
                    {
                        highestPriority_TreeNode.Parent!.Data.FullyVisited = true;
                        treeNode = highestPriority_TreeNode.Parent;
                        notification.Show($"{{{Iterations}}} All siblings visited, marking parent as FullyVisited", notificationType);
                        continue;
                    }
                    else
                    {
                        treeNode = highestPriority_TreeNode;
                        notification.Show($"{{{Iterations}}} Continuing with next child", notificationType); // continuing to child generated in previous loop iteration
                        continue;
                    }
                }
                else
                {
                    var topPriorityNode = PickNeverincorectMovesFirst(treeNode, hashedSteps);
                    if (topPriorityNode.GetType() != typeof(NullTreeNode))
                    {
                        treeNode = topPriorityNode;
                        continue; // vygeneroval jsem dalsi stav, takze zbytek preskakuju
                    }

                    (var movableLiquids, var mostFrequentColors) = GetMovableLiquids(treeNode.Data.GameState);

                    Debug_IterateThroughList(movableLiquids, "movableLiquids:", (liquid) => $"[{liquid.X},{liquid.Y}] {{{treeNode.Data.GameState[liquid.X, liquid.Y].Name}}} {{{liquid.AllIdenticalLiquids}}} {{{liquid.NumberOfRepeatingLiquids}}}");

                    var emptySpots = GetEmptySpots(treeNode.Data.GameState, movableLiquids);
                    var validMoves = GetValidMoves(treeNode.Data.GameState, movableLiquids, emptySpots);

                    //if (treeNode.Data.StepNumber == -1 // i.e. first node
                    //    || treeNode.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Data.StepNumber == -1
                    //    || treeNode.Parent is not null && treeNode.Parent.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Parent.Data.StepNumber == -1
                    //    || treeNode.Parent.Parent is not null && treeNode.Parent.Parent.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Parent.Parent.Data.StepNumber == -1)
                    //if (treeNode.Data.StepNumber == -1 // i.e. first node
                    //    || treeNode.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Data.StepNumber == -1
                    //    || treeNode.Data.MoveType == MoveType.NeverWrong && treeNode.Parent is not null && treeNode.Parent.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Parent is not null && treeNode.Parent.Parent.Data.StepNumber == -1
                    //    || treeNode.Data.MoveType == MoveType.NeverWrong && treeNode.Parent is not null && treeNode.Parent.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Parent is not null && treeNode.Parent.Parent.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Parent.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Parent.Parent is not null && treeNode.Parent.Parent.Parent.Data.StepNumber == -1)
                    //if (treeNode.Data.StepNumber == -1 // i.e. first node
                    //    || treeNode.Data.MoveType == MoveType.NeverWrong && (treeNode.Parent.Data.StepNumber == -1
                    //        || treeNode.Parent is not null && treeNode.Parent.Data.MoveType == MoveType.NeverWrong && (treeNode.Parent.Parent.Data.StepNumber == -1
                    //            || treeNode.Parent.Parent is not null && treeNode.Parent.Parent.Data.MoveType == MoveType.NeverWrong && treeNode.Parent.Parent.Parent.Data.StepNumber == -1)))
                    if (treeNode.Data.StepNumber == -1) // i.e. first node
                    {
                        validMoves = OrderList(validMoves, mostFrequentColors);
                        //validMoves = validMoves.OrderByDescending(x => x.Priority).ToList();
                    }

                    Debug_IterateThroughList(validMoves, "validMoves:", (move) => $"[{move.Source.X},{move.Source.Y}] => [{move.Target.X},{move.Target.Y}] {{{treeNode.Data.GameState[move.Source.X, move.Source.Y].Name}}} {{HowMany {move.Source.NumberOfRepeatingLiquids}}}");

                    //var mostFrequentColors = PickMostFrequentColor(movableLiquids); // ## tohle jsem jeste nezacal nikde pouzivat!

                    RemoveEqualColorMoves(validMoves);
                    RemoveUselessMoves(validMoves);
                    RemoveSolvedTubesFromMoves(treeNode.Data.GameState, validMoves);

                    // Pro kazdy validMove vytvorim sibling ve strome:
                    CreateAllPossibleFutureStates(hashedSteps, treeNode, validMoves); // also checks for repeating moves

                    if (UnvisitedChildrenExist(treeNode) == false)
                    {
                        if (mainWindowVM.GameState.IsLevelCompleted(treeNode.Data.GameState) is false)
                        {
                            treeNode.Data.FullyVisited = true;
                            //if (treeNode.Parent is not null)
                            //    treeNode.Parent.Data.Visited = true;

                            notification.Show($"{{{Iterations}}} Reached a dead end.", notificationType);
                            continue;
                        }

                        break;
                    }

                    // Projdu vsechny siblingy a vyberu ten s nejvetsi prioritou:
                    highestPriority_TreeNode = PickHighestPriorityNonVisitedChild(treeNode);
                    treeNode = highestPriority_TreeNode;

                    if (treeNode.GetType() == typeof(NullTreeNode))
                    {
                        notification.Show($"{{{Iterations}}} highestPriority_TreeNode is null, continuing.", notificationType);
                        continue;
                    }
#if DEBUG
                    if (LimitToOneStep) MakeAMove(treeNode.Data);
#endif
                }
            }
            var duration = DateTime.Now.Subtract(startTime);
            CreateListOfSteps(treeNode!);
            if (CompleteSolution.Count > 0)
                notification.Show($"Total states taken to generate: {Iterations}. Steps required to solve the puzzle {CompleteSolution.Count}. Duration: {duration.TotalSeconds} seconds", MessageType.Debug, 60000);
            else 
                notification.Show($"Total states taken to generate: {Iterations}. Puzzle wasn't solved, something went wrong ({CompleteSolution.Count} steps generated). Duration: {duration.TotalSeconds} seconds", MessageType.Debug, 60000);
        }
        private List<ValidMove> OrderList(List<ValidMove> validMoves, ColorCount mostFrequentColors)
        {
            int priorityNum = 100;
            foreach (var color in mostFrequentColors)
            {
                foreach (var validMove in validMoves)
                {
                    if (color.Key == validMove.Source.ColorName)
                    {
                        validMove.Priority = priorityNum;
                    }
                }
                priorityNum--;
            }
            return validMoves.OrderByDescending(x => x.Priority).ToList();
        }
        private TreeNode<ValidMove> PickNeverincorectMovesFirst(TreeNode<ValidMove> parentNode, CollisionDictionary<int, TreeNode<ValidMove>> hashedSteps) // dat to hned na zacatek jeste nez delam valid move a podobny veci
        { // name implies that its not always the best or optimal move, but its never wrong. Can at worst generate one extra move, but at best remove whole branch and cut the whole solution tree in half if its early on in the solution.
            if (HasEmptyTubes(parentNode.Data.GameState).Count == 0)
            {
                return new NullTreeNode(parentNode);
            }
            
            var singleColorTubeList = HasSingleColorTube(parentNode.Data.GameState);
            if (singleColorTubeList.Count() == 0)
            {
                return new NullTreeNode(parentNode);
            }

            (var dualColorTube, var singleColorTube) = HasCorrespondingDualColorTube(parentNode.Data.GameState, singleColorTubeList);
            if (dualColorTube is null)
            {
                return new NullTreeNode(parentNode);
            }

            var newGameState = mainWindowVM.GameState.CloneGrid(parentNode.Data.GameState);
            var validMove = new ValidMove(dualColorTube, singleColorTube, newGameState, MoveType.NeverWrong);

            return GeneratePriorityFutureState(parentNode, validMove, hashedSteps);
        }
        private TreeNode<ValidMove> GeneratePriorityFutureState(TreeNode<ValidMove> parentNode, ValidMove validMove, CollisionDictionary<int, TreeNode<ValidMove>> hashedSteps)
        {
            parentNode.Data.Visited = true; // have it here to prevent infinitely repeating gameStates like for example [1133],[-155] into - [-133],[1155]. The Visited state is checked while generating new moves in 'CreateAllPossibleFutureStates'

            ForceChangeGameState(validMove);

            var nextNode = new TreeNode<ValidMove>(validMove);
            nextNode.Data.UpdateHash();

            if (GameStateAlreadyExists(hashedSteps, nextNode)) // excludes duplicate moves
            {
                return new NullTreeNode(parentNode);
            }

            hashedSteps.Add(nextNode.Data.Hash, nextNode);
            parentNode.AddChild(nextNode);

            return nextNode;
        }
        private void ForceChangeGameState(ValidMove validMove)
        {
            // singleColorTube is target
            // dualColorTube is source

            //for (int y = validMove.GameState.GetLength(1) - 2; y >= 0 ; y--)
            //{
            //    validMove.GameState[validMove.Target.X, y] = validMove.GameState[validMove.Target.X, y + 1];
            //}
            for (int y = 0; y < validMove.GameState.GetLength(1); y++)
            {
                if (validMove.GameState[validMove.Target.X, y] is null) 
                {
                    validMove.GameState[validMove.Target.X, y] = validMove.GameState[validMove.Source.X, validMove.Source.Y];
                }
            }
            //newGameState[singleColorTube.X, 0] = null;

            //validMove.GameState[validMove.Target.X, 0] = validMove.GameState[validMove.Source.X, validMove.Source.Y];
            for (int y = 0; y < validMove.GameState.GetLength(1) - 1; y++)
            {
                validMove.GameState[validMove.Source.X, y] = validMove.GameState[validMove.Source.X, y + 1];
            }
            validMove.GameState[validMove.Source.X, validMove.GameState.GetLength(1) - 1] = null;
        }
        private List<int> HasEmptyTubes(LiquidColor[,] gameState)
        {
            List<int> emptyTubes = new List<int>();
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                if (gameState[x, 0] is null)
                {
                    emptyTubes.Add(x);
                }
            }
            return emptyTubes;
        }
        /// <summary>
        /// Looks for tube that has format such as [1112] where 2 is the same color number as is the singleColorTube
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="singleColorTubeList"></param>
        /// <returns></returns>
        private (PositionPointer?, PositionPointer?) HasCorrespondingDualColorTube(LiquidColor[,] gameState, List<PositionPointer> singleColorTubeList)
        {
            foreach (var singleColorTube in singleColorTubeList)
            {
                for (int x = 0; x < gameState.GetLength(0); x++)
                {
                    if (gameState[x, 0] is null) break;
                    if (singleColorTube is not null && singleColorTube.X == x) continue;

                    if (singleColorTube is not null && gameState[x, 0] is not null
                        && singleColorTube.ColorName == gameState[x, 0].Name
                        && gameState[x, 1] is not null
                        && gameState[x, 1].Name != singleColorTube.ColorName)
                    {
                        bool dualColorTube = true;
                        LiquidColorName colorName = gameState[x, 1].Name;

                        // check if remaining liquids of that same tube are all the same color:
                        for (int y = 2; y < gameState.GetLength(1); y++) // ano, fakt to ma byt y=2. 0 je jine barvy a pak porovnavam 1-3 jestli jsou stejny. Kdyz jsou tak je to skutecne dual-color-tube
                        {
                            if (gameState[x, y] is null)
                            {
                                break;
                            }
                            if (colorName != gameState[x, y].Name)
                            {
                                dualColorTube = false;
                                break;
                            }
                        }
                        if (dualColorTube)
                        {
                            return (new PositionPointer(gameState[x, 0].Name, x, 0, 0), singleColorTube);
                        }
                    }
                }
            }
            return (null, null);
        }
        /// <summary>
        /// Determines whether this is a move that should always be taken because it is always correct. Such as moving [--12][-222] into [---1][2222]
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        private List<PositionPointer> HasSingleColorTube(LiquidColor[,] gameState)
        {
            List<PositionPointer> singleColorTubes = new List<PositionPointer>();
            //List<SimpleTube> singleColorTubes = new List<SimpleTube>();
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                if (gameState[x, 0] is null) continue;
                
                bool isSingleColor = true;
                int y;
                for (y = 0; y < gameState.GetLength(1) - 1; y++)
                {
                    if (gameState[x, y] is null)
                    {
                        isSingleColor = false;
                        break;
                    }
                    if (gameState[x, y + 1] is null) 
                    {
                        break;
                    }

                    if (gameState[x, y].Name != gameState[x, y + 1].Name)
                    {
                        isSingleColor = false;
                        break;
                    }
                }
                var singleColorTube = new PositionPointer(gameState[x, 0].Name, x, 0, y);
                singleColorTube.AllIdenticalLiquids = true;
                if (isSingleColor) singleColorTubes.Add(singleColorTube);
            }
            return singleColorTubes;
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
        private void CreateListOfSteps(TreeNode<ValidMove> treeNode)
        {
            while (treeNode.Parent is not null)
            {
                CompleteSolution.Add(treeNode.Data);
                treeNode = treeNode.Parent;
            }
            CurrentSolutionStep = CompleteSolution.Count;
        }
        /// <summary>
        /// basically checks if there are any valid moves. If there is at least one children and it is unvisited, it returns true.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        private bool UnvisitedChildrenExist(TreeNode<ValidMove> parentNode)
        {
            var node = parentNode.FirstChild;
            while (node is not null)
            {
                if (node.Data.FullyVisited is false && node.Data.Visited is false)
                {
                    return true;
                }

                node = node.NextSibling;
            }
            return false;
        }
        private void CreateAllPossibleFutureStates(CollisionDictionary<int, TreeNode<ValidMove>> hashedSteps, TreeNode<ValidMove> parentNode, List<ValidMove> validMoves)
        {
            parentNode.Data.Visited = true; // have it here to prevent infinitely repeating gameStates like for example [1133],[-155] into - [-133],[1155]. The Visited state is checked while generating new moves in 'CreateAllPossibleFutureStates'
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

                Debug_WriteToFileAutoSolveSteps(nextNode, $"[{hashedSteps.DebugData.Count}][{nextNode.Data.Hash}] Generating node");

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
            if (node == parentNode) // pokud se ani jeden node nepridal protoze byly vsechno duplikaty:
            {
                parentNode.Data.FullyVisited = true;
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
                    if (hashItem.Data.Equals(nextNode.Data.GameState) && hashItem.Data.Visited == true) // pokud neni Visited == true tak jsem to jen vygeneroval jako dalsi krok, ale jeste nikdy neprozkoumal, takze stale muzu pouzit
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
        private TreeNode<ValidMove> PickHighestPriorityNonVisitedChild(TreeNode<ValidMove> parentNode)
        {
            TreeNode<ValidMove> node = parentNode.FirstChild;
            TreeNode<ValidMove> resultNode = new NullTreeNode(parentNode);
            while (node is not null)
            {
                if (node.Data.FullyVisited is false && node.Data.Visited is false)
                {
                    resultNode = node;
                    break; // i have got it sorted by highest priority, so first non-visited is fine
                }

                node = node.NextSibling;
            }

            if (resultNode.GetType() == typeof(NullTreeNode)) // tohle znamena - prosel jsem vsechno a nenasel jsem zadnej unvisited child
            {
                if (resultNode.Parent is not null) // null by mel byt jen v pripade ze jsme uplne na zacatku
                {
                    resultNode.Parent.Data.Visited = true;
                    resultNode.Parent.Data.FullyVisited = true;
                }
                resultNode.Data.SolutionValue = GetStepValue(resultNode.Data.GameState);
            }

            return resultNode;
        }
        private void MakeAMove(ValidMove move)
        {
            //Debug.WriteLine($"# [{node.Source.X},{node.Source.Y}] => [{node.Target.X},{node.Target.Y}] {{{node.Source.ColorName}}} {{HowMany {node.Source.NumberOfRepeatingLiquids}}}");
            
            //Node.Data.GameState = newGameState;

            //var upcomingStep = new SolutionStepsOLD(newGameState, Node.Data);
            //SolvingStepsOLD.Add(upcomingStep);

            //previousGameState = node.Data.GameState; // tohle je gamestate kterej uchovavam jen uvnitr autosolvu
            mainWindowVM.GameState.SetGameState(move.GameState);
            var currentTubeReference = new TubeReference(
                move.Target.X,
                move.GameState[move.Target.X, move.Target.Y],
                move.Target.Y,
                move.Source.NumberOfRepeatingLiquids
            );
            mainWindowVM.DrawTubes();

            mainWindowVM.RippleSurfaceAnimation(currentTubeReference);
            mainWindowVM.OnChangingGameState();
        }
        /// <summary>
        /// Determines how close we are to a solution. Higher value means closer to a solution
        /// </summary>
        private int GetStepValue(LiquidColor[,] gameState)
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
        private (List<PositionPointer>, ColorCount) GetMovableLiquids(LiquidColor[,] gameState)
        {
            //var pointer = new List<Tuple<int, int>>();
            var colorCount = new ColorCount();
            var pointer = new List<PositionPointer>();
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                for (int y = gameState.GetLength(1) - 1; y >= 0; y--)
                {
                    if (gameState[x, y] == null) continue;

                    var currentItem = new PositionPointer(gameState, x, y);
                    colorCount.AddColor(gameState[x, y].Name);
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
            colorCount.OrderDesc();

            return (pointer, colorCount);
        }
        private List<PositionPointer> GetEmptySpots(LiquidColor[,] gameState, List<PositionPointer> movableLiquids)
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
        private List<ValidMove> GetValidMoves(LiquidColor[,] gameState, List<PositionPointer> movableLiquids, List<PositionPointer> emptySpots)
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
        private void DebugGrid(LiquidColor[,] grid, string header)
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
        private (bool, int) AreAllLayersIdentical(LiquidColor[,] gameState, int x, int y)
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
        //private List<KeyValuePair<LiquidColorName, int>> PickMostFrequentColor(List<PositionPointer> movableLiquids)
        //{


        //    //Tuple<LiquidColorNames, int>[] colorCount = new Tuple<LiquidColorNames, int>[LiquidColorNew.ColorKeys.Count()];
        //    //for (int i = 0; i < LiquidColorNew.ColorKeys.Count; i++)
        //    //    colorCount[i] = new Tuple<LiquidColorNames, int>(LiquidColorNew.ColorKeys[i].Name, 0);

        //    Dictionary<LiquidColorName, int> colorCount = new Dictionary<LiquidColorName, int>();
        //    foreach (var colorItem in LiquidColor.ColorKeys)
        //    {
        //        //colorCount.Add(new KeyValuePair<LiquidColorNames, int>(colorItem.Name, 0));
        //        colorCount.Add(colorItem.Value.Name, 0);
        //    }


        //    foreach (var liquid in movableLiquids)
        //    {
        //        colorCount[(LiquidColorName)liquid.ColorName]++;
        //    }
        //    //var colorCountSorted = (Dictionary<LiquidColorNames, int>)from entry in colorCount orderby entry.Value ascending select entry;
        //    //var colorCountSorted = from entry in colorCount orderby entry.Value ascending select entry;
        //    //var colorCountSorted = colorCount.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        //    var mostFrequentColors = colorCount.OrderByDescending(x => x.Value).ToList();




        //    //Tuple<LiquidColorNames, int>[] colorCountSortedTuple = new Tuple<LiquidColorNames, int>[LiquidColorNew.ColorKeys.Count()];


        //    //foreach (var liquid in movableLiquids)
        //    //{
        //    //    if (!mostFrequentColors.Exists(x => x.ColorName == liquid.ColorName))
        //    //    {
        //    //        mostFrequentColors.Add(liquid);
        //    //    }
        //    //}

        //    return mostFrequentColors;
        //}
        /// <summary>
        /// If there are multiple moves for the same color, and in one of them the target is singleColor tube, always choose that one.
        /// </summary>
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
        private void RemoveSolvedTubesFromMoves(LiquidColor[,] gameState, List<ValidMove> validMoves)
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

        public void CalculateNextStep(LiquidColor[,] gameState)
        {
            //Notification.Show("Game grid locked while automatic solution is engaged",MessageType.Information, 10000);
            ResumeRequest = true; // provede se i pri prvnim spusteni, protoze je pauza na zacatku
            //ResumeRequestCounterDebug++;
            if (mainWindowVM.UIEnabled == true) // disable UI once starting the Auto Solve process
            {
                mainWindowVM.UIEnabled = false;
                Start(gameState);
                return;
            }
        }
        private async Task WaitForButtonPress()
        {
            while (ResumeRequest is false)
            {
                await Task.Delay(100);
            }
            ResumeRequest = false;
        }
        //public async void StepThroughMethod()
        //{
        //    ResumeRequest = false;
        //    StepThrough = () => ResumeRequest = true;
        //    for (CurrentSolutionStep = CompleteSolution.Count - 1; CurrentSolutionStep >= 0; CurrentSolutionStep--)
        //    {
        //        MakeAMove(CompleteSolution[CurrentSolutionStep]);
        //        //CompleteSolution.Remove(CompleteSolution[CurrentSolutionStep]);
        //        await WaitForButtonPress();
        //    }
        //}
        public void StepThroughMethod()
        {
            if (CurrentSolutionStep == 0)
                CurrentSolutionStep = CompleteSolution.Count - 1;

            MakeAMove(CompleteSolution[--CurrentSolutionStep]);
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
        //[System.Diagnostics.Conditional("DEBUG")]
        //void DebugWriteFromList(List<PositionPointer> movableLiquids,TreeNode<ValidMove> treeNode)
        //{
        //    Debug.WriteLine("movableLiquids:");
        //    foreach (var liquid in movableLiquids)
        //        Debug.WriteLine($"[{liquid.X},{liquid.Y}] {{{treeNode.Data.GameState[liquid.X, liquid.Y].Name}}} {{{liquid.AllIdenticalLiquids}}} {{{liquid.NumberOfRepeatingLiquids}}}");
        //}
        [System.Diagnostics.Conditional("DEBUG")]
        void Debug_IterateThroughList<T>(List<T> movableLiquids, string title, Func<T, string> action)
        {
            Debug.WriteLine(title);
            foreach (var liquid in movableLiquids)
                Debug.WriteLine(action(liquid));
        }
        [System.Diagnostics.Conditional("DEBUG")]
        private void Debug_WriteToFileAutoSolveSteps(TreeNode<ValidMove> treeNode, string note = "")
        {
            string exportString = DateTime.Now.ToString("[MM/dd/yyyy HH:mm:ss]");

            exportString += GameState.GameStateToString(treeNode.Data.GameState, StringFormat.Numbers, true);
            exportString += "{" + note + "}" + "\n";
            System.IO.File.AppendAllText(exportLogFilename, exportString);
        }
        #endregion
    }
}
