using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class Liquid : ViewModelBase
    {
        //public Color Color { get; set; }
        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }
        [Obsolete]public int TubeNumber { get; set; }
        [Obsolete] public int LayerNumber { get; set; } // lowest layer starting at 0

        private bool isFilled = true;
        public bool IsFilled
        {
            get { return isFilled; }
            set
            {
                isFilled = value;
                OnPropertyChanged();
            }
        }

        public Liquid() { }
        public Liquid(int color, int tubeNumber, int layerNumber)
        {
            Color = new Color(color);
            TubeNumber = tubeNumber;
            LayerNumber = layerNumber;
        }

        public static ObservableCollection<Liquid> liquids { get; set; } = new ObservableCollection<Liquid>()
        {
            new Liquid(9, 0, 0),
            new Liquid(2, 0, 1),
            new Liquid(4, 0, 2),
            new Liquid(1, 0, 3),

            new Liquid(3, 1, 0),
            new Liquid(8, 1, 1),
            new Liquid(11, 1, 2),
            new Liquid(5, 1, 3),

            new Liquid(9, 2, 0),
            new Liquid(11, 2, 1),
            new Liquid(11, 2, 2),
            new Liquid(12, 2, 3),

            new Liquid(3, 3, 0),
            new Liquid(3, 3, 1),
            new Liquid(2, 3, 2),
            new Liquid(5, 3, 3),

            new Liquid(1, 4, 0),
            new Liquid(7, 4, 1),
            new Liquid(6, 4, 2),
            new Liquid(10, 4, 3),

            new Liquid(3, 5, 0),
            new Liquid(4, 5, 1),
            new Liquid(7, 5, 2),
            new Liquid(4, 5, 3),

            new Liquid(2, 6, 0),
            new Liquid(8, 6, 1),
            new Liquid(5, 6, 2),
            new Liquid(10, 6, 3),

            new Liquid(6, 7, 0),
            new Liquid(1, 7, 1),
            new Liquid(2, 7, 2),
            new Liquid(9, 7, 3),

            new Liquid(11, 8, 0),
            new Liquid(10, 8, 1),
            new Liquid(7, 8, 2),
            new Liquid(6, 8, 3),

            new Liquid(5, 9, 0),
            new Liquid(7, 9, 1),
            new Liquid(10, 9, 2),
            new Liquid(4, 9, 3),

            new Liquid(8, 10, 0),
            new Liquid(12, 10, 1),
            new Liquid(6, 10, 2),
            new Liquid(12, 10, 3),

            new Liquid(1, 11, 0),
            new Liquid(12, 11, 1),
            new Liquid(8, 11, 2),
            new Liquid(9, 11, 3)
        };

        public static ObservableCollection<Liquid> GetList()
        {
            return liquids;
        }
        public static Liquid GetLiquid(int tubeId, int layerNumber)
        { //Liquid.GetLiquid(tubeId, 4).Color;
            var result = liquids
                .Where(liquid => liquid.TubeNumber == tubeId)
                .Where(liquid => liquid.LayerNumber == layerNumber).ToList();

            if (result.Count() > 0)
                return result[0];
            else
                return new Liquid();

            //return liquids
            //    .Where(liquid => liquid.TubeNumber == tubeId)
            //    .Where(liquid => liquid.LayerNumber == layerNumber).ToList()[0];


            //foreach (Liquid liquid in liquids)
            //{
            //    if (liquid.TubeNumber == tubeId)
            //    {
            //        return liquid;
            //    }
            //}
        }
    }
}
