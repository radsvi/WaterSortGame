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
                if (selectedTube is null)
                {
                    selectedTube = value;
                    //selectedTube.Margin = "0,0,0,30";
                    selectedTube.Selected = true;
                    OnPropertyChanged();
                    return;
                }
                if (selectedTube == value) // pokud clicknu na stejnou zkumavku znova
                {
                    //Debug.WriteLine("test");
                    //selectedTube.Margin = "0,30,0,0";
                    selectedTube.Selected = false;
                    selectedTube = null;
                    OnPropertyChanged();
                    return;
                }
                if (selectedTube is not null && selectedTube != value)
                {
                    selectedTube.Selected = false;
                    //selectedTube.Margin = "0,30,0,0";
                    selectedTube = value;
                    selectedTube = null;
                    OnPropertyChanged();
                    return;
                }
                
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
            else // if selecting different different tube
            {
                SelectLiquid(SelectedTube);
                if (AddLiquidToTube(tube) == true)
                    RemoveLiquidFromSourceTube(SelectedTube);
            }

        }

        private void SelectLiquid(Tube tube)
        {
            //for (int i = 0; i < tube.Layers.Count; i++)
            for (int i = tube.Layers.Count - 1; i >= 0; i--)
            {
                if (tube.Layers[i].Color.Id != 0)
                {
                    SelectedLiquid = tube.Layers[i];
                    //tube.Layers[i] = null;
                    return;
                }
            }
        }
        private void RemoveLiquidFromSourceTube(Tube tube)
        {
            //tube.Layers.Remove(SelectedLiquid);
            SelectedLiquid.IsFilled = false;
            SelectedTube.Selected = false;
            SelectedTube = null;
            SelectedLiquid = null;
        }

        private bool AddLiquidToTube(Tube tube)
        {
            //if (tube.FirstLayer == null)
            //for (int i = tube.Layers.Count - 1; i >= 0; i--)
            int count = tube.Layers.Count; // this is needed, otherwise the loop will be skipped for empty tubes.

            //if (count == 0)
            //{
            //    tube.Layers.Insert(0, SelectedLiquid);
            //    return true;
            //}

            for (int i = 0; i < count + 1; i++)
            {
                //if (tube.Layers[i] is null)
                //if (tube.Layers[i].Color.Id == 0)
                if (tube.Layers[i].Color is null)
                {
                    tube.Layers.Remove(tube.Layers[i]);
                    //insert
                    //tube.Layers.Add(SelectedLiquid);
                    tube.Layers.Insert(i, SelectedLiquid);
                    return true;
                }
            }
            return false;

            //AddLiquidToLayer(tube, 0);
        }
        //private void AddLiquidToLayer(Tube tube, int layerNumber)
        //{
        //    if (SelectedLiquid is not null)
        //    {
        //        tube.Layers[layerNumber] = SelectedLiquid;
        //        SelectedTube.Selected = false;
        //        SelectedTube = null;
        //        SelectedLiquid = null;
        //        //SelectedTube = null;
        //    }
        //}

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
