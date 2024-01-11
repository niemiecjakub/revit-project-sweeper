
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
    public class ElementRemover : IElementRemover
    {
        public async Task Remove(ElementId eId)
        {
            Debug.WriteLine("REMOVER");
            try
            {
                Debug.WriteLine("INSIDE");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
