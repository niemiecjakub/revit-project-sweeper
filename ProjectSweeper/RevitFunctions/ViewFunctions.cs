using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.RevitFunctions
{
    public class ViewFunctions
    {
        public static ISet<ViewTemplateModel> GetAllViewTemplates(Document doc)
        {
            ISet<ViewTemplateModel> viewTemplateModelList = new HashSet<ViewTemplateModel>();

            FilteredElementCollector viewportCollector = new FilteredElementCollector(doc);
            ICollection<View> views = viewportCollector.OfClass(typeof(View)).Cast<View>().ToList();

            foreach (View view in views)
            {
                if (!view.IsTemplate) { continue; }
                ViewTemplateModel viewTemplateModel = new ViewTemplateModel(view);
                viewTemplateModel.CanBeRemoved = DocumentValidation.CanDeleteElement(doc, view.Id);
                viewTemplateModelList.Add(viewTemplateModel);
            }

            SetUsedViewTemplates(doc, viewTemplateModelList);

            return viewTemplateModelList;
        }

        public static void SetUsedViewTemplates(Document doc, ISet<ViewTemplateModel> viewportModelList)
        {
            FilteredElementCollector viewsCollector = new FilteredElementCollector(doc);
            ICollection<View> views = viewsCollector.OfClass(typeof(View)).Cast<View>().ToList();

            foreach (View view in views)
            {
                if (view.IsTemplate) { continue; }
                ElementId templateId = view.ViewTemplateId;
                ViewTemplateModel viewporModel = viewportModelList.FirstOrDefault(v => v.Id == templateId);
                if (viewporModel == null) { continue; }
                viewporModel.IsUsed = true;
            }
        }
    }
}
