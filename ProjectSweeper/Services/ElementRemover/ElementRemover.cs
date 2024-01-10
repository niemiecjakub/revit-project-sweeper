
using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.ElementRemover
{
    public class ElementRemover : IRemoveElement
    {
        private readonly Document _doc;

        public ElementRemover(Document doc)
        {
            _doc = doc;
        }
        public void Remove(IEnumerable<IElement> elementsToBeRemoved)
        {
            foreach (IElement element in elementsToBeRemoved)
            {
                DocumentFunctions.Remove(_doc, element);
              
            }
        }
    }
}
