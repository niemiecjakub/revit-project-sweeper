using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
    public class RemoveElementsCommand : CommandBase
    {
        private readonly CleanerStore _cleanerStore;
        private readonly IEnumerable<IElement> _elementsToBeDeleted;

        public RemoveElementsCommand(CleanerStore cleanerStore, IEnumerable<IElement> elementsToBeDeleted)
        {
            _cleanerStore = cleanerStore;
            _elementsToBeDeleted = elementsToBeDeleted;
        }
        public override void Execute(object parameter)
        {
            Debug.WriteLine($"Got {_elementsToBeDeleted.Count()} to be deleted");
        }
    }
}
