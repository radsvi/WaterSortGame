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
        //public List<object> Contents { get; set; } = new List<object>();
        //public Tube Tube { get; set; }
        public Button ButtonElement { get; set; }
        public int TubeId { get; set; }
        public TubeControl TubeControl { get; set; }
        public Grid GridElement { get; set; }
        public LiquidColorNew TopMostLiquid {  get; set; }
        public int TargetEmptyRow { get; set; }
    }
}
