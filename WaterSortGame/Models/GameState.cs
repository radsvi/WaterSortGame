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



        //public int NumberOfTubes { get; private set; }
        public int NumberOfLayers { get; } = 4;
        public LiquidColorNew[,] gameGrid;
        public LiquidColorNew this[int tubes, int layers]
        {
            get => gameGrid[tubes, layers];
            set
            {
                gameGrid[tubes, layers] = value;
                //OnLiquidMoving();
            }
        }
        public int GetLength(int dimension)
        {
            return gameGrid.GetLength(dimension);
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
        public LiquidColorNew[,] StartingPosition { get; set; }
        private ObservableCollection<LiquidColorNew[,]> savedGameSteps = new ObservableCollection<LiquidColorNew[,]>();
        public ObservableCollection<LiquidColorNew[,]> SavedGameStates
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
        public LiquidColorNew[,] LastGameState { get; set; }

        public GameState() { }
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
        //private void SetGameGrid(int numberOfTubes)
        //{
        //    //gameGrid = new LiquidColorNew[(NumberOfTubes + ExtraTubesAdded + 2), NumberOfLayers];
        //    gameGrid = new LiquidColorNew[numberOfTubes, NumberOfLayers];
        //}
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
            gameGrid = new LiquidColorNew[5, NumberOfLayers];
            //Tube.ResetCounter();
            SetFreshGameState();
            //Tubes?.Clear();
            
            int i = 0;
            //AddTube(i++, new int[] { 1, 1, 4, 4 });
            //AddTube(i++, new int[] { 8, 8, 1, 1 });
            //AddTube(i++, new int[] { 4, 4, 8, 8 });
            //AddTube(i++, new int[] { 3, 3, 3 });
            //AddTube(i++, new int[] { 7, 7, 7 });
            //AddTube(i++, new int[] { 3 });
            //AddTube(i++, new int[] { 7 });

            //AddTube(i++, new int[] { 1, 1, 4, 4 });
            //AddTube(i++, new int[] { 8, 8, 1, 1 });
            //AddTube(i++, new int[] { 4, 4, 8, 8 });
            //AddTube(i++, new int[] { 3, 3, 3 });
            //AddTube(i++, new int[] { 7, 7, 7 });
            //AddTube(i++, new int[] { 3 });
            //AddTube(i++, new int[] { 7 });

            //AddTube(i++, new int[] { 1, 1, 1, 2 });
            //AddTube(i++, new int[] {  });
            //AddTube(i++, new int[] { 2, 2, 2 });
            //AddTube(i++, new int[] { 1 });
            //AddTube(i++, new int[] { 4, 4, 4, 4, });

            //AddTube(i++, new int[] { });
            //AddTube(i++, new int[] { 1 });
            //AddTube(i++, new int[] { 1,1,1 });
            //AddTube(i++, new int[] { 3,3,3,3 });
            //AddTube(i++, new int[] { 4, 4, 4, 4, });


            //AddTube(i++, new int[] { 1,2,2,3 });
            //AddTube(i++, new int[] { 4,3,2,1 });
            //AddTube(i++, new int[] { 1,3,1,4 });
            //AddTube(i++, new int[] { 2,3,4,4 });
            //AddTube(i++, new int[] {  });
            //AddTube(i++, new int[] {  });

            //AddTube(i++, new int[] { 1, 1, 1, 1 });
            //AddTube(i++, new int[] { 2, 3, 3, 3 });
            //AddTube(i++, new int[] { });
            //AddTube(i++, new int[] { 2, 3 });
            //AddTube(i++, new int[] { 4,4,4,4 });
            //AddTube(i++, new int[] { 5,5,});
            //AddTube(i++, new int[] { 6,6,6,6});

            AddTube(i++, new int[] { });
            AddTube(i++, new int[] { 1, 1, 1, 1 });
            AddTube(i++, new int[] { 2, 3, 3 });
            AddTube(i++, new int[] { 2, 2, 2 });
            AddTube(i++, new int[] { 3, 3 });

            //AddTube(i++, new int[] { });
            //AddTube(i++, new int[] { 1, 1, 2, 2 });
            //AddTube(i++, new int[] { 2, 2, 1, 1 });
            //AddTube(i++, new int[] {  });

            StoreStartingGrid();
        }
        private void AddTube(int tubeNumber, int[] layers)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                this[tubeNumber, i] = new LiquidColorNew(layers[i]);
            }
        }
        /// <summary>
        /// Adding extra (empty) tube during gameplay
        /// </summary>
        public bool CanAddExtraTube()
        {
            return CountColors() + appSettings.MaximumExtraTubes + 2 - gameGrid.GetLength(0) > 0;
        }
        public void AddExtraTube()
        {
            if (CanAddExtraTube())
            {
                gameGrid = CloneGrid(gameGrid, gameGrid.GetLength(0) + 1);
            }
        }
        public static LiquidColorNew[,] CloneGridStatic(LiquidColorNew[,] grid)
        {
            return new GameState().CloneGrid(grid, grid.GetLength(0));
        }
        public LiquidColorNew[,] CloneGrid(LiquidColorNew[,] grid)
        {
            return CloneGrid(grid, grid.GetLength(0));
        }
        public LiquidColorNew[,] CloneGrid(LiquidColorNew[,] gameGrid, int numberOfTubes)
        {
            LiquidColorNew[,] gridClone = new LiquidColorNew[numberOfTubes, gameGrid.GetLength(1)];
            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                for (int y = 0; y < gameGrid.GetLength(1); y++)
                {
                    if (gameGrid[x, y] is not null)
                    {
                        gridClone[x, y] = gameGrid[x, y].Clone();
                    }
                }
            }
            return gridClone;
        }
        public void SetGameState(LiquidColorNew[,] newGameState)
        {
            gameGrid = CloneGrid(newGameState);
        }
        public void RestartLevel()
        {
            SetFreshGameState();
            gameGrid = CloneGrid(StartingPosition);
        }
        private void GenerateStandardLevel()
        {
            gameGrid = new LiquidColorNew[appSettings.NumberOfColorsToGenerate + 2, NumberOfLayers];
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
            //ExtraTubesAdded = 0; // resets how much extra tubes has been added
        }
        private void StoreStartingGrid()
        {
            StartingPosition = CloneGrid(gameGrid);
        }
        public void SaveGameState()
        {
            if (DidGameStateChange() == true)
            //if (SolvingSteps[SolvingSteps.Count - 1] != Tubes)
            {
                if (LastGameState != null) // pridavam to tady, protoze nechci v game states mit i current game state.
                {
                    SavedGameStates.Add(LastGameState);
                    LastGameState = null;
                }

                LastGameState = CloneGrid(gameGrid);
                return;
            }
        }
        private bool DidGameStateChange()
        {
            if (SavedGameStates.Count == 0 && LastGameState == null)
            {
                return true;
            }
            //if (LastGameState.Length != gameGrid.Length) // pokud jen pridavam extra prazdnou zkumavku tak to neukladat!
            //{
            //    return false;
            //}

            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                for (int y = 0; y < gameGrid.GetLength(1); y++)
                {
                    if (LastGameState[x, y] is null && gameGrid[x, y] is null)
                    {
                        continue;
                    }
                    if (LastGameState[x, y] is null && gameGrid[x, y] is not null || LastGameState[x, y] is not null && gameGrid[x, y] is null)
                    {
                        return true;
                    }
                    if (LastGameState[x,y].Name != gameGrid[x,y].Name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsLevelCompleted()
        {
            for (int x = 0; x < gameGrid.GetLength(0); x++)
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
            if (SavedGameStates.Count == 0)
            {
                return;
            }

            LiquidColorNew[,] lastGameStatus = SavedGameStates[SavedGameStates.Count - 1];

            MainWindowVM.PropertyChangedEventPaused = true;
            gameGrid = lastGameStatus;
            MainWindowVM.PropertyChangedEventPaused = false;

            LastGameState = CloneGrid(lastGameStatus);

            SavedGameStates.Remove(lastGameStatus);
            MainWindowVM.DrawTubes();
        }
        private int CountColors()
        {
            int numberOfColors = 0;
            List<LiquidColorNames?> liquidColors = new List<LiquidColorNames?>();
            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                for (int y = 0; y < gameGrid.GetLength(1); y++)
                {
                    if (gameGrid[x, y] is null)
                    {
                        continue;
                    }
                    if (liquidColors.Contains(gameGrid[x, y].Name) == false)
                    {
                        liquidColors.Add(gameGrid[x, y].Name);
                        numberOfColors++;
                    }
                }
            }
            return numberOfColors;
        }


    }
}
