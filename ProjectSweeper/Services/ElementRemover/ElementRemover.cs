
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectSweeper.Services.ElementRemover
{
    public class ElementRemover : IRemoveElement
    {
        private readonly Document _doc;

        public ElementRemover(Document doc)
        {
            _doc = doc;
        }
        public async Task Remove(ElementId eId)
        {
            Debug.WriteLine("REMOVER");
            using (Transaction transaction = new Transaction(_doc, "YourUniqueTransactionName"))
            {
                if (transaction.Start() == TransactionStatus.Started)
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
