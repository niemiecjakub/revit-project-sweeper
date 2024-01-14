using Microsoft.Extensions.DependencyInjection;
using ProjectSweeper.Commands;
using ProjectSweeper.Services;
using ProjectSweeper.Stores;
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
        public ICommand lineStyleNavigaitonCommand { get; }
        public ICommand linePatternNavigationCommand { get; }
        public ICommand FilledRegionNavigationComand { get; }
        public ICommand FillPatternNavigationCommand { get; }

        public NavigationBarViewModel(INavigationService lineStyleNavigation, INavigationService linePatternNavigation, INavigationService filledRegionNavigation, INavigationService fillPatternNavigation)
        {
            lineStyleNavigaitonCommand = new NavigateCommand(lineStyleNavigation);
            linePatternNavigationCommand = new NavigateCommand(linePatternNavigation);
            FilledRegionNavigationComand = new NavigateCommand(filledRegionNavigation);
            FillPatternNavigationCommand = new NavigateCommand(fillPatternNavigation);
        }
    }
}
