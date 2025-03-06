using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WaterSortGame.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WaterSortGame.Models
{
    internal class TubeManagerNew : ViewModelBase
    {
        private MainWindowVM MainWindowVM;
        private AppSettings AppSettings;
        //private MainWindowVM MainWindow;
        //private readonly int[,] gameState;
        public int Tubes { get; set; }
        public int Layers { get; } = 4;
        //public int this[int tubes, int layers]
        //{
        //    get => gameState[tubes, layers];
        //    set
        //    {
        //        gameState[tubes, layers] = value;
        //        //OnLiquidMoving();
        //    }
        //}

        private List<List<LiquidColorNew>> gameState;
        public LiquidColorNew this[int tube, int layer]
        {
            get
            {
                //return gameStateList.ElementAt(tubes).ElementAt(layers);
                return gameState[tube][layer];
            }
            set
            {
                gameState[tube][layer] = value;
            }
        }
        //ElementAt

        public int ExtraTubesAdded { get; set; } = 0;
        public ObservableCollection<Tube> StartingPosition = new ObservableCollection<Tube>();

        //public TubeManagerNew(int tubes, MainWindowVM mainWindow)
        public TubeManagerNew(MainWindowVM mainWindowVM, int tubes)
        {
            Tubes = tubes;
            //gameState = new int[Tubes, Layers];
            gameState = new List<List<LiquidColorNew>>();
            for (int i = 0; i < Tubes; i++)
            {
                var t = new List<LiquidColorNew>();
                for (int j = 0; j < Layers; j++)
                {
                    t.Add(new LiquidColorNew());
                }
                gameState.Add(t);
            }

            MainWindowVM = mainWindowVM;
            AppSettings = mainWindowVM.AppSettings;
        }
        public TubeManagerNew(int tubes)
        {
            gameState = new List<List<LiquidColorNew>>();
            for (int i = 0; i < Tubes; i++)
            {
                var t = new List<LiquidColorNew>();
                for (int j = 0; j < Layers; j++)
                {
                    t.Add(new LiquidColorNew());
                }
                gameState.Add(t);
            }
        }
        //private void OnLiquidMoving()
        //{
        //    MainWindow.DrawTubes();
        //}
        private void AddTube()
        {
            Tubes++;
            for (int i = 0; i < Layers; i++)
            {
                this[++Tubes, i] = new LiquidColorNew();
            }
        }
        private void AddTube(LiquidColorNames[] colorNames)
        {
            Tubes++;
            int i = 0;
            foreach (LiquidColorNames name in colorNames)
            {
                this[Tubes, i] = new LiquidColorNew(name);
            }
            //for (int i = 0; i < Layers; i++)
            //{
            //    this[Tubes, i] = new LiquidColorNew(colorName);
            //}
        }
        private void AddTube(int[] colorIds)
        {
            var colorNames = Array.ConvertAll(colorIds, item => (LiquidColorNames)item);
            AddTube(colorNames);
        }

        public void GenerateNewLevel()
        {
            if (AppSettings.LoadDebugLevel is true)
                GenerateDebugLevel();
            else
                GenerateStandardLevel();
        }
        private void GenerateDebugLevel()
        {
            SettingFreshGameState();
            gameState?.Clear();

            AddTube(new int[] { 1, 1, 4, 4 });
            AddTube(new int[] { 8, 8, 1, 1 });
            AddTube(new int[] { 4, 4, 8, 8 });
            AddTube();
            AddTube();

            StoreStartingTubes();
        }
        public void AddExtraTube() // this is for adding extra tube during gameplay
        {
            if (ExtraTubesAdded <= AppSettings.MaximumExtraTubes)
            {
                AddTube();
                ExtraTubesAdded++;
            }
        }
        public void RestartLevel()
        {
            SettingFreshGameState();
            //SavedStartingTubes?.Clear();
            gameState?.Clear();
            foreach (var tube in StartingPosition)
            {
                Tubes.Add((Tube)tube.DeepCopy());
                AddTube()
            }
        }
        private void GenerateStandardLevel()
        {
            SettingFreshGameState();
            Random rnd = new Random();

            ObservableCollection<LiquidColor> colorsList = new ObservableCollection<LiquidColor>();
            if (AppSettings.RandomNumberOfTubes)
            {
                AppSettings.NumberOfColorsToGenerate = rnd.Next(3, LiquidColor.ColorKeys.Count);
            }

            List<int> selectedColors = new List<int>();
            for (int i = 0; i < LiquidColor.ColorKeys.Count; i++) // generate list of all 12 colors
            {
                selectedColors.Add(i);
            }

            for (int i = 0; i < LiquidColor.ColorKeys.Count - AppSettings.NumberOfColorsToGenerate; i++) // now remove some random ones
            {
                //selectedColors.Remove(selectedColors[NumberOfColorsToGenerate]); // this always keeps the same colors
                selectedColors.Remove(selectedColors[rnd.Next(0, selectedColors.Count)]);
            }
            foreach (var color in selectedColors)
            {
                colorsList.Add(new LiquidColor(color));
                colorsList.Add(new LiquidColor(color));
                colorsList.Add(new LiquidColor(color));
                colorsList.Add(new LiquidColor(color));
            }

            Tubes?.Clear();
            //var tubes = new ObservableCollection<Tube>();
            for (int i = 0; i < AppSettings.NumberOfColorsToGenerate; i++)
            {
                LiquidColor[] layer = new LiquidColor[4];
                for (int j = 0; j < 4; j++)
                {
                    int colorNumber = rnd.Next(0, colorsList.Count);
                    layer[j] = colorsList[colorNumber];
                    colorsList.Remove(colorsList[colorNumber]);
                }

                Tubes.Add(new Tube(layer[0], layer[1], layer[2], layer[3]));
            }
            Tubes.Add(new Tube());
            Tubes.Add(new Tube());

            //Tubes?.Clear();
            //Tubes.AddRange(tubes); // have it here like this because I dont want to call CollectionChanged event during the generation
            // ## change into BulkObservableCollection<T> Class ? 


            StoreStartingTubes();
        }
        private void SettingFreshGameState()
        {
            ExtraTubesAdded = 0; // resets how much extra tubes has been added
        }
        private void StoreStartingTubes()
        {
            StartingPosition?.Clear();
            foreach (Tube tube in Tubes)
            {
                StartingPosition.Add(tube.DeepCopy());
            }
        }

    }
}
