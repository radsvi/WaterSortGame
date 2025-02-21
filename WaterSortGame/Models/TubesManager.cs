using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaterSortGame.Properties;
using WaterSortGame.ViewModels;
using WaterSortGame.Views;

namespace WaterSortGame.Models
{
    internal class TubesManager : ViewModelBase
    {
        //public static ObservableCollection<Tube> _tubes = new ObservableCollection<Tube>();
        private static ObservableCollection<Tube> _tubes = new ObservableCollection<Tube>();
        public static ObservableCollection<Tube> Tubes
        {
            get { return _tubes; }
            set
            {
                if (value != _tubes)
                {
                    _tubes = value;
                    OnGlobalPropertyChanged("NumberOfColorsToGenerate");
                }
            }
        }
        static TubesManager()
        {
            if (Tubes.Count == 0)
                GenerateNewLevel();
        }

        //public event PropertyChangedEventHandler? PropertyChanged;
        //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public static event PropertyChangedEventHandler GlobalPropertyChanged = delegate { };
        public static void OnGlobalPropertyChanged(string propertyName)
        {
            GlobalPropertyChanged?.Invoke(
                typeof(TubesManager),
                new PropertyChangedEventArgs(propertyName));
        }
        //public void TubesManager()
        //{
        //    // This should use a weak event handler instead of normal handler
        //    GlobalPropertyChanged += this.HandleGlobalPropertyChanged;
        //}
        //void HandleGlobalPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case "NumberOfColorsToGenerate":
        //            if (length > MinimumLength)
        //                length = MinimumLength;
        //            break;
        //    }
        //}

        public static int ExtraTubes { get; set; } = 0;
        private static int numberOfColorsToGenerate = Settings.Default.NumberOfColorsToGenerate;
        public static int NumberOfColorsToGenerate
        {
            get { return numberOfColorsToGenerate; }
            set
            {
                if (numberOfColorsToGenerate != value)
                {
                    if (value >= 3 && value <= Color.ColorKeys.Count)
                    {
                        numberOfColorsToGenerate = value;
                    }
                    else if (value < 3)
                    {
                        numberOfColorsToGenerate = 3;
                    }
                    else if (value > Color.ColorKeys.Count)
                    {
                        numberOfColorsToGenerate = Color.ColorKeys.Count;
                    }
                    //OnPropertyChanged();
                    //OnGlobalPropertyChanged("NumberOfColorsToGenerate");
                    Settings.Default.NumberOfColorsToGenerate = numberOfColorsToGenerate;
                    Settings.Default.Save();
                }
            }
        }
        private static bool randomNumberOfTubes = Settings.Default.RandomNumberOfTubes;
        public static bool RandomNumberOfTubes
        {
            get { return randomNumberOfTubes; }
            set
            {
                randomNumberOfTubes = value;
                Settings.Default.RandomNumberOfTubes = value;
                Settings.Default.Save();
            }
        }

        private static int maximumExtraTubes = Settings.Default.MaximumExtraTubes;
        public static int MaximumExtraTubes
        {
            get { return maximumExtraTubes; }
            set
            {
                if (maximumExtraTubes != value)
                {
                    if (value >= 0 && value <= 20)
                    {
                        maximumExtraTubes = value;
                    }
                    else if (value < 0)
                    {
                        maximumExtraTubes = 0;
                    }
                    else if (value > 20)
                    {
                        maximumExtraTubes = 20;
                    }
                    Settings.Default.MaximumExtraTubes = value;
                    Settings.Default.Save();
                }
            }
        }
        public static ObservableCollection<Tube> SavedStartingTubes = new ObservableCollection<Tube>();
        public static void GenerateNewLevel()
        {
            GenerateNewTubes();
        }

