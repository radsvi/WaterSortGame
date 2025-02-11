using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    interface ITube
    {
        int TubeId { get; set; }
        ObservableCollection<Color> Layers { get; set; }
        bool Selected { get; set; }
        ITube DeepCopy();
    }
}
