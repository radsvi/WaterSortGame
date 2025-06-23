using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WaterSortGame.Models;
using WaterSortGame.MVVM;
using WaterSortGame.Properties;
using WaterSortGame.Views.UserControls;

namespace WaterSortGame.ViewModels
{
    class MainWindowVM : ViewModelBase
    {
        #region Properties
        public IWindowService WindowService { get; }
        public MainWindow MainWindow { get; }
        public AppSettings AppSettings { get; }
        public Notification Notification { get; }
        //public AutoSolve AutoSolve { get; set; }
        private AutoSolve autoSolve;
        public AutoSolve AutoSolve
        {
            get { return autoSolve; }
            set
            {
                if (value != autoSolve)
                {
                    autoSolve = value;
                    OnPropertyChanged();
                }
            }
        }
        public GameState GameState { get; }
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
        private UniformGrid ContainerForTubes;

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
        
        private int tubeCount;
        public int TubeCount
        {
            get
            {
                return tubeCount;
                //return (int)Math.Ceiling((decimal)TubesManager.NumberOfColorsToGenerate / 2);
            }
            set
            {
                tubeCount = value;
                OnPropertyChanged();
            }
        }

        private bool uiEnabled;
        public bool UIEnabled // also used to mean that level is completed
        {
            get { return uiEnabled; }
            set
            {
                if (value != uiEnabled)
                {
                    uiEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool PropertyChangedEventPaused { get; set; } = false;
        public ObservableCollection<PopupScreenActions> PopupActions { get; set; }
        internal readonly string logFolderName = "log";
        #endregion
        #region Constructor
        public MainWindowVM(MainWindow mainWindow, UniformGrid containerForTubes)
        {
            this.WindowService = new WindowService();
            MainWindow = mainWindow;
            AppSettings = new AppSettings();
            Notification = new Notification(this);

            GameState = new GameState(this);
            //Tubes = TubesManager.Tubes;

            //PropertyChanged += RegenerateTubeDisplay;
            //PropertyChanged += TubeCount_PropertyChanged;
            //TubesManager.GlobalPropertyChanged += TubeCount_PropertyChanged;
            //Tubes.CollectionChanged += Tubes_CollectionChanged;

            PopupWindow = new PopupScreenCommand(this);
            if (AppSettings.DontShowHelpScreenAtStart == false)
            {
                SelectedViewModel = new HelpVM(this);
            }

            LoadLevelVM = new LoadLevelVM(this);
            PopupActions = new ObservableCollection<PopupScreenActions>
            {
                new PopupScreenActions(PopupParams.NewLevel, new NewLevelVM(this), null, () => GenerateNewLevel()),
                new PopupScreenActions(PopupParams.RestartLevel, new RestartLevelVM(this), null, () => RestartLevel()),
                new PopupScreenActions(PopupParams.LevelComplete, new LevelCompleteVM(this), null, () => GenerateNewLevel()),
                new PopupScreenActions(PopupParams.Help, new HelpVM(this), null, () => ClosePopupWindow()),
                new PopupScreenActions(PopupParams.LoadLevel, LoadLevelVM, () => LoadLevelVM.LoadLevelScreen(), () => LoadLevelVM.LoadLevel()),
                //new PopupScreenActions(PopupParams.LoadLevel, loadLevelVM, () => loadLevelVM.LoadLevelScreen(), () => loadLevelVM.LoadLevel()),
                new PopupScreenActions(PopupParams.GameSaved, new GameSavedNotificationVM(this), null, () => CloseNotification()),
                new PopupScreenActions(PopupParams.SaveLevel, new SaveLevelVM(this), null, () => SaveLevel()),
            };

            ContainerForTubes = containerForTubes;


            if (System.IO.Directory.Exists(logFolderName) == false) System.IO.Directory.CreateDirectory(logFolderName);

            AutoSolve = new AutoSolve(this);

            OnStartingLevel();
        }
        #endregion
        #region Navigation
        public RelayCommand CloseWindowCommand => new RelayCommand(execute => CloseWindow());
        private void CloseWindow()
        {
            if (SelectedViewModel == null)
            {
                WindowService?.CloseWindow();
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
        public RelayCommand AddExtraTubeCommand => new RelayCommand(execute => AddExtraTube(), canExecute => GameState.CanAddExtraTube());
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
        public void OnStartingLevel()
        {
            UIEnabled = true;
            DeselectTube();
            GameState.SavedGameStates.Clear();
            GameState.LastGameState = null;
            GameState.SaveGameState();
            AutoSolve = new AutoSolve(this); // guarantees that we remove stuff like previous moves in autosolving
            DrawTubes();
        }
        public string NoteForSavedLevel { get; set; }
        private void SaveLevel()
        {
            ClosePopupWindow();

            ObservableCollection<StoredLevel>? savedLevelList;
            try
            {
                savedLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);
            }
            catch
            {
                return;
            }
            if (savedLevelList is null)
            {
                savedLevelList = new ObservableCollection<StoredLevel>();
            }

            savedLevelList.Add(new StoredLevel(GameState.StartingPosition, NoteForSavedLevel));

            Settings.Default.SavedLevels = JsonConvert.SerializeObject(savedLevelList);
            //Settings.Default.SavedLevels = JsonConvert.SerializeObject(new ObservableCollection<StoredLevel>() { new StoredLevel(TubesManager.SavedStartingTubes) });
            Settings.Default.Save();
            NoteForSavedLevel = null;

            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            PopupWindowNotification(token);
        }
        public RelayCommand CopyExportStringCommand => new RelayCommand(execute => GameState.CopyExportString());
        public RelayCommand AddPresetLevels_Command => new RelayCommand(execute => LoadLevelVM.AddPresetLevels());
        public RelayCommand ExportStepBack_Command => new RelayCommand(execute => GameState.WriteToFileStepBack());
        public CancellationTokenSource TokenSource { get; set; } = null;
        public async void PopupWindowNotification(CancellationToken token)
        {
            PopupWindow.Execute(PopupParams.GameSaved);
            //Thread.Sleep(500);
            //await Task.Delay(2000);
            //Task.Delay(2000);
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(100);
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            ClosePopupWindow();
        }
        private void CloseNotification()
        {
            TokenSource?.Cancel();
        }
        public RelayCommand StepBackCommand => new RelayCommand(execute => GameState.StepBack(), canExecute => GameState.SavedGameStates.Count > 0 && AutoSolve.LimitToOneStep is false);
        public RelayCommand OpenOptionsWindowCommand => new RelayCommand(execute => WindowService?.OpenOptionsWindow(this));
        //public RelayCommand LevelCompleteWindowCommand => new RelayCommand(execute => windowService?.OpenLevelCompleteWindow(this));
        public RelayCommand OpenHelpFromOptionsCommand => new RelayCommand(execute =>
        {
            WindowService?.CloseWindow();
            SelectedViewModel = new HelpVM(this);
        });
        //public RelayCommand DisplayQuickNotificationCommand => new RelayCommand(execute => DisplayQuickNotification("asdf"));
        //private void DisplayQuickNotification(string displayText)
        //{
        //    Notification.Show(displayText);
        //}
        public RelayCommand TestMethodCommand => new RelayCommand(execute => TestMethod());
        private void TestMethod()
        {
#if DEBUG
            TreeNode<ValidMove> node = new TreeNode<ValidMove>(new ValidMove(null));
            var startingNode = node;
            TreeNode<ValidMove> nextNode;

            node.Data.Priority = 100;

            nextNode = new TreeNode<ValidMove>(new ValidMove(null));
            node.AddSibling(nextNode);
            node = nextNode;

            node.Data.Priority = 1;

            nextNode = new TreeNode<ValidMove>(new ValidMove(null));
            node.AddSibling(nextNode);
            node = nextNode;

            node.Data.Priority = 5;

            nextNode = new TreeNode<ValidMove>(new ValidMove(null));
            node.AddSibling(nextNode);
            node = nextNode;

            node.Data.Priority = 3;

            //int number = TreeNodeHelper.CountSiblings(startingNode) + 1; // siblings, plus the original node
            //Notification.Show("Total Nodes: " + number);

            //Notification.Show("StepNumber of last node: " + TreeNodeHelper.GetTailNode(startingNode).Data.StepNumber, 10000);

            TreeNodeHelper.QuickSort(startingNode);

            node = startingNode;
            string displayText = string.Empty;
            while (node is not null)
            {
                //displayText += $"[{node.Data.StepNumber}: {node.Data.Priority}], ";
                displayText += $"[{node.Data.Priority}], ";

                node = node.NextSibling;
            }
            Notification.Show("Priorities list: " + displayText, 10000);
#endif
        }

        #endregion
        #region Moving Liquids
        public RelayCommand SelectTubeCommand => new RelayCommand(obj => OnTubeButtonClick(obj));
        internal void OnTubeButtonClick(object obj)
        {
            if (UIEnabled == false)
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
                currentTubeReference.NumberOfRepeatingLiquids = successAtLeastOnce;
                RippleSurfaceAnimation(currentTubeReference);
                OnChangingGameState();
            }
            if (successAtLeastOnce == 0 && AppSettings.UnselectTubeEvenOnIllegalMove == true)
            {
                DeselectTube();
            }
        }
        public void OnChangingGameState()
        {
            DeselectTube();

            IsLevelCompleted();
            GameState.SaveGameState();
        }
        private void GetTopmostLiquid(TubeReference sourceTube) // selects topmost liquid in a sourceTube
        {
            for (int i = GameState.Layers - 1; i >= 0; i--)
            {
                if (GameState[sourceTube.TubeId, i] is not null)
                {
                    if (LastClickedTube != sourceTube)
                        LastClickedTube = sourceTube;
                    sourceTube.TopMostLiquid = GameState[sourceTube.TubeId, i];
                    VerticallyMoveTube(sourceTube, VerticalAnimation.Raise);
                    //MoveAndTiltTube(sourceTube);
                    return;
                }
            }
        }

        private bool AddLiquidToTargetTube(TubeReference currentTubeReference)
        {
            int firstEmptyLayer = -1;
            for (int y = 0; y < GameState.Layers; y++)
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
            for (int y = GameState.Layers - 1; y >= 0; y--)
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
                VerticallyMoveTube(SourceTube, VerticalAnimation.Lower);
                LastClickedTube = null;
            }
        }
        private void IsLevelCompleted()
        {
            if (GameState.IsLevelCompleted())
            {
                if (AppSettings.AdvancedOptionsVisible == false)
                    UIEnabled = false;
                PopupWindow.Execute(PopupParams.LevelComplete);
            }
        }

        #endregion
        #region Draw tubes from code
        [Obsolete] public RelayCommand TestDraw_Command => new RelayCommand(execute => DrawTubes());
        public void DrawTubes()
        {
            TubeCount = (int)Math.Ceiling((decimal)GameState.GetLength(0) / 2);

            ContainerForTubes.Children.Clear(); // deletes classes of type Visual

            //for (int x = 0; x < GameState.NumberOfTubes; x++)
            for (int x = 0; x < GameState.GetLength(0); x++)
            {
                LiquidColor[] liquidColorsArray = new LiquidColor[GameState.Layers];
                for (int y = 0; y < GameState.Layers; y++)
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
        //public static Visual GetDescendantByType(Visual element, Type type)
        //{
        //    if (element == null)
        //    {
        //        return null;
        //    }
        //    if (element.GetType() == type)
        //    {
        //        return element;
        //    }
        //    Visual foundElement = null;
        //    if (element is FrameworkElement)
        //    {
        //        (element as FrameworkElement).ApplyTemplate();
        //    }
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        //    {
        //        Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
        //        foundElement = GetDescendantByType(visual, type);
        //        if (foundElement != null)
        //        {
        //            break;
        //        }
        //    }
        //    return foundElement;
        //}
        #endregion
        #region Animation
        private void VerticallyMoveTube(TubeReference tubeReference, VerticalAnimation tubeAnimation)
        {
            if (tubeReference.ButtonElement is null)
            {
                return;
            }
            var HeightAnimation = new ThicknessAnimation() { Duration = TimeSpan.FromSeconds(0.1) };
            if (tubeAnimation == VerticalAnimation.Raise)
            {
                HeightAnimation.To = new Thickness(0, 0, 0, 15);
            }
            else
            {
                HeightAnimation.From = new Thickness(0, 0, 0, 15);
                HeightAnimation.To = new Thickness(0, 15, 0, 0);
            }
            tubeReference.ButtonElement.BeginAnimation(Button.MarginProperty, HeightAnimation);
        }
        private int GetFirstEmptyLayer(TubeReference lastClickedTube)
        {
            for (int y = 0; y < GameState.Layers; y++)
            {
                if (GameState[lastClickedTube.TubeId, y] is null)
                {
                    return y;
                }
            }
            throw new Exception("This tube should always have empty space.");
        }
        private (ImageBrush, Grid) CreateVerticalTubeAnimationBackground(TubeReference currentTubeReference)
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

            tileSizeRectangle.Height = 52 * currentTubeReference.NumberOfRepeatingLiquids;

            tileSizeRectangle.Fill = brush;

            gridElement.Children.Add(tileSizeRectangle);

            return (brush, gridElement);
        }
        internal void RippleSurfaceAnimation(TubeReference currentTubeReference)
        {
            TubeControl tubeControl = ContainerForTubes.Children[currentTubeReference.TubeId] as TubeControl;

            // Getting reference to the main grid that contains individual liquids in a tube.
            Grid container = (GetDescendantByTypeAndName(tubeControl, typeof(Grid), "TubeGrid")) as Grid;

            (var brush, var gridElement) = CreateVerticalTubeAnimationBackground(currentTubeReference);
            container.Children.Add(gridElement);

            Grid.SetRow(gridElement, GameState.Layers - 1 - currentTubeReference.TargetEmptyRow - currentTubeReference.NumberOfRepeatingLiquids + 1);
            Grid.SetRowSpan(gridElement, currentTubeReference.NumberOfRepeatingLiquids);

            //Canvas.SetZIndex(borderElement, 3);
            //Grid.SetZIndex(borderElement, 4);

            StartAnimatingSurface(brush, container, gridElement, currentTubeReference.NumberOfRepeatingLiquids);
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
        private void MoveAndTiltTube(TubeReference tubeReference)
        {
            if (tubeReference.ButtonElement is null)
            {
                return;
            }

            //tubeReference.ButtonElement.RenderTransform = new RotateTransform();

            //Storyboard storyboard = new Storyboard();
            //storyboard.Duration = new Duration(TimeSpan.FromSeconds(10.0));

            //DoubleAnimation rotateAnimation = new DoubleAnimation()
            //{
            //    From = 0,
            //    To = 60,
            //    Duration = TimeSpan.FromSeconds(2)
            //};
            //Storyboard.SetTarget(rotateAnimation, tubeReference.ButtonElement);
            //Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

            //storyboard.Children.Add(rotateAnimation);
            //MainWindow.Resources.Add("Storyboard", storyboard);

            //storyboard.Begin();



            //var HeightAnimation = new ThicknessAnimation() { To = new Thickness(0, 0, 0, 15), Duration = TimeSpan.FromSeconds(0.1) };
            //tubeReference.ButtonElement.BeginAnimation(Button.MarginProperty, HeightAnimation);

            //tubeReference.ButtonElement.RenderTransform = new RotateTransform();
            //var bodymove = new TranslateTransform();


            //DoubleAnimation rotateAnimation = new DoubleAnimation()
            //{
            //    From = 0,
            //    To = 60,
            //    Duration = TimeSpan.FromSeconds(2)
            //};
            ////var propertypath = new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)");

            //tubeReference.ButtonElement.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);



            RotateTransform rotateTransform = new RotateTransform();
            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                From = 0,
                To = 60,
                Duration = TimeSpan.FromSeconds(1)
            };
            //doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            //tubeReference.ButtonElement.RenderTransformOrigin = new Point(x, y);
            tubeReference.ButtonElement.RenderTransform = rotateTransform;
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);

        }
        #endregion
        #region Other Methods
        //private void Tubes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    TubesPerLineCalculation();
        //}
        //private void TubesPerLineCalculation()
        //{
        //    //TubeCount = Tubes.Count;
        //    //TubeCount = Tubes.Where(tube => tube.Layers.Count > 0).Count();

        //    TubeCount = (int)Math.Ceiling((decimal)GameState.GetLength(0) / 2);
        //}
        //private void TubeCount_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    //TubeCount = (int)Math.Ceiling((decimal)Tubes.Count / 2);
        //    TubeCount = Tubes.Count;
        //}
        public RelayCommand AutoSolveCommand => new RelayCommand(execute => AutoSolve.CalculateNextStep(GameState.gameGrid));
        public RelayCommand StepThroughCommand => new RelayCommand(execute => AutoSolve.StepThrough(), canExecute => AutoSolve.CompleteSolution.Count > 0);
        #endregion
    }
}