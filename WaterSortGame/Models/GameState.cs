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
    internal class GameState// : ViewModelBase
    {
        private MainWindowVM MainWindowVM;
        private AppSettings appSettings;



        public int NumberOfTubes { get; private set; }
        public int NumberOfLayers { get; } = 4;
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
        public int ExtraTubesAdded { get; private set; }
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

            GenerateNewLevel();
        }
        private void InitializeGameGrid(int numberOfTubes)
        {
            NumberOfTubes = numberOfTubes;
            //gameGrid = new LiquidColorNew[(NumberOfTubes + ExtraTubesAdded + 2), NumberOfLayers];
            gameGrid = new LiquidColorNew[NumberOfTubes, NumberOfLayers];
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
            InitializeGameGrid(7);
            //Tube.ResetCounter();
            SetFreshGameState();
            //Tubes?.Clear();
            
            int i = 0;
            AddTube(i++, new int[] { 1, 1, 4, 4 });
            AddTube(i++, new int[] { 8, 8, 1, 1 });
            AddTube(i++, new int[] { 4, 4, 8, 8 });
            AddTube(i++, new int[] { 3, 3, 3, 7 });
            AddTube(i++, new int[] { 7, 7, 7, 3 });

            //AddTube(i++, new int[] { 7, 7, 7, 7 });
            //AddTube(i++, new int[] { 5, 5, 5 });
            //AddTube(i++, new int[] { 5 });

            StoreStartingGrid();
        }
        private void AddTube(int tubeNumber, int[] layers)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                this[tubeNumber, i] = new LiquidColorNew(layers[i]);
            }
        }
        public void AddExtraTube() // this is for adding extra (empty) tube during gameplay
        {
            if (ExtraTubesAdded <= appSettings.MaximumExtraTubes)
            {
                var temp = gameGrid;
                var length = temp.Length;
                ExtraTubesAdded++;
                NumberOfTubes++;
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
            return CloneGrid(grid, ExtraTubesAdded);
        }
        private LiquidColorNew[,] CloneGrid(LiquidColorNew[,] grid, int numberOfTubes)
        {
            LiquidColorNew[,] clonedGrid = new LiquidColorNew[grid.GetLength(0) + numberOfTubes, grid.GetLength(1)];
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    clonedGrid[x, y] = grid[x, y];
                    //if (grid[x, y] is not null)
                    //{
                    //    clonedGrid[x, y] = grid[x, y];
                    //}
                    //else
                    //{
                    //    clonedGrid[x, y] = null;
                    //}
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
            InitializeGameGrid(appSettings.NumberOfColorsToGenerate + 2);
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
            //InitializeGameGrid(appSettings.NumberOfColorsToGenerate + ExtraTubesAdded);
            
            //var tubes = new ObservableCollection<Tube>();
            for (int x = 0; x < appSettings.NumberOfColorsToGenerate; x++)
            {
                LiquidColorNew[] layer = new LiquidColorNew[NumberOfLayers];
                for (int y = 0; y < NumberOfLayers; y++)
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
            ExtraTubesAdded = 0; // resets how much extra tubes has been added
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

            for (int x = 0; x < NumberOfTubes; x++)
            {
                for (int y = 0; y < NumberOfLayers; y++)
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
        public bool IsLevelCompleted()
        {
            for (int x = 0; x < NumberOfTubes; x++)
            {
                //if (gameGrid[x, 0] is null || gameGrid[x, 1] is null || gameGrid[x, 2] is null || gameGrid[x, 3] is null)
                //{ // tohle tu je abych nikdy neporovnaval hodnoty GameGridu kdyz je moznost ze budou null:
                //    continue;
                //}
                
                // tohle tu je abych nikdy neporovnaval hodnoty GameGridu kdyz je moznost ze budou null:
                if (gameGrid[x, 0] is not null &&
                    gameGrid[x, 1] is not null &&
                    gameGrid[x, 2] is not null &&
                    gameGrid[x, 3] is not null)
                {
                    if (!(gameGrid[x, 0].Name == gameGrid[x, 1].Name &&
                        gameGrid[x, 0].Name == gameGrid[x, 2].Name &&
                        gameGrid[x, 0].Name == gameGrid[x, 3].Name))
                    {
                        return false;
                    }
                }
                else
                { // v pripade ze aspon jeden objekt je null, otestovat jestli jsou vsechny null
                    if (!(gameGrid[x, 0] is null &&
                    gameGrid[x, 1] is null &&
                    gameGrid[x, 2] is null &&
                    gameGrid[x, 3] is null))
                    {
                        return false;
                    }
                }
            }
            return true;
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
