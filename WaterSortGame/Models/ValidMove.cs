using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class ValidMove
    {
        public ValidMove(PositionPointer source, PositionPointer target)
        {
            Source = source;
            Target = target;
        }

        public PositionPointer Source { get; set; }
        public PositionPointer Target { get; set; }
    }
}
