using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class TubesManager : ViewModelBase
    {
        private MainWindowVM MainWindowVM;
        private AppSettings AppSettings;
        //private MainWindowVM MainWindow;
        //private readonly int[,] gameState;
        private ObservableCollection<Tube> tubes = new ObservableCollection<Tube>();
        public ObservableCollection<Tube> Tubes
        {
            get { return tubes; }
            set
            {
                if (value != tubes)
                {
                    tubes = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ExtraTubesAdded { get; set; } = 0;
        public ObservableCollection<Tube> StartingPosition = new ObservableCollection<Tube>();

        public TubesManager(MainWindowVM mainWindowVM)
        {
            MainWindowVM = mainWindowVM;
            AppSettings = MainWindowVM.AppSettings;

            if (Tubes.Count == 0)
            {
                GenerateNewLevel();
            }
        }
        //private void OnLiquidMoving()
        //{
        //    MainWindow.DrawTubes();
        //}
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
            Tubes?.Clear();

            Tubes.Add(new Tube(1, 1, 4, 4));
            Tubes.Add(new Tube(8, 8, 1, 1));
            Tubes.Add(new Tube(4, 4, 8, 8));
            Tubes.Add(new Tube());
            Tubes.Add(new Tube());

            StoreStartingTubes();
        }
        public void AddExtraTube() // this is for adding extra tube during gameplay
        {
            if (ExtraTubesAdded <= AppSettings.MaximumExtraTubes)
            {
                Tubes.Add(new Tube());
                ExtraTubesAdded++;
            }
        }
        public void RestartLevel()
        {
            SettingFreshGameState();
            //SavedStartingTubes?.Clear();
            Tubes?.Clear();
            foreach (var tube in StartingPosition)
            {
                Tubes.Add((Tube)tube.DeepCopy());
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
