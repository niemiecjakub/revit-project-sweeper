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
        public ICommand FilterNavigationCommand { get; }
        public ICommand ViewTemplateNavigationCommand { get; }
        public ICommand ViewportNavigationCommand { get; }
        public ICommand TextNavigationCommand { get; }
        public ICommand ObjectStyleNavigationCommand { get; }
        public NavigationBarViewModel(INavigationService lineStyleNavigation, 
            INavigationService linePatternNavigation, 
            INavigationService filledRegionNavigation, 
            INavigationService fillPatternNavigation,
            INavigationService filterNavigation,
            INavigationService viewTemplateNavigation,
            INavigationService viewportNavigation,
            INavigationService textNavigation,
            INavigationService objectStyleNavigation)
        
        {
            lineStyleNavigaitonCommand = new NavigateCommand(lineStyleNavigation);
            linePatternNavigationCommand = new NavigateCommand(linePatternNavigation);
            FilledRegionNavigationComand = new NavigateCommand(filledRegionNavigation);
            FillPatternNavigationCommand = new NavigateCommand(fillPatternNavigation);

            FilterNavigationCommand = new NavigateCommand(filterNavigation);
            ViewTemplateNavigationCommand = new NavigateCommand(viewTemplateNavigation);
            ViewportNavigationCommand = new NavigateCommand(viewportNavigation);

            TextNavigationCommand = new NavigateCommand(textNavigation);
            ObjectStyleNavigationCommand = new NavigateCommand(objectStyleNavigation);
        }
    }
}
