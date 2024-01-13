using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.FilterProvider
{
    public class FilterProvider : IFilterProvider
    {
        private readonly Document _doc;

        public FilterProvider(Document doc)
        {
            _doc = doc;
        }
        public async Task<IEnumerable<IElement>> GetAllElements()
        {
            Debug.WriteLine("Getting all FILTERS in provider");
            IEnumerable<IElement> filters = FilterFunctions.GetAllFilters(_doc);
            return filters;
        }
    }
}
