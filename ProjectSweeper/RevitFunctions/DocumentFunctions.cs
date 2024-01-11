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
            if (CanBeRemoved(doc, eId))
            {
                //using (Transaction transaction = new Transaction(doc, "Delete Element"))
                //{
                //    transaction.Start("Delete linestyles");

                //    try
                //    {
                //        ICollection<ElementId> collectionIds = eIds.ToList();
                //        doc.Delete(collectionIds);

                //        // Display a success message with "OK" button
                //        TaskDialog taskDialog = new TaskDialog("Revit");
                //        taskDialog.MainContent = "Click either [OK] to Commit, or [Cancel] to Roll back the transaction.";
                //        TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                //        taskDialog.CommonButtons = buttons;

                //        if (TaskDialogResult.Ok == taskDialog.Show())
                //        {

                //            if (TransactionStatus.Committed != transaction.Commit())
                //            {
                //                TaskDialog.Show("Failure", "Transaction could not be committed");
                //            }
                //        }
                //        else
                //        {
                //            transaction.RollBack();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        transaction.RollBack();

                //        // Display an error message with "Cancel" button
                //        TaskDialogResult result = TaskDialog.Show("Error", $"Error deleting elements: {ex.Message}", TaskDialogCommonButtons.Cancel);

                //        // Check if the user clicked "Cancel"
                //        if (result == TaskDialogResult.Cancel)
                //        {
                //            // Additional logic after the user clicks "Cancel"
                //        }
                //    }
                //}
            }
        }
    }
}
