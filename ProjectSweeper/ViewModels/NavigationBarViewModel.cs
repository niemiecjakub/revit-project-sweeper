using ProjectSweeper.Commands;
using ProjectSweeper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectSweeper.ViewModels
{
    public class NavigationBarViewModel : ViewModelBase
    {
        public ICommand NavigateLineStyleCommand { get; }
        public ICommand NavigateLinePatternCommand { get; }


        public NavigationBarViewModel(INavigationService lineStyleNavigationService, INavigationService linePatternNavigationService)
        {
            NavigateLineStyleCommand = new NavigateCommand(lineStyleNavigationService);
            NavigateLinePatternCommand = new NavigateCommand(linePatternNavigationService);
        }
    }
}
