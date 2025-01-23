using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    class CCodeType
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Rgb { get; set; }
        public int? Opacity { get; set; }

        public CCodeType(string name, string rgb, int opacity)
        {
            Name = name;
            Rgb = rgb;
            Opacity = opacity;
        }

        public string GetName()
        {
            return Name;
        }
        public CCodeType() { }
    }
}
