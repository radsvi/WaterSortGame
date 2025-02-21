using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.Models;
using WaterSortGame.MVVM;
using WaterSortGame.Properties;

namespace WaterSortGame.ViewModels
{
    class LoadLevelVM : PopupScreenBase
    {
        public LoadLevelVM(object viewModel) : base(viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
            LoadLevelList.CollectionChanged += LoadLevelList_CollectionChanged;
            //MainWindowVM.LoadLevelScreen();

            //MainWindowVM.LoadLevelList = LoadLevelList;
            MainWindowVM.LoadLevelVM = this;
        }

        private StoredLevel selectedLevelForLoading;
        public StoredLevel SelectedLevelForLoading
        {
            get { return selectedLevelForLoading; }
            set
            {
                if (value != selectedLevelForLoading)
                {
                    selectedLevelForLoading = value;
                    //OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<StoredLevel> loadLevelList = new ObservableCollection<StoredLevel>();
        public ObservableCollection<StoredLevel> LoadLevelList
        {
            get { return loadLevelList; }
            set
            {
                if (value != loadLevelList)
                {
                    loadLevelList = value;
                    //OnPropertyChanged();
                    //OnLoadLevelListChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        private int loadLevelScreenHeight;
        public int LoadLevelScreenHeight
        {
            get { return loadLevelScreenHeight; }
            set
            {
                if (value != loadLevelScreenHeight)
                {
                    loadLevelScreenHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        //public event EventHandler? OnLoadLevelListChanged;
        internal void LoadLevelList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int increaseHeight = 0;
            if (LoadLevelList.Count > 3 && LoadLevelList.Count < 12)
            {
                increaseHeight = (LoadLevelList.Count - 3) * 45; //vyska jedne polozky je 45
                LoadLevelScreenHeight = 280 + increaseHeight;
            }
            else if (LoadLevelList.Count <= 3)
            {
                LoadLevelScreenHeight = 280;
            }
            else if (LoadLevelList.Count >= 12)
            {
                LoadLevelScreenHeight = 640;
                LoadLevelScreenScroll = true;
            }
        }
        private bool loadLevelScreenScroll;
        public bool LoadLevelScreenScroll
        {
            get { return loadLevelScreenScroll; }
            set
            {
                if (value != loadLevelScreenScroll)
                {
                    loadLevelScreenScroll = value;
                    OnPropertyChanged();
                }
            }
        }
        //private void LoadLevelListChangedCalculation(object? sender, EventArgs e)
        //{

        //    if (LoadLevelList.Count > 3)
        //    {
        //        var increaseHeight = (LoadLevelList.Count - 3) * 45; //vyska jedne polozky je 45
        //        LoadLevelScreenHeight = 280 + increaseHeight;
        //    }
        //    else
        //    {
        //        LoadLevelScreenHeight = 280;
        //    }
        //}
        internal void LoadLevelScreen()
        {
            //LoadLevelList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);

            LoadLevelList?.Clear();
            var deserializedList = JsonConvert.DeserializeObject<ObservableCollection<StoredLevel>>(Settings.Default.SavedLevels);
            foreach (var item in deserializedList)
            {
                LoadLevelList.Add(item);
            }

            //OnLoadLevelListChanged?.Invoke(this, EventArgs.Empty);
            //LoadLevelScreenHeight = 280;
            //if (LoadLevelList.Count > 3)
            //{
            //    var increaseHeight = (LoadLevelList.Count - 3) * 45; //vyska jedne polozky je 45
            //    LoadLevelScreenHeight += increaseHeight;
            //}
            //else
            //{
            //    LoadLevelScreenHeight = 280;
            //}
        }
        public RelayCommand LoadLevelCommand => new RelayCommand(execute => LoadLevel(), canExecute => SelectedLevelForLoading is not null);
        //private void LoadLevel(bool force = false)
        internal void LoadLevel()
        {
            if (SelectedLevelForLoading == null)
            {
                return;
            }
            MainWindowVM.ClosePopupWindow();
            MainWindowVM.PropertyChangedEventPaused = true;
            TubesManager.SavedStartingTubes = MainWindowVM.DeepCopyTubesCollection(SelectedLevelForLoading.GameState);
            

            //TubesManager.Tubes = DeepCopyTubesCollection(TubesManager.SavedStartingTubes);

            //TubesManager.Tubes?.Clear();
            //foreach (Tube tube in TubesManager.SavedStartingTubes)
            //{ // kdyz bych to udelal takhle, tak se prestane refreshovat TubesPerLineCalculation(); a GenerateNewLevel() taky
            //    TubesManager.Tubes.Add(tube.DeepCopy());
            //}
            //OnStartingLevel();

            MainWindowVM.RestartLevel();
            MainWindowVM.PropertyChangedEventPaused = false;
        }
        public RelayCommand DeleteSelectedLevelsCommand => new RelayCommand(execute => DeleteSelectedLevels(), canExecute => CanDelete());
        private bool CanDelete()
        {
            foreach (var savedLevel in LoadLevelList)
            {
                if (savedLevel.MarkedForDeletion is true)
                {
                    return true;
                }
            }
            return false;
        }
        private void DeleteSelectedLevels()
        {
            var levelsToRemove = LoadLevelList.Where(item => item.MarkedForDeletion == true).ToList();
            foreach (var levelToRemove in levelsToRemove)
            {
                LoadLevelList.Remove(levelToRemove);
            }

            Settings.Default.SavedLevels = JsonConvert.SerializeObject(LoadLevelList);
            Settings.Default.Save();
        }
        public RelayCommand MarkForDeletionCommand => new RelayCommand(savedGame => MarkForDeletion(savedGame));

        private void MarkForDeletion(object obj)
        {
            var savedGame = obj as StoredLevel;
            savedGame.MarkedForDeletion = !savedGame.MarkedForDeletion;
        }
    }
}
