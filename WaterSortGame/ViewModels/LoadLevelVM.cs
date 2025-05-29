using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using WaterSortGame.Models;
using WaterSortGame.MVVM;
using WaterSortGame.Properties;

namespace WaterSortGame.ViewModels
{
    class LoadLevelVM : PopupScreenBase
    {
        private AppSettings AppSettings;
        public LoadLevelVM(object viewModel) : base(viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
            AppSettings = MainWindowVM.AppSettings;
            LoadLevelList.CollectionChanged += LoadLevelList_CollectionChanged;
            //MainWindowVM.LoadLevelScreen();

            //MainWindowVM.LoadLevelList = LoadLevelList;
            //MainWindowVM.LoadLevelVM = this;
        }
        private StoredLevel selectedLevelForLoading;
        public StoredLevel SelectedLevelForLoading
        {
            get { return selectedLevelForLoading; }
            set
            {
                if (value != selectedLevelForLoading)
                {
                    selectedLevelForLoading = value;
                    //OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<StoredLevel> loadLevelList = new ObservableCollection<StoredLevel>();
        public ObservableCollection<StoredLevel> LoadLevelList
        {
            get { return loadLevelList; }
            set
            {
                if (value != loadLevelList)
                {
                    loadLevelList = value;
                    //OnPropertyChanged();
                    //OnLoadLevelListChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        private int loadLevelScreenHeight;
        public int LoadLevelScreenHeight
        {
            get { return loadLevelScreenHeight; }
            set
            {
                if (value != loadLevelScreenHeight)
                {
                    loadLevelScreenHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        //public event EventHandler? OnLoadLevelListChanged;
        internal void LoadLevelList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int increaseHeight = 0;
            if (LoadLevelList.Count > 3 && LoadLevelList.Count < 12)
            {
                increaseHeight = (LoadLevelList.Count - 3) * 45; //vyska jedne polozky je 45
                LoadLevelScreenHeight = 280 + increaseHeight;
            }
            else if (LoadLevelList.Count <= 3)
            {
                LoadLevelScreenHeight = 280;
            }
            else if (LoadLevelList.Count >= 12)
            {
                LoadLevelScreenHeight = 640;
                LoadLevelScreenScroll = true;
            }
        }
        private bool loadLevelScreenScroll;
        public bool LoadLevelScreenScroll
        {
            get { return loadLevelScreenScroll; }
            set
            {
                if (value != loadLevelScreenScroll)
                {
                    loadLevelScreenScroll = value;
                    OnPropertyChanged();
                }
            }
        }
        //private void LoadLevelListChangedCalculation(object? sender, EventArgs e)
        //{

        //    if (LoadLevelList.Count > 3)
        //    {
        //        var increaseHeight = (LoadLevelList.Count - 3) * 45; //vyska jedne polozky je 45
        //        LoadLevelScreenHeight = 280 + increaseHeight;
        //    }
        //    else
        //    {
        //        LoadLevelScreenHeight = 280;
        //    }
        //}
        internal void LoadLevelScreen()
        {
            //LoadLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);

            FillImportBoxFromClipboard();

            LoadLevelList?.Clear();
            
            ObservableCollection<StoredLevel>? deserializedList;
            try
            {
                deserializedList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);
            }
            catch
            {
                return;
            }

            if (deserializedList is null) return;
            foreach (var item in deserializedList)
            {
                item.GameGridDisplayList = ArrayToTubeList(item.GameGrid);
                LoadLevelList.Add(item);
            }

            //OnLoadLevelListChanged?.Invoke(this, EventArgs.Empty);
            //LoadLevelScreenHeight = 280;
            //if (LoadLevelList.Count > 3)
            //{
            //    var increaseHeight = (LoadLevelList.Count - 3) * 45; //vyska jedne polozky je 45
            //    LoadLevelScreenHeight += increaseHeight;
            //}
            //else
            //{
            //    LoadLevelScreenHeight = 280;
            //}
        }
        public RelayCommand LoadLevelCommand => new RelayCommand(execute => LoadLevel(), canExecute => SelectedLevelForLoading is not null);
        //private void LoadLevel(bool force = false)
        internal void LoadLevel()
        {
            if (SelectedLevelForLoading == null)
            {
                return;
            }
            MainWindowVM.ClosePopupWindow();
            MainWindowVM.PropertyChangedEventPaused = true;
            //MainWindowVM.GameState.StartingPosition = MainWindowVM.GameState.CloneGrid(SelectedLevelForLoading.GameGrid);
            MainWindowVM.GameState.StartingPosition = CloneGrid(SelectedLevelForLoading.GameGrid);


            //TubesManager.Tubes = DeepCopyTubesCollection(TubesManager.SavedStartingTubes);

            //TubesManager.Tubes?.Clear();
            //foreach (Tube tube in TubesManager.SavedStartingTubes)
            //{ // kdyz bych to udelal takhle, tak se prestane refreshovat TubesPerLineCalculation(); a GenerateNewLevel() taky
            //    TubesManager.Tubes.Add(tube.DeepCopy());
            //}
            //OnStartingLevel();

            MainWindowVM.RestartLevel();
            MainWindowVM.PropertyChangedEventPaused = false;
        }
        private LiquidColorNew[,] CloneGrid(LiquidColorNew[,] gameGrid)
        {
            LiquidColorNew[,] gridClone = new LiquidColorNew[gameGrid.GetLength(0), gameGrid.GetLength(1)];
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
        //private ObservableCollection<Tube> DeepCopyTubesCollection(ObservableCollection<Tube> tubes)
        //{
        //    ObservableCollection<Tube> newTubes = new ObservableCollection<Tube>();
        //    foreach (Tube tube in tubes)
        //    {
        //        newTubes.Add(tube.DeepCopy());
        //    }
        //    return newTubes;
        //}
        public RelayCommand DeleteSelectedLevelsCommand => new RelayCommand(execute => DeleteSelectedSaves(), canExecute => CanDelete());
        private bool CanDelete()
        {
            foreach (var savedLevel in LoadLevelList)
            {
                if (savedLevel.MarkedForDeletion is true)
                {
                    return true;
                }
            }
            return false;
        }
        private void DeleteSelectedSaves()
        {
            var levelsToRemove = LoadLevelList.Where(item => item.MarkedForDeletion == true).ToList();
            foreach (var levelToRemove in levelsToRemove)
            {
                LoadLevelList.Remove(levelToRemove);
            }

            Settings.Default.SavedLevels = JsonConvert.SerializeObject(LoadLevelList);
            Settings.Default.Save();
        }
        public RelayCommand MarkForDeletionCommand => new RelayCommand(savedGame => MarkForDeletion(savedGame));

        private void MarkForDeletion(object obj)
        {
            var savedGame = obj as StoredLevel;
            savedGame.MarkedForDeletion = !savedGame.MarkedForDeletion;
        }
        //private List<List<LiquidColorNew>> ArrayToList2D(LiquidColorNew[,] array)
        //{
        //    List<List<LiquidColorNew>> list = new List<List<LiquidColorNew>>();

        //    for (int x = 0; x < array.GetLength(0); x++)
        //    {
        //        var row = new List<LiquidColorNew>();
        //        for (int y = 0; y < array.GetLength(1); y++)
        //        {
        //            row.Add(array[x, y]);
        //        }
        //        list.Add(row);
        //    }
        //    return list;
        //}
        private List<TubeNew> ArrayToTubeList(LiquidColorNew[,] array)
        {
            List<TubeNew> list = new List<TubeNew>();

            for (int x = 0; x < array.GetLength(0); x++)
            {
                var row = new TubeNew(array[x, 0], array[x, 1], array[x, 2], array[x, 3]);

                list.Add(row);
            }
            return list;
        }
        //private LiquidColorNew[] AddTube(int tubeNumber, int[] layers)
        //{
        //    var returnArray = new LiquidColorNew[layers.Length];
        //    for (int i = 0; i < layers.Length; i++)
        //    {
        //        returnArray[i] = new LiquidColorNew(layers[i]);
        //    }

        //    return returnArray;
        //}
        private LiquidColorNew?[,] ConvertToColorBrush(int?[,] intArray)
        {
            var returnArray = new LiquidColorNew?[intArray.GetLength(0), intArray.GetLength(1)];
            for (int x = 0; x < intArray.GetLength(0); x++)
            {
                for (int y = 0; y < intArray.GetLength(1); y++)
                {
                    if (intArray[x, y] is not null)
                    {
                        returnArray[x, y] = new LiquidColorNew((int)intArray[x, y]);
                    }
                    else
                    {
                        returnArray[x, y] = null;
                    }
                }
            }
            return returnArray;
        }
        public void AddPresetLevels()
        {
            MainWindowVM.WindowService?.CloseWindow(); // close options menu

            ObservableCollection<StoredLevel> savedLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);

            //var additionalLevels = new List<StoredLevel>();
            var firstLevel = new int?[,]
            {
                //{8, 1, 3, 0},
                //{2, 7, 10, 4},
                //{8, 10, 10, 11},
                //{2, 2, 1, 4},
                //{0, 6, 5, 9},
                //{2, 3, 6, 3},
                //{3, 7, 4, 9},
                //{5, 0, 1, 8},
                //{10, 9, 6, 5},
                //{4, 6, 9, 3},
                //{7, 11, 5, 11},
                //{0, 11, 7, 8},
                {9, 2, 4, 1},
                {3, 8, 11, 5},
                {9, 11, 11, 12},
                {3, 3, 2, 5},
                {1, 7, 6, 10},
                {3, 4, 7, 4},
                {2, 8, 5, 10},
                {6, 1, 2, 9},
                {11, 10, 7, 6},
                {5, 7, 10, 4},
                {8, 12, 6, 12},
                {1, 12, 8, 9},
                {null, null, null,null },
                {null, null, null,null }
            };
            savedLevelList.Insert(0, new StoredLevel(ConvertToColorBrush(firstLevel), "Never solved this level without adding extra tubes."));

            //{ new Tube(8, 1, 3, 0) },
            //{ new Tube(2, 7, 10, 4) },
            //{ new Tube(8, 10, 10, 11) },
            //{ new Tube(2, 2, 1, 4) },
            //{ new Tube(0, 6, 5, 9) },
            //{ new Tube(2, 3, 6, 3) },
            //{ new Tube(3, 7, 4, 9) },
            //{ new Tube(5, 0, 1, 8) },
            //{ new Tube(10, 9, 6, 5) },
            //{ new Tube(4, 6, 9, 3) },
            //{ new Tube(7, 11, 5, 11) },
            //{ new Tube(0, 11, 7, 8) },
            //{ new Tube() },
            //{ new Tube() },
            //var secondLevel = new StoredLevel(new int?[,] { 
            //    { 0, 0, 0, 0 },
            //    { 1, 1, 1, 1 },
            //    { 2, 2, 2, 2 },
            //    { 3, 3, 3, 3 },
            //    { 4, 4, 4, 4 },
            //    { 5, 5, 5, 5 },
            //    { 6, 6, 6, 6 },
            //    { 7, 7, 7, 7 },
            //    { 8, 8, 8, 8 },
            //    { 9, 9, 9, 9 },
            //    { 10, 10, 10, 10 },
            //    { 11, null, null, null },
            //    { 11, 11, 11, null },
            //    { null, null, null, null },
            //}, "One step before finish.");

            var secondLevel = new int?[,]
            {
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 2, 2, 2, 2 },
                { 3, 3, 3, 3 },
                { 4, 4, 4, 4 },
                { 5, 5, 5, 5 },
                { 6, 6, 6, 6 },
                { 7, 7, 7, 7 },
                { 8, 8, 8, 8 },
                { 9, 9, 9, 9 },
                { 10, 10, 10, 10 },
                { 11, null, null, null },
                { 11, 11, 11, null },
                { null, null, null, null },
            };
            savedLevelList.Insert(0, new StoredLevel(ConvertToColorBrush(secondLevel), "asdfTesting"));


            Settings.Default.SavedLevels = JsonConvert.SerializeObject(savedLevelList);
            //Settings.Default.SavedLevels = JsonConvert.SerializeObject(new ObservableCollection<StoredLevel>() { new StoredLevel(TubesManager.SavedStartingTubes) });
            Settings.Default.Save();
            MainWindowVM.NoteForSavedLevel = null;

            //MainWindowVM.TokenSource = new CancellationTokenSource();
            //var token = MainWindowVM.TokenSource.Token;
            //MainWindowVM.PopupWindowNotification(token);

            Settings.Default.SavedLevels = JsonConvert.SerializeObject(savedLevelList);
            Settings.Default.Save();
        }
        public string ImportGameStateString { get; set; } = string.Empty;
        //public string ImportGameStateString { get; set; } = "[-.-.-.-][-.-.-.-][-.-.03.03][-.02.02.02][-.03.02.03][01.01.01.01]";
        private void FillImportBoxFromClipboard()
        {
            var content = Clipboard.GetText();

            if (content is not null)
                if (content.Substring(0,1) == "[")
                    ImportGameStateString = Clipboard.GetText();
        }
        public RelayCommand ImportExactGameStateCommand => new RelayCommand(execute => ImportExactGameState(), canExecute => ImportGameStateString != string.Empty);
        private void ImportExactGameState()
        {
            var importedGameState = DecodeImportedString(ImportGameStateString);

            MainWindowVM.GameState.SetGameState(importedGameState);
            MainWindowVM.OnStartingLevel();
            MainWindowVM.ClosePopupWindow();
            ImportGameStateString = string.Empty;
        }
        private LiquidColorNew[,] DecodeImportedString(string importString)
        {
            // "[-.-.-.-][-.-.-.-][-.-.03.03][-.02.02.02][-.03.02.03][01.01.01.01]"
            //var countTubes = importString.Count(f => f == '[');
            var importStringTrimmed = importString.Trim(new char[] { '[', ']', '"' });
            var splitToTubes = importStringTrimmed.Split("][");
            LiquidColorNew[,] importedGameState = new LiquidColorNew[splitToTubes.Count(), MainWindowVM.GameState.gameGrid.GetLength(1)];

            for (int x = 0; x < splitToTubes.Length; x++)
            {
                string? tube = splitToTubes[x];
                var layer = tube.Split('.');
                Array.Reverse(layer);

                for (int y = 0; y < layer.Length; y++)
                //for (int y = layer.Length - 1; y >= 0 ; y--)
                {
                    if (layer[y] != "-")
                    {
                        var liquid = Int32.Parse(layer[y]);
                        importedGameState[x, y] = new LiquidColorNew(liquid);
                    }
                }
            }

            return importedGameState;
        }
    }
}
