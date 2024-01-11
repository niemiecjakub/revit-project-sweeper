using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Commands
{
    public class RemoveLinePatternsCommand : AsyncCommandBase
    {
        private readonly CleanerStore _cleanerStore;
        public RemoveLinePatternsCommand(CleanerStore cleanerStore)
        {
            _cleanerStore = cleanerStore;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            IEnumerable<LinePatternModel> elementsToBeDeleted = _cleanerStore.LinePatterns.Where(x => !x.IsUsed && x.CanBeRemoved);
            Debug.WriteLine($"Got {elementsToBeDeleted.Count()} to be deleted");
            await _cleanerStore.DeleteLinePattern(elementsToBeDeleted);
        }
    }
}
