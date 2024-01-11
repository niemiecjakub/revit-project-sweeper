using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjectSweeper.Models;
using ProjectSweeper.Stores;
using ProjectSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Commands
{
    public class DeleteElementsCommand
    {
        private readonly CleanerStore _cleanerStore;
        private readonly Document _doc;

        public DeleteElementsCommand(CleanerStore cleanerStore, Document doc)
        {
            _cleanerStore = cleanerStore;
            _doc = doc;
        }

        public void Execute(IEnumerable<LineStyleViewModel> parameter)
        {
            Debug.WriteLine($"Got {parameter.Count()} to be deleted");
            using (Transaction transaction = new Transaction(_doc))
            {
                if (transaction.Start("Deleting element") == TransactionStatus.Started)
                {
                    TaskDialog taskDialog = new TaskDialog("Revit");
                    taskDialog.MainContent = "Click either [OK] to Commit, or [Cancel] to Roll back the transaction.";
                    TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                    taskDialog.CommonButtons = buttons;

                    if (TaskDialogResult.Ok == taskDialog.Show())
                    {

                        if (TransactionStatus.Committed != transaction.Commit())
                        {
                            TaskDialog.Show("Failure", "Transaction could not be committed");
                        }
                    }
                    else
                    {
                        transaction.RollBack();
                    }
                }
            }
        }
    }
}
