using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using WaterSortGame.Models;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal enum LiquidColorNames
    {
        Blue,
        GrayBlue,
        LightBlue,
        Orange,
        Gray,
        Purple,
        Yellow,
        Pink,
        Green,
        LightGreen,
        Olive,
        Red,
    }
    internal class LiquidColorsNew
    {
        public LiquidColorNames Name { get; set; }
        public SolidColorBrush Brush { get; set; }
        public static List<LiquidColorsNew> ColorKeys { get; set; } = new List<LiquidColorsNew>() {
            //new Color{ Id = 0, Name="EMPTY", Rgb = "#C0C0C0" },
            new LiquidColorsNew(LiquidColorNames.Blue, new SolidColorBrush(Color.FromRgb(20,93,239))),
            new LiquidColorsNew(LiquidColorNames.GrayBlue, new SolidColorBrush(Color.FromRgb(63,68,130))),
            new LiquidColorsNew(LiquidColorNames.LightBlue, new SolidColorBrush(Color.FromRgb(136,170,255))),
            new LiquidColorsNew(LiquidColorNames.Orange, new SolidColorBrush(Color.FromRgb(242,121,20))),
            new LiquidColorsNew(LiquidColorNames.Gray, new SolidColorBrush(Color.FromRgb(108,116,144))),
            new LiquidColorsNew(LiquidColorNames.Purple, new SolidColorBrush(Color.FromRgb(191,60,191))),
            new LiquidColorsNew(LiquidColorNames.Yellow, new SolidColorBrush(Color.FromRgb(244,201,22))),
            new LiquidColorsNew(LiquidColorNames.Pink, new SolidColorBrush(Color.FromRgb(255,148,209))),
            new LiquidColorsNew(LiquidColorNames.Green, new SolidColorBrush(Color.FromRgb(0,129,96))),
            new LiquidColorsNew(LiquidColorNames.LightGreen, new SolidColorBrush(Color.FromRgb(179,214,102))),
            new LiquidColorsNew(LiquidColorNames.Olive, new SolidColorBrush(Color.FromRgb(128,153,23))),
            new LiquidColorsNew(LiquidColorNames.Red, new SolidColorBrush(Color.FromRgb(188,36,94))),
        };
        private LiquidColorsNew(LiquidColorNames name, SolidColorBrush brush)
        {
            Name = name;
            Brush = brush;
        }
    }
}
