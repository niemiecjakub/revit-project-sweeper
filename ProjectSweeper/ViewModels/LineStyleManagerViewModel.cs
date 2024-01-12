using ProjectSweeper.Commands;
using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using ProjectSweeper.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace ProjectSweeper.ViewModels
{
    public class LineStyleManagerViewModel : ManagerViewModelBase<LineStyleModel, LineStyleViewModel>
    {
        public string Name => "Line style manager";
        protected readonly CleanerStore _cleanerStore;

        public ICommand LoadLineStylesCommand { get; }
        public ICommand RemoveLineStylesCommand { get; }

        public LineStyleManagerViewModel(CleanerStore cleanerStore) : base()
        {
            _cleanerStore = cleanerStore;

            LoadLineStylesCommand = new LoadLineStylesCommand(this, _cleanerStore);
            RemoveLineStylesCommand = new RemoveElementsCommand(cleanerStore, _cleanerStore.LineStyles);

            _cleanerStore.LineStyleDeleted += OnItemDeleted;
        }

        public override void Dispose()
        {
            _cleanerStore.LineStyleDeleted -= OnItemDeleted;
            base.Dispose();
        }

        public static LineStyleManagerViewModel LoadViewModel(CleanerStore cleanerStore)
        {
            LineStyleManagerViewModel viewModel = new LineStyleManagerViewModel(cleanerStore);
            viewModel.LoadLineStylesCommand.Execute(null);
            return viewModel;
        }

        public override void UpdateElements(IEnumerable<IElement> lineStyles)
        {
            _elements.Clear();
            ISet<IElement> unusedItems = lineStyles.Where(l => !l.IsUsed).ToHashSet();
            ISet<IElement> unusedItemsToBeRemoved = unusedItems.Where(l => l.CanBeRemoved).ToHashSet();
            Debug.WriteLine($"{lineStyles.Count()} - Items found - {unusedItems.Count} Unused - out of which {unusedItemsToBeRemoved.Count} can be removed");

            foreach (IElement lineStyle in lineStyles)
            {
                LineStyleViewModel lineStyleViewModel = new LineStyleViewModel(lineStyle as LineStyleModel);
                _elements.Add(lineStyleViewModel);
            }
        }
    }
}

