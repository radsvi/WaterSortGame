using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class PositionPointer
    {
        public PositionPointer(LiquidColorNew[,] gameState, int x, int y)
        {
            X = x;
            Y = y;
            ColorName = (gameState[x, y] != null) ? gameState[x, y].Name : null;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public LiquidColorNames? ColorName { get; set; }
        public bool? AllIdenticalLiquids { get; set; } // true if all liquids in one tube are the same color
        public int NumberOfRepeatingLiquids { get; set; } = 1;
    }
}
