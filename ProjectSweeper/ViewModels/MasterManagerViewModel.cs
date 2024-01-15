using ProjectSweeper.Commands;
using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace ProjectSweeper.ViewModels
{
    public class MasterManagerViewModel : ViewModelBase
    {
        public string Name => "Master manager view model";
        private readonly CleanerStore _cleanerStore;
        private readonly ModelTypes _modelType;

        private ObservableCollection<IElementViewModel> _elements;
        public ObservableCollection<IElementViewModel> Elements
        {
            get { return _elements; }
            set
            {
                _elements = value;
                OnPropertyChanged(nameof(Elements));
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
        public ICommand RemoveUnusedElementsCommand { get; }
        public ICommand LoadElementsCommand { get; }
        public MasterManagerViewModel(CleanerStore cleanerStore, ModelTypes modelType)
        {
            _cleanerStore = cleanerStore;
            _modelType = modelType;
            _elements = new ObservableCollection<IElementViewModel>();

            LoadElementsCommand = new LoadElementsCommand(this, cleanerStore, _modelType);
            RemoveUnusedElementsCommand = new RemoveElementsCommand(cleanerStore, modelType);
            _cleanerStore.ElementDeleted += OnElementDeleted;
        }
        public override void Dispose()
        {
            _cleanerStore.ElementDeleted -= OnElementDeleted;
            base.Dispose();
        }

        public void OnElementDeleted(IEnumerable<IElement> elementsToBeLeft)
        {
            ObservableCollection<IElementViewModel> elements = new ObservableCollection<IElementViewModel>();
            foreach (IElement element in elementsToBeLeft.ToList())
            {
                IElementViewModel itemViewModel = _elements.FirstOrDefault(vm => vm.Id == element.Id);
                if (itemViewModel != null)
                {
                    elements.Add(itemViewModel);
                }
            }
            _elements = elements;
            OnPropertyChanged(nameof(Elements));
        }

        public static MasterManagerViewModel LoadViewModel(CleanerStore cleanerStore, ModelTypes modelType)
        {
            MasterManagerViewModel viewModel = new MasterManagerViewModel(cleanerStore, modelType);
            Console.WriteLine($"INITIALIZED WITH {modelType}");
            viewModel.LoadElementsCommand.Execute(null);
            return viewModel;
        }

        public void UpdateElements(IEnumerable<IElement> elements)
        {
            _elements.Clear();
            ISet<IElement> unusedElements = elements.Where(l => !l.IsUsed).ToHashSet();
            ISet<IElement> unusedElementsToBeRemoved = unusedElements.Where(l => l.CanBeRemoved).ToHashSet();
            Debug.WriteLine($"{elements.Count()} - Items found - {unusedElements.Count} Unused - out of which {unusedElementsToBeRemoved.Count} can be removed");

            foreach (IElement element in elements)
            {
                IElementViewModel elementViewModel = new ElementViewModel(element);
                _elements.Add(elementViewModel);
            }
        }
    }
}
