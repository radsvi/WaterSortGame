using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class Tube
    {
        //public CCodeType? FirstLayer { get; set; }
        //public CCodeType? SecondLayer { get; set; }
        //public CCodeType? ThirdLayer { get; set; }
        //public CCodeType? FourthLayer { get; set; }
        //public Tube() { }

        ////public Tube(CCodeType firstLayer, CCodeType secondLayer, CCodeType thirdLayer, CCodeType fourthLayer)
        ////{
        ////    FirstLayer = firstLayer;
        ////    SecondLayer = secondLayer;
        ////    ThirdLayer = thirdLayer;
        ////    FourthLayer = fourthLayer;
        ////}
        //public Tube(int firstLayer, int secondLayer, int thirdLayer, int fourthLayer)
        //{
        //    //FirstLayer = ColorKeys.GetValues().Where(key => key.Id == firstLayer).ToList()[0];
        //    //SecondLayer = ColorKeys.GetValues().Where(key => key.Id == secondLayer).ToList()[0];
        //    //ThirdLayer = ColorKeys.GetValues().Where(key => key.Id == thirdLayer).ToList()[0];
        //    //FourthLayer = ColorKeys.GetValues().Where(key => key.Id == fourthLayer).ToList()[0];
        //    FirstLayer = ColorKeys.ContainsKey(firstLayer);
        //    SecondLayer = ColorKeys.ContainsKey(secondLayer);
        //    ThirdLayer = ColorKeys.ContainsKey(thirdLayer);
        //    FourthLayer = ColorKeys.ContainsKey(fourthLayer);
        //}

        public int TubeId { get; set; }
        public Color FourthLayer { get; set; }
        public Color ThirdLayer { get; set; }
        public Color SecondLayer { get; set; }
        public Color FirstLayer { get; set; }

        public Tube(int tubeId)
        {
            TubeId = tubeId;
            FourthLayer = Liquid.GetLiquid(tubeId, 4).Color;
            ThirdLayer = Liquid.GetLiquid(tubeId, 3).Color;
            SecondLayer = Liquid.GetLiquid(tubeId, 2).Color;
            FirstLayer = Liquid.GetLiquid(tubeId, 1).Color;

            //FourthLayer = Color.GetCode(1);
            //ThirdLayer = Color.GetCode(1);
            //SecondLayer = Color.GetCode(1);
            //FirstLayer = Color.GetCode(1);
        }

    }
}
