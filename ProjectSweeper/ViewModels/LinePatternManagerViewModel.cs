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
            RemoveLinePatternsCommand = new GeneralRemoveCommand(cleanerStore, _cleanerStore.LinePatterns);

            
            _cleanerStore.LinePatternDeleted += OnLinePatternsDeleted;
        }

        public override void Dispose()
        {
            _cleanerStore.LinePatternDeleted -= OnLinePatternsDeleted;
            base.Dispose();
        }

        private void OnLinePatternsDeleted(IEnumerable<IElement> linePatternsToBeLeft)
        {
            ObservableCollection<LinePatternViewModel> lpToBeLeft = new ObservableCollection<LinePatternViewModel>();
            foreach (IElement style in linePatternsToBeLeft.ToList())
            {
                LinePatternViewModel linePatternViewModel = _linePatterns.FirstOrDefault(lp => lp.Id == style.Id);
                if (linePatternViewModel != null)
                {
                    lpToBeLeft.Add(linePatternViewModel);
                }
            }
            _linePatterns = lpToBeLeft;
            OnPropertyChanged(nameof(LinePatterns));
        }

        public static LinePatternManagerViewModel LoadViewModel(CleanerStore cleanerStore)
        {
            LinePatternManagerViewModel viewModel = new LinePatternManagerViewModel(cleanerStore);
            viewModel.LoadLinePatternsCommand.Execute(null);
            return viewModel;
        }

        public void UpdateLinePatterns(IEnumerable<IElement> linePatterns)
        {
            _linePatterns.Clear();
            ISet<IElement> unusedLinePatterns = linePatterns.Where(l => !l.IsUsed).ToHashSet();
            ISet<IElement> unusedLinePatternsToBeRemoved = unusedLinePatterns.Where(l => l.CanBeRemoved).ToHashSet();
            Debug.WriteLine($"{linePatterns.Count()} -  Line Patterns found - {unusedLinePatterns.Count} Unused - out of which {unusedLinePatternsToBeRemoved.Count} can be removed");

            foreach (IElement linePattern in linePatterns)
            {
                LinePatternModel linePatternModel = new LinePatternModel(linePattern.Name, linePattern.Id);
                LinePatternViewModel linePatternViewModel = new LinePatternViewModel(linePatternModel);
                _linePatterns.Add(linePatternViewModel);
            }
        }
    }
}
