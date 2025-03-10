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
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;
using WaterSortGame.Models;
using WaterSortGame.MVVM;
using WaterSortGame.Properties;
using WaterSortGame.Views;
using WaterSortGame.Views.UserControls;

namespace WaterSortGame.ViewModels
{
    class MainWindowVM : ViewModelBase
    {
        #region Properties
        private IWindowService windowService;
        public MainWindow MainWindow { get; }
        public AppSettings AppSettings { get; }
        public GameState GameState { get; set; }
        private WrapPanel ContainerForTubes;

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

        //private LiquidColorNew[] selectedTube;
        //public LiquidColorNew[] SelectedTube
        //{
        //    get { return selectedTube; }
        //    set
        //    {
        //        if (selectedTube is null)
        //        {
        //            selectedTube = value;
        //            //selectedTube.Margin = "0,0,0,30";
        //            //selectedTube.Selected = true;
        //            //OnPropertyChanged();
        //            return;
        //        }
        //        if (selectedTube == value) // pokud clicknu na stejnou zkumavku znova
        //        {
        //            //Debug.WriteLine("test");
        //            //selectedTube.Margin = "0,30,0,0";
        //            //selectedTube.Selected = false;
        //            selectedTube = null;
        //            //OnPropertyChanged();
        //            return;
        //        }
        //        if (selectedTube is not null && selectedTube != value)
        //        {
        //            //selectedTube.Selected = false;
        //            //selectedTube.Margin = "0,30,0,0";
        //            selectedTube = value;
        //            selectedTube = null;
        //            //OnPropertyChanged();
        //            return;
        //        }

        //    }
        //}
        public TubeReference LastClickedTube { get; set; }
        public TubeReference SourceTube { get; set; }
        [Obsolete]private Tube targetTube;
        [Obsolete]public Tube TargetTube
        {
            get { return targetTube; }
            set
            {
                targetTube = value;
                //OnPropertyChanged();
            }
        }

