using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using ProjectSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            IEnumerable<IElement> elementsToBeDeleted = _elementsToBeDeleted.Where(x => !x.IsUsed && x.CanBeRemoved);
            Debug.WriteLine($"Got {elementsToBeDeleted.Count()} to be deleted");
            _cleanerStore.DeleteElements(elementsToBeDeleted);
            OnCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return _elementsToBeDeleted.Where(x => !x.IsUsed && x.CanBeRemoved).Count() > 0;
        }
    }
}
