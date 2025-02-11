using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    class StoredLevel
    {
        public int NumberOfColors { get; set; }
        public DateTime Date { get; set; }
        public ObservableCollection<Tube> GameState { get; set; }

        [JsonConstructor]
        public StoredLevel(ObservableCollection<Tube> tubes)
        {
            this.GameState = tubes;
            Date = DateTime.Now;

            List<int?> colorIds = new List<int?>();
            foreach (var tube in tubes)
            {
                foreach (var layer in tube.Layers)
                {
                    if (colorIds.Contains(layer.Id) == false)
                    {
                        colorIds.Add(layer.Id);
                        NumberOfColors++;
                    }
                }
            }
        }
    }
}
