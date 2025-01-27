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
            new Liquid(9, 0, 1),
            new Liquid(2, 0, 2),
            new Liquid(4, 0, 3),
            new Liquid(1, 0, 4),

            new Liquid(3, 1, 1),
            new Liquid(8, 1, 2),
            new Liquid(11, 1, 3),
            new Liquid(5, 1, 4),

            new Liquid(9, 2, 1),
            new Liquid(11, 2, 2),
            new Liquid(11, 2, 3),
            new Liquid(12, 2, 4),

            new Liquid(3, 3, 1),
            new Liquid(3, 3, 2),
            new Liquid(2, 3, 3),
            new Liquid(5, 3, 4),

            new Liquid(1, 4, 1),
            new Liquid(7, 4, 2),
            new Liquid(6, 4, 3),
            new Liquid(10, 4, 4),

            new Liquid(3, 5, 1),
            new Liquid(4, 5, 2),
            new Liquid(7, 5, 3),
            new Liquid(4, 5, 4),

            new Liquid(2, 6, 1),
            new Liquid(8, 6, 2),
            new Liquid(5, 6, 3),
            new Liquid(10, 6, 4),

            new Liquid(6, 7, 1),
            new Liquid(1, 7, 2),
            new Liquid(2, 7, 3),
            new Liquid(9, 7, 4),

            new Liquid(11, 8, 1),
            new Liquid(10, 8, 2),
            new Liquid(7, 8, 3),
            new Liquid(6, 8, 4),

            new Liquid(5, 9, 1),
            new Liquid(7, 9, 2),
            new Liquid(10, 9, 3),
            new Liquid(4, 9, 4),

            new Liquid(8, 10, 1),
            new Liquid(12, 10, 2),
            new Liquid(6, 10, 3),
            new Liquid(12, 10, 4),

            new Liquid(1, 11, 1),
            new Liquid(12, 11, 2),
            new Liquid(8, 11, 3),
            new Liquid(9, 11, 4)
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
