using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.LinePatternProvider
{
    public class LinePatternProvider : ILinePatternProvider
    {
        private readonly Document _doc;

        public LinePatternProvider(Document doc)
        {
            _doc = doc;
        }
        public async Task<IEnumerable<LinePatternModel>> GetAllElements()
        {
            Debug.WriteLine("Getting all linePatterns in provider");
            IEnumerable<LinePatternModel> linePatterns = LineFunctions.GetLinePatterns(_doc);
            return linePatterns;
        }
    }
}
