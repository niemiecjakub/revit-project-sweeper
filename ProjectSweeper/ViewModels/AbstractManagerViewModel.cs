using Autodesk.Revit.DB;
using ProjectSweeper.Commands;
using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace ProjectSweeper.ViewModels
{
    public abstract class ManagerViewModelBase<Model, ViewModel> : ViewModelBase
        where Model : IElement
        where ViewModel : IElementViewModel
    {
        public string Name { get; }
        protected ObservableCollection<ViewModel> _elements;
        public ObservableCollection<ViewModel> Elements
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

        public ManagerViewModelBase()
        {
            _elements = new ObservableCollection<ViewModel>();
            
        }

        public void OnItemDeleted(IEnumerable<IElement> itemsToBeLeft)
        {
            ObservableCollection<ViewModel> itemsToBeLeftCollection = new ObservableCollection<ViewModel>();
            foreach (Model item in itemsToBeLeft.ToList())
            {
                ViewModel itemViewModel = _elements.FirstOrDefault(vm => vm.Id == item.Id);
                if (itemViewModel != null)
                {
                    itemsToBeLeftCollection.Add(itemViewModel);
                }
            }
            _elements = itemsToBeLeftCollection;
            OnPropertyChanged(nameof(Elements));
        }
    }
}
