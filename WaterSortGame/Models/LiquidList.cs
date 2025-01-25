using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.Views;

namespace WaterSortGame.Models // ## smazat
{
    [ObsoleteAttribute]
    internal class LiquidList
    {
        public static ObservableCollection<Liquid> liquids { get; set; } = new ObservableCollection<Liquid>()
        {
            new Liquid(1, 1, 1),
            new Liquid(1, 1, 2),
            new Liquid(1, 1, 3),
            new Liquid(1, 1, 4),

            new Liquid(2, 1, 1),
            new Liquid(2, 1, 2),
            new Liquid(2, 1, 3),
            new Liquid(2, 1, 4),

            new Liquid(3, 1, 1),
            new Liquid(3, 1, 2),
            new Liquid(3, 1, 3),
            new Liquid(3, 1, 4),
        };

        public static ObservableCollection<Liquid> GetList()
        {
            return liquids;
        }
        public static Liquid GetLiquid(int tubeId, int layerNumber)
        {
            return liquids
                .Where(liquid => liquid.TubeNumber == tubeId)
                .Where(liquid => liquid.LayerNumber == layerNumber).ToList()[0];
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
