using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
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

        public ObservableCollection<PopupScreenActions> PopupActions { get; set; }

        private LoadLevelVM loadLevelVM;
        public LoadLevelVM LoadLevelVM
        {
            get { return loadLevelVM; }
            set
            {
                if (value != loadLevelVM)
                {
                    loadLevelVM = value;
                    OnPropertyChanged();
                }
            }
        }

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
            PopupWindow = new PopupScreenCommand(this);
            if (dontShowHelpScreenAtStart == false)
            {
                SelectedViewModel = new HelpVM(this);
            }

            var loadLevelVM = new LoadLevelVM(this);
            //loadLevelVM.LoadLevelList.CollectionChanged += loadLevelVM.LoadLevelList_CollectionChanged;
            PopupActions = new ObservableCollection<PopupScreenActions>
            {
                new PopupScreenActions(PopupParams.NewLevel, new NewLevelVM(this), null, () => GenerateNewLevel()),
                new PopupScreenActions(PopupParams.RestartLevel, new RestartLevelVM(this), null, () => RestartLevel()),
                new PopupScreenActions(PopupParams.LevelComplete, new LevelCompleteVM(this), null, () => GenerateNewLevel()),
                new PopupScreenActions(PopupParams.Help, new HelpVM(this), null, () => ClosePopupWindow()),
                new PopupScreenActions(PopupParams.LoadLevel, loadLevelVM, () => loadLevelVM.LoadLevelScreen(), () => loadLevelVM.LoadLevel()),
                new PopupScreenActions(PopupParams.GameSaved, new GameSavedNotificationVM(this), null, () => CloseNotification()),
                new PopupScreenActions(PopupParams.SaveLevel, new SaveLevelVM(this), null, () => SaveLevel()),
            };
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
                ClosePopupWindow();
            }
        }
        public RelayCommand ConfirmCommand => new RelayCommand(execute => ConfirmPopup());
        private void ConfirmPopup()
        {
            if (SelectedViewModel == null)
            {
                return;
            }
            //var action = Array.Find(PopupActions, x => x.SelectedViewModel.GetType() == SelectedViewModel.GetType());
            var action = PopupActions.Where(x => x.SelectedViewModel.GetType() == SelectedViewModel.GetType());
            action.ElementAt(0)?.ConfirmationAction.Invoke();
        }
        internal void ClosePopupWindow()
        {
            PopupWindow.Execute(null);
        }
        public RelayCommand AddExtraTubeCommand => new RelayCommand(execute => TubesManager.AddExtraTube(), canExecute => TubesManager.ExtraTubes < TubesManager.MaximumExtraTubes);
        private void GenerateNewLevel()
        {
            ClosePopupWindow();
            TubesManager.GenerateNewLevel();
            OnStartingLevel();
        }
        public RelayCommand RestartLevel_Command => new RelayCommand(execute => RestartLevel());
        internal void RestartLevel()
        {
            ClosePopupWindow();
            TubesManager.RestartLevel();
            OnStartingLevel();
        }
        private void OnStartingLevel()
        {
            LevelComplete = false;
            DeselectTube();
            GameStates.Clear();
            LastGameState = new ObservableCollection<Tube>();
        }
        public string NoteForSavedLevel { get; set; }
        private void SaveLevel()
        {
            ClosePopupWindow();

            ObservableCollection<StoredLevel> savedLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);

            savedLevelList.Add(new StoredLevel(TubesManager.SavedStartingTubes, NoteForSavedLevel));

            Settings.Default.SavedLevels = JsonConvert.SerializeObject(savedLevelList);
            //Settings.Default.SavedLevels = JsonConvert.SerializeObject(new ObservableCollection<StoredLevel>() { new StoredLevel(TubesManager.SavedStartingTubes) });
            Settings.Default.Save();
            NoteForSavedLevel = null;

            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            PopupWindowNotification(token);
        }
        public RelayCommand AddPresetLevels_Command => new RelayCommand(execute => AddPresetLevels());
        private void AddPresetLevels()
        {
            windowService?.CloseWindow(); // close options menu

            ObservableCollection<StoredLevel> savedLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);

            savedLevelList.Insert(0, new StoredLevel(new ObservableCollection<Tube> {
                { new Tube(8, 1, 3, 0) },
                { new Tube(2, 7, 10, 4) },
                { new Tube(8, 10, 10, 11) },
                { new Tube(2, 2, 1, 4) },
                { new Tube(0, 6, 5, 9) },
                { new Tube(2, 3, 6, 3) },
                { new Tube(3, 7, 4, 9) },
                { new Tube(5, 0, 1, 8) },
                { new Tube(10, 9, 6, 5) },
                { new Tube(4, 6, 9, 3) },
                { new Tube(7, 11, 5, 11) },
                { new Tube(0, 11, 7, 8) },
                { new Tube() },
                { new Tube() },
            }, "Never solved this level without adding extra tubes."));

            savedLevelList.Insert(0, new StoredLevel(new ObservableCollection<Tube> {
                { new Tube(0, 0, 0, 0) },
                { new Tube(1, 1, 1, 1) },
                { new Tube(2, 2, 2, 2) },
                { new Tube(3, 3, 3, 3) },
                { new Tube(4, 4, 4, 4) },
                { new Tube(5, 5, 5, 5) },
                { new Tube(6, 6, 6, 6) },
                { new Tube(7, 7, 7, 7) },
                { new Tube(8, 8, 8, 8) },
                { new Tube(9, 9, 9, 9) },
                { new Tube(10, 10, 10, 10) },
                { new Tube(11) },
                { new Tube(11, 11, 11) },
                { new Tube() },
            }, "One step before finish."));

            Settings.Default.SavedLevels = JsonConvert.SerializeObject(savedLevelList);
            Settings.Default.Save();
        }
        CancellationTokenSource tokenSource = null;
        private async void PopupWindowNotification(CancellationToken token)
        {
            PopupWindow.Execute(PopupParams.GameSaved);
            //Thread.Sleep(500);
            //await Task.Delay(2000);
            //Task.Delay(2000);
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(100);
                if(token.IsCancellationRequested)
                {
                    break;
                }
            }
            
            ClosePopupWindow();
        }
        private void CloseNotification()
        {
            tokenSource?.Cancel();
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
        private bool unselectTubeEvenOnIllegalMove = Settings.Default.UnselectTubeEvenOnIllegalMove;
        public bool UnselectTubeEvenOnIllegalMove
        {
            get { return unselectTubeEvenOnIllegalMove; }
            set
            {
                if (value != unselectTubeEvenOnIllegalMove)
                {
                    unselectTubeEvenOnIllegalMove = value;
                    Settings.Default.UnselectTubeEvenOnIllegalMove = unselectTubeEvenOnIllegalMove;
                    Settings.Default.Save();
                    //OnPropertyChanged();
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
            var button = (Button)obj;
            



            if (SelectedTube == null)
            {
                SelectLiquid(tube);
                
                //var HeightAnimation = new DoubleAnimation() { To = 150, Duration = TimeSpan.FromSeconds(0.3) };
                //button.BeginAnimation(Button.HeightProperty, HeightAnimation);

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

            do
            {
                success = AddLiquidToTargetTube(tube);
                if (success == true)
                {
                    successAtLeastOnce = true;
                    SelectLiquid(SelectedTube); // vyber dalsi liquid ze stejne zkumavky
                }
            } while (success == true && SourceLiquid is not null);
            if (successAtLeastOnce == true || UnselectTubeEvenOnIllegalMove == true)
            {
                DeselectTube();
            }
        }
        //private void SelectTube(object obj)
        //{
        //    if (LevelComplete == true)
        //    {
        //        return;
        //    }

        //    var tube = obj as Tube;

        //    if (SelectedTube == null)
        //    {
        //        SelectLiquid(tube);
        //        return;
        //    }
        //    if (SelectedTube == tube)
        //    {
        //        DeselectTube();
        //        return;
        //    }

        //    // if selecting different tube
        //    bool success = false;
        //    bool successAtLeastOnce = false;

        //    do {
        //        success = AddLiquidToTargetTube(tube);
        //        if (success == true)
        //        {
        //            successAtLeastOnce = true;
        //            SelectLiquid(SelectedTube); // vyber dalsi liquid ze stejne zkumavky
        //        }
        //    } while (success == true && SourceLiquid is not null);
        //    if (successAtLeastOnce == true || UnselectTubeEvenOnIllegalMove == true)
        //    {
        //        DeselectTube();
        //    }
        //}
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
                PopupWindow.Execute(PopupParams.LevelComplete);
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
        private ObservableCollection<ObservableCollection<Tube>> gameStates = new ObservableCollection<ObservableCollection<Tube>>();
        public ObservableCollection<ObservableCollection<Tube>> GameStates
        {
            get { return gameStates; }
            set
            {
                if (value != gameStates)
                {
                    gameStates = value;
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
        internal ObservableCollection<Tube> DeepCopyTubesCollection(ObservableCollection<Tube> tubes)
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