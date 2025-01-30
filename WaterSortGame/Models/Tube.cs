using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;
using WaterSortGame.Views;

namespace WaterSortGame.Models
{
    internal class Tube : ViewModelBase
    {
        public int TubeId { get; set; }
        private static int TubeIdCounter;

        private ObservableCollection<Color> layers = new ObservableCollection<Color>();
        public ObservableCollection<Color> Layers
        {
            get { return layers; }
            set
            {
                layers = value;
                OnPropertyChanged();
            }
        }

        private string margin;
        public string Margin
        {
            get {
                if (selected == true)
                    return "0,0,0,30";
                else
                    return "0,30,0,0";
            }
        }
        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged();
                OnPropertyChanged("Margin");
            }
        }

        public Tube()
        {
            TubeId = TubeIdCounter++;
        }
        public Tube(int firstLayer, int secondLayer, int thirdLayer, int fourthLayer)
        {
            TubeId = TubeIdCounter++;

            Layers.Add(new Color(firstLayer, TubeId, 0));
            Layers.Add(new Color(secondLayer, TubeId, 1));
            Layers.Add(new Color(thirdLayer, TubeId, 2));
            Layers.Add(new Color(fourthLayer, TubeId, 3));
        }
    }
}
