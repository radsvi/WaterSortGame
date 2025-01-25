using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaterSortGame.Models;
using WaterSortGame.MVVM;

namespace WaterSortGame.ViewModels
{
    class MainWindowVM : ViewModelBase
    {
        public string MyProperty { get; set; } = "Testuju";

        private ObservableCollection<Tube> tubes;
        public ObservableCollection<Tube> Tubes
        {
            get { return tubes; }
            set {
                tubes = value;
                OnPropertyChanged();
            }
        }

        public MainWindowVM()
        {
            Tubes = TubesList.GetTubes();

            
        }

        public RelayCommand EscKeyCommand => new RelayCommand(execute => CloseApplication());
        private void CloseApplication()
        {
            // ## predelat na MVVM model: https://www.youtube.com/watch?v=U7Qclpe2joo
            // ## C:\Users\svihe\Dropbox\Coding\C#\Testing\WpfTutorialsOther\How to Close Windows in MVVM\MainWindowViewModel.cs
            System.Windows.Application.Current.Shutdown();
        }

        public RelayCommand SelectTubeCommand => new RelayCommand(execute => SelectTubeMethod());

        private void SelectTubeMethod()
        {
            MessageBox.Show("qwer");
            //Tubes[1].FirstLayer.Rgb = "#FF0000";
        }
        
    }
}
