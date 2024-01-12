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
    public class LoadFilledRegionsCommand : AsyncCommandBase
    {
        private readonly FilledRegionManagerViewModel _filledRegionManagerViewModel;
        private readonly CleanerStore _cleanerStore;

        public LoadFilledRegionsCommand(FilledRegionManagerViewModel filledRegionManagerViewModel, CleanerStore cleanerStore)
        {
            _filledRegionManagerViewModel = filledRegionManagerViewModel;
            _cleanerStore = cleanerStore;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            _filledRegionManagerViewModel.IsLoading = true;
            try
            {
                Debug.WriteLine("Load FILLED REGIONS command");
                await _cleanerStore.LoadFilledRegions();
                _filledRegionManagerViewModel.UpdateElements(_cleanerStore.FiilledRegions);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load filled regions", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

                _filledRegionManagerViewModel.IsLoading = false;
            }
        }
    }
}
