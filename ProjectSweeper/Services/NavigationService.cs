using ProjectSweeper.Stores;
using ProjectSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services
{
    public class NavigationService<TViewModel> where TViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _crateViewModel;

        public NavigationService(NavigationStore navigationStore, Func<TViewModel> crateViewModel)
        {
            _navigationStore = navigationStore;
            _crateViewModel = crateViewModel;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _crateViewModel();

        }
    }
}
