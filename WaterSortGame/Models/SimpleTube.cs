using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class SimpleTube
    {
        public LiquidColorNew Color { get; set; }
        public int TubeNumber { get; set; }
        public int Count { get; set; }
        public SimpleTube(LiquidColorNew color, int tubeNumber, int count)
        {
            Color = color;
            TubeNumber = tubeNumber;
            Count = count;
        }
    }
}
