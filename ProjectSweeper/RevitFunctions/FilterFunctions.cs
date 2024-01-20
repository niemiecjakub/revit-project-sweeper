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
                filterModel.CanBeRemoved = DocumentValidation.CanDeleteElement(doc, filter.Id);
                filterModelList.Add(filterModel);
            }

            SetUsedFilters(doc, filterModelList);
            return filterModelList;
        }

        public static void SetUsedFilters(Document doc, ISet<FilterModel> filterModelList)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            ICollection<View> views = viewCollector.OfClass(typeof(View)).Cast<View>().ToList();

            foreach (View view in views)
            {
                try
                {
                    ICollection<ElementId> filterIds = view.GetFilters();
                    foreach (ElementId filterId in filterIds)
                    {
                        FilterModel filterModel = filterModelList.FirstOrDefault(f => f.Id == filterId);
                        if (filterModel == null) { continue; }
                        filterModel.IsUsed = true;
                    }
                } catch(Exception ex)
                {
                    //Debug.WriteLine($"{view.Name} doesnt support filters");
                }

            }
        }
    }
}
