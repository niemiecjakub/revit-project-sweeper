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
        public ICommand NavigateFilledRegionComamnd { get; }
        public ICommand NavigateFillPatternCommand { get; }


        public NavigationBarViewModel(INavigationService lineStyleNavigationService, INavigationService linePatternNavigationService, INavigationService filledRegionNavigationService, INavigationService fillPatternNavigationService)
        {
            NavigateLineStyleCommand = new NavigateCommand(lineStyleNavigationService);
            NavigateLinePatternCommand = new NavigateCommand(linePatternNavigationService);
            NavigateFilledRegionComamnd = new NavigateCommand(filledRegionNavigationService);
            NavigateFillPatternCommand = new NavigateCommand(fillPatternNavigationService);
        }
    }
}
