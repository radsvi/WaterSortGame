using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaterSortGame.Views.UserControls;

namespace WaterSortGame.Models
{
    class TubeReference
    {
        private int targetEmptyRow;
        public TubeReference(TubeControl tubeControl, Button buttonElement, int tubeId, Grid gridElement)
        {
            TubeControl = tubeControl;
            ButtonElement = buttonElement;
            TubeId = tubeId;
            GridElement = gridElement;
        }
        public TubeReference(int tubeId, LiquidColor lastColorMoved, int targetEmptyRow, int numberOfRepeatingLiquids)
        {
            TubeId = tubeId;
            LastColorMoved = lastColorMoved;
            TargetEmptyRow = targetEmptyRow;
            NumberOfRepeatingLiquids = numberOfRepeatingLiquids;
        }

        //public List<object> Contents { get; set; } = new List<object>();
        //public Tube Tube { get; set; }
        public Button ButtonElement { get; private set; }
        public int TubeId { get; private set; }
        public TubeControl TubeControl { get; private set; }
        public Grid GridElement { get; private set; }
        public LiquidColor TopMostLiquid {  get; set; }
        public LiquidColor LastColorMoved { get; set; }
        public int TargetEmptyRow
        {
            get => targetEmptyRow;
            set
            {
                if (value != targetEmptyRow)
                {
                    if (targetEmptyRow <= GameState.Layers)
                        targetEmptyRow = value;
                    else
                        targetEmptyRow = GameState.Layers;
                }
            }
        }
        private int numberOfRepeatingLiquids;
        public int NumberOfRepeatingLiquids
        {
            get => numberOfRepeatingLiquids;
            set
            {
                if (value != numberOfRepeatingLiquids)
                {
                    if (TargetEmptyRow + value <= GameState.Layers)
                        numberOfRepeatingLiquids = value;
                    else
                        numberOfRepeatingLiquids = GameState.Layers - TargetEmptyRow;
                }
            }
        }
    }
}
