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
                    OnGlobalPropertyChanged("NumberOfColorsToGenerate");
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
        //static TubesList()
        //{
        //    MaximumExtraTubes = Settings.Default.MaximumExtraTubes;
        //}

        //public static ObservableCollection<Tube> _tubes = new ObservableCollection<Tube>()
        //{ // posunul jsem IDcka zkumavek o jedna. spravit!
        //    new Tube(0),
        //    new Tube(1),
        //    new Tube(2),
        //    new Tube(3),
        //    new Tube(4),
        //    new Tube(5),
        //    new Tube(6),
        //    new Tube(7),
        //    new Tube(8),
        //    new Tube(9),
        //    new Tube(10),
        //    new Tube(11),
        //    new Tube(12),
        //    new Tube(13),
        //};

        //private static readonly ObservableCollection<Tube> _defaultTubes = new ObservableCollection<Tube>()
        //{ // posunul jsem IDcka zkumavek o jedna. spravit!
        //    new Tube(9, 2, 4, 1),
        //    new Tube(3, 8, 11, 5),
        //    new Tube(9, 11, 11, 12),
        //    new Tube(3, 3, 2, 5),
        //    new Tube(1, 7, 6, 10),
        //    new Tube(3, 4, 7, 4),
        //    new Tube(2, 8, 5, 10),
        //    new Tube(6, 1, 2, 9),
        //    new Tube(11, 10, 7, 6),
        //    new Tube(5, 7, 10, 4),
        //    new Tube(8, 12, 6, 12),
        //    new Tube(1, 12, 8, 9),
        //    //new Tube(0, 0, 0, 0),
        //    //new Tube(0, 0, 0, 0)
        //    new Tube(),
        //    new Tube(),
        //};

        //public static ObservableCollection<Tube> GetDefaultValues()
        //{
        //    return new ObservableCollection<Tube>()
        //    { // posunul jsem IDcka zkumavek o jedna. spravit!
        //        new Tube(9, 2, 4, 1),
        //        new Tube(3, 8, 11, 5),
        //        new Tube(9, 11, 11, 12),
        //        new Tube(3, 3, 2, 5),
        //        new Tube(1, 7, 6, 10),
        //        new Tube(3, 4, 7, 4),
        //        new Tube(2, 8, 5, 10),
        //        new Tube(6, 1, 2, 9),
        //        new Tube(11, 10, 7, 6),
        //        new Tube(5, 7, 10, 4),
        //        new Tube(8, 12, 6, 12),
        //        new Tube(1, 12, 8, 9),
        //        //new Tube(0, 0, 0, 0),
        //        //new Tube(0, 0, 0, 0)
        //        new Tube(),
        //        new Tube(),
        //    };
        //}

        public static ObservableCollection<Tube> _tubes = new ObservableCollection<Tube>();
        public static ObservableCollection<Tube> SavedStartingTubes = new ObservableCollection<Tube>();

        private int countTubes;
        public int CountTubes
        {
            get
            {
                return (int)Math.Ceiling((decimal)_tubes.Count / 2);
            }
            //set
            //{
            //    countTubes = value;
            //}
        }
        //public static void StartNewLevel(bool regenerate = false)
        public static void StartNewLevel()
        {
            //if (regenerate is true)
            //{
            //    _tubes.Clear();
            //}

            //if (false)
            //{ // posunul jsem IDcka zkumavek o jedna. spravit!
            //    _tubes.Add(new Tube(9, 2, 4, 1));
            //    _tubes.Add(new Tube(3, 8, 11, 5));
            //    _tubes.Add(new Tube(9, 11, 11, 12));
            //    _tubes.Add(new Tube(3, 3, 2, 5));
            //    _tubes.Add(new Tube(1, 7, 6, 10));
            //    _tubes.Add(new Tube(3, 4, 7, 4));
            //    _tubes.Add(new Tube(2, 8, 5, 10));
            //    _tubes.Add(new Tube(6, 1, 2, 9));
            //    _tubes.Add(new Tube(11, 10, 7, 6));
            //    _tubes.Add(new Tube(5, 7, 10, 4));
            //    _tubes.Add(new Tube(8, 12, 6, 12));
            //    _tubes.Add(new Tube(1, 12, 8, 9));
            //    _tubes.Add(new Tube());
            //    _tubes.Add(new Tube());
            //    _tubes.Add(new Tube()); // extra
            //}
            //else
            //{
            //    _tubes.Add(new Tube(1, 1, 1, 1));
            //    _tubes.Add(new Tube(2, 2, 2, 2));
            //    _tubes.Add(new Tube(3, 3, 3, 3));
            //    _tubes.Add(new Tube(4, 4, 4, 4));
            //    _tubes.Add(new Tube(5, 5, 5, 5));
            //    _tubes.Add(new Tube(6, 6, 6, 6));
            //    _tubes.Add(new Tube(7, 7, 7, 7));
            //    _tubes.Add(new Tube(8, 8, 8, 8));
            //    _tubes.Add(new Tube(9, 9, 9, 9));
            //    _tubes.Add(new Tube(10, 10, 10, 10));
            //    _tubes.Add(new Tube(11, 11, 11, 12));
            //    _tubes.Add(new Tube(12, 12, 12, 11));
            //    _tubes.Add(new Tube());
            //    _tubes.Add(new Tube());
            //}

            GenerateNewTubes();
        }

        public static void AddExtraTube()
        {
            if (ExtraTubes <= MaximumExtraTubes)
            {
                _tubes.Add(new Tube());
                ExtraTubes++;
            }
        }
        public static void RestartLevel()
        {
            //SavedStartingTubes?.Clear();
            _tubes?.Clear();
            foreach (var tube in SavedStartingTubes)
            {
                _tubes.Add((Tube)tube.DeepCopy());
            }
        }
        public static void LoadLevel()
        {
            _tubes?.Clear();
            if (false)
            {
                _tubes.Add(new Tube(8, 1, 3, 0));
                _tubes.Add(new Tube(2, 7, 10, 4));
                _tubes.Add(new Tube(8, 10, 10, 11));
                _tubes.Add(new Tube(2, 2, 1, 4));
                _tubes.Add(new Tube(0, 6, 5, 9));
                _tubes.Add(new Tube(2, 3, 6, 3));
                _tubes.Add(new Tube(3, 7, 4, 9));
                _tubes.Add(new Tube(5, 0, 1, 8));
                _tubes.Add(new Tube(10, 9, 6, 5));
                _tubes.Add(new Tube(4, 6, 9, 3));
                _tubes.Add(new Tube(7, 11, 5, 11));
                _tubes.Add(new Tube(0, 11, 7, 8));
                _tubes.Add(new Tube());
                _tubes.Add(new Tube());
            }
            else
            {
                _tubes.Add(new Tube(0, 0, 0, 0));
                _tubes.Add(new Tube(1, 1, 1, 1));
                _tubes.Add(new Tube(2, 2, 2, 2));
                _tubes.Add(new Tube(3, 3, 3, 3));
                _tubes.Add(new Tube(4, 4, 4, 4));
                _tubes.Add(new Tube(5, 5, 5, 5));
                _tubes.Add(new Tube(6, 6, 6, 6));
                _tubes.Add(new Tube(7, 7, 7, 7));
                _tubes.Add(new Tube(8, 8, 8, 8));
                //_tubes.Add(new Tube(9, 9, 9, 9));
                //_tubes.Add(new Tube(10, 10, 10, 10));
                //_tubes.Add(new Tube(11, 11, 11, 11));
                _tubes.Add(new Tube(9, 11, 10, 11));
                _tubes.Add(new Tube(9, 10, 11, 10));
                _tubes.Add(new Tube());
                _tubes.Add(new Tube());
            }

            SaveStartingTubes();
        }
        public static void GenerateNewTubes()
        {
            ExtraTubes = 0; // resets how much extra tubes has been added
            Random rnd = new Random();
            
            _tubes?.Clear();
            ObservableCollection<Color> colorsList = new ObservableCollection<Color>();
            if (RandomNumberOfTubes)
            {
                NumberOfColorsToGenerate = rnd.Next(3, Color.ColorKeys.Count);
            }

            List<int> selectedColors = new List<int>();
            for (int i = 0; i < NumberOfColorsToGenerate; i++) // generate list of all 12 colors
            {
                selectedColors.Add(i);
            }

            for (int i = 0; i < selectedColors.Count - NumberOfColorsToGenerate; i++) // now remove some random ones
            {
                selectedColors.Remove(selectedColors[NumberOfColorsToGenerate]);
            }
            foreach (var color in selectedColors)
            {
                colorsList.Add(new Color(color));
                colorsList.Add(new Color(color));
                colorsList.Add(new Color(color));
                colorsList.Add(new Color(color));
            }

            for (int i = 0; i < NumberOfColorsToGenerate; i++)
            {
                Color[] layer = new Color[4];
                for (int j = 0; j < 4; j++)
                {
                    int colorNumber = rnd.Next(0, colorsList.Count);
                    layer[j] = colorsList[colorNumber];
                    colorsList.Remove(colorsList[colorNumber]);
                }

                _tubes.Add(new Tube(layer[0], layer[1], layer[2], layer[3]));
            }
            _tubes.Add(new Tube());
            _tubes.Add(new Tube());

            SaveStartingTubes();
        }
        private static void SaveStartingTubes()
        {
            SavedStartingTubes?.Clear();
            foreach (var tube in _tubes)
            {
                SavedStartingTubes.Add((Tube)tube.DeepCopy());
            }
        }

        public static ObservableCollection<Tube> GetTubes()
        {
            if (_tubes.Count == 0)
                StartNewLevel();

            return _tubes;
        }
        private static ObservableCollection<Tube> tubesProperty;
        public static ObservableCollection<Tube> TubesProperty
        {
            get { return _tubes; }
        }
    }
}
