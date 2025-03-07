using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class GameState : ViewModelBase
    {
        private MainWindowVM MainWindowVM;
        private AppSettings appSettings;



        private int numberOfTubes;
        private readonly int numberOfLayers = 4;
        private LiquidColorNew[,] gameGrid;
        public LiquidColorNew this[int tubes, int layers]
        {
            get => gameGrid[tubes, layers];
            set
            {
                gameGrid[tubes, layers] = value;
                //OnLiquidMoving();
            }
        }

        //private List<List<LiquidColorNew>> gameState;
        //public LiquidColorNew this[int tube, int layer]
        //{
        //    get
        //    {
        //        //return gameStateList.ElementAt(tubes).ElementAt(layers);
        //        return gameState[tube][layer];
        //    }
        //    set
        //    {
        //        gameState[tube][layer] = value;
        //    }
        //}


        //private ObservableCollection<Tube> tubes = new ObservableCollection<Tube>();
        //public ObservableCollection<Tube> Tubes
        //{
        //    get { return tubes; }
        //    set
        //    {
        //        if (value != tubes)
        //        {
        //            tubes = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        private int extraTubesAdded = 0;
        public int ExtraTubesAdded { get; }
        private LiquidColorNew[,] startingPosition;
        public LiquidColorNew[,] StartingPosition { get; }
        private List<LiquidColorNew[,]> savedGameSteps = new List<LiquidColorNew[,]>();
        public List<LiquidColorNew[,]> SavedGameSteps
        {
            get { return savedGameSteps; }
            private set
            {
                if (value != savedGameSteps)
                {
                    savedGameSteps = value;
                    //OnPropertyChanged();
                }
            }
        }
        public LiquidColorNew[,] LastGameStep { get; set; }


        public GameState(MainWindowVM mainWindowVM)
        {
            MainWindowVM = mainWindowVM;
            appSettings = MainWindowVM.AppSettings;

            //if (Tubes.Count == 0)
            //{
            //    GenerateNewLevel();
            //}

            //gameState = new int[Tubes, Layers];
            

        }
        //private void OnLiquidMoving()
        //{
        //    MainWindow.DrawTubes();
        //}
        public void GenerateNewLevel()
        {
            if (appSettings.LoadDebugLevel is true)
                GenerateDebugLevel();
            else
                GenerateStandardLevel();
        }
        private void GenerateDebugLevel()
        {
            //Tube.ResetCounter();
            SetFreshGameState();
            //Tubes?.Clear();
            gameGrid = new LiquidColorNew[numberOfTubes + extraTubesAdded, numberOfLayers];

            AddTube(0, new int[] { 1, 1, 4, 4 });
            AddTube(1, new int[] { 8, 8, 1, 1 });
            AddTube(2, new int[] { 4, 4, 8, 8 });
            AddTube(3, new int[] { 3, 3, 3, 7 });
            AddTube(4, new int[] { 7, 7, 7, 3 });

            StoreStartingGrid();
        }
        private void AddTube(int tubeNumber, int[] layers)
        {
            for (int i = 0; i < numberOfLayers; i++)
            {
                this[tubeNumber, i] = new LiquidColorNew(layers[i]);
            }
        }
        public void AddExtraTube() // this is for adding extra tube during gameplay
        {
            if (extraTubesAdded <= appSettings.MaximumExtraTubes)
            {
                var temp = gameGrid;
                var length = temp.Length;
                extraTubesAdded++;
                //Array.Resize<LiquidColorNew>(ref gameGrid, 20);
                //gameGrid = CloneGrid(gameGrid);
                //LiquidColorNew[,] clonedGrid = new LiquidColorNew[gameGrid.GetLength(0) + 1, gameGrid.GetLength(1)];
                //for (int x = 0; x < gameGrid.GetLength(0); x++)
                //{
                //    for (int y = 0; y < gameGrid.GetLength(1); y++)
                //    {
                //        clonedGrid[x, y] = this[x, y];
                //    }
                //}
                //gameGrid = clonedGrid;
                gameGrid = CloneGrid(gameGrid);


            }
        }
        public LiquidColorNew[,] CloneGrid(LiquidColorNew[,] grid)
        {
            return CloneGrid(grid, extraTubesAdded);
        }
        private LiquidColorNew[,] CloneGrid(LiquidColorNew[,] grid, int numberOfTubes)
        {
            LiquidColorNew[,] clonedGrid = new LiquidColorNew[grid.GetLength(0) + numberOfTubes, grid.GetLength(1)];
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    clonedGrid[x, y] = grid[x, y];
                }
            }
            return clonedGrid;
        }
        public void RestartLevel()
        {
            SetFreshGameState();
            //SavedStartingTubes?.Clear();
            //Tubes?.Clear();
            //foreach (var tube in StartingPosition)
            //{
            //    Tubes.Add((Tube)tube.DeepCopy());
            //}

            gameGrid = CloneGrid(startingPosition);
        }
        private void GenerateStandardLevel()
        {
            //Tube.ResetCounter();
            SetFreshGameState();
            Random rnd = new Random();

            List<LiquidColorNew> colorsList = new List<LiquidColorNew>();
            if (appSettings.RandomNumberOfTubes)
            {
                appSettings.NumberOfColorsToGenerate = rnd.Next(3, LiquidColorNew.ColorKeys.Count);
            }

            List<int> selectedColors = new List<int>();
            for (int i = 0; i < LiquidColorNew.ColorKeys.Count; i++) // generate list of all colors
            {
                selectedColors.Add(i);
            }

            for (int i = 0; i < LiquidColorNew.ColorKeys.Count - appSettings.NumberOfColorsToGenerate; i++) // now remove some random ones
            {
                //selectedColors.Remove(selectedColors[NumberOfColorsToGenerate]); // this always keeps the same colors
                selectedColors.Remove(selectedColors[rnd.Next(0, selectedColors.Count)]);
            }
            foreach (var color in selectedColors)
            {
                colorsList.Add(new LiquidColorNew(color));
                colorsList.Add(new LiquidColorNew(color));
                colorsList.Add(new LiquidColorNew(color));
                colorsList.Add(new LiquidColorNew(color));
            }

            //Tubes?.Clear();
            gameGrid = new LiquidColorNew[appSettings.NumberOfColorsToGenerate + extraTubesAdded, numberOfLayers];
            //var tubes = new ObservableCollection<Tube>();
            for (int x = 0; x < appSettings.NumberOfColorsToGenerate; x++)
            {
                LiquidColorNew[] layer = new LiquidColorNew[numberOfLayers];
                for (int y = 0; y < numberOfLayers; y++)
                {
                    int colorNumber = rnd.Next(0, colorsList.Count);
                    //layer[y] = colorsList[colorNumber];

                    gameGrid[x, y] = colorsList[colorNumber];

                    colorsList.Remove(colorsList[colorNumber]);
                }

                //Tubes.Add(new Tube(layer[0], layer[1], layer[2], layer[3]));
                
            }
            //Tubes.Add(new Tube());
            //Tubes.Add(new Tube());

            //Tubes?.Clear();
            //Tubes.AddRange(tubes); // have it here like this because I dont want to call CollectionChanged event during the generation
            // ## change into BulkObservableCollection<T> Class ? 


            StoreStartingGrid();
        }
        private void SetFreshGameState()
        {
            extraTubesAdded = 0; // resets how much extra tubes has been added
        }
        private void StoreStartingGrid()
        {
            startingPosition = CloneGrid(gameGrid);
        }
        public void SaveGameState()
        {
            if (DidGameStateChange() == true)
            //if (SolvingSteps[SolvingSteps.Count - 1] != Tubes)
            {
                CopyTubes();
                return;
            }
        }
        private bool DidGameStateChange()
        {
            //if (SavedGameSteps.Count == 0 && LastGameStep.Count == 0)
            if (SavedGameSteps.Count == 0 && LastGameStep == null)
            {
                return true;
            }
            //var lastStateTubes = GameStates[GameStates.Count - 1];

            if (LastGameStep.Length != gameGrid.Length) // pokud jen pridavam extra prazdnou zkumavku tak to neukladat!
            {
                return false;
            }

            for (int x = 0; x < numberOfTubes; x++)
            {
                for (int y = 0; y < numberOfLayers; y++)
                {
                    if (LastGameStep[x,y] != gameGrid[x,y])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void CopyTubes()
        {
            if (LastGameStep == null) // pridavam to tady, protoze nechci v game states mit i current game state.
            {
                SavedGameSteps.Add(LastGameStep);
                LastGameStep = null;
            }

            LastGameStep = CloneGrid(gameGrid);
        }
        public bool AreColorsSorted()
        {
            for (int x = 0; x < numberOfTubes; x++)
            {
                if (gameGrid[x, 0] == gameGrid[x, 1] && gameGrid[x, 0] == gameGrid[x, 2] && gameGrid[x, 0] == gameGrid[x, 3])
                {
                    return true;
                }
            }
            return false;
        }
        public void StepBack()
        {
            if (SavedGameSteps.Count == 0)
            {
                return;
            }

            LiquidColorNew[,] lastGameStatus = SavedGameSteps[SavedGameSteps.Count - 1];

            MainWindowVM.PropertyChangedEventPaused = true;
            gameGrid = lastGameStatus;
            MainWindowVM.PropertyChangedEventPaused = false;

            LastGameStep = CloneGrid(lastGameStatus);

            SavedGameSteps.Remove(lastGameStatus);
        }
    }
}
