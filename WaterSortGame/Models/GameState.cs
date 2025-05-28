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
            
            //Tube.ResetCounter();
            SetFreshGameState();
            //Tubes?.Clear();
            
            int i = 0;
            gameGrid = new LiquidColorNew[20, NumberOfLayers];

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

            //AddTube(i++, new int[] { });
            //AddTube(i++, new int[] { 1, 1, 1, 1 });
            //AddTube(i++, new int[] { 2, 3, 3 });
            //AddTube(i++, new int[] { 2, 2, 2 });
            //AddTube(i++, new int[] { 3, 3 });

            //AddTube(i++, new int[] { 1,2,3,2 });
            //AddTube(i++, new int[] { 1,1,2,4 });
            //AddTube(i++, new int[] { 5,2,3,4 });
            //AddTube(i++, new int[] { 4,1,3,5 });
            //AddTube(i++, new int[] { 5,3,5,4 });

            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Purple, LiquidColorName.LightBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Red, LiquidColorName.Blue, LiquidColorName.LightBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Olive, LiquidColorName.Orange, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Purple, LiquidColorName.GrayBlue, LiquidColorName.Yellow });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.LightBlue, LiquidColorName.Yellow, LiquidColorName.GrayBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightGreen, LiquidColorName.Blue, LiquidColorName.Olive, LiquidColorName.LightGreen });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Green, LiquidColorName.LightGreen, LiquidColorName.Olive, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Gray, LiquidColorName.Gray, LiquidColorName.Green });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Olive, LiquidColorName.LightGreen });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.Orange, LiquidColorName.Purple, LiquidColorName.Orange });

            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Purple });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Red, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Olive, LiquidColorName.Orange, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Purple, LiquidColorName.GrayBlue, LiquidColorName.Yellow });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.LightBlue, LiquidColorName.Yellow, LiquidColorName.GrayBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightGreen, LiquidColorName.Blue, LiquidColorName.Olive, LiquidColorName.LightGreen });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Green, LiquidColorName.LightGreen, LiquidColorName.Olive });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Gray, LiquidColorName.Gray, LiquidColorName.Green });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Olive, LiquidColorName.LightGreen });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.Orange, LiquidColorName.Purple, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.LightBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue });


            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Red, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Red });






            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Gray, LiquidColorName.Red, LiquidColorName.Blue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Gray });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Red });

            //// Nikdy nevyresenej level:
            //AddTube(i++, new int[] { 9, 2, 4, 1 });
            //AddTube(i++, new int[] { 3, 8, 11, 5 });
            //AddTube(i++, new int[] { 9, 11, 11, 12 });
            //AddTube(i++, new int[] { 3, 3, 2, 5 });
            //AddTube(i++, new int[] { 1, 7, 6, 10 });
            //AddTube(i++, new int[] { 3, 4, 7, 4 });
            //AddTube(i++, new int[] { 2, 8, 5, 10 });
            //AddTube(i++, new int[] { 6, 1, 2, 9 });
            //AddTube(i++, new int[] { 11, 10, 7, 6 });
            //AddTube(i++, new int[] { 5, 7, 10, 4 });
            //AddTube(i++, new int[] { 8, 12, 6, 12 });
            //AddTube(i++, new int[] { 1, 12, 8, 9 });

            // sejvnutej level z te novejsi hry. Level jsem vyresil, ale byl tezkej (jeste jednou jsem to overil ze je to resitelny. Uspesne!):
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Red, LiquidColorName.Blue, LiquidColorName.Orange });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.GrayBlue, LiquidColorName.LightBlue, LiquidColorName.LightGreen });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.LightBlue, LiquidColorName.LightBlue, LiquidColorName.Gray });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Purple, LiquidColorName.Red, LiquidColorName.LightGreen });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Orange, LiquidColorName.Yellow, LiquidColorName.Olive, LiquidColorName.Green });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Blue, LiquidColorName.Yellow, LiquidColorName.Blue });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.GrayBlue, LiquidColorName.LightGreen, LiquidColorName.Green });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Olive, LiquidColorName.Orange, LiquidColorName.Red, LiquidColorName.Pink });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.Green, LiquidColorName.Yellow, LiquidColorName.Olive });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.LightGreen, LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Blue });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.Gray, LiquidColorName.Olive, LiquidColorName.Gray });
            AddTube(i++, new LiquidColorName[] { LiquidColorName.Orange, LiquidColorName.Gray, LiquidColorName.GrayBlue, LiquidColorName.Pink });

            //AddTube(i++, new int[] { 1, 1 });
            //AddTube(i++, new int[] { 2, 2 });
            //AddTube(i++, new int[] { 3, 3, 1, 1 });
            //AddTube(i++, new int[] { 3, 2, 3, 2 });

            //AddTube(i++, new LiquidColorName[] { });
            //AddTube(i++, new LiquidColorName[] { });

            //gameGrid = CloneGrid(gameGrid, i + 2);
            gameGrid = CloneGrid(gameGrid, i + 2);
            StoreStartingGrid();
        }
        private void AddTube(int tubeNumber, int[] layers)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                this[tubeNumber, i] = new LiquidColorNew(layers[i]);
            }
        }
        private void AddTube(int tubeNumber, LiquidColorName[] liquids)
        {
            for (int i = 0; i < liquids.Length; i++)
            {
                this[tubeNumber, i] = new LiquidColorNew((int)liquids[i]);
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
            for (int i = 1; i < LiquidColorNew.ColorKeys.Count; i++) // generate list of all colors. Starting at i=1 because color number 0 is blank and used for other purposes.
            {
                selectedColors.Add(i);
            }

            for (int i = 1; i < LiquidColorNew.ColorKeys.Count - appSettings.NumberOfColorsToGenerate; i++) // now remove some random ones. Starting at i=1 because color number 0 is blank and used for other purposes.
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
            return IsLevelCompleted(gameGrid);
        }
        public bool IsLevelCompleted(LiquidColorNew[,] internalGameGrid)
        {
            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                //if (gameGrid[x, 0] is null || gameGrid[x, 1] is null || gameGrid[x, 2] is null || gameGrid[x, 3] is null)
                //{ // tohle tu je abych nikdy neporovnaval hodnoty GameGridu kdyz je moznost ze budou null:
                //    continue;
                //}
                
                // tohle tu je abych nikdy neporovnaval hodnoty GameGridu kdyz je moznost ze budou null:
                if (internalGameGrid[x, 0] is not null &&
                    internalGameGrid[x, 1] is not null &&
                    internalGameGrid[x, 2] is not null &&
                    internalGameGrid[x, 3] is not null)
                {
                    if (!(internalGameGrid[x, 0].Name == internalGameGrid[x, 1].Name &&
                        internalGameGrid[x, 0].Name == internalGameGrid[x, 2].Name &&
                        internalGameGrid[x, 0].Name == internalGameGrid[x, 3].Name))
                    {
                        return false;
                    }
                }
                else
                { // v pripade ze aspon jeden objekt je null, otestovat jestli jsou vsechny null
                    if (!(internalGameGrid[x, 0] is null &&
                    internalGameGrid[x, 1] is null &&
                    internalGameGrid[x, 2] is null &&
                    internalGameGrid[x, 3] is null))
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
            List<LiquidColorName?> liquidColors = new List<LiquidColorName?>();
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
