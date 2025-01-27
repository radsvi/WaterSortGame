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
        public Color FourthLayer { get; set; }
        public Color ThirdLayer { get; set; }
        public Color SecondLayer { get; set; }
        public Color FirstLayer { get; set; }
        private string margin;
        public string Margin
        {
            get { return margin; }
            set
            {
                margin = value;
                OnPropertyChanged();
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
            }
        }

        public Tube(int tubeId)
        {
            TubeId = tubeId;
            FourthLayer = Liquid.GetLiquid(tubeId, 4).Color;
            ThirdLayer = Liquid.GetLiquid(tubeId, 3).Color;
            SecondLayer = Liquid.GetLiquid(tubeId, 2).Color;
            FirstLayer = Liquid.GetLiquid(tubeId, 1).Color;
            Margin = "0,30,0,0";
        }

    }
}
