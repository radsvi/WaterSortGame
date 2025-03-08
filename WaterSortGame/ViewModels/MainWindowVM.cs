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
        public MainWindowVM(MainWindow mainWindow)
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
        public RelayCommand AddExtraTubeCommand => new RelayCommand(execute => GameState.AddExtraTube(), canExecute => GameState.ExtraTubesAdded < AppSettings.MaximumExtraTubes);
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
            //ClosePopupWindow();

            //ObservableCollection<StoredLevel> savedLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);

            //savedLevelList.Add(new StoredLevel(GameState.StartingPosition, NoteForSavedLevel));

            //Settings.Default.SavedLevels = JsonConvert.SerializeObject(savedLevelList);
            ////Settings.Default.SavedLevels = JsonConvert.SerializeObject(new ObservableCollection<StoredLevel>() { new StoredLevel(TubesManager.SavedStartingTubes) });
            //Settings.Default.Save();
            //NoteForSavedLevel = null;

            //tokenSource = new CancellationTokenSource();
            //var token = tokenSource.Token;
            //PopupWindowNotification(token);
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
        public RelayCommand StepBackCommand => new RelayCommand(execute => GameState.StepBack(), canExecute => GameState.SavedGameSteps.Count > 0);
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

            //var tubeButton = obj as TubeButton;

            ////var tube = tubeButton?.Contents[0] as Tube;
            //var tubeControl = tubeButton?.Contents[0] as TubeControl;
            ////var tube = tubeControl.LiquidColors as Tube;
            //LiquidColorNew[] tubeLiquids = tubeControl.LiquidColors;
            //var buttonElement = tubeButton?.Contents[1] as Button;


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
            if (successAtLeastOnce > 0 || AppSettings.UnselectTubeEvenOnIllegalMove == true)
            {

                //RedrawTubeVisual(tube);
                DrawTubes();
                //RippleSurfaceAnimation(tubeLiquids, tubeLiquids.Length - 1, successAtLeastOnce);
                DeselectTube();
                IsLevelCompleted();
            }
        }
        private void GetTopmostLiquid(TubeReference sourceTube) // selects topmost liquid in a sourceTube
        {
            //if (SourceTube == null)
            //{
            //    SourceTube = new SourceTube { TubeId = tubeNumber };
            //}
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
        
        //private Tuple<bool, int, int> AddLiquidToTargetTube(Tube targetTube)
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

            GameState[currentTubeReference.TubeId, firstEmptyLayer] = SourceTube.TopMostLiquid;
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
                //LowerTubeAnimation(buttonElement, SelectedTubeNumber);

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
        private void LowerTubeAnimation(Button buttonElement, int? tubeNumber)
        {
            if (buttonElement is null)
            {
                return;
            }
            var HeightAnimation = new ThicknessAnimation() { To = new Thickness(0, 15, 0, 0), Duration = TimeSpan.FromSeconds(0.1) };
            buttonElement.BeginAnimation(Button.MarginProperty, HeightAnimation);

        }
        private void RippleSurfaceAnimation(Tube tube, int layer, int numberOfLiquids)
        {
            Grid container = GetContainerForAnimation(tube, layer);
            //var originalChild = container.Child;
            //var grid = DrawSurfaceFromSin(container); // ## tohle mozna udelat permanentni. nemusim to generovat vzdy znova.
            (var brush, var borderElement) = DrawSurface(container);
            container.Children.Add(borderElement);
            SurfaceAnimation(brush, container, borderElement, numberOfLiquids);

            // wait and then return:
            //container.Child = originalChild;
            // hm, tohle mozna nebude nutny delat takhle. spis proste odstranit jakykoliv subChild -> container.Child.Child
        }
        private (ImageBrush, Border) DrawSurface(Grid container)
        {
            Border borderElement = new Border();
            borderElement.VerticalAlignment = VerticalAlignment.Top;
            borderElement.CornerRadius = new CornerRadius(0, 0, 25, 25);
            ImageBrush brush = new ImageBrush();
            BitmapImage bmpImg = new BitmapImage();
            bmpImg.BeginInit();
            //bmpImg.UriSource = new Uri("Images\\TubeSurfaceRippleTall2.png", UriKind.Relative);
            //bmpImg.UriSource = new Uri("Images\\TubeSurfaceRippleTallNonTransparent.png", UriKind.Relative);
            bmpImg.UriSource = new Uri("Images\\JustLine.png", UriKind.Relative);
            //bmpImg.UriSource = new Uri("Images\\NarrowLine.png", UriKind.Relative);
            
            bmpImg.EndInit();
            brush.ImageSource = bmpImg;
            brush.TileMode = TileMode.Tile;
            brush.ViewportUnits = BrushMappingMode.Absolute;
            //brush.Viewport = new Rect(0, 200, 129, 52);

            Rectangle tileSizeRectangle = new Rectangle();
            tileSizeRectangle.Width = 50;
            //tileSizeRectangle.Width = 20;
            tileSizeRectangle.Height = 52;
            // ## smazat:
            //tileSizeRectangle.Height = 108;

            tileSizeRectangle.Fill = brush;
            //tileSizeRectangle.Fill = Brushes.Pink;
            //tileSizeRectangle.Margin = new Thickness(15, 24, 15, 0);
            tileSizeRectangle.Margin = new Thickness(0, 0, 0, 0);


            
            


            //grid.Background = brush;
            //grid.Children.Add(brush);
            //grid.Children.Add(tileSizeRectangle);
            borderElement.Child = tileSizeRectangle;
            //container.Child = borderElement;
            //container.Children.Add(borderElement);

            return (brush, borderElement);
        }
        //private void DrawSurfaceFromSin(Tube tube)
        //{
        //    Brush color = Brushes.LightSteelBlue;
        //    var container = GetContainerForAnimation(tube);
        //    var originalChild = container.Child;
        //    DrawSurfaceFromSin(container, color);
        //}
        private Grid GetContainerForAnimation(Tube tube, int layer)
        {
            Button button = tube.ButtonElement as Button;
            var descendant = GetDescendantByTypeAndName(button, typeof(Grid), "Layer" + layer);
            Grid container = descendant as Grid;

            return container;
        }
        //private void DrawSurfaceFromSin(Border container, Brush color)
        //[Obsolete]private Grid DrawSurfaceFromSin(Border container)
        //{
        //    //Button buttonElement = ((container as Tube).ButtonElement as Button);

        //    //buttonElement.Template.Template.
        //    //PropertyInfo highlightedItemProperty = cb.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "HighlightedItem");
        //    //object highlightedItemValue = highlightedItemProperty.GetValue(cb, null);
        //    //PropertyInfo highlightedItemProperty = buttonElement.Template.Template.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "HighlightedItem");
        //    //object highlightedItemValue = highlightedItemProperty.GetValue(buttonElement.Template.Template, null);

        //    //Grid gridElement = ((container as Tube).GridElement as Grid);

            


        //    float lengthMultiplier = 20;
        //    float topMargin = 10;
        //    float leftMargin = 0;

        //    float x1 = 0;
        //    float y1 = 0;
        //    float x2 = x1;
        //    float y2 = y1;
        //    Polygon newShape;
        //    int iterations = (int)x1 + 100;

        //    Grid grid = new Grid();
        //    grid.MaxWidth = container.ActualWidth;
            
        //    grid.Margin = new Thickness(-200, 0, 0, 0);

        //    //for (float x1 = 0; x1 < 20; x1 += 0.1F)
        //    do
        //    {
        //        y2 = (float)Math.Sin(x1);

        //        newShape = new Polygon();
        //        float drawX1 = x2 * lengthMultiplier + leftMargin;
        //        float drawY1 = y1 * lengthMultiplier / 2 + topMargin;
        //        float drawX2 = x1 * lengthMultiplier + leftMargin;
        //        float drawY2 = y2 * lengthMultiplier / 2 + topMargin;
        //        newShape.Points = new PointCollection() {
        //            new Point(drawX1, 0),
        //            new Point(drawX2, 0),
        //            new Point(drawX2, drawY2),
        //            new Point(drawX1, drawY1),
        //        };



        //        //newShape.StrokeThickness = 2;
        //        //newShape.Fill = color;
        //        newShape.Fill = new SolidColorBrush(Colors.LightBlue);
        //        //newShape.Fill = Brushes.OrangeRed;
        //        //newShape.Fill = new SolidColorBrush(Color.FromRgb(255,255,255));



        //        //newShape.MaxWidth = "{Binding ActualWidth, ElementName=NameOfYourParentElement}";

        //        //TextBlock textBlock = new TextBlock { Text = "qwer" };
        //        //grid.Children.Add(textBlock);
        //        grid.Children.Add(newShape);
                

        //        //gridElement.Children.Add(newShape);
        //        //container.Child = newShape;


        //        x2 = x1;
        //        y1 = y2;

        //        x1 += 0.1F;
        //    }
        //    while (x1 < iterations);

        //    //grid.Background = Brushes.White;
        //    return grid;
        //}
        public static Visual GetDescendantByTypeAndName(Visual element, Type type, string name)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                Panel foundElementPanel = element as Panel;
                if (foundElementPanel.Name == name)
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
                foundElement = GetDescendantByTypeAndName(visual, type, name);
                if (foundElement != null)
                {
                    Panel foundElementPanel = foundElement as Panel;
                    if (foundElementPanel.Name == name)
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
        private void SurfaceAnimation(ImageBrush brush, Grid container, Border borderElement, int numberOfLiquids)
        {
            if (brush is null)
            {
                return;
            }

            //var HeightAnimation = new ThicknessAnimation() { From = new Thickness(0, 0, 0, 0), To = new Thickness(-200, 0, 0, 0), Duration = TimeSpan.FromSeconds(1) };
            //container.
            //container.BeginAnimation(Button.MarginProperty, HeightAnimation);

            //var HeightAnimation = new ThicknessAnimation() { To = new Thickness(0, 0, 0, 15), Duration = TimeSpan.FromSeconds(0.1) };
            //sourceTube.ButtonElement.BeginAnimation(Button.MarginProperty, HeightAnimation);
            if (numberOfLiquids <= 1 ) // ## smazat if a prepocitat to numericky
            {
                var viewportAnimation = new RectAnimation() { From = new Rect(0, 200 * numberOfLiquids, 129, 300), To = new Rect(400 * numberOfLiquids, 50, 129, 300), Duration = TimeSpan.FromSeconds(2 * numberOfLiquids) };
                viewportAnimation.Completed += new EventHandler((sender, e) => ViewportAnimation_Completed(sender, e, container, borderElement));
                brush.BeginAnimation(ImageBrush.ViewportProperty, viewportAnimation);
            }
            else
            {
                //var tileSizeRectangle = borderElement.Child;
                //tileSizeRectangle.Height = 108;


                //var viewportAnimation = new RectAnimation() { From = new Rect(0, 170 * numberOfLiquids, 129, 500), To = new Rect(400 * numberOfLiquids, 230, 129, 500), Duration = TimeSpan.FromSeconds(2 * numberOfLiquids) };
                //container.Height = 108;
                var viewportAnimation = new RectAnimation() { From = new Rect(0, 170 * numberOfLiquids, 129, 500), To = new Rect(400, 230, 129, 500), Duration = TimeSpan.FromSeconds(2 * numberOfLiquids) };
                viewportAnimation.Completed += new EventHandler((sender, e) => ViewportAnimation_Completed(sender, e, container, borderElement));
                brush.BeginAnimation(ImageBrush.ViewportProperty, viewportAnimation);
            }
            
        }
        private void ViewportAnimation_Completed(object? sender, EventArgs e, Grid container, Border borderElement)
        {
            //container.Child = null;
            container.Children.Remove(borderElement);
            container.Height = 52;
        }
        #endregion
        #region Generating Tube display from code
        //private void RedrawTubeVisual(Tube targetTube) // ## smazat?
        //{
        //    Grid container = GetContainer(targetTube) as Grid;
        //    //RemoveAllLiquid(container);
        //    AddLiquid(container, targetTube);
        //    //RemoveColorFromSourceTube(targetTube);

        //}
        private Visual GetContainer(Tube targetTube)
        {
            Button button = targetTube.ButtonElement as Button;
            var descendant = GetDescendantByTypeAndName(button, typeof(Grid), "TubeGrid");
            Grid container = descendant as Grid;

            return container;
        }
        private void RemoveAllLiquid(Grid container)
        {
            container.Children?.Clear();
        }
        //private void AddLiquid(Grid container, Tube targetTube) // ## smazat?
        //{
        //    //int i = 0;

        //    int layer = targetTube.Layers.Count - 1;
        //    Grid grid = new Grid();

        //    grid.Name = "Layer" + layer;
        //    grid.VerticalAlignment = VerticalAlignment.Top;
        //    grid.Height = 52;
        //    grid.Margin = new Thickness(5, 0, 5, 0);

        //    foreach (var liquid in targetTube.Layers)
        //    {
        //        var border = new Border();
        //        //border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(SourceLiquid.Rgb);
        //        border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(liquid.Rgb);
        //        grid.Children.Add(border);
        //    }

        //    container.Children.Add(grid);

        //    Grid.SetRow(grid, 3 - layer);
        //}
        [Obsolete]public RelayCommand TestDraw_Command => new RelayCommand(execute => DrawTubes());
        public void DrawTubes()
        {
            MainWindow.GridForTubes.Children.Clear(); // deletes classes of type Visual

            for (int x = 0; x < GameState.NumberOfTubes; x++)
            {
                LiquidColorNew[] liquidColorsArray = new LiquidColorNew[GameState.NumberOfLayers];
                for (int y = 0; y < GameState.NumberOfLayers; y++)
                {
                    liquidColorsArray[y] = GameState[x, y];
                }
                var tubeControl = new TubeControl(this, x, liquidColorsArray);

                // mozna to tu udelat pres ten <ContentControl> nejak

                // ## predelat na MVVM
                MainWindow.GridForTubes.Children.Add(tubeControl);
            }
        }
        //public Border TubeDisplay { get; set; }
        //public Grid TubeGrid { get; set; }
        //private void RegenerateTubeDisplay(object? sender, PropertyChangedEventArgs e)
        //{
        //    // mozna tu ne zacatku udelat nejakou podminku. treba PropertyChangedEventPaused
        //    if (PropertyChangedEventPaused is true)
        //    {
        //        return;
        //    }

        //    GetDescendantByType

        //}
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