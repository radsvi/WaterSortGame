using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
                if (layers != value)
                {
                    layers = value;
                    //OnPropertyChanged();
                }
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
                //OnPropertyChanged(); // Tube_PropertyChanged se provadi 2x kdyz jsem to tu mel
                OnPropertyChanged("Margin");
            }
        }

        public Tube()
        {
            TubeId = TubeIdCounter++;
        }
        public Tube(int? firstLayer = null, int? secondLayer = null, int? thirdLayer = null, int? fourthLayer = null)
        {
            TubeId = TubeIdCounter++;

            if (firstLayer is not null) Layers.Add(new Color((int)firstLayer));
            if (secondLayer is not null) Layers.Add(new Color((int)secondLayer));
            if (thirdLayer is not null) Layers.Add(new Color((int)thirdLayer));
            if (fourthLayer is not null) Layers.Add(new Color((int)fourthLayer));
        }

        //private void Tube_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    Debug.WriteLine("asdf");
        //}
    }
}
