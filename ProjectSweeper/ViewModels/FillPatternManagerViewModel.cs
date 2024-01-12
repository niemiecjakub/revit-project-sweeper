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
    public class FillPatternManagerViewModel : ManagerViewModelBase<FillPatternModel, FillPatternViewModel>
    {
        public string Name => "Fill Pattern manager";
        protected readonly CleanerStore _cleanerStore;

        public ICommand LoadFillPatternsCommand { get; }
        public ICommand RemoveFillPatternsCommand { get; }

        public FillPatternManagerViewModel(CleanerStore cleanerStore) : base()
        {
            _cleanerStore = cleanerStore;

            LoadFillPatternsCommand = new LoadFillPatternsCommand(this, _cleanerStore);
            RemoveFillPatternsCommand = new RemoveElementsCommand(cleanerStore, _cleanerStore.FillPatterns);
            _cleanerStore.FillPatternDeleted += OnItemDeleted;
        }

        public override void Dispose()
        {
            _cleanerStore.FillPatternDeleted -= OnItemDeleted;
            base.Dispose();
        }

        public static FillPatternManagerViewModel LoadViewModel(CleanerStore cleanerStore)
        {
            FillPatternManagerViewModel viewModel = new FillPatternManagerViewModel(cleanerStore);
            viewModel.LoadFillPatternsCommand.Execute(null);
            return viewModel;
        }


        public override void UpdateElements(IEnumerable<IElement> fillPatterns)
        {
            _elements.Clear();
            ISet<IElement> unusedfillPatterns = fillPatterns.Where(l => !l.IsUsed).ToHashSet();
            ISet<IElement> unusedfillPatternsToBeRemoved = unusedfillPatterns.Where(l => l.CanBeRemoved).ToHashSet();
            Debug.WriteLine($"{fillPatterns.Count()} -  Line Patterns found - {unusedfillPatterns.Count} Unused - out of which {unusedfillPatternsToBeRemoved.Count} can be removed");

            foreach (IElement fillPattern in fillPatterns)
            {
                FillPatternModel fillPatternModel = new FillPatternModel(fillPattern.Id, fillPattern.Name);
                FillPatternViewModel fillPatternViewModel = new FillPatternViewModel(fillPatternModel);
                _elements.Add(fillPatternViewModel);
            }
        }
    }
}
