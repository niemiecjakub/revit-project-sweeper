using ProjectSweeper.Commands;
using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectSweeper.ViewModels
{
    public class LinePatternManagerViewModel : ViewModelBase
    {
        public string Name => "Line pattern manager";
        private readonly CleanerStore _cleanerStore;

        private ObservableCollection<LinePatternViewModel> _linePatterns;
        public ObservableCollection<LinePatternViewModel> LinePatterns => _linePatterns;

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

        public ICommand LoadLinePatternsCommand { get; }
        public ICommand RemoveLinePatternsCommand { get; }
        public LinePatternManagerViewModel(CleanerStore cleanerStore)
        {
            Debug.WriteLine("new line style");

            _cleanerStore = cleanerStore;
            _linePatterns = new ObservableCollection<LinePatternViewModel>();
            LoadLinePatternsCommand = new LoadLinePatternsCommand(this, cleanerStore);

            //IEnumerable<LineStyleViewModel> lineStyleViewModelsToBeDeleted = _lineStyles.Where(ls => !ls.IsUsed && ls.CanBeRemoved);
            //RemoveElementCommand = new RemoveElementsCommand(cleanerStore, lineStyleViewModelsToBeDeleted);
            RemoveLinePatternsCommand = new RemoveLinePatternsCommand(cleanerStore);

            //_cleanerStore.LineStyleDeleted += OnLineStyleDeleted;
            _cleanerStore.LinePatternDeleted += OnLinePatternsDeleted;
        }

        public override void Dispose()
        {
            _cleanerStore.LinePatternDeleted -= OnLinePatternsDeleted;
            base.Dispose();
        }

        private void OnLinePatternsDeleted(IEnumerable<LinePatternModel> linePatternsToBeLeft)
        {
            ObservableCollection<LinePatternViewModel> lpToBeLeft = new ObservableCollection<LinePatternViewModel>();
            foreach (LinePatternModel style in linePatternsToBeLeft.ToList())
            {
                // Use ToList() to create a copy of the collection, avoiding modification during iteration
                LinePatternViewModel linePatternViewModel = _linePatterns.FirstOrDefault(lp => lp.Id == style.Id);
                if (linePatternViewModel != null)
                {
                    lpToBeLeft.Add(linePatternViewModel);
                }
            }
            _linePatterns = lpToBeLeft;
            OnPropertyChanged(nameof(LinePatterns)); // Notify about the change if needed
        }

        public static LinePatternManagerViewModel LoadViewModel(CleanerStore cleanerStore)
        {
            LinePatternManagerViewModel viewModel = new LinePatternManagerViewModel(cleanerStore);
            viewModel.LoadLinePatternsCommand.Execute(null);
            return viewModel;
        }

        public void UpdateLinePatterns(IEnumerable<LinePatternModel> linePatterns)
        {
            _linePatterns.Clear();
            ISet<LinePatternModel> unusedLinePatterns = linePatterns.Where(l => !l.IsUsed).ToHashSet();
            ISet<LinePatternModel> unusedLinePatternsToBeRemoved = unusedLinePatterns.Where(l => l.CanBeRemoved).ToHashSet();
            Debug.WriteLine($"{linePatterns.Count()} -  Line Patterns found - {unusedLinePatterns.Count} Unused - out of which {unusedLinePatternsToBeRemoved.Count} can be removed");

            foreach (LinePatternModel linePattern in linePatterns)
            {
                LinePatternViewModel linePatternViewModel = new LinePatternViewModel(linePattern);
                _linePatterns.Add(linePatternViewModel);
            }
        }
    }
}
