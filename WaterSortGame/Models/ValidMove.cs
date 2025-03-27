using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class ValidMove
    {
        public ValidMove(PositionPointer source, PositionPointer target, LiquidColorNew[,] gameState)
        {
            Source = source;
            Target = target;

            Liquid = gameState[source.X, source.Y];
        }

        public PositionPointer Source { get; set; }
        public PositionPointer Target { get; set; }
        public LiquidColorNew Liquid { get; set; }

        //public static bool operator ==(ValidMove first, ValidMove second)
        private static bool OperatorOverload(ValidMove first, ValidMove second)
        {
            //Debug.WriteLine($"first.Source.X [{first.Source.X}] == second.Source.X [{second.Source.X}] && first.Source.Y [{first.Source.Y}] == second.Source.Y [{second.Source.Y}]");
            //Debug.WriteLine($"&& first.Target.X [{first.Target.X}] == second.Target.X[{second.Target.X}] && first.Target.Y [{first.Target.Y}] == second.Target.Y [{second.Target.Y}]");
            //Debug.WriteLine($"&& first.Liquid.Name [{first.Liquid.Name}] == second.Liquid.Name [{second.Liquid.Name}]");
            
            //Debug.WriteLine($"[{first.Source.X}] == [{second.Source.X}] && [{first.Source.Y}] == [{second.Source.Y}]");
            //Debug.WriteLine($"&& [{first.Target.X}] == [{second.Target.X}] && [{first.Target.Y}] == [{second.Target.Y}]");
            //Debug.WriteLine($"&& [{first.Liquid.Name}] == [{second.Liquid.Name}]");
            if (first.Source.X == second.Source.X && first.Source.Y == second.Source.Y
                && first.Target.X == second.Target.X && first.Target.Y == second.Target.Y
                && first.Liquid.Name == second.Liquid.Name)
            {
                return true;
            }
            return false;
        }
        public static bool operator ==(ValidMove first, ValidMove second)
        {
            return OperatorOverload(first, second);
        }
        public static bool operator !=(ValidMove first, ValidMove second)
        {
            return !OperatorOverload(first, second);
        }
    }

}
