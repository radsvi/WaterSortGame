using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.Models
{
    internal class Status
    {
        //public static ObservableCollection<Tube> Tubes
        private ObservableCollection<Tube> step;
        public ObservableCollection<Tube> Step
        {
            get { return step; }
            set
            {
                if (value != step)
                {
                    step = value;
                }
            }
        }
        public Status(ObservableCollection<Tube> step)
        {
            Step = step;
        }

    }
}
