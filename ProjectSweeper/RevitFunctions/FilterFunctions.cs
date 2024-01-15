using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.RevitFunctions
{
    public class FilterFunctions
    {
        public static ISet<FilterModel> GetAllFilters(Document doc)
        {
            ISet<FilterModel> filterModelList = new HashSet<FilterModel>();

            FilteredElementCollector filterCollector = new FilteredElementCollector(doc);
            ICollection<ParameterFilterElement> filters = filterCollector.OfClass(typeof(ParameterFilterElement)).Cast<ParameterFilterElement>().ToList();

            foreach (ParameterFilterElement filter in filters)
            {
                FilterModel filterModel = new FilterModel(filter);
                filterModel.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, filter.Id);
                filterModelList.Add(filterModel);
            }

            SetUsedViewports(doc, filterModelList);
            return filterModelList;
        }

        public static void SetUsedViewports(Document doc, ISet<FilterModel> viewportModelList)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            ICollection<View> views = viewCollector.OfClass(typeof(View)).Cast<View>().ToList();

            foreach (View view in views)
            {
                ICollection<ElementId> filterIds = view.GetFilters();
                foreach (ElementId filterId in filterIds)
                {
                    FilterModel filterModel = viewportModelList.FirstOrDefault(f => f.Id == filterId);
                    if (filterModel == null) { continue; }
                    filterModel.IsUsed = true;

                }
            }
        }
    }
}
