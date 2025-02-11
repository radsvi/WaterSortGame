using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    class StoredLevel : Tube
    {
        public int NumberOfColors { get; set; }
        public DateTime Date { get; set; }

        public StoredLevel(Tube tube)
        {
            TubeId = Tube.TubeIdCounter++;

            foreach (var layer in tube.Layers)
            {
                if (layer is not null)
                {
                    NumberOfColors++;
                    Layers.Add(layer);
                }
                Date = DateTime.Now;
            }
        }
        public StoredLevel(int? firstLayer = null, int? secondLayer = null, int? thirdLayer = null, int? fourthLayer = null)
        {
            TubeId = TubeIdCounter++;

            if (firstLayer is not null) Layers.Add(new Color((int)firstLayer));
            if (secondLayer is not null) Layers.Add(new Color((int)secondLayer));
            if (thirdLayer is not null) Layers.Add(new Color((int)thirdLayer));
            if (fourthLayer is not null) Layers.Add(new Color((int)fourthLayer));

            foreach (var layer in Layers)
            {
                NumberOfColors++;
            }
            Date = DateTime.Now;
        }
        public StoredLevel(int[] layers = null)
        {
            TubeId = TubeIdCounter++;
            foreach (var layer in layers)
            {
                Layers.Add(new Color(layer));
                NumberOfColors++;
            }
            Date = DateTime.Now;
        }
        [JsonConstructor]
        public StoredLevel(int tubeId, int[] layers = null)
        {
            TubeId = tubeId;
            foreach (var layer in layers)
            {
                Layers.Add(new Color(layer));
                NumberOfColors++;
            }
            Date = DateTime.Now;
        }

    }
}