        [Obsolete] private ObservableCollection<Tube> tubes;
        [Obsolete]
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
        public MainWindowVM(MainWindow mainWindow, WrapPanel containerForTubes)
        {
            this.windowService = new WindowService();
            MainWindow = mainWindow;
            AppSettings = new AppSettings();

            GameState = new GameState(this);
            //Tubes = TubesManager.Tubes;

            //PropertyChanged += RegenerateTubeDisplay;
            //PropertyChanged += TubeCount_PropertyChanged;
            //TubesManager.GlobalPropertyChanged += TubeCount_PropertyChanged;
            //Tubes.CollectionChanged += Tubes_CollectionChanged;
            TubesPerLineCalculation();
            PopupWindow = new PopupScreenCommand(this);
            if (AppSettings.DontShowHelpScreenAtStart == false)
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

            ContainerForTubes = containerForTubes;

            DrawTubes();
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
        public RelayCommand CancelScreenCommand => new RelayCommand(execute => ClosePopupWindow());
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
        public RelayCommand AddExtraTubeCommand => new RelayCommand(execute => AddExtraTube(), canExecute => GameState.ExtraTubesAdded < AppSettings.MaximumExtraTubes);
        private void AddExtraTube()
        {
            GameState.AddExtraTube();
            DrawTubes();
        }
        private void GenerateNewLevel()
        {
            ClosePopupWindow();
            GameState.GenerateNewLevel();
            OnStartingLevel();
        }
        public RelayCommand RestartLevel_Command => new RelayCommand(execute => RestartLevel());
        public void RestartLevel()
        {
            ClosePopupWindow();
            GameState.RestartLevel();
            OnStartingLevel();
        }
        private void OnStartingLevel()
        {
            LevelComplete = false;
            DeselectTube();
            GameState.SavedGameSteps.Clear();
            GameState.LastGameStep = null;
            DrawTubes();
        }
        public string NoteForSavedLevel { get; set; }
        private void SaveLevel()
        {
            ClosePopupWindow();

            ObservableCollection<StoredLevel> savedLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);

            savedLevelList.Add(new StoredLevel(GameState.StartingPosition, NoteForSavedLevel));

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
            //windowService?.CloseWindow(); // close options menu

            //ObservableCollection<StoredLevel> savedLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);

            //savedLevelList.Insert(0, new StoredLevel(new ObservableCollection<Tube> {
            //    { new Tube(8, 1, 3, 0) },
            //    { new Tube(2, 7, 10, 4) },
            //    { new Tube(8, 10, 10, 11) },
            //    { new Tube(2, 2, 1, 4) },
            //    { new Tube(0, 6, 5, 9) },
            //    { new Tube(2, 3, 6, 3) },
            //    { new Tube(3, 7, 4, 9) },
            //    { new Tube(5, 0, 1, 8) },
            //    { new Tube(10, 9, 6, 5) },
            //    { new Tube(4, 6, 9, 3) },
            //    { new Tube(7, 11, 5, 11) },
            //    { new Tube(0, 11, 7, 8) },
            //    { new Tube() },
            //    { new Tube() },
            //}, "Never solved this level without adding extra tubes."));

            //savedLevelList.Insert(0, new StoredLevel(new ObservableCollection<Tube> {
            //    { new Tube(0, 0, 0, 0) },
            //    { new Tube(1, 1, 1, 1) },
            //    { new Tube(2, 2, 2, 2) },
            //    { new Tube(3, 3, 3, 3) },
            //    { new Tube(4, 4, 4, 4) },
            //    { new Tube(5, 5, 5, 5) },
            //    { new Tube(6, 6, 6, 6) },
            //    { new Tube(7, 7, 7, 7) },
            //    { new Tube(8, 8, 8, 8) },
            //    { new Tube(9, 9, 9, 9) },
            //    { new Tube(10, 10, 10, 10) },
            //    { new Tube(11) },
            //    { new Tube(11, 11, 11) },
            //    { new Tube() },
            //}, "One step before finish."));

            //Settings.Default.SavedLevels = JsonConvert.SerializeObject(savedLevelList);
            //Settings.Default.Save();
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
        public RelayCommand StepBackCommand => new RelayCommand(execute => GameState.StepBack(), canExecute => GameState.SavedGameSteps.Count > 0);
        public RelayCommand OpenOptionsWindowCommand => new RelayCommand(execute => windowService?.OpenOptionsWindow(this));
        //public RelayCommand LevelCompleteWindowCommand => new RelayCommand(execute => windowService?.OpenLevelCompleteWindow(this));
        public RelayCommand OpenHelpFromOptionsCommand => new RelayCommand(execute =>
        {
            windowService?.CloseWindow();
            SelectedViewModel = new HelpVM(this);
        });
        #endregion
        #region Moving Liquids
        public RelayCommand SelectTubeCommand => new RelayCommand(obj => OnTubeButtonClick(obj));
        internal void OnTubeButtonClick(object obj)
        {
            if (LevelComplete == true)
            {
                return;
            }

            TubeReference currentTubeReference = obj as TubeReference;

            if (LastClickedTube == null)
            {
                SourceTube = currentTubeReference;
                GetTopmostLiquid(SourceTube);
                return;
            }
            if (LastClickedTube == currentTubeReference)
            {
                DeselectTube();
                return;
            }

            // if selecting different tube:
            bool success = false;
            int successAtLeastOnce = 0;

            do
            {
                success = AddLiquidToTargetTube(currentTubeReference);
                if (success == true)
                {
                    successAtLeastOnce++;
                    RemoveColorFromSourceTube();
                    GetTopmostLiquid(SourceTube); // picks another liquid from the same tube
                }
            } while (success == true && SourceTube.TopMostLiquid is not null);
            if (successAtLeastOnce > 0)
            {
                DrawTubes();
                RippleSurfaceAnimation(currentTubeReference, successAtLeastOnce);
                DeselectTube();
                
                IsLevelCompleted();
            }
            if (successAtLeastOnce == 0 && AppSettings.UnselectTubeEvenOnIllegalMove == true)
            {
                DeselectTube();
            }
        }
        private void GetTopmostLiquid(TubeReference sourceTube) // selects topmost liquid in a sourceTube
        {
            for (int i = GameState.NumberOfLayers - 1; i >= 0; i--)
            {
                if (GameState[sourceTube.TubeId, i] is not null)
                {
                    if (LastClickedTube != sourceTube)
                        LastClickedTube = sourceTube;
                    sourceTube.TopMostLiquid = GameState[sourceTube.TubeId, i];
                    RaiseTubeAnimation(sourceTube);
                    return;
                }
            }
        }
        
        private bool AddLiquidToTargetTube(TubeReference currentTubeReference)
        {
            int firstEmptyLayer = - 1;
            for (int y = 0; y < GameState.NumberOfLayers; y++)
            {
                if (GameState[currentTubeReference.TubeId, y] == null)
                {
                    firstEmptyLayer = y;
                    break;
                }
            }
            if (firstEmptyLayer == -1)
            {
                return false;
            }

            if (firstEmptyLayer > 0)
            {
                if (SourceTube.TopMostLiquid.Name != GameState[currentTubeReference.TubeId, firstEmptyLayer - 1].Name)
                {
                    return false; // Pokud ma zkumavka v sobe uz nejaky barvy a nejvrchnejsi barva neshoulasi se SourceLiquid tak vratit false
                }
            }

            currentTubeReference.LastColorMoved = SourceTube.TopMostLiquid.Clone(); // saving this to use in CreateImageBackground(). Musim dat Clone protoze jinak se to deselectne
            GameState[currentTubeReference.TubeId, firstEmptyLayer] = SourceTube.TopMostLiquid;
            currentTubeReference.TargetEmptyRow = firstEmptyLayer;
            return true;
        }
        private void RemoveColorFromSourceTube()
        {
            for (int y = GameState.NumberOfLayers - 1; y >= 0; y--)
            {
                if (GameState[SourceTube.TubeId, y] is not null)
                {
                    GameState[SourceTube.TubeId, y] = null;
                    SourceTube.TopMostLiquid = null;
                    return;
                }
            }
        }
        private void DeselectTube()
        {
            if (LastClickedTube is not null)
            {
                //LowerTubeAnimation(GetTubeReference(SourceTube.TubeId));
                LowerTubeAnimation(SourceTube);
                LastClickedTube = null;
            }
        }
        private void IsLevelCompleted()
        {
            if (GameState.IsLevelCompleted() && LevelComplete == false)
            {
                LevelComplete = true;
                PopupWindow.Execute(PopupParams.LevelComplete);
            }
        }

        #endregion
        #region Draw tubes from code
        [Obsolete] public RelayCommand TestDraw_Command => new RelayCommand(execute => DrawTubes());
        public void DrawTubes()
        {
            ContainerForTubes.Children.Clear(); // deletes classes of type Visual

            //for (int x = 0; x < GameState.NumberOfTubes; x++)
            for (int x = 0; x < GameState.GetLength(0); x++)
            {
                LiquidColorNew[] liquidColorsArray = new LiquidColorNew[GameState.NumberOfLayers];
                for (int y = 0; y < GameState.NumberOfLayers; y++)
                {
                    liquidColorsArray[y] = GameState[x, y];
                }
                var tubeControl = new TubeControl(this, x, liquidColorsArray);

                // mozna to tu udelat pres ten <ContentControl> nejak

                ContainerForTubes.Children.Add(tubeControl);
            }
        }
        /// <summary>
        /// Draws border that is filled with an image that will later be animated.
        /// Surface as in water surface
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static Visual GetDescendantByTypeAndName(Visual element, Type type, string layerName)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                Panel foundElementPanel = element as Panel;
                if (foundElementPanel.Name == layerName)
                {
                    return element;
                }
            }
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByTypeAndName(visual, type, layerName);
                if (foundElement != null)
                {
                    Panel foundElementPanel = foundElement as Panel;
                    if (foundElementPanel.Name == layerName)
                    {
                        break;
                    }
                }
            }
            return foundElement;
        }
        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                return element;
            }
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }
        #endregion
        #region Animation
        private void RaiseTubeAnimation(TubeReference tubeReference)
        {
            if (tubeReference.ButtonElement is null)
            {
                return;
            }

            var HeightAnimation = new ThicknessAnimation() { To = new Thickness(0, 0, 0, 15), Duration = TimeSpan.FromSeconds(0.1) };
            tubeReference.ButtonElement.BeginAnimation(Button.MarginProperty, HeightAnimation);
        }
        private void LowerTubeAnimation(TubeReference tubeReference)
        {
            if (tubeReference.ButtonElement is null)
            {
                return;
            }

            var HeightAnimation = new ThicknessAnimation() { From = new Thickness(0, 0, 0, 15), To = new Thickness(0, 15, 0, 0), Duration = TimeSpan.FromSeconds(0.1) };
            tubeReference.ButtonElement.BeginAnimation(Button.MarginProperty, HeightAnimation);
        }
        private int GetFirstEmptyLayer(TubeReference lastClickedTube)
        {
            for (int y = 0; y < GameState.NumberOfLayers; y++)
            {
                if (GameState[lastClickedTube.TubeId, y] is null)
                {
                    return y;
                }
            }
            throw new Exception("This tube should always have empty space.");
        }
        private (ImageBrush, Grid) CreateVerticalTubeAnimationBackground(TubeReference currentTubeReference, int numberOfLiquids)
        {
            Grid gridElement = new Grid();

            Border borderRoundedCorner = new Border();
            gridElement.Children.Add(borderRoundedCorner);
            borderRoundedCorner.CornerRadius = new CornerRadius(0, 0, 16, 16);
            borderRoundedCorner.Background = currentTubeReference.LastColorMoved.Brush; // sem poslat barvu kterou presouvam
            borderRoundedCorner.Margin = new Thickness(5);

            Binding binding = new Binding();
            binding.Source = borderRoundedCorner;


            //BindingOperations.SetBinding(column, GridViewColumn.WidthProperty, binding);
            //BindingOperations.SetBinding("referal elementu v kterym definuju binding", "nazev/typ property kterou chci bindovat", bindingPromenna);
            VisualBrush visualBrush = new VisualBrush();
            BindingOperations.SetBinding(visualBrush, VisualBrush.VisualProperty, binding);

            gridElement.OpacityMask = visualBrush;

            ImageBrush brush = new ImageBrush();
            BitmapImage bmpImg = new BitmapImage();

            bmpImg.BeginInit();
            bmpImg.UriSource = new Uri("Images\\TubeSurfaceRippleTallest.png", UriKind.Relative);
            //bmpImg.UriSource = new Uri("Images\\TubeSurfaceRippleTallNonTransparent.png", UriKind.Relative);
            //bmpImg.UriSource = new Uri("Images\\JustLine.png", UriKind.Relative);
            //bmpImg.UriSource = new Uri("Images\\NarrowLine.png", UriKind.Relative);
            bmpImg.EndInit();

            brush.ImageSource = bmpImg;
            brush.TileMode = TileMode.Tile;
            brush.ViewportUnits = BrushMappingMode.Absolute;
            //brush.Viewport = new Rect(0, 200, 129, 52);

            Rectangle tileSizeRectangle = new Rectangle();
            tileSizeRectangle.VerticalAlignment = VerticalAlignment.Top;
            tileSizeRectangle.Margin = new Thickness(0, -1, 0, 0);
            tileSizeRectangle.Width = 50;

            tileSizeRectangle.Height = 52 * numberOfLiquids;

            tileSizeRectangle.Fill = brush;

            gridElement.Children.Add(tileSizeRectangle);

            return (brush, gridElement);
        }
        private void RippleSurfaceAnimation(TubeReference currentTubeReference, int numberOfLiquids)
        {
            TubeControl tubeControl = ContainerForTubes.Children[currentTubeReference.TubeId] as TubeControl;
            
            // Getting reference to the main grid that contains individual liquids in a tube.
            Grid container = (GetDescendantByTypeAndName(tubeControl, typeof(Grid), "TubeGrid")) as Grid;

            (var brush, var gridElement) = CreateVerticalTubeAnimationBackground(currentTubeReference, numberOfLiquids);
            container.Children.Add(gridElement);

            Grid.SetRow(gridElement, 3 - currentTubeReference.TargetEmptyRow); // ## v tenhle moment uz je Liquid presunutej. Mel bych to sem posilat z nejake drivejsi kalkulace a ne to detekovat znova.

            Grid.SetRowSpan(gridElement, 4);
            //Canvas.SetZIndex(borderElement, 3);
            //Grid.SetZIndex(borderElement, 4);
            
            StartAnimatingSurface(brush, container, gridElement, numberOfLiquids);
        }
        private void StartAnimatingSurface(ImageBrush brush, Grid container, Grid gridElement, int numberOfLiquids)
        {
            if (brush is null)
            {
                return;
            }

            int yPosFrom = 390;
            int xSize = 129;
            int ySize = 800;

            var viewportAnimation = new RectAnimation()
            {
                From = new Rect(0, yPosFrom + 52 * numberOfLiquids, xSize, ySize),
                To = new Rect(180 * numberOfLiquids, yPosFrom, xSize, ySize),
                Duration = TimeSpan.FromSeconds(0.8 * numberOfLiquids)
            };
            viewportAnimation.Completed += new EventHandler((sender, e) => ViewportAnimation_Completed(sender, e, container, gridElement));
            brush.BeginAnimation(ImageBrush.ViewportProperty, viewportAnimation);
        }
        private void ViewportAnimation_Completed(object? sender, EventArgs e, Grid container, Grid gridElement)
        {
            container.Children.Remove(gridElement);
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

            //TubeCount = (int)Math.Ceiling((decimal)Tubes.Count / 2);
            TubeCount = 6; // stejne to budu menit, tak jsem docasne dal jen fixnuty cislo
        }
        //private void TubeCount_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    //TubeCount = (int)Math.Ceiling((decimal)Tubes.Count / 2);
        //    TubeCount = Tubes.Count;
        //}
        #endregion
    }
}