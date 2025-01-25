using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WaterSortGame.Models
{
    internal class Color
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Rgb { get; set; }
        public int? Opacity { get; set; }
        //public static List<CCodeType> GetValues()
        //{
        //    return new List<CCodeType>()
        //    {
        //        new CCodeType{ Id = 0, Name="EMPTY", Rgb = "#C0C0C0", Opacity = 0 },
        //        new CCodeType{ Id = 1, Name="blue", Rgb = "#145DEF", Opacity = 1 },
        //        new CCodeType{ Id = 2, Name="gray-blue", Rgb = "#3F4482", Opacity = 1 }, // indigo / dark blue
        //        new CCodeType{ Id = 3, Name="light-blue", Rgb = "#88AAFF", Opacity = 1 },
        //        new CCodeType{ Id = 4, Name="orange", Rgb = "#F27914", Opacity = 1 },
        //        new CCodeType{ Id = 5, Name="gray", Rgb = "#6C7490", Opacity = 1 },
        //        new CCodeType{ Id = 6, Name="purple", Rgb = "#BF3CBF", Opacity = 1 },
        //        new CCodeType{ Id = 7, Name="yellow", Rgb = "#F4C916", Opacity = 1 },
        //        new CCodeType{ Id = 8, Name="pink", Rgb = "#FF94D1", Opacity = 1 },
        //        new CCodeType{ Id = 9, Name="green", Rgb = "#008160", Opacity = 1 },
        //        new CCodeType{ Id = 10, Name="light-green", Rgb = "#B3D666", Opacity = 1 },
        //        new CCodeType{ Id = 11, Name="olive", Rgb = "#809917", Opacity = 1 },
        //        new CCodeType{ Id = 12, Name="red", Rgb = "#BC245E", Opacity = 1 }
        //    };
        //}
        //private static CCodeType ContainsKey(int id)
        //{
        //    return GetValues().Where(key => key.Id == id).ToList()[0];
        //}
        private static List<Color> _colorKeys { get; set; } = new List<Color>() {
            new Color{ Id = 0, Name="EMPTY", Rgb = "#C0C0C0", Opacity = 0 },
            new Color{ Id = 1, Name="blue", Rgb = "#145DEF", Opacity = 1 },
            new Color{ Id = 2, Name="gray-blue", Rgb = "#3F4482", Opacity = 1 }, // indigo / dark blue
            new Color{ Id = 3, Name="light-blue", Rgb = "#88AAFF", Opacity = 1 },
            new Color{ Id = 4, Name="orange", Rgb = "#F27914", Opacity = 1 },
            new Color{ Id = 5, Name="gray", Rgb = "#6C7490", Opacity = 1 },
            new Color{ Id = 6, Name="purple", Rgb = "#BF3CBF", Opacity = 1 },
            new Color{ Id = 7, Name="yellow", Rgb = "#F4C916", Opacity = 1 },
            new Color{ Id = 8, Name="pink", Rgb = "#FF94D1", Opacity = 1 },
            new Color{ Id = 9, Name="green", Rgb = "#008160", Opacity = 1 },
            new Color{ Id = 10, Name="light-green", Rgb = "#B3D666", Opacity = 1 },
            new Color{ Id = 11, Name="olive", Rgb = "#809917", Opacity = 1 },
            new Color{ Id = 12, Name="red", Rgb = "#BC245E", Opacity = 1 }
        };

        public static Color GetCode(int id)
        {
            return _colorKeys.Where(key => key.Id == id).ToList()[0];
        }

        public Color() { }
        public Color(int id)
        {
            //CCodeType colorObject = ContainsKey(id);

            //Id = id;
            //Name = colorObject.Name;
            //Rgb = colorObject.Rgb;

            Id = id;
            Name = GetCode(id).Name;
            Rgb = GetCode(id).Rgb;
            Opacity = GetCode(id).Opacity;
        }
    }
}
