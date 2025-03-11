using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WaterSortGame.Models
{
    [Obsolete]internal class LiquidColor : ViewModelBase
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Rgb { get; set; }

        public static List<LiquidColor> ColorKeys { get; set; } = new List<LiquidColor>() {
            //new Color{ Id = 0, Name="EMPTY", Rgb = "#C0C0C0" },
            new LiquidColor{ Id = 0, Name="blue", Rgb = "#145DEF" },
            new LiquidColor{ Id = 1, Name="gray-blue", Rgb = "#3F4482" }, // indigo / dark blue
            new LiquidColor{ Id = 2, Name="light-blue", Rgb = "#88AAFF" },
            new LiquidColor{ Id = 3, Name="orange", Rgb = "#F27914" },
            new LiquidColor{ Id = 4, Name="gray", Rgb = "#6C7490" },
            new LiquidColor{ Id = 5, Name="purple", Rgb = "#BF3CBF" },
            new LiquidColor{ Id = 6, Name="yellow", Rgb = "#F4C916" },
            new LiquidColor{ Id = 7, Name="pink", Rgb = "#FF94D1" },
            new LiquidColor{ Id = 8, Name="green", Rgb = "#008160" },
            new LiquidColor{ Id = 9, Name="light-green", Rgb = "#B3D666" },
            new LiquidColor{ Id = 10, Name="olive", Rgb = "#809917" },
            new LiquidColor{ Id = 11, Name="red", Rgb = "#BC245E" }
        };

        public static LiquidColor GetCode(int id)
        {
            return ColorKeys.Where(key => key.Id == id).ToList()[0];
        }

        public LiquidColor() { }
        public LiquidColor(int id)
        {
            Id = id;
            Name = GetCode(id).Name;
            Rgb = GetCode(id).Rgb;
        }
    }
}
