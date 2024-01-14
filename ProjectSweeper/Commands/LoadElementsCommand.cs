using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using ProjectSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSweeper.Commands
{
    public class LoadElementsCommand : AsyncCommandBase
    {
        private readonly MasterManagerViewModel _masterManagerViewModel;
        private readonly CleanerStore _cleanerStore;
        private readonly ModelTypes _modelType;

        public LoadElementsCommand(MasterManagerViewModel masterManagerViewModel, CleanerStore cleanerStore, ModelTypes modelType)
        {
            _masterManagerViewModel = masterManagerViewModel;
            _cleanerStore = cleanerStore;
            _modelType = modelType;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _masterManagerViewModel.IsLoading = true;
            try
            {
                Debug.WriteLine("Loading elements...");
                await _cleanerStore.LoadElements(_modelType);
                IEnumerable<IElement> elements = _cleanerStore.GetElementCollection(_modelType);
                _masterManagerViewModel.UpdateElements(elements);
            } catch (Exception ex)
            {
                MessageBox.Show("Failed to load filled regions", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _masterManagerViewModel.IsLoading = false;
            }
        }
    }
}
