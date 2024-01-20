using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProjectSweeper.RevitFunctions
{
    public static class ViewportFunctions
    {

        public static ISet<ViewportModel> GetAllViewports(Document doc)
        {
            ISet<ViewportModel> viewportModelList = new HashSet<ViewportModel>();

            FilteredElementCollector viewportTypeCollector = new FilteredElementCollector(doc);
            ICollection<ElementType> viewportTypes = viewportTypeCollector.OfClass(typeof(ElementType)).Cast<ElementType>().ToList();

            foreach (ElementType viewportType in viewportTypes)
            {
                if (viewportType.FamilyName != "Viewport") { continue; }
                ViewportModel viewportModel = new ViewportModel(viewportType);
                viewportModel.CanBeRemoved = DocumentValidation.CanDeleteElement(doc, viewportType.Id);
                viewportModelList.Add(viewportModel);
            }

            SetUsedViewports(doc, viewportModelList);

            return viewportModelList;
        }

        public static void SetUsedViewports(Document doc, ISet<ViewportModel> viewportModelList)
        {
            FilteredElementCollector sheetCollector = new FilteredElementCollector(doc);
            ICollection<ViewSheet> sheets = sheetCollector.OfClass(typeof(ViewSheet)).Cast<ViewSheet>().ToList();

            foreach (ViewSheet sheet in sheets)
            {
                ICollection<ElementId> viewportIds = sheet.GetAllViewports();
                foreach (ElementId viewportId in viewportIds)
                {
                    Viewport viewport = doc.GetElement(viewportId) as Viewport;
                    ElementId viewportTypeId = viewport.GetTypeId();

                    ViewportModel viewporModel = viewportModelList.FirstOrDefault(v => v.Id == viewportTypeId);
                    if (viewporModel == null) { continue; }
                    viewporModel.IsUsed = true;
                }

            }

        }
    }
}
