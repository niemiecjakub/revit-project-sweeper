using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using ProjectSweeper.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSweeper.Commands
{
    public class RemoveElementsCommand : AsyncCommandBase
    {
        private readonly CleanerStore _cleanerStore;
        private readonly IEnumerable<LineStyleViewModel> _elementsToBeDeleted;

        public RemoveElementsCommand(CleanerStore cleanerStore, IEnumerable<LineStyleViewModel> elementsToBeDeleted)
        {
            _cleanerStore = cleanerStore;
            _elementsToBeDeleted = elementsToBeDeleted;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            Debug.WriteLine($"Got {_elementsToBeDeleted.Count()} to be deleted");
            foreach (LineStyleViewModel lineStyleViewModel in _elementsToBeDeleted)
            {
                LineStyle ls = _cleanerStore.LineStyles.Where(e => e.Id == lineStyleViewModel.Id).First();
                Debug.WriteLine($"COMMAND: ATTEMPTING TO DELETE   {ls.Name}");
                await _cleanerStore.DeleteLineStyle(ls);
            }
        }
    }
}
