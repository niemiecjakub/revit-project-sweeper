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
    public class ViewportFunctions
    {
        public static ISet<ViewportModel> GetAllViewports(Document doc)
        {
            ISet<ViewportModel> viewportModelList = new HashSet<ViewportModel>();

            FilteredElementCollector viewportCollector = new FilteredElementCollector(doc);
            ICollection<Viewport> viewports = viewportCollector.OfClass(typeof(Viewport)).Cast<Viewport>().ToList();

            foreach (Viewport viewport in viewports)
            {
                ViewportModel viewportModel = new ViewportModel(viewport);
                viewportModel.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, viewport.Id);
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
                    ViewportModel viewporModel = viewportModelList.FirstOrDefault(v => v.Id == viewportId);
                    if (viewporModel == null) { continue; }
                    viewporModel.IsUsed = true;
                }

            }

        }
    }
}
