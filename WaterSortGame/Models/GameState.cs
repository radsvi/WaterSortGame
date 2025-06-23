using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WaterSortGame.ViewModels;
using ColN = WaterSortGame.Models.LiquidColorName; // creating alias so that I dont have to have long names in GenerateDebugLevel();

namespace WaterSortGame.Models
{
    internal class GameState : ViewModelBase
    {
        MainWindowVM mainWindowVM;
        AppSettings appSettings;
        Notification notification;

        private string readableGameState;
        public string ReadableGameState
        {
            get { 
                
                return GameStateToString(gameGrid, StringFormat.Numbers); 
            }
            set
            {
                if (value != readableGameState)
                {
                    readableGameState = value;
                    OnPropertyChanged();
                }
            }
        }
        //public int NumberOfTubes { get; private set; }
        public const int Layers = 4;
        public LiquidColor[,] gameGrid;
        public LiquidColor this[int tubes, int layers]
        {
            get => gameGrid[tubes, layers];
            set
            {
                gameGrid[tubes, layers] = value;
                //OnLiquidMoving();
            }
        }
        public int GetLength(int dimension)
        {
            return gameGrid.GetLength(dimension);
        }

        //private List<List<LiquidColorNew>> gameState;
        //public LiquidColorNew this[int tube, int layer]
        //{
        //    get
        //    {
        //        //return gameStateList.ElementAt(tubes).ElementAt(layers);
        //        return gameState[tube][layer];
        //    }
        //    set
        //    {
        //        gameState[tube][layer] = value;
        //    }
        //}


