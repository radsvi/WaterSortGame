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

        public MainWindowVM(IWindowService windowService)
        //public MainWindowVM()
        {
            _windowService = windowService;
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

        public RelayCommand RestartCommand => new RelayCommand(execute => Restart());
        private void Restart()
        {
            TubesList.GenerateTubes(true);
        }
        public RelayCommand AddExtraTubeCommand => new RelayCommand(execute => TubesList.AddExtraTube(), canExecute => TubesList.ExtraTubes < TubesList.MaximumExtraTubes);
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
            for (int i = sourceTube.Layers.Count - 1; i >= 0; i--)
            {
                if (sourceTube.Layers[i] is not null)
                {
                    SelectedTube = sourceTube;
                    SourceColor = sourceTube.Layers[i];
                    return;
                }
            }
        }

        private bool AddLiquidToTargetTube(Tube targetTube)
        {
            if (targetTube.Layers.Count >= 4)
                return false;

            if (targetTube.Layers.Count != 0)
                if (targetTube.Layers[targetTube.Layers.Count - 1].Id != SourceColor.Id)
                    return false;

            targetTube.Layers.Add(SourceColor);
            RemoveColorFromSourceTube(targetTube);
            //SourceColor.LayerNumber = targetTube.Layers.Count - 1;
            //SourceColor.LayerNumber = targetTube.Layers.IndexOf(SourceColor);
            return true;
        }

        private void RemoveColorFromSourceTube(Tube targetTube)
        {
            SelectedTube.Layers.Remove(SourceColor);
            //SourceColor.TubeNumber = targetTube.TubeId;
        }

        private void DeselectTube()
        {
            SelectedTube.Selected = false;
            SelectedTube = null;
        }

        #region OptionsWindow
        private IWindowService _windowService;
        public RelayCommand OpenOptionsWindowCommand => new RelayCommand(execute => OpenOptionsWindow());
        private void OpenOptionsWindow()
        {
            _windowService?.OpenWindow(this);
        }
        private void OnCloseWindow()
        {
            _windowService?.CloseWindow();
        }
        #endregion
    }
}
