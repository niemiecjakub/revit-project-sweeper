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
    public class LoadLinePatternsCommand : AsyncCommandBase
    {
        private readonly LinePatternManagerViewModel _linePatternManagerViewModel;
        private readonly CleanerStore _cleanerStore;

        public LoadLinePatternsCommand(LinePatternManagerViewModel linePatternManagerViewModel, CleanerStore cleanerStore)
        {
            _linePatternManagerViewModel = linePatternManagerViewModel;
            _cleanerStore = cleanerStore;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _linePatternManagerViewModel.IsLoading = true;
            try
            {
                Debug.WriteLine("Load line PATTERN command");
                await _cleanerStore.Load();
                _linePatternManagerViewModel.UpdateLinePatterns(_cleanerStore.LinePatterns);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load line patterns", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

                _linePatternManagerViewModel.IsLoading = false;
            }
        }
    }
}
