using Autodesk.Revit.DB;
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
using System.Xml.Linq;

namespace ProjectSweeper.ViewModels
{
    public class LinePatternManagerViewModel : ManagerViewModelBase<LinePatternModel, LinePatternViewModel>
    {
        public string Name => "Line pattern manager";
        protected readonly CleanerStore _cleanerStore;

        public ICommand LoadLinePatternsCommand { get; }
        public ICommand RemoveLinePatternsCommand { get; }

        public LinePatternManagerViewModel(CleanerStore cleanerStore) : base()
        {
            _cleanerStore = cleanerStore;

            LoadLinePatternsCommand = new LoadLinePatternsCommand(this, _cleanerStore);
            RemoveLinePatternsCommand = new RemoveElementsCommand(cleanerStore, _cleanerStore.LinePatterns);
            _cleanerStore.LinePatternDeleted += OnItemDeleted;
        }

        public override void Dispose()
        {
            _cleanerStore.LinePatternDeleted -= OnItemDeleted;
            base.Dispose();
        }

        public static LinePatternManagerViewModel LoadViewModel(CleanerStore cleanerStore)
        {
            LinePatternManagerViewModel viewModel = new LinePatternManagerViewModel(cleanerStore);
            viewModel.LoadLinePatternsCommand.Execute(null);
            return viewModel;
        }

        public override void UpdateElements(IEnumerable<IElement> linePatterns)
        {
            _elements.Clear();
            ISet<IElement> unusedLinePatterns = linePatterns.Where(l => !l.IsUsed).ToHashSet();
            ISet<IElement> unusedLinePatternsToBeRemoved = unusedLinePatterns.Where(l => l.CanBeRemoved).ToHashSet();
            Debug.WriteLine($"{linePatterns.Count()} -  Line Patterns found - {unusedLinePatterns.Count} Unused - out of which {unusedLinePatternsToBeRemoved.Count} can be removed");

            foreach (IElement linePattern in linePatterns)
            {
                LinePatternModel linePatternModel = new LinePatternModel(linePattern.Name, linePattern.Id);
                LinePatternViewModel linePatternViewModel = new LinePatternViewModel(linePatternModel);
                _elements.Add(linePatternViewModel);
            }
        }
    }
}
