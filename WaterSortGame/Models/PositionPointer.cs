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
        public PositionPointer(LiquidColor[,] gameState, int x, int y)
        {
            X = x;
            Y = y;
            ColorName = (gameState[x, y] != null) ? gameState[x, y].Name : null;
        }
        public PositionPointer(LiquidColorName colorName, int x, int y, int numberOfRepeatingLiquids)
        {
            X = x;
            Y = y;
            ColorName = colorName;
            NumberOfRepeatingLiquids = numberOfRepeatingLiquids;
        }
        public int X { get; private set; }
        public int Y { get; private set; }
        public LiquidColorName? ColorName { get; set; }
        public bool? AllIdenticalLiquids { get; set; } // true if all liquids in one tube are the same color
        public int NumberOfRepeatingLiquids { get; set; } = 1;
    }
    internal class NullPositionPointer : PositionPointer
    {
        public NullPositionPointer() : base(LiquidColorName.Blank, -1, -1, -1) {}
    }
}
