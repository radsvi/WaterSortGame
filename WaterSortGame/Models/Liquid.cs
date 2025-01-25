using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class Liquid
    {
        public Color Color { get; set; }
        public int TubeNumber { get; set; }
        public int LayerNumber { get; set; } // lowest layer starting at 0

        public Liquid(int color, int tubeNumber, int layerNumber)
        {
            Color = new Color(color);
            TubeNumber = tubeNumber;
            LayerNumber = layerNumber;
        }

        public static ObservableCollection<Liquid> liquids { get; set; } = new ObservableCollection<Liquid>()
        {
            new Liquid(9, 1, 1),
            new Liquid(2, 1, 2),
            new Liquid(4, 1, 3),
            new Liquid(1, 1, 4),

            new Liquid(3, 2, 1),
            new Liquid(8, 2, 2),
            new Liquid(11, 2, 3),
            new Liquid(5, 2, 4),

            new Liquid(9, 3, 1),
            new Liquid(11, 3, 2),
            new Liquid(11, 3, 3),
            new Liquid(12, 3, 4),

            new Liquid(3, 4, 1),
            new Liquid(3, 4, 2),
            new Liquid(2, 4, 3),
            new Liquid(5, 4, 4),

            new Liquid(1, 5, 1),
            new Liquid(7, 5, 2),
            new Liquid(6, 5, 3),
            new Liquid(10, 5, 4),

            new Liquid(3, 6, 1),
            new Liquid(4, 6, 2),
            new Liquid(7, 6, 3),
            new Liquid(4, 6, 4),

            new Liquid(2, 7, 1),
            new Liquid(8, 7, 2),
            new Liquid(5, 7, 3),
            new Liquid(10, 7, 4),

            new Liquid(6, 8, 1),
            new Liquid(1, 8, 2),
            new Liquid(2, 8, 3),
            new Liquid(9, 8, 4),

            new Liquid(11, 9, 1),
            new Liquid(10, 9, 2),
            new Liquid(7, 9, 3),
            new Liquid(6, 9, 4),

            new Liquid(5, 10, 1),
            new Liquid(7, 10, 2),
            new Liquid(10, 10, 3),
            new Liquid(4, 10, 4),

            new Liquid(8, 11, 1),
            new Liquid(12, 11, 2),
            new Liquid(6, 11, 3),
            new Liquid(12, 11, 4),

            new Liquid(1, 12, 1),
            new Liquid(12, 12, 2),
            new Liquid(8, 12, 3),
            new Liquid(9, 12, 4)
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
                return new Liquid(0, tubeId, layerNumber);

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
