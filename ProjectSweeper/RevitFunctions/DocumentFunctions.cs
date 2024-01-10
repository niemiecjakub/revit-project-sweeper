using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSweeper.RevitFunctions
{
    public class DocumentFunctions
    {
        public static bool CanBeRemoved(Document doc, IElement element)
        {
            return DocumentValidation.CanDeleteElement(doc, element.Id);
        }

        public static void Remove(Document doc, IElement element)
        {
            try
            {
                doc.Delete(element.Id);

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
