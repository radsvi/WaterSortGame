using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                return GetLiquid(3);
            }
            set
            {
                //fourthLayer = value;
                //OnPropertyChanged();

                //LiquidsManager.LiquidProperty
            }
        }
        private Liquid thirdLayer;
        public Liquid ThirdLayer
        {
            get { return GetLiquid(2); }
            set
            {
                thirdLayer = value;
                OnPropertyChanged();
            }
        }
        private Liquid secondLayer;
        public Liquid SecondLayer
        {
            get { return GetLiquid(1); }
            set
            {
                secondLayer = value;
                OnPropertyChanged();
            }
        }
        private Liquid firstLayer;
        public Liquid FirstLayer
        {
            get { return GetLiquid(0); }
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
            
        }

        public Liquid GetLiquid(int layerNumber)
        { //Liquid.GetLiquid(tubeId, 4).Color;
            var result = LiquidsManager.LiquidProperty
                .Where(liquid => liquid.TubeNumber == TubeId)
                .Where(liquid => liquid.LayerNumber == layerNumber).ToList();

            if (result.Count() > 0)
                return result[0];
            else
                return new Liquid();
        }

    }
}
