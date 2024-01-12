using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.ElementProviders
{
    public class LineStyleProvider : IElementProvider
    {
        private readonly Document _doc;

        public LineStyleProvider(Document doc)
        {
            _doc = doc;
        }

        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            Debug.WriteLine("Getting all linestyles in provider");
            IEnumerable<IElement> lineStyles = LineFunctions.GetLineStyles(_doc);

            foreach (IElement lineStyle in lineStyles)
            {
                string name = lineStyle.Name;
                bool canBeRemoved = lineStyle.CanBeRemoved;

                //Debug.WriteLine($"{name} -- can be removed? == {canBeRemoved}");
            }

            return lineStyles;
        }
    }
}
