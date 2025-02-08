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
using WaterSortGame.Views;

namespace WaterSortGame.ViewModels
{
    class MainWindowVM : ViewModelBase
    {
        #region Properties

        private IWindowService windowService;
        private bool dontShowHelpScreenAtStart = Settings.Default.DontShowHelpScreenAtStart;
        public bool DontShowHelpScreenAtStart
        {
            get { return dontShowHelpScreenAtStart; }
            set
            {
                if (value != dontShowHelpScreenAtStart)
                {
                    dontShowHelpScreenAtStart = value;
                    Settings.Default.DontShowHelpScreenAtStart = dontShowHelpScreenAtStart;
                    Settings.Default.Save();
                    //OnPropertyChanged();
                }
            }
        }
        public MainWindow MainWindow { get; set; }

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
        public ICommand PopupWindow { get; set; }

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
                OnPropertyChanged("TubeCount");
            }
        }
        private int tubeCount;
        public int TubeCount
        {
            get {
                return tubeCount; 
                //return (int)Math.Ceiling((decimal)TubesManager.NumberOfColorsToGenerate / 2);
            }
            set
            {
                tubeCount = value;
                OnPropertyChanged();
            }
        }

        private bool levelComplete;
        public bool LevelComplete
        {
            get { return levelComplete; }
            set
            {
                if (value != levelComplete)
                {
                    levelComplete = value;
                }
            }
        }
        public bool PropertyChangedEventPaused { get; set; } = false;
        #endregion
        #region Constructor
        public MainWindowVM(MainWindow mainWindow)
        {
            this.windowService = new WindowService();
            MainWindow = mainWindow;
            
            Tubes = TubesManager.Tubes;
            CopyTubes();

            PropertyChanged += Tube_PropertyChanged;
            //PropertyChanged += TubeCount_PropertyChanged;
            //TubesManager.GlobalPropertyChanged += TubeCount_PropertyChanged;
            Tubes.CollectionChanged += Tubes_CollectionChanged;
            TubesPerLineCalculation();
            PopupWindow = new PopupWindowCommand(this);
            if (dontShowHelpScreenAtStart == false)
            {
                SelectedViewModel = new HelpVM(this);
            }
        }
        #endregion
        #region Navigation
        public RelayCommand CloseWindowCommand => new RelayCommand(execute => CloseWindow());
        private void CloseWindow()
        {
            if (SelectedViewModel == null)
            {
                windowService?.CloseWindow();
            }
            else
            {
                PopupWindow.Execute(null);
            }
        }
        public RelayCommand ConfirmCommand => new RelayCommand(execute => ConfirmPopup());
        private void ConfirmPopup()
        {
            //if (SelectedViewModel == null)
            //{
            //    return;
            //}
            if (SelectedViewModel is NewLevelVM)
            {
                StartNewLevel();
            }
            else if (SelectedViewModel is RestartLevelVM)
            {
                Restart();
            }
            else if (SelectedViewModel is LevelCompleteVM)
            {
                StartNewLevel();
            }
            else if (SelectedViewModel is HelpVM)
            {
                PopupWindow.Execute(null);
            }
        }

        public RelayCommand AddExtraTubeCommand => new RelayCommand(execute => TubesManager.AddExtraTube(), canExecute => TubesManager.ExtraTubes < TubesManager.MaximumExtraTubes);
        //public RelayCommand NewLevelCommand => new RelayCommand(execute => StartNewLevel());
        //private void StartNewLevel(bool force = false)
        //{
        //    if (force == false)
        //    {
        //        var result = MessageBox.Show("Do you want to start new level?", "New level", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        //        if (result != MessageBoxResult.OK)
        //        {
        //            return;
        //        }
        //    }
        //    TubesManager.StartNewLevel();
        //    OnStartingLevel();
        //}
        #region Popup Screens
        //public RelayCommand ShowScreen_StartNewLevel_Command => new RelayCommand(execute => StartNewLevelScreenVisibility = "true", canExecute => true);
        public RelayCommand StartNewLevel_Command => new RelayCommand(execute => StartNewLevel());
        private void StartNewLevel()
        {
            PopupWindow.Execute(null);
            TubesManager.StartNewLevel();
            OnStartingLevel();
        }
        public RelayCommand RestartLevel_Command => new RelayCommand(execute => Restart());
        private void Restart()
        {
            PopupWindow.Execute(null);
            TubesManager.RestartLevel();
            OnStartingLevel();
        }
        #endregion
        //private void LevelWonMessage()
        //{
        //    var result = MessageBox.Show("Level complete!\nYes - next level\nNo - restart current level", "Level complete!", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        StartNewLevel(true);
        //    }
        //    else if (result == MessageBoxResult.No)
        //    {
        //        Restart(true);
        //    }
        //    // zkusit udelat nejakou grafiku, ohnostroj
        //}
        public RelayCommand LoadLevelCommand => new RelayCommand(execute => LoadLevel());
        private void LoadLevel(bool force = false)
        {
            if (force == false)
            {
                var result = MessageBox.Show("Do you want to load level?", "Load level", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (result != MessageBoxResult.OK)
                {
                    return;
                }
            }
            TubesManager.LoadLevel();
            OnStartingLevel();
        }
        private void OnStartingLevel()
        {
            LevelComplete = false;
            DeselectTube();
            GameStates.Clear();
        }
        public RelayCommand SaveLevelCommand => new RelayCommand(execute => SaveLevel());

        private void SaveLevel()
        {
            //throw new NotImplementedException();
            MessageBox.Show("NotImplementedException");
        }

        public RelayCommand StepBackCommand => new RelayCommand(execute => StepBack(), canExecute => GameStates.Count > 0);

        public RelayCommand OpenOptionsWindowCommand => new RelayCommand(execute => windowService?.OpenOptionsWindow(this));
        //public RelayCommand LevelCompleteWindowCommand => new RelayCommand(execute => windowService?.OpenLevelCompleteWindow(this));

        public RelayCommand OpenHelpFromOptionsCommand => new RelayCommand(execute =>
        {
            windowService?.CloseWindow();
            SelectedViewModel = new HelpVM(this);
        });

        #endregion
        #region OptionsWindow

        private int optionsWindowHeight = Settings.Default.OptionsWindowHeight;
        public int OptionsWindowHeight
        {
            get { return optionsWindowHeight; }
            set
            {
                optionsWindowHeight = value;
                Settings.Default.OptionsWindowHeight = value;
                Settings.Default.Save();
            }
        }
        private int optionsWindowWidth = Settings.Default.OptionsWindowWidth;
        public int OptionsWindowWidth
        {
            get { return optionsWindowWidth; }
            set
            {
                optionsWindowWidth = value;
                Settings.Default.OptionsWindowWidth = value;
                Settings.Default.Save();
            }
        }

        private bool developerOptionsVisibleBool = Settings.Default.DeveloperOptionsVisibleBool;
        public bool DeveloperOptionsVisibleBool
        {
            get { return developerOptionsVisibleBool; }
            set
            {
                if (value != developerOptionsVisibleBool)
                {
                    developerOptionsVisibleBool = value;
                    Settings.Default.DeveloperOptionsVisibleBool = developerOptionsVisibleBool;
                    Settings.Default.Save();
                    OnPropertyChanged(nameof(DeveloperOptionsVisible));
                }
            }
        }
        public string DeveloperOptionsVisible {
            get
            {
                if (developerOptionsVisibleBool == true)
                {
                    return "Visible";
                }
                else
                {
                    return "Hidden";
                }
            }
            
        }
        #endregion
        #region Moving Liquids
        public RelayCommand SelectTubeCommand => new RelayCommand(tube => SelectTube(tube));
        private void SelectTube(object obj)
        {
            if (LevelComplete == true)
            {
                return;
            }
            
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
            if (PropertyChangedEventPaused == true)
            {
                return;
            }
            SaveGameState();

            if (CompareAllTubes() && LevelComplete == false)
            {
                LevelComplete = true;
                //windowService?.OpenLevelCompleteWindow(this);
                //LevelWonMessage();
                PopupWindow.Execute("LevelComplete");
            }
        }
        private void SaveGameState()
        {
            //if (GameStates.Count == 0) // tohle by nemelo nikdy nastat, rovnou to kopiruju v constructoru
            //{
            //    DeepCopyTubes();
            //    return;
            //}
            if (DidGameStateChange() == true)
            //if (SolvingSteps[SolvingSteps.Count - 1] != Tubes)
            {
                CopyTubes();
                return;
            }
        }


        private bool DidGameStateChange()
        {
            if(GameStates.Count == 0 && LastGameState.Count == 0)
            {
                return true;
            }
            //var lastStateTubes = GameStates[GameStates.Count - 1];

            if (LastGameState.Count != Tubes.Count) // pokud jen pridavam extra prazdnou zkumavku tak to neukladat!
            {
                return false;
            }

            for (int i = 0; i < LastGameState.Count; i++)
            {
                if (LastGameState[i].Layers.Count != Tubes[i].Layers.Count)
                {
                    return true;
                }
                
                //var storedTube = lastState[i];
                //var currentTube = Tubes[i];
                //// ## jak to budu porovnavat kdyz jsem mezi stepy pridal extra prazdnou Tube?
                //// ## prvne to udelam bez tohohle checku

                //// ono mozna bude stacit kdyz porovnam pocet layeru a nemusim ani nic dalsiho!


                //for (int j = 0; j < storedTube.Layers.Count; j++)
                //{
                //    // co kdyz se mi ale zmeni pocet layeru mezi kroky?? prvne porov
                //}
            }
            return false;
        }
        private void CopyTubes()
        {
            //if (LastGameState.Count != 0) // pridavam to tady, protoze nechci v game states mit i current game state.
            //{
            //    //GameStates.Add(LastGameState);
            //    ObservableCollection<Tube> clone = new ObservableCollection<Tube>();
            //    foreach (Tube tube in LastGameState)
            //    {
            //        clone.Add(tube.DeepCopy());
            //    }
            //    GameStates.Add(clone);
            //}

            if (LastGameState.Count != 0) // pridavam to tady, protoze nechci v game states mit i current game state.
            {
                GameStates.Add(LastGameState);
                LastGameState = new ObservableCollection<Tube>();
            }

            LastGameState?.Clear();
            //ObservableCollection<Tube> tubesClone = new ObservableCollection<Tube>();
            //foreach (Tube tube in Tubes)
            //{
            //    LastGameState.Add(tube.DeepCopy());
            //}
            LastGameState = DeepCopyTubesCollection(Tubes);
            foreach (Tube tube in LastGameState)
            {
                if (tube.Selected == true)
                {
                    tube.Selected = false;
                }
            }
            
            //GameStates.Add(CurrentGameStateClone);
        }
        private bool CompareAllTubes()
        {
            if (Tubes.Count == 0) return false; // this is here in case we are calling the function from CollectionChanged event and it is currently count=0
            foreach (var tube in Tubes)
            {
                if (tube.Layers.Count != 4 && tube.Layers.Count != 0) // pokud zkumavka neni plna, nebo uplne prazdna, nemuze to byt level dokoncenej
                {
                    return false;
                }
                for (int i = 0; i < tube.Layers.Count; i++)
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

        //ObservableCollection<Tube> status;
        private ObservableCollection<ObservableCollection<Tube>> solvingSteps = new ObservableCollection<ObservableCollection<Tube>>();
        public ObservableCollection<ObservableCollection<Tube>> GameStates
        {
            get { return solvingSteps; }
            set
            {
                if (value != solvingSteps)
                {
                    solvingSteps = value;
                    //OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<Tube> LastGameState { get; set; } = new ObservableCollection<Tube>();

        private void StepBack()
        {
            if (GameStates.Count == 0)
            {
                return;
            }

            //GameStates.Remove(GameStates[GameStates.Count - 1]); // vymazu posledni, protoze to odpovida current state-u.//zmenil jsem to ze pridavam current state az o iteraci pozdeji

            ObservableCollection<Tube> lastGameStatus = GameStates[GameStates.Count - 1];
            //Tubes.CollectionChanged -= Tubes_CollectionChanged;
            //PropertyChanged -= Tube_PropertyChanged;
            PropertyChangedEventPaused = true;
            Tubes?.Clear();
            foreach (Tube tubes in lastGameStatus)
            {
                Tubes.Add(tubes);
            }
            //Tubes = new ObservableCollection<Tube>(DeepCopyTubesCollection(lastGameStatus));
            PropertyChangedEventPaused = false;
            //if (GameStates.Count > 1)
            //{
            //LastGameState = new ObservableCollection<Tube>(lastGameStatus);

            LastGameState = DeepCopyTubesCollection(lastGameStatus);
            //}
            GameStates.Remove(lastGameStatus);

            //Tubes.CollectionChanged += Tubes_CollectionChanged;
            //PropertyChanged += Tube_PropertyChanged;
            
        }
        private ObservableCollection<Tube> DeepCopyTubesCollection(ObservableCollection<Tube> tubes)
        {
            ObservableCollection <Tube> newTubes = new ObservableCollection<Tube>();
            foreach (Tube tube in tubes)
            {
                newTubes.Add(tube.DeepCopy());
            }
            return newTubes;
        }
        #endregion
        #region Other Methods
        private void Tubes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TubesPerLineCalculation();
        }
        private void TubesPerLineCalculation()
        {
            //TubeCount = Tubes.Count;
            //TubeCount = Tubes.Where(tube => tube.Layers.Count > 0).Count();

            TubeCount = (int)Math.Ceiling((decimal)Tubes.Count / 2);
        }

        //private void TubeCount_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    //TubeCount = (int)Math.Ceiling((decimal)Tubes.Count / 2);
        //    TubeCount = Tubes.Count;
        //}
        #endregion
    }
}
