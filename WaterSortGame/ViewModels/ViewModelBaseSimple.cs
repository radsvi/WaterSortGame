using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.MVVM;

namespace WaterSortGame.ViewModels
{
    public class ViewModelBaseSimple
    {
        public SimpleCommand SimpleCommand { get; set; }
        public ViewModelBaseSimple()
        {
            this.SimpleCommand = new SimpleCommand(this);
        }
        public void SimpleMethod()
        {
            Debug.WriteLine("hello");
        }
        public void ParameterMethod(string person)
        {
            Debug.WriteLine(person);
        }
    }
}
