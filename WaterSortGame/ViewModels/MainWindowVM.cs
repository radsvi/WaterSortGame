using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private Color selectedLiquid;
        public Color SourceColor
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

            if (SelectedTube == null || SelectedTube == tube)
            {
                SelectLiquid(tube);
            }
                
            else // if selecting different different tube
            {
                if (AddLiquidToTargetTube(tube) == true) // kdyz je target tube plna tak by to nemelo udelat nic.
                {
                    DeselectTube();
                }
            }
        }

        private void SelectLiquid(Tube sourceTube)
        {
            ////for (int i = 0; i < tube.Layers.Count; i++)
            //for (int i = tube.Layers.Count - 1; i >= 0; i--)
            //{
            //    if (tube.Layers[i].Color.Id != 0)
            //    {
            //        SelectedLiquid = tube.Layers[i];
            //        //tube.Layers[i] = null;
            //        return;
            //    }
            //}
            
            if (sourceTube.FourthLayer is not null)
            {
                SelectedTube = sourceTube;
                SourceColor = sourceTube.FourthLayer;
                return;
            }
            if (sourceTube.ThirdLayer is not null)
            {
                SelectedTube = sourceTube;
                SourceColor = sourceTube.ThirdLayer;
                return;
            }
            if (sourceTube.SecondLayer is not null)
            {
                SelectedTube = sourceTube;
                SourceColor = sourceTube.SecondLayer;
                return;
            }
            if (sourceTube.FirstLayer is not null)
            {
                SelectedTube = sourceTube;
                SourceColor = sourceTube.FirstLayer;
                return;
            }

            //MessageBox.Show("Cannot select empty Vial.");
        }

        private bool AddLiquidToTargetTube(Tube targetTube)
        {
            if (targetTube.FirstLayer is null)
            {
                targetTube.FirstLayer = SourceColor;
                RemoveColorFromSourceTube(targetTube);
                SourceColor.LayerNumber = 0;
                return true;
            }
            if (targetTube.SecondLayer is null)
            {
                targetTube.SecondLayer = SourceColor;
                RemoveColorFromSourceTube(targetTube);
                SourceColor.LayerNumber = 1;
                return true;
            }
            if (targetTube.ThirdLayer is null)
            {
                targetTube.ThirdLayer = SourceColor;
                RemoveColorFromSourceTube(targetTube);
                SourceColor.LayerNumber = 2;
                return true;
            }
            if (targetTube.FourthLayer is null)
            {
                targetTube.FourthLayer = SourceColor;
                RemoveColorFromSourceTube(targetTube);
                SourceColor.LayerNumber = 3;
                return true;
            }

            return false;
        }
        private void RemoveColorFromSourceTube(Tube targetTube)
        {
            if (SourceColor.LayerNumber == 3)
                SelectedTube.FourthLayer = null;
            else if (SourceColor.LayerNumber == 2)
                SelectedTube.ThirdLayer = null;
            else if (SourceColor.LayerNumber == 1)
                SelectedTube.SecondLayer = null;
            else if (SourceColor.LayerNumber == 0)
                SelectedTube.FirstLayer = null;

            SourceColor.TubeNumber = targetTube.TubeId;
            
        }

        private void DeselectTube()
        {
            //tube.Layers.Remove(SelectedLiquid);
            //SelectedLiquid.IsFilled = false;
            SelectedTube.Selected = false;
            SelectedTube = null;
            //SelectedLiquid = null;
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
