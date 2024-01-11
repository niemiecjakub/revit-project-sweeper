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
    public class RemoveLineStylesCommand : AsyncCommandBase
    {
        private readonly CleanerStore _cleanerStore;

        public RemoveLineStylesCommand(CleanerStore cleanerStore)
        {
            _cleanerStore = cleanerStore;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            IEnumerable <LineStyleModel> elementsToBeDeleted = _cleanerStore.LineStyles.Where(x => !x.IsUsed && x.CanBeRemoved);
            Debug.WriteLine($"Got {elementsToBeDeleted.Count()} to be deleted");
            await _cleanerStore.DeleteLineStyle(elementsToBeDeleted);
        }
    }
}