        //private ObservableCollection<Tube> tubes = new ObservableCollection<Tube>();
        //public ObservableCollection<Tube> Tubes
        //{
        //    get { return tubes; }
        //    set
        //    {
        //        if (value != tubes)
        //        {
        //            tubes = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        private int extraTubesAdded = 0;
        public int ExtraTubesAdded { get; private set; }
        private LiquidColor[,] startingPosition;
        public LiquidColor[,] StartingPosition { get; set; }
        private ObservableCollection<LiquidColor[,]> savedGameSteps = new ObservableCollection<LiquidColor[,]>();
        public ObservableCollection<LiquidColor[,]> SavedGameStates
        {
            get { return savedGameSteps; }
            private set
            {
                if (value != savedGameSteps)
                {
                    savedGameSteps = value;
                    //OnPropertyChanged();
                }
            }
        }
        public LiquidColor[,] LastGameState { get; set; }
        public GameState() { }
        public GameState(MainWindowVM mainWindowVM)
        {
            this.mainWindowVM = mainWindowVM;
            appSettings = this.mainWindowVM.AppSettings;
            notification = mainWindowVM.Notification;

            //if (Tubes.Count == 0)
            //{
            //    GenerateNewLevel();
            //}

            //gameState = new int[Tubes, Layers];

            GenerateNewLevel();
        }
        //private void SetGameGrid(int numberOfTubes)
        //{
        //    //gameGrid = new LiquidColorNew[(NumberOfTubes + ExtraTubesAdded + 2), NumberOfLayers];
        //    gameGrid = new LiquidColorNew[numberOfTubes, NumberOfLayers];
        //}
        //private void OnLiquidMoving()
        //{
        //    MainWindow.DrawTubes();
        //}
        public void GenerateNewLevel()
        {
            if (appSettings.LoadDebugLevel is true)
                GenerateDebugLevel();
            else
                GenerateStandardLevel();
        }
        private void GenerateDebugLevel()
        {
            
            //Tube.ResetCounter();
            //Tubes?.Clear();
            
            int i = 0;
            gameGrid = new LiquidColor[20, Layers];

            //AddTube(i++, new int[] { 1, 1, 4, 4 });
            //AddTube(i++, new int[] { 8, 8, 1, 1 });
            //AddTube(i++, new int[] { 4, 4, 8, 8 });
            //AddTube(i++, new int[] { 3, 3, 3 });
            //AddTube(i++, new int[] { 7, 7, 7 });
            //AddTube(i++, new int[] { 3 });
            //AddTube(i++, new int[] { 7 });

            //AddTube(i++, new int[] { 1, 1, 4, 4 });
            //AddTube(i++, new int[] { 8, 8, 1, 1 });
            //AddTube(i++, new int[] { 4, 4, 8, 8 });
            //AddTube(i++, new int[] { 3, 3, 3 });
            //AddTube(i++, new int[] { 7, 7, 7 });
            //AddTube(i++, new int[] { 3 });
            //AddTube(i++, new int[] { 7 });

            //AddTube(i++, new int[] { 1, 1, 1, 2 });
            //AddTube(i++, new int[] {  });
            //AddTube(i++, new int[] { 2, 2, 2 });
            //AddTube(i++, new int[] { 1 });
            //AddTube(i++, new int[] { 4, 4, 4, 4, });

            //AddTube(i++, new int[] { });
            //AddTube(i++, new int[] { 1 });
            //AddTube(i++, new int[] { 1,1,1 });
            //AddTube(i++, new int[] { 3,3,3,3 });
            //AddTube(i++, new int[] { 4, 4, 4, 4, });


            //AddTube(i++, new int[] { 1,2,2,3 });
            //AddTube(i++, new int[] { 4,3,2,1 });
            //AddTube(i++, new int[] { 1,3,1,4 });
            //AddTube(i++, new int[] { 2,3,4,4 });
            //AddTube(i++, new int[] {  });
            //AddTube(i++, new int[] {  });

            //AddTube(i++, new int[] { 1, 1, 1, 1 });
            //AddTube(i++, new int[] { 2, 3, 3, 3 });
            //AddTube(i++, new int[] { });
            //AddTube(i++, new int[] { 2, 3 });
            //AddTube(i++, new int[] { 4,4,4,4 });
            //AddTube(i++, new int[] { 5,5,});
            //AddTube(i++, new int[] { 6,6,6,6});

            //AddTube(i++, new int[] { });
            //AddTube(i++, new int[] { 1, 1, 1, 1 });
            //AddTube(i++, new int[] { 2, 3, 3 });
            //AddTube(i++, new int[] { 2, 2, 2 });
            //AddTube(i++, new int[] { 3, 3 });

            //AddTube(i++, new int[] { 1,2,3,2 });
            //AddTube(i++, new int[] { 1,1,2,4 });
            //AddTube(i++, new int[] { 5,2,3,4 });
            //AddTube(i++, new int[] { 4,1,3,5 });
            //AddTube(i++, new int[] { 5,3,5,4 });

            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Purple, LiquidColorName.LightBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Red, LiquidColorName.Blue, LiquidColorName.LightBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Olive, LiquidColorName.Orange, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Purple, LiquidColorName.GrayBlue, LiquidColorName.Yellow });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.LightBlue, LiquidColorName.Yellow, LiquidColorName.GrayBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightGreen, LiquidColorName.Blue, LiquidColorName.Olive, LiquidColorName.LightGreen });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Green, LiquidColorName.LightGreen, LiquidColorName.Olive, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Gray, LiquidColorName.Gray, LiquidColorName.Green });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Olive, LiquidColorName.LightGreen });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.Orange, LiquidColorName.Purple, LiquidColorName.Orange });

            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Purple });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Red, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Olive, LiquidColorName.Orange, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Purple, LiquidColorName.GrayBlue, LiquidColorName.Yellow });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.LightBlue, LiquidColorName.Yellow, LiquidColorName.GrayBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightGreen, LiquidColorName.Blue, LiquidColorName.Olive, LiquidColorName.LightGreen });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Green, LiquidColorName.LightGreen, LiquidColorName.Olive });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Gray, LiquidColorName.Gray, LiquidColorName.Green });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Olive, LiquidColorName.LightGreen });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.GrayBlue, LiquidColorName.Orange, LiquidColorName.Purple, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.LightBlue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue });


            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Red, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.LightBlue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Pink, LiquidColorName.Red });






            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Gray, LiquidColorName.Red, LiquidColorName.Blue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Gray });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Blue, LiquidColorName.Gray, LiquidColorName.Red });

            //// Nikdy nevyresenej level (stary cislovani):
            //AddTube(i++, new int[] { 9, 2, 4, 1 });
            //AddTube(i++, new int[] { 3, 8, 11, 5 });
            //AddTube(i++, new int[] { 9, 11, 11, 12 });
            //AddTube(i++, new int[] { 3, 3, 2, 5 });
            //AddTube(i++, new int[] { 1, 7, 6, 10 });
            //AddTube(i++, new int[] { 3, 4, 7, 4 });
            //AddTube(i++, new int[] { 2, 8, 5, 10 });
            //AddTube(i++, new int[] { 6, 1, 2, 9 });
            //AddTube(i++, new int[] { 11, 10, 7, 6 });
            //AddTube(i++, new int[] { 5, 7, 10, 4 });
            //AddTube(i++, new int[] { 8, 12, 6, 12 });
            //AddTube(i++, new int[] { 1, 12, 8, 9 });

            //// Ten starej "nikdy" nevyresenej level (vyresenej za 1772 steps to generate, 46 steps to solve, 17.68 sec):
            //AddTube(i++, new int[] { 8, 1, 3, 0 });
            //AddTube(i++, new int[] { 2, 7, 10, 4 });
            //AddTube(i++, new int[] { 8, 10, 10, 11 });
            //AddTube(i++, new int[] { 2, 2, 1, 4 });
            //AddTube(i++, new int[] { 0, 6, 5, 9 });
            //AddTube(i++, new int[] { 2, 3, 6, 3 });
            //AddTube(i++, new int[] { 1, 7, 4, 9 });
            //AddTube(i++, new int[] { 5, 0, 1, 8 });
            //AddTube(i++, new int[] { 10, 9, 6, 5 });
            //AddTube(i++, new int[] { 4, 6, 9, 3 });
            //AddTube(i++, new int[] { 7, 11, 5, 11 });
            //AddTube(i++, new int[] { 0, 11, 7, 8 });

            //// Ten starej "nikdy" nevyresenej level (testuju napul vyreseny:
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Green, LiquidColorName.Indigo, LiquidColorName.Orange, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Turquoise, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Green, LiquidColorName.Green, LiquidColorName.Green });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Turquoise, LiquidColorName.Turquoise, LiquidColorName.Indigo, LiquidColorName.Indigo });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Lime, LiquidColorName.Lime, LiquidColorName.Lime, LiquidColorName.Lime});
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Turquoise, LiquidColorName.Orange, LiquidColorName.Yellow, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Indigo, LiquidColorName.Pink, LiquidColorName.Pink, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Red, LiquidColorName.Red, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Olive, LiquidColorName.Olive, LiquidColorName.Olive, LiquidColorName.Olive });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Gray, LiquidColorName.Yellow, LiquidColorName.Yellow, LiquidColorName.Yellow });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Gray, LiquidColorName.Gray, LiquidColorName.Gray });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Blue, LiquidColorName.Blue, LiquidColorName.Blue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Purple, LiquidColorName.Purple, LiquidColorName.Purple });


            //// sejvnutej level z te novejsi hry. Level jsem vyresil, ale byl tezkej (jeste jednou jsem to overil ze je to resitelny. Uspesne!):
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Red, LiquidColorName.Blue, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Indigo, LiquidColorName.Turquoise, LiquidColorName.Lime });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Pink, LiquidColorName.Turquoise, LiquidColorName.Turquoise, LiquidColorName.Gray });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Purple, LiquidColorName.Red, LiquidColorName.Lime });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Orange, LiquidColorName.Yellow, LiquidColorName.Olive, LiquidColorName.Green });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Blue, LiquidColorName.Yellow, LiquidColorName.Blue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Indigo, LiquidColorName.Lime, LiquidColorName.Green });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Olive, LiquidColorName.Orange, LiquidColorName.Red, LiquidColorName.Pink });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Turquoise, LiquidColorName.Green, LiquidColorName.Yellow, LiquidColorName.Olive });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Lime, LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Blue });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Indigo, LiquidColorName.Gray, LiquidColorName.Olive, LiquidColorName.Gray });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Orange, LiquidColorName.Gray, LiquidColorName.Indigo, LiquidColorName.Pink });

            //AddTube(i++, new int[] { 1, 1, 1, 7 });
            //AddTube(i++, new int[] { 2, 6, 7, 4 });
            //AddTube(i++, new int[] { 3, 1, 11, 5 });
            //AddTube(i++, new int[] { 5, 0, 4, 3 });
            //AddTube(i++, new int[] { 5, 11, 6, 6 });
            //AddTube(i++, new int[] { 8, 2, 1, 7 });
            //AddTube(i++, new int[] { 8, 3, 5, 3 });
            //AddTube(i++, new int[] { 8, 1, 4, 9 });
            //AddTube(i++, new int[] { 9, 0, 8, 2 });
            //AddTube(i++, new int[] { 9, 11, 6, 4 });
            //AddTube(i++, new int[] { 1, 0, 7, 1 });
            //AddTube(i++, new int[] { 11, 0, 2, 9 });


            //AddTube(i++, new int[] { 1,1,1 });

            //// tenhle level jsem nevyresil manualne, tim bruteforce exponencialnim scriptem to trvalo 7 sekund a 685 kroku:
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Red, LiquidColorName.Blue, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Turquoise, LiquidColorName.Turquoise, LiquidColorName.Lime, LiquidColorName.Purple });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Scarlet, LiquidColorName.Orange, LiquidColorName.Indigo, LiquidColorName.Indigo });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Indigo, LiquidColorName.Orange, LiquidColorName.Green, LiquidColorName.Gray });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Brown, LiquidColorName.Blue, LiquidColorName.Purple, LiquidColorName.Gray });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Turquoise, LiquidColorName.Scarlet, LiquidColorName.Indigo, LiquidColorName.Lime });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Gray, LiquidColorName.Scarlet, LiquidColorName.Yellow, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Scarlet, LiquidColorName.Blue, LiquidColorName.Brown, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Blue, LiquidColorName.Yellow, LiquidColorName.Green, LiquidColorName.Lime });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Gray, LiquidColorName.Yellow, LiquidColorName.Turquoise, LiquidColorName.Green });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Brown, LiquidColorName.Purple, LiquidColorName.Lime });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Brown, LiquidColorName.Green, LiquidColorName.Purple });

            //// Random level kterej zacina s 3 stejnejma barvama nahore (178 states, 48 steps, 2.37sec - s tim checkovanim na zacatku):
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Purple, LiquidColorName.Red, LiquidColorName.Blue, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Indigo, LiquidColorName.Turquoise, LiquidColorName.Orange, LiquidColorName.Lime });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Green, LiquidColorName.Gray, LiquidColorName.Indigo, LiquidColorName.Scarlet });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Brown, LiquidColorName.Brown, LiquidColorName.Brown, LiquidColorName.Purple });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Green, LiquidColorName.Turquoise, LiquidColorName.Green, LiquidColorName.Scarlet });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Indigo, LiquidColorName.Lime, LiquidColorName.Orange });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Turquoise, LiquidColorName.Gray, LiquidColorName.Yellow, LiquidColorName.Indigo });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Yellow, LiquidColorName.Lime, LiquidColorName.Green, LiquidColorName.Purple });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Gray, LiquidColorName.Red, LiquidColorName.Scarlet, LiquidColorName.Purple });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Red, LiquidColorName.Orange, LiquidColorName.Blue, LiquidColorName.Red });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Gray, LiquidColorName.Lime, LiquidColorName.Turquoise, LiquidColorName.Yellow });
            //AddTube(i++, new LiquidColorName[] { LiquidColorName.Brown, LiquidColorName.Scarlet, LiquidColorName.Blue, LiquidColorName.Blue });

            //// Dalsi random level kterej zaina s 3 stejnejma barvama nahore (114 states, 50 steps, 1.94sec - s tim checkovanim na zacatku):
            //// (175 states, 45 steps, 2.62sec - bez tim checkovanim na zacatku):
            //AddTube(i++, new ColN[] { ColN.Indigo, ColN.Blue, ColN.Turquoise, ColN.Lime });
            //AddTube(i++, new ColN[] { ColN.Scarlet, ColN.Indigo, ColN.Gray, ColN.Scarlet });
            //AddTube(i++, new ColN[] { ColN.Purple, ColN.Brown, ColN.Green, ColN.Orange });
            //AddTube(i++, new ColN[] { ColN.Purple, ColN.Red, ColN.Blue, ColN.Orange });
            //AddTube(i++, new ColN[] { ColN.Yellow, ColN.Lime, ColN.Blue, ColN.Purple });
            //AddTube(i++, new ColN[] { ColN.Turquoise, ColN.Scarlet, ColN.Blue, ColN.Green });
            //AddTube(i++, new ColN[] { ColN.Gray, ColN.Yellow, ColN.Yellow, ColN.Green });
            //AddTube(i++, new ColN[] { ColN.Yellow, ColN.Turquoise, ColN.Brown, ColN.Orange });
            //AddTube(i++, new ColN[] { ColN.Lime, ColN.Red, ColN.Indigo, ColN.Gray });
            //AddTube(i++, new ColN[] { ColN.Green, ColN.Red, ColN.Orange, ColN.Red });
            //AddTube(i++, new ColN[] { ColN.Brown, ColN.Brown, ColN.Scarlet, ColN.Indigo });
            //AddTube(i++, new ColN[] { ColN.Lime, ColN.Gray, ColN.Turquoise, ColN.Purple });

            ////AddTube(i++, new int[] { 1,1,1 });
            //AddTube(i++, new int[] { 3, 4, 5, 6 });
            //AddTube(i++, new int[] { 2, 1 });
            //AddTube(i++, new int[] { 2, 2, 2 });
            //AddTube(i++, new int[] { 3, 4, 5, 6 });
            //AddTube(i++, new int[] { 3, 4, 5, 6 });
            //AddTube(i++, new int[] { 3, 4, 5, 6 });
            //AddTube(i++, new int[] { 1, 1, 1 });

            AddTube(i++, new LiquidColorName[] { ColN.Pink, ColN.Pink, ColN.Olive, ColN.Gray });
            AddTube(i++, new LiquidColorName[] { ColN.Gray, ColN.Red, ColN.Pink, ColN.Red });
            AddTube(i++, new LiquidColorName[] { ColN.Olive, ColN.Red, ColN.Gray, ColN.Olive });
            AddTube(i++, new LiquidColorName[] { ColN.Pink, ColN.Red, ColN.Gray, ColN.Olive });


            //AddTube(i++, new LiquidColorName[] { ColN. });
            //AddTube(i++, new LiquidColorName[] { });

            // check if puzzle has correct number for each color:
            CheckCorrectColorNumber(gameGrid);

            //gameGrid = CloneGrid(gameGrid, i + 2);
            gameGrid = CloneGrid(gameGrid, i + 2);
            StoreStartingGrid();
        }
        private void CheckCorrectColorNumber(LiquidColor[,] gameGrid)
        {
            ColorCount colorList = new ColorCount();
            foreach (LiquidColor color in gameGrid)
            {
                if (color is not null)
                    colorList.AddColor(color.Name);
            }
            foreach (var color in colorList)
            {
                if (color.Value != gameGrid.GetLength(1))
                notification.Show($"{color.Key}: {color.Value}", 60000);
            }
        }
        private void AddTube(int tubeNumber, int[] layers)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                this[tubeNumber, i] = new LiquidColor(layers[i]);
            }
        }
        private void AddTube(int tubeNumber, LiquidColorName[] liquids)
        {
            for (int i = 0; i < liquids.Length; i++)
            {
                this[tubeNumber, i] = new LiquidColor((int)liquids[i]);
            }
        }
        /// <summary>
        /// Adding extra (empty) tube during gameplay
        /// </summary>
        public bool CanAddExtraTube()
        {
            return CountColors() + appSettings.MaximumExtraTubes + 2 - gameGrid.GetLength(0) > 0;
        }
        public void AddExtraTube()
        {
            if (CanAddExtraTube())
            {
                gameGrid = CloneGrid(gameGrid, gameGrid.GetLength(0) + 1);
            }
        }
        public static LiquidColor[,] CloneGridStatic(LiquidColor[,] grid)
        {
            return new GameState().CloneGrid(grid, grid.GetLength(0));
        }
        public LiquidColor[,] CloneGrid(LiquidColor[,] grid)
        {
            return CloneGrid(grid, grid.GetLength(0));
        }
        public LiquidColor[,] CloneGrid(LiquidColor[,] gameGrid, int numberOfTubes)
        {
            LiquidColor[,] gridClone = new LiquidColor[numberOfTubes, gameGrid.GetLength(1)];
            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                for (int y = 0; y < gameGrid.GetLength(1); y++)
                {
                    if (gameGrid[x, y] is not null)
                    {
                        gridClone[x, y] = gameGrid[x, y].Clone();
                    }
                }
            }
            return gridClone;
        }
        public void SetGameState(LiquidColor[,] newGameState)
        {
            gameGrid = CloneGrid(newGameState);
        }
        public void RestartLevel()
        {
            SetFreshGameState();
            gameGrid = CloneGrid(StartingPosition);
        }
        private void GenerateStandardLevel()
        {
            Random rnd = new Random();

            List<LiquidColor> colorsList = new List<LiquidColor>();
            if (appSettings.RandomNumberOfTubes)
            {
                appSettings.NumberOfColorsToGenerate = rnd.Next(3, LiquidColor.ColorKeys.Count - 1);
            }

            gameGrid = new LiquidColor[appSettings.NumberOfColorsToGenerate + 2, Layers];
            //Tube.ResetCounter();
            SetFreshGameState();

            List<int> selectedColors = new List<int>();
            for (int i = 0; i < LiquidColor.ColorKeys.Count - 1; i++) // generate list of all colors. Doing '- 1' because color number 0 is blank (and is used for other purposes) but still count towards total.
            {
                selectedColors.Add(i);
            }

            for (int i = 0; i < LiquidColor.ColorKeys.Count - 1 - appSettings.NumberOfColorsToGenerate; i++) // now remove some random colors. 
            {
                //selectedColors.Remove(selectedColors[NumberOfColorsToGenerate]); // this always keeps the same colors
                selectedColors.Remove(selectedColors[rnd.Next(0, selectedColors.Count)]);
            }
            foreach (var color in selectedColors)
            {
                colorsList.Add(new LiquidColor(color));
                colorsList.Add(new LiquidColor(color));
                colorsList.Add(new LiquidColor(color));
                colorsList.Add(new LiquidColor(color));
            }

            // add colors randomly to the grid
            for (int x = 0; x < appSettings.NumberOfColorsToGenerate; x++)
            {
                for (int y = 0; y < Layers; y++)
                {
                    //var maxNumber = colorsList.Count > 0 ? colorsList.Count - 1 : 0;
                    //int colorNumber = rnd.Next(0, maxNumber);
                    int colorNumber = rnd.Next(0, colorsList.Count);

                    //var asdf = colorsList[colorNumber];
                    gameGrid[x, y] = colorsList[colorNumber];
                    colorsList.Remove(colorsList[colorNumber]);
                }
            }

            StoreStartingGrid();
        }
        private void SetFreshGameState()
        {
            //ExtraTubesAdded = 0; // resets how much extra tubes has been added
            // commented out, because its reset automatically every time new GameState object is created
        }
        private void StoreStartingGrid()
        {
            StartingPosition = CloneGrid(gameGrid);
        }
        public void SaveGameState()
        {
            if (DidGameStateChange() == true)
            //if (SolvingSteps[SolvingSteps.Count - 1] != Tubes)
            {
                if (LastGameState != null) // pridavam to tady, protoze nechci v game states mit i current game state.
                {
                    SavedGameStates.Add(LastGameState);
                    LastGameState = null;
                }

                LastGameState = CloneGrid(gameGrid);
                return;
            }
        }
        private bool DidGameStateChange()
        {
            if (SavedGameStates.Count == 0 && LastGameState == null)
            {
                return true;
            }
            //if (LastGameState.Length != gameGrid.Length) // pokud jen pridavam extra prazdnou zkumavku tak to neukladat!
            //{
            //    return false;
            //}

            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                for (int y = 0; y < gameGrid.GetLength(1); y++)
                {
                    if (LastGameState[x, y] is null && gameGrid[x, y] is null)
                    {
                        continue;
                    }
                    if (LastGameState[x, y] is null && gameGrid[x, y] is not null || LastGameState[x, y] is not null && gameGrid[x, y] is null)
                    {
                        return true;
                    }
                    if (LastGameState[x,y].Name != gameGrid[x,y].Name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsLevelCompleted()
        {
            return IsLevelCompleted(gameGrid);
        }
        public bool IsLevelCompleted(LiquidColor[,] internalGameGrid)
        {
            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                //if (gameGrid[x, 0] is null || gameGrid[x, 1] is null || gameGrid[x, 2] is null || gameGrid[x, 3] is null)
                //{ // tohle tu je abych nikdy neporovnaval hodnoty GameGridu kdyz je moznost ze budou null:
                //    continue;
                //}
                
                // tohle tu je abych nikdy neporovnaval hodnoty GameGridu kdyz je moznost ze budou null:
                if (internalGameGrid[x, 0] is not null &&
                    internalGameGrid[x, 1] is not null &&
                    internalGameGrid[x, 2] is not null &&
                    internalGameGrid[x, 3] is not null)
                {
                    if (!(internalGameGrid[x, 0].Name == internalGameGrid[x, 1].Name &&
                        internalGameGrid[x, 0].Name == internalGameGrid[x, 2].Name &&
                        internalGameGrid[x, 0].Name == internalGameGrid[x, 3].Name))
                    {
                        return false;
                    }
                }
                else
                { // v pripade ze aspon jeden objekt je null, otestovat jestli jsou vsechny null
                    if (!(internalGameGrid[x, 0] is null &&
                    internalGameGrid[x, 1] is null &&
                    internalGameGrid[x, 2] is null &&
                    internalGameGrid[x, 3] is null))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void StepBack()
        {
            if (SavedGameStates.Count == 0)
            {
                return;
            }

            LiquidColor[,] lastGameStatus = SavedGameStates[SavedGameStates.Count - 1];

            mainWindowVM.PropertyChangedEventPaused = true;
            gameGrid = lastGameStatus;
            mainWindowVM.PropertyChangedEventPaused = false;

            LastGameState = CloneGrid(lastGameStatus);

            SavedGameStates.Remove(lastGameStatus);

            if (mainWindowVM.AutoSolve.CompleteSolution.Count > 0)
                mainWindowVM.AutoSolve.CurrentSolutionStep++;

            mainWindowVM.DrawTubes();
        }
        public void WriteToFileStepBack()
        {
            string exportString = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "\n";
            foreach (var savedState in SavedGameStates)
            {
                exportString += GameStateToString(savedState, StringFormat.Names, true) + "\n";
            }
            exportString += GameStateToString(gameGrid, StringFormat.Names) + "\n";
            exportString += "===================================\n";

            //System.IO.File.WriteAllText("ExportStepBack.log", exportString);
            System.IO.File.AppendAllText("Export-StepBack.log", exportString);
            mainWindowVM.WindowService?.CloseWindow(); // close options menu
        }

        private int CountColors()
        {
            int numberOfColors = 0;
            List<LiquidColorName?> liquidColors = new List<LiquidColorName?>();
            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                for (int y = 0; y < gameGrid.GetLength(1); y++)
                {
                    if (gameGrid[x, y] is null)
                    {
                        continue;
                    }
                    if (liquidColors.Contains(gameGrid[x, y].Name) == false)
                    {
                        liquidColors.Add(gameGrid[x, y].Name);
                        numberOfColors++;
                    }
                }
            }
            return numberOfColors;
        }
        public void CopyExportString()
        {
            Clipboard.SetText(ReadableGameState);
            mainWindowVM.ClosePopupWindow();
        }
        //public static string GameStateToString(LiquidColorNew[,] gameState, bool enableSort = false)
        //{
        //    return GameStateToString(gameState, StringFormat.Names, enableSort);
        //}
        public static string GameStateToString(LiquidColor[,] gameState, StringFormat format = StringFormat.Names, bool enableSort = true)
        {
            List<string> intGameState = new List<string>();
            for (int x = 0; x < gameState.GetLength(0); x++)
            {
                string tubeString = "[";
                for (int y = gameState.GetLength(1) - 1; y >= 0; y--)
                {
                    if (gameState[x, y] is not null)
                    {
                        //tubeInt += (int)gameState[x, y].Name * (int)Math.Pow(100,y);
                        if (format == StringFormat.Names)
                        {
                            tubeString += (gameState[x, y].Name).ToString();
                        }
                        else
                        {
                            tubeString += ((int)gameState[x, y].Name).ToString("00"); // this format is used for debugging. To easily export the gamestate as a string.
                        }
                    }
                    else
                        tubeString += "-";
                    if (y > 0) tubeString += ".";
                }
                tubeString += "]";
                intGameState.Add(tubeString);
            }
            if (enableSort)
            {
                intGameState.Sort(); // nechci sortovat kdyz chci vizualizaci
            }
            string stringGameState = string.Empty;
            foreach (var tube in intGameState)
            {
                stringGameState += tube.ToString();
            }
            return stringGameState;
        }


    }
}
