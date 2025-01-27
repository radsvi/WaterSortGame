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
        public Liquid FourthLayer { get; set; }
        public Liquid ThirdLayer { get; set; }
        public Liquid SecondLayer { get; set; }
        public Liquid FirstLayer { get; set; }
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
            FourthLayer = Liquid.GetLiquid(tubeId, 4);
            ThirdLayer = Liquid.GetLiquid(tubeId, 3);
            SecondLayer = Liquid.GetLiquid(tubeId, 2);
            FirstLayer = Liquid.GetLiquid(tubeId, 1);
            Margin = "0,30,0,0";
        }

    }
}
