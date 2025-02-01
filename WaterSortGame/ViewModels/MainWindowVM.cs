using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using WaterSortGame.Models;
using WaterSortGame.MVVM;
using WaterSortGame.Properties;

namespace WaterSortGame.ViewModels
{
    class MainWindowVM : ViewModelBase
    {
        #region Properties

        private ViewModelBase _selectedViewModel;
        public ViewModelBase SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged();
            }
        }
        public ICommand UpdateViewCommand { get; set; }

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
        private Color sourceLiquid;
        public Color SourceLiquid
        {
            get { return sourceLiquid; }
            set
            {
                sourceLiquid = value;
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
        #endregion
        public MainWindowVM(IWindowService windowService)
        //public MainWindowVM()
        {
            this.windowService = windowService;
            Tubes = TubesList.GetTubes();
            PropertyChanged += Tube_PropertyChanged;
        }
        public bool LevelComplete { get; set; }

        #region Navigation
        public RelayCommand EscKeyCommand => new RelayCommand(execute => CloseWindow());
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
            LevelComplete = false;
            DeselectTube();
        }
        public RelayCommand AddExtraTubeCommand => new RelayCommand(execute => TubesList.AddExtraTube(), canExecute => TubesList.ExtraTubes < TubesList.MaximumExtraTubes);
        #endregion
        #region OptionsWindow
        private IWindowService windowService;

        public RelayCommand OpenOptionsWindowCommand => new RelayCommand(execute => OpenOptionsWindow());
        private void OpenOptionsWindow()
        {
            windowService?.OpenWindow(this);
        }
        private void CloseWindow()
        {
            windowService?.CloseWindow();
        }
        #endregion
        

        #region Moving Liquids
        public RelayCommand SelectTubeCommand => new RelayCommand(execute => SelectTube(execute));
        private void SelectTube(object obj)
        {
            var tube = obj as Tube;

            if (SelectedTube == null)
            {
                SelectLiquid(tube);
                return;
            }
            if (SelectedTube == tube)
            {
                DeselectTube();
                return;
            }

            // if selecting different tube
            bool success = false;
            bool successAtLeastOnce = false;

            do {
                success = AddLiquidToTargetTube(tube);
                if (success == true)
                {
                    successAtLeastOnce = true;
                    SelectLiquid(SelectedTube); // vyber dalsi liquid ze stejne zkumavky
                }
            } while (success == true && SourceLiquid is not null);
            if (successAtLeastOnce == true)
            {
                DeselectTube();
            }
        }
        private void SelectLiquid(Tube sourceTube) // selects topmost liquid in a sourceTube
        {
            SourceLiquid = null;
            for (int i = sourceTube.Layers.Count - 1; i >= 0; i--)
            {
                if (sourceTube.Layers[i] is not null)
                {
                    if (SelectedTube != sourceTube)
                        SelectedTube = sourceTube;
                    SourceLiquid = sourceTube.Layers[i];
                    return;
                }
            }
        }
        private bool AddLiquidToTargetTube(Tube targetTube)
        {
            if (targetTube.Layers.Count >= 4)
                return false;

            if (targetTube.Layers.Count != 0)
                if (targetTube.Layers[targetTube.Layers.Count - 1].Id != SourceLiquid.Id)
                    return false;

            targetTube.Layers.Add(SourceLiquid);
            RemoveColorFromSourceTube(targetTube);
            //SourceColor.LayerNumber = targetTube.Layers.Count - 1;
            //SourceColor.LayerNumber = targetTube.Layers.IndexOf(SourceColor);
            return true;
        }

        private void RemoveColorFromSourceTube(Tube targetTube)
        {
            SelectedTube.Layers.Remove(SourceLiquid);
            //SourceColor.TubeNumber = targetTube.TubeId;
        }

        private void DeselectTube()
        {
            if (SelectedTube is not null)
            {
                SelectedTube.Selected = false;
                SelectedTube = null;
            }
        }

        private void Tube_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            
            if (CompareAllTubes() && LevelComplete == false)
            {
                LevelComplete = true;
                LevelWonMessage();
            }
        }
        private bool CompareAllTubes()
        {
            foreach (var tube in Tubes)
            {
                if (tube.Layers.Count != 4 && tube.Layers.Count != 0) // pokud zkumavka neni plna, nebo uplne prazdna, nemuze to byt level dokoncenej
                {
                    return false;
                }
                for (var i = 0; i < tube.Layers.Count; i++)
                {
                    for (int j = i + 1; j < tube.Layers.Count; j++)
                    {
                        if (tube.Layers[i].Id != tube.Layers[j].Id)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private void LevelWonMessage()
        {
            MessageBox.Show("Level complete!");

            // pridat veci jako otazka jestli chci zopakovat level, nebo vygenerovat novej
            // pripadne zkusit udelat nejakou grafiku, ohnostroj
        }
        #endregion


    }
}
