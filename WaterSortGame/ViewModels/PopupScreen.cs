using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSortGame.ViewModels
{
    internal class PopupScreenBase : ViewModelBase
    {
        public MainWindowVM MainWindowVM { get; set; }
        public PopupScreenBase(object viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
        }
    }
    internal class HelpVM : PopupScreenBase
    {
        public HelpVM(object viewModel) : base(viewModel) { }
    }
    internal class LevelCompleteVM : PopupScreenBase
    {
        public LevelCompleteVM(object viewModel) : base(viewModel) { }
    }
    class LoadLevelVM : PopupScreenBase
    {
        public LoadLevelVM(object viewModel) : base(viewModel)
        {
            MainWindowVM = (MainWindowVM)viewModel;
            MainWindowVM.LoadLevelScreen();
        }
    }
    internal class NewLevelVM : PopupScreenBase
    {
        public NewLevelVM(object viewModel) : base(viewModel) { }
    }
    internal class RestartLevelVM : PopupScreenBase
    {
        public RestartLevelVM(object viewModel) : base(viewModel) { }
    }
    class GameSavedVM : PopupScreenBase
    {
        public GameSavedVM(object viewModel) : base(viewModel) { }
    }
}
