using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WaterSortGame.Models;
using WaterSortGame.MVVM;

namespace WaterSortGame.ViewModels
{
    class MainWindowVM : ViewModelBase
    {
        public string MyProperty { get; set; } = "Testuju";

        private Tube selectedTube;
        public Tube SelectedTube
        {
            get { return selectedTube; }
            set
            {
                if (selectedTube is not null)
                    selectedTube.Margin = "0,30,0,0";

                if (selectedTube == value)
                {
                    //Debug.WriteLine("test");
                    selectedTube.Margin = "0,30,0,0";
                    selectedTube = null;
                }
                else
                {
                    selectedTube = value;
                    selectedTube.Margin = "0,0,0,30";
                }
                OnPropertyChanged();
            }
        }
        private Liquid selectedLiquid;
        public Liquid SelectedLiquid
        {
            get { return selectedLiquid; }
            set
            {
                selectedLiquid = value;
                OnPropertyChanged();
            }
        }
        private Tube targetTube;
        public Tube TargetTube
        {
            get { return targetTube; }
            set
            {
                targetTube = value;
                OnPropertyChanged();
            }
        }

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

        //public RelayCommand SelectTubeCommand => new RelayCommand(execute => SelectTubeMethod());

        public RelayCommand SelectTubeCommand => new RelayCommand(execute => SelectTube(execute));

        private void SelectTube(object obj)
        {
            var tube = obj as Tube;

            if (selectedTube == null || selectedTube == tube)
                SelectedTube = tube;
            else
            {
                removeTopLiquid(SelectedTube);
            }

        }

        private void removeTopLiquid(Tube tube)
        {
            if (tube.FourthLayer.Id != 0)
            {
                //SelectedLiquid = ;
            }
            //
        }

        //public RelayCommand ResetTubesCommand => new RelayCommand(execute => ResetTubes());

        //private void ResetTubes()
        //{
        //    foreach (var tube in Tubes)
        //    {
        //        tube.Margin = "0,30,0,0";
        //    }
        //}


    }
}
