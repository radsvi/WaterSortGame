using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.MVVM
{
    interface IWindowService
    {
        void OpenWindow(object dataContext);
        void CloseWindow();
    }
}
