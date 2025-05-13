using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WaterSortGame.Models
{
    internal class PositionPointer
    {
        private protected PositionPointer() {}
        public PositionPointer(LiquidColorNew[,] gameState, int x, int y)
        {
            X = x;
            Y = y;
            ColorName = (gameState[x, y] != null) ? gameState[x, y].Name : null;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public LiquidColorName? ColorName { get; set; }
        public bool? AllIdenticalLiquids { get; set; } // true if all liquids in one tube are the same color
        public int NumberOfRepeatingLiquids { get; set; } = 1;
    }
    internal class NullPositionPointer : PositionPointer
    {
        public NullPositionPointer() : base()
        {
            X = -1;
            Y = -1;
            ColorName = LiquidColorName.Blank;
            NumberOfRepeatingLiquids = -1;
        }
    }
}
