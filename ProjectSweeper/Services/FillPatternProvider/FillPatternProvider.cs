using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.FillPatternProvider
{
    public class FillPatternProvider : IFillPatternProvider
    {
        private readonly Document _doc;

        public FillPatternProvider(Document doc)
        {
            _doc = doc;
        }
        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            Debug.WriteLine("Getting all linePatterns in provider");
            IEnumerable<IElement> linePatterns = FillPatternFunctions.GetAllFillPatterns(_doc);
            return linePatterns;
        }
    }
}
