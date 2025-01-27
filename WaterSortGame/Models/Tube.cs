using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class Tube : ViewModelBase
    {
        public int TubeId { get; set; }
        private Liquid fourthLayer;
        public Liquid FourthLayer
        {
            get { return fourthLayer; }
            set
            {
                fourthLayer = value;
                OnPropertyChanged();
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
            FourthLayer = Liquid.GetLiquid(tubeId, 4);
            ThirdLayer = Liquid.GetLiquid(tubeId, 3);
            SecondLayer = Liquid.GetLiquid(tubeId, 2);
            FirstLayer = Liquid.GetLiquid(tubeId, 1);
            
        }

    }
}
