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
    public class LoadLineStylesCommand : AsyncCommandBase
    {
        private readonly LineStyleManagerViewModel _lineStyleManagerViewModel;
        private readonly CleanerStore _cleanerStore;

        public LoadLineStylesCommand(LineStyleManagerViewModel lineStyleManagerViewModel, CleanerStore cleanerStore)
        {
            _lineStyleManagerViewModel = lineStyleManagerViewModel;
            _cleanerStore = cleanerStore;
        }
        public override async Task ExecuteAsync(object parameter)
        {
            _lineStyleManagerViewModel.IsLoading = true;
            try
            {
                Debug.WriteLine("Load line style command");
                await _cleanerStore.Load();
                _lineStyleManagerViewModel.UpdateLineStyles(_cleanerStore.LineStyles);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load games", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

                _lineStyleManagerViewModel.IsLoading = false;
            }
        }
    }
}