        public static void AddExtraTube()
        {
            if (ExtraTubes <= MaximumExtraTubes)
            {
                Tubes.Add(new Tube());
                ExtraTubes++;
            }
        }
        public static void RestartLevel()
        {
            SettingFreshGameState();
            //SavedStartingTubes?.Clear();
            Tubes?.Clear();
            foreach (var tube in SavedStartingTubes)
            {
                Tubes.Add((Tube)tube.DeepCopy());
            }
        }
        //public static void LoadLevel()
        //{
        //    Tubes?.Clear();
        //    if (false)
        //    {
        //        Tubes.Add(new Tube(8, 1, 3, 0));
        //        Tubes.Add(new Tube(2, 7, 10, 4));
        //        Tubes.Add(new Tube(8, 10, 10, 11));
        //        Tubes.Add(new Tube(2, 2, 1, 4));
        //        Tubes.Add(new Tube(0, 6, 5, 9));
        //        Tubes.Add(new Tube(2, 3, 6, 3));
        //        Tubes.Add(new Tube(3, 7, 4, 9));
        //        Tubes.Add(new Tube(5, 0, 1, 8));
        //        Tubes.Add(new Tube(10, 9, 6, 5));
        //        Tubes.Add(new Tube(4, 6, 9, 3));
        //        Tubes.Add(new Tube(7, 11, 5, 11));
        //        Tubes.Add(new Tube(0, 11, 7, 8));
        //        Tubes.Add(new Tube());
        //        Tubes.Add(new Tube());
        //    }
        //    else
        //    {
        //        Tubes.Add(new Tube(0, 0, 0, 0));
        //        Tubes.Add(new Tube(1, 1, 1, 1));
        //        Tubes.Add(new Tube(2, 2, 2, 2));
        //        Tubes.Add(new Tube(3, 3, 3, 3));
        //        Tubes.Add(new Tube(4, 4, 4, 4));
        //        Tubes.Add(new Tube(5, 5, 5, 5));
        //        Tubes.Add(new Tube(6, 6, 6, 6));
        //        Tubes.Add(new Tube(7, 7, 7, 7));
        //        Tubes.Add(new Tube(8, 8, 8, 8));
        //        Tubes.Add(new Tube(9, 9, 9, 9));
        //        Tubes.Add(new Tube(10, 10, 10, 10));
        //        Tubes.Add(new Tube(11, 11, 11));
        //        Tubes.Add(new Tube(11));
        //        //Tubes.Add(new Tube(9, 11, 10, 11));
        //        //Tubes.Add(new Tube(9, 10, 11, 10));
        //        Tubes.Add(new Tube());
        //        Tubes.Add(new Tube());
        //    }

        //    StoreStartingTubes();
        //}
        public static void GenerateNewTubes()
        {
            SettingFreshGameState();
            Random rnd = new Random();

            ObservableCollection<Color> colorsList = new ObservableCollection<Color>();
            if (RandomNumberOfTubes)
            {
                NumberOfColorsToGenerate = rnd.Next(3, Color.ColorKeys.Count);
            }

            List<int> selectedColors = new List<int>();
            for (int i = 0; i < Color.ColorKeys.Count; i++) // generate list of all 12 colors
            {
                selectedColors.Add(i);
            }

            for (int i = 0; i < Color.ColorKeys.Count - NumberOfColorsToGenerate; i++) // now remove some random ones
            {
                //selectedColors.Remove(selectedColors[NumberOfColorsToGenerate]); // this always keeps the same colors
                selectedColors.Remove(selectedColors[rnd.Next(0, selectedColors.Count)]);
            }
            foreach (var color in selectedColors)
            {
                colorsList.Add(new Color(color));
                colorsList.Add(new Color(color));
                colorsList.Add(new Color(color));
                colorsList.Add(new Color(color));
            }

            Tubes?.Clear();
            //var tubes = new ObservableCollection<Tube>();
            for (int i = 0; i < NumberOfColorsToGenerate; i++)
            {
                Color[] layer = new Color[4];
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
        private static void SettingFreshGameState()
        {
            ExtraTubes = 0; // resets how much extra tubes has been added
        }
        private static void StoreStartingTubes()
        {
            SavedStartingTubes?.Clear();
            foreach (Tube tube in Tubes)
            {
                SavedStartingTubes.Add(tube.DeepCopy());
            }
        }
    }
}