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
    public class LoadFillPatternsCommand : AsyncCommandBase
    {
        private readonly FillPatternManagerViewModel _fillPatternManagerViewModel;
        private readonly CleanerStore _cleanerStore;

        public LoadFillPatternsCommand(FillPatternManagerViewModel fillPatternManagerViewModel, CleanerStore cleanerStore)
        {
            _fillPatternManagerViewModel = fillPatternManagerViewModel;
            _cleanerStore = cleanerStore;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _fillPatternManagerViewModel.IsLoading = true;
            try
            {
                Debug.WriteLine("Load Fill Patetrns command");
                await _cleanerStore.LoadFillPatterns();
                _fillPatternManagerViewModel.UpdateElements(_cleanerStore.FillPatterns);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load Fill Patetrns", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

                _fillPatternManagerViewModel.IsLoading = false;
            }
        }
    }
}
