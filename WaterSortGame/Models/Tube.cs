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
        //private Liquid[] layers = new Liquid[4];
        //public Liquid[] Layers
        //{
        //    get { return layers; }
        //    set
        //    {
        //        layers = value;
        //        OnPropertyChanged();
        //    }
        //}
        //private ObservableCollection<Liquid> layers = new ObservableCollection<Liquid>();
        //public ObservableCollection<Liquid> Layers
        //{
        //    get { return layers; }
        //    set
        //    {
        //        layers = value;
        //        OnPropertyChanged();
        //    }
        //}


        private Liquid fourthLayer;
        public Liquid FourthLayer
        {
            get {
                //return fourthLayer;
                return fourthLayer;
            }
            set
            {
                fourthLayer = value;
                OnPropertyChanged();

                //LiquidsManager.LiquidProperty
            }
        }
        private Liquid thirdLayer;
        public Liquid ThirdLayer
        {
            get { return thirdLayer; }
            set
            {
                thirdLayer = value;
                OnPropertyChanged();
            }
        }
        private Liquid secondLayer;
        public Liquid SecondLayer
        {
            get { return secondLayer; }
            set
            {
                secondLayer = value;
                OnPropertyChanged();
            }
        }
        private Liquid firstLayer;
        public Liquid FirstLayer
        {
            get { return firstLayer; }
            set
            {
                firstLayer = value;
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
            //set
            //{
            //    margin = value;
            //    OnPropertyChanged();
            //}
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

        public Tube(int tubeId)
        {
            TubeId = tubeId;
            //for (int i = 0; i < 4; i++)
            //{
            //    //layers[i] = Liquid.GetLiquid(tubeId, i);
            //    layers.Add(null);
            //}

            //FourthLayer = Liquid.GetLiquid(tubeId, 4);
            //ThirdLayer = Liquid.GetLiquid(tubeId, 3);
            //SecondLayer = Liquid.GetLiquid(tubeId, 2);
            //FirstLayer = Liquid.GetLiquid(tubeId, 1);

            FourthLayer = GetLiquid(3);
            ThirdLayer = GetLiquid(2);
            SecondLayer = GetLiquid(1);
            FirstLayer = GetLiquid(0);

            if (FourthLayer is not null)
                FourthLayer.InternalPropertyChanged += InternalPropertyChanged;
            if (ThirdLayer is not null)
                ThirdLayer.InternalPropertyChanged += InternalPropertyChanged;
            if (SecondLayer is not null)
                SecondLayer.InternalPropertyChanged += InternalPropertyChanged;
            if (FirstLayer is not null)
                FirstLayer.InternalPropertyChanged += InternalPropertyChanged;

        }

        private void InternalPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Liquid liquid = sender as Liquid;
            if (liquid.LayerNumber == 3)
                FourthLayer = GetLiquid(3);
            if (liquid.LayerNumber == 2)
                ThirdLayer = GetLiquid(2);
            if (liquid.LayerNumber == 1)
                SecondLayer = GetLiquid(1);
            if (liquid.LayerNumber == 0)
                FirstLayer = GetLiquid(0);
        }

        public Liquid GetLiquid(int layerNumber)
        { //Liquid.GetLiquid(tubeId, 4).Color;
            var result = LiquidsManager.LiquidProperty
                .Where(liquid => liquid.TubeNumber == TubeId)
                .Where(liquid => liquid.LayerNumber == layerNumber).ToList();

            if (result.Count() > 0)
                return result[0];
            else
                return null;
        }
        public void UpdateLayer(int layerNumber)
        {
            Liquid liquid = GetLiquid(layerNumber);

            if (layerNumber == 3)
                FourthLayer = liquid;
            if (layerNumber == 2)
                ThirdLayer = liquid;
            if (layerNumber == 1)
                SecondLayer = liquid;
            if (layerNumber == 0)
                FirstLayer = liquid;
        }

        public event PropertyChangedEventHandler DestinationTubeChanged;
        protected virtual void OnDestinationTubeChanged([CallerMemberName] string propertyName = null)
        {
            DestinationTubeChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }
}
