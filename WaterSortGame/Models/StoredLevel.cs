using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    class StoredLevel : ViewModelBase
    {
        public int NumberOfColors { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public LiquidColor[,] GameGrid { get; set; }
        public List<Flask> GameGridDisplayList { get; set; }
        private bool markedForDeletion;
        public bool MarkedForDeletion
        {
            get { return markedForDeletion; }
            set
            {
                if (value != markedForDeletion)
                {
                    markedForDeletion = value;
                    OnPropertyChanged();
                }
            }
        }
        public string GetMarkedStatus()
        {
            if (markedForDeletion == true)
            {
                return "marked";
            }
            else
            {
                return null;
            }
        }

        [JsonConstructor]
        public StoredLevel(LiquidColor[,] gameGrid, string noteForSavedLevel)
        {
            if (gameGrid is null)
            {
                return;
            }
            this.GameGrid = gameGrid;
            this.Date = DateTime.Now;
            this.Note = noteForSavedLevel;

            List<LiquidColorName?> colorIds = new List<LiquidColorName?>();
            for (int x = 0; x < gameGrid.GetLength(0); x++)
            {
                for (int y = 0; y < gameGrid.GetLength(1); y++)
                {
                    if (gameGrid[x, y] is null)
                    {
                        continue;
                    }
                    if (colorIds.Contains(gameGrid[x,y].Name) == false)
                    {
                        colorIds.Add(gameGrid[x, y].Name);
                        this.NumberOfColors++;
                    }
                }
            }
        }
        //public StoredLevel(int?[,] gameGridInt, string noteForSavedLevel)
        //{
        //    if (gameGridInt is null)
        //    {
        //        return;
        //    }
        //    this.GameGrid = new LiquidColorNew[gameGridInt.GetLength(0), gameGridInt.GetLength(1)];
        //    this.Date = DateTime.Now;
        //    this.Note = noteForSavedLevel;

        //    List<LiquidColorNames?> colorIds = new List<LiquidColorNames?>();
        //    for (int x = 0; x < gameGridInt.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < gameGridInt.GetLength(1); y++)
        //        {
        //            if (gameGridInt[x, y] is null)
        //            {
        //                continue;
        //            }
        //            this.GameGrid[x, y] = new LiquidColorNew((int)gameGridInt[x, y]);
        //            if (colorIds.Contains(this.GameGrid[x, y].Name) == false)
        //            {
        //                colorIds.Add(this.GameGrid[x, y].Name);
        //                this.NumberOfColors++;
        //            }
        //        }
        //    }
        //}
        //public StoredLevel(int?[,] gameGridInt, string noteForSavedLevel) : this(gameGrid, noteForSavedLevel)
        //{
        //    var gameGrid = new LiquidColorNew[gameGridInt.GetLength(0), gameGridInt.GetLength(1)];
        //    for (int x = 0; x < gameGridInt.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < gameGridInt.GetLength(1); y++)
        //        {
        //            if (gameGridInt[x, y] is null)
        //            {
        //                continue;
        //            }
        //            GameGrid[x, y] = new LiquidColorNew((int)gameGridInt[x, y]);
        //        }
        //    }


        //}
    }
}
