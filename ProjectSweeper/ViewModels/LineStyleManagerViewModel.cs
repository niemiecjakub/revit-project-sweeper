using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjectSweeper.Commands;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using ProjectSweeper.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectSweeper.ViewModels
{
    public class LineStyleManagerViewModel : ViewModelBase
    {
        public string Name => "Line style manager";
        private readonly CleanerStore _cleanerStore;

        private ObservableCollection<LineStyleViewModel> _lineStyles;
        public ObservableCollection<LineStyleViewModel> LineStyles => _lineStyles;

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

        public ICommand LoadLineStylesCommand { get; }
        public ICommand RemoveLineStylesCommand { get; }
        public LineStyleManagerViewModel(CleanerStore cleanerStore)
        {
            Debug.WriteLine("new line style");

            _lineStyles = new ObservableCollection<LineStyleViewModel>();
            _cleanerStore = cleanerStore;
            LoadLineStylesCommand = new LoadLineStylesCommand(this, cleanerStore);

            //IEnumerable<LineStyleViewModel> lineStyleViewModelsToBeDeleted = _lineStyles.Where(ls => !ls.IsUsed && ls.CanBeRemoved);
            //RemoveElementCommand = new RemoveElementsCommand(cleanerStore, lineStyleViewModelsToBeDeleted);
            RemoveLineStylesCommand = new RemoveLineStylesCommand(cleanerStore);

            _cleanerStore.LineStyleDeleted += OnLineStyleDeleted;
        }


        public override void Dispose()
        {
            _cleanerStore.LineStyleDeleted -= OnLineStyleDeleted;
            base.Dispose();
        }

        private void OnLineStyleDeleted(IEnumerable<LineStyleModel> lineStylesToBeLeft)
        {
            ObservableCollection<LineStyleViewModel> lsToBeLeft = new ObservableCollection<LineStyleViewModel>();
            foreach (LineStyleModel style in lineStylesToBeLeft.ToList())
            {
                // Use ToList() to create a copy of the collection, avoiding modification during iteration
                LineStyleViewModel lineStyleViewModel = _lineStyles.FirstOrDefault(ls => ls.Id == style.Id);
                if (lineStyleViewModel != null)
                {
                    lsToBeLeft.Add(lineStyleViewModel);
                }
            }
            _lineStyles = lsToBeLeft;
            OnPropertyChanged(nameof(LineStyles)); // Notify about the change if needed
        }



        public static LineStyleManagerViewModel LoadViewModel(CleanerStore cleanerStore)
        {
            LineStyleManagerViewModel viewModel = new LineStyleManagerViewModel(cleanerStore);
            viewModel.LoadLineStylesCommand.Execute(null);
            return viewModel;
        }

        public void UpdateLineStyles(IEnumerable<LineStyleModel> lineStyles)
        {
            _lineStyles.Clear();
            ISet<LineStyleModel> unusedLineStyles = lineStyles.Where(l => !l.IsUsed).ToHashSet();
            ISet<LineStyleModel> unusedLineStylesToBeRemoved = unusedLineStyles.Where(l => l.CanBeRemoved).ToHashSet();
            Debug.WriteLine($"{lineStyles.Count()} -  LineStyles found - {unusedLineStyles.Count} Unused - out of which {unusedLineStylesToBeRemoved.Count} can be removed");

            foreach (LineStyleModel lineStyle in lineStyles)
            {
                LineStyleViewModel lineStyleViewModel = new LineStyleViewModel(lineStyle);
                _lineStyles.Add(lineStyleViewModel);
            }
        }
    }
}
