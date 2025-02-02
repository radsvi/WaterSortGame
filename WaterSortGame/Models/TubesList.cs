using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaterSortGame.Properties;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class TubesList : ViewModelBase
    {
        public static int ExtraTubes { get; set; } = 0;
        public static int NumberOfColorsGenerated { get; set; }
        public static bool RandomNumberOfTubes { get; set; }

        //public static int MaximumExtraTubes { get; set; } = Settings.Default.MaximumExtraTubes;
        private static int maximumExtraTubes = Settings.Default.MaximumExtraTubes;
        public static int MaximumExtraTubes
        {
            get { return maximumExtraTubes; }
            set
            {
                maximumExtraTubes = value;
                //OnPropertyChanged(nameof(MaximumExtraTubes));
                //OnPropertyChanged("MaximumExtraTubes");
                Settings.Default.MaximumExtraTubes = value;
                Settings.Default.Save();
            }
        }
        //static TubesList()
        //{
        //    MaximumExtraTubes = Settings.Default.MaximumExtraTubes;
        //}

        //public static ObservableCollection<Tube> _tubes = new ObservableCollection<Tube>()
        //{
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
        //{
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
        //    {
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
        //public static void StartNewLevel(bool regenerate = false)
        public static void StartNewLevel()
        {
            //if (regenerate is true)
            //{
            //    _tubes.Clear();
            //}

            //if (false)
            //{
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
        public static void GenerateNewTubes()
        {
            ExtraTubes = 0; // resets how much extra tubes has been added
            Random rnd = new Random();
            
            _tubes?.Clear();
            ObservableCollection<Color> colorsList = new ObservableCollection<Color>();
            if (RandomNumberOfTubes)
            {
                NumberOfColorsGenerated = rnd.Next(6, 12);
            }
            else
            {
                NumberOfColorsGenerated = 12;
            }

            foreach (var color in Color.ColorKeys)
            {
                colorsList.Add(color);
                colorsList.Add(color);
                colorsList.Add(color);
                colorsList.Add(color);
            }

            for (int i = 0; i < NumberOfColorsGenerated; i++)
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
