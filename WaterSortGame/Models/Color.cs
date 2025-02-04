using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WaterSortGame.Models
{
    internal class Color : ViewModelBase
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Rgb { get; set; }
        
        public static List<Color> ColorKeys { get; set; } = new List<Color>() {
            //new Color{ Id = 0, Name="EMPTY", Rgb = "#C0C0C0" },
            new Color{ Id = 0, Name="blue", Rgb = "#145DEF" },
            new Color{ Id = 1, Name="gray-blue", Rgb = "#3F4482" }, // indigo / dark blue
            new Color{ Id = 2, Name="light-blue", Rgb = "#88AAFF" },
            new Color{ Id = 3, Name="orange", Rgb = "#F27914" },
            new Color{ Id = 4, Name="gray", Rgb = "#6C7490" },
            new Color{ Id = 5, Name="purple", Rgb = "#BF3CBF" },
            new Color{ Id = 6, Name="yellow", Rgb = "#F4C916" },
            new Color{ Id = 7, Name="pink", Rgb = "#FF94D1" },
            new Color{ Id = 8, Name="green", Rgb = "#008160" },
            new Color{ Id = 9, Name="light-green", Rgb = "#B3D666" },
            new Color{ Id = 10, Name="olive", Rgb = "#809917" },
            new Color{ Id = 11, Name="red", Rgb = "#BC245E" }
        };

        public static Color GetCode(int id)
        {
            return ColorKeys.Where(key => key.Id == id).ToList()[0];
        }

        public Color() { }
        public Color(int id)
        {
            Id = id;
            Name = GetCode(id).Name;
            Rgb = GetCode(id).Rgb;
        }
    }
}
