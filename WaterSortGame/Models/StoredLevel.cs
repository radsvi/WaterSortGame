using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    class StoredLevel : ViewModelBase
    {
        public int NumberOfColors { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public ObservableCollection<Tube> GameState { get; set; }
        private bool markedForDeletion;
        public bool MarkedForDeletion
        {
            get { return markedForDeletion; }
            set
            {
                if (value != markedForDeletion)
                {
                    markedForDeletion = value;
                    OnPropertyChanged();
                }
            }
        }
        public string GetMarkedStatus()
        {
            if (markedForDeletion == true)
            {
                return "marked";
            }
            else
            {
                return null;
            }
        }

        [JsonConstructor]
        public StoredLevel(ObservableCollection<Tube> tubes, string noteForSavedLevel)
        {
            this.GameState = tubes;
            this.Date = DateTime.Now;
            this.Note = noteForSavedLevel;

            if(tubes is null)
            {
                return;
            }
            List<int?> colorIds = new List<int?>();
            foreach (var tube in tubes)
            {
                foreach (var layer in tube.Layers)
                {
                    if (colorIds.Contains(layer.Id) == false)
                    {
                        colorIds.Add(layer.Id);
                        this.NumberOfColors++;
                    }
                }
            }
        }
    }
}
