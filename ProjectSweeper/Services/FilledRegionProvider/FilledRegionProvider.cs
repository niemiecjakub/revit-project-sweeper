using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.FilledRegionProvider
{
    public class FilledRegionProvider : IFilledRegionProvider
    {
        private readonly Document _doc;

        public FilledRegionProvider(Document doc)
        {
            _doc = doc;
        }
        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            Debug.WriteLine("Getting all filled regions in provider");
            IEnumerable<IElement> linePatterns = FilledRegionFunctions.GetAllFilledRegions(_doc);
            return linePatterns;
        }
    }
}
