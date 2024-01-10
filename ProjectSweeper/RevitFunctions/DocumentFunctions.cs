using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSweeper.RevitFunctions
{
    public class DocumentFunctions
    {
        public static bool CanBeRemoved(Document doc, ElementId eId)
        {
            Element e = doc.GetElement(eId);
            return DocumentValidation.CanDeleteElement(doc, eId);
        }

        public static void Remove(Document doc, ElementId eId)
        {
            Debug.WriteLine("REMOVE FUNCTION");
            //if (CanBeRemoved(doc, eId))
            //{
            //Debug.WriteLine("THIS CAN BE DELETED");
            //using (Transaction transaction = new Transaction(doc, "Delete Element"))
            //{
            //    transaction.Start();

            //    try
            //    {
            //        // Use the Delete method to delete the element
            //        doc.Delete(eId);

            //        // Commit the transaction
            //        transaction.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        // Handle exceptions if necessary
            //        transaction.RollBack();
            //        TaskDialog.Show("Error", ex.Message);
            //    }
            //}
            //}

            using (Transaction transaction = new Transaction(doc))
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
