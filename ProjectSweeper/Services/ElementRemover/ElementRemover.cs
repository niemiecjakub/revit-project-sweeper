
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectSweeper.Services.ElementRemover
{
    public class ElementRemover : IElementRemover
    {
        private readonly Document _doc;

        public ElementRemover(Document doc)
        {
            _doc = doc;
        }
        public async Task Remove(IEnumerable<ElementId> eIds)
        {
            Debug.WriteLine($"REMVOER: inside remover");
            try
            {
                using (Transaction transaction = new Transaction(_doc, "Delete Element"))
                {
                    transaction.Start("Delete all unused linestyles");

                    try
                    {
                        // Use the Delete method to delete the element
                        ICollection<ElementId> collectionIds = eIds.ToList();
                        _doc.Delete(collectionIds);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions if necessary
                        transaction.RollBack();
                        TaskDialog.Show("Error", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
