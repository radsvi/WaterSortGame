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
    internal class LiquidColorNew
    {
        public LiquidColorNames Name { get; set; }
        public SolidColorBrush Brush { get; set; }
        public static List<LiquidColorNew> ColorKeys { get; set; } = new List<LiquidColorNew>() {
            new LiquidColorNew(LiquidColorNames.Blue, new SolidColorBrush(Color.FromRgb(20,93,239))),
            new LiquidColorNew(LiquidColorNames.GrayBlue, new SolidColorBrush(Color.FromRgb(63,68,130))),
            new LiquidColorNew(LiquidColorNames.LightBlue, new SolidColorBrush(Color.FromRgb(136,170,255))),
            new LiquidColorNew(LiquidColorNames.Orange, new SolidColorBrush(Color.FromRgb(242,121,20))),
            new LiquidColorNew(LiquidColorNames.Gray, new SolidColorBrush(Color.FromRgb(108,116,144))),
            new LiquidColorNew(LiquidColorNames.Purple, new SolidColorBrush(Color.FromRgb(191,60,191))),
            new LiquidColorNew(LiquidColorNames.Yellow, new SolidColorBrush(Color.FromRgb(244,201,22))),
            new LiquidColorNew(LiquidColorNames.Pink, new SolidColorBrush(Color.FromRgb(255,148,209))),
            new LiquidColorNew(LiquidColorNames.Green, new SolidColorBrush(Color.FromRgb(0,129,96))),
            new LiquidColorNew(LiquidColorNames.LightGreen, new SolidColorBrush(Color.FromRgb(179,214,102))),
            new LiquidColorNew(LiquidColorNames.Olive, new SolidColorBrush(Color.FromRgb(128,153,23))),
            new LiquidColorNew(LiquidColorNames.Red, new SolidColorBrush(Color.FromRgb(188,36,94))),
        };
        public LiquidColorNew() { }
        public LiquidColorNew(int colorId)
        {
            Name = (LiquidColorNames)colorId;
            Brush = GetColor((LiquidColorNames)colorId);
        }
        public LiquidColorNew(LiquidColorNames colorName)
        {
            Name = (LiquidColorNames)colorName;
            Brush = GetColor((LiquidColorNames)colorName);
        }
        private LiquidColorNew(LiquidColorNames name, SolidColorBrush brush)
        {
            Name = name;
            Brush = brush;
        }
        private SolidColorBrush GetColor(LiquidColorNames Name)
        {
            return ColorKeys.Where(key => key.Name == Name).ToList()[0].Brush;

        }
    }
}
