using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class TubesList
    {
        public static ObservableCollection<Tube> _tubes = new ObservableCollection<Tube>()
        {
            new Tube(0),
            new Tube(1),
            new Tube(2),
            new Tube(3),
            new Tube(4),
            new Tube(5),
            new Tube(6),
            new Tube(7),
            new Tube(8),
            new Tube(9),
            new Tube(10),
            new Tube(11),
            new Tube(12),
            new Tube(13),
        };

        public static ObservableCollection<Tube> GetTubes()
        {
            return _tubes;
        }
        private static ObservableCollection<Tube> tubesProperty;
        public static ObservableCollection<Tube> TubesProperty
        {
            get { return _tubes; }
        }

        public static void ChangeLiquidPosition(Color color, int sourceTube, int sourceLayer, int targetTube, int targetLayer)
        {
            //_tubes.Remove(sourceTube);
            //_tubes.Add(targetTube);

            if (sourceLayer == 3)
            {
                _tubes[sourceTube].FourthLayer = null;

                if (sourceLayer == 0)
                {
                    _tubes[targetTube].FirstLayer.Color = color;
                }
            }
            
        }
    }
}
