using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class Tube
    {
        public CCodeType? FirstLayer { get; set; }
        public CCodeType? SecondLayer { get; set; }
        public CCodeType? ThirdLayer { get; set; }
        public CCodeType? FourthLayer { get; set; }
        public Tube() { }

        //public Tube(CCodeType firstLayer, CCodeType secondLayer, CCodeType thirdLayer, CCodeType fourthLayer)
        //{
        //    FirstLayer = firstLayer;
        //    SecondLayer = secondLayer;
        //    ThirdLayer = thirdLayer;
        //    FourthLayer = fourthLayer;
        //}
        public Tube(int firstLayer, int secondLayer, int thirdLayer, int fourthLayer)
        {
            //FirstLayer = ColorKeys.GetValues().Where(key => key.Id == firstLayer).ToList()[0];
            //SecondLayer = ColorKeys.GetValues().Where(key => key.Id == secondLayer).ToList()[0];
            //ThirdLayer = ColorKeys.GetValues().Where(key => key.Id == thirdLayer).ToList()[0];
            //FourthLayer = ColorKeys.GetValues().Where(key => key.Id == fourthLayer).ToList()[0];
            FirstLayer = ColorKeys.ContainsKey(firstLayer);
            SecondLayer = ColorKeys.ContainsKey(secondLayer);
            ThirdLayer = ColorKeys.ContainsKey(thirdLayer);
            FourthLayer = ColorKeys.ContainsKey(fourthLayer);
        }
    }
}
