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
    public class LinePatternProvider : IElementProvider
    {
        private readonly Document _doc;

        public LinePatternProvider(Document doc)
        {
            _doc = doc;
        }
        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            Debug.WriteLine("Getting all linePatterns in provider");
            IEnumerable<IElement> linePatterns = LineFunctions.GetLinePatterns(_doc);
            return linePatterns;
        }
    }
}
