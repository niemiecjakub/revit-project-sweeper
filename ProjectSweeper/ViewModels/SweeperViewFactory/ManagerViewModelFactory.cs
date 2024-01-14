using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels.SweeperViewFactory
{
    public class ManagerViewModelFactory : IMasterManagerViewModelFactory
    {
        public MasterManagerViewModel CreateViewModel(CleanerStore cleanerStore, ModelTypes modelType)
        {
            return new MasterManagerViewModel(cleanerStore, modelType);
        }
    }
}
