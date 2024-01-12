using ProjectSweeper.Commands;
using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectSweeper.ViewModels
{
    public class FilledRegionManagerViewModel : ManagerViewModelBase<FilledRegionModel, FilledRegionViewModel>
    {
        public string Name => "Filled Region manager";
        protected readonly CleanerStore _cleanerStore;

        public ICommand LoadFilledRegionsCommand { get; }
        public ICommand RemoveFilledRegionsCommand { get; }

        public FilledRegionManagerViewModel(CleanerStore cleanerStore) : base()
        {
            _cleanerStore = cleanerStore;

            LoadFilledRegionsCommand = new LoadFilledRegionsCommand(this, _cleanerStore);
            RemoveFilledRegionsCommand = new RemoveElementsCommand(cleanerStore, _cleanerStore.FiilledRegions);
            _cleanerStore.FilledRegionDeleted += OnItemDeleted;
        }

        public override void Dispose()
        {
            _cleanerStore.FilledRegionDeleted -= OnItemDeleted;
            base.Dispose();
        }

        public static FilledRegionManagerViewModel LoadViewModel(CleanerStore cleanerStore)
        {
            FilledRegionManagerViewModel viewModel = new FilledRegionManagerViewModel(cleanerStore);
            viewModel.LoadFilledRegionsCommand.Execute(null);
            return viewModel;
        }

        public override void UpdateElements(IEnumerable<IElement> filledRegions)
        {
            _elements.Clear();
            ISet<IElement> unusedfilledRegions = filledRegions.Where(l => !l.IsUsed).ToHashSet();
            ISet<IElement> unusedfilledRegionsToBeRemoved = unusedfilledRegions.Where(l => l.CanBeRemoved).ToHashSet();
            Debug.WriteLine($"{filledRegions.Count()} -  Filled Regions found - {unusedfilledRegions.Count} Unused - out of which {unusedfilledRegionsToBeRemoved.Count} can be removed");

            foreach (IElement filledRegion in filledRegions)
            {
                FilledRegionModel filledRegionModel = new FilledRegionModel(filledRegion.Id, filledRegion.Name);
                FilledRegionViewModel filledRegionViewModel = new FilledRegionViewModel(filledRegionModel);
                _elements.Add(filledRegionViewModel);
            }
        }
    }
}
