using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper
{
    [Transaction(TransactionMode.Manual)]
    public class SelectionCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            ISet<ObjectStyleModel> objectStyleList = GetObjectStyles(doc);

            //FilteredElementCollector collector = new FilteredElementCollector(doc);
            //List<Element> selectedElements = collector.WhereElementIsNotElementType().OfType<FamilyInstance>().Cast<Element>().ToList();

            List<Element> selectedElements = uidoc.Selection.PickElementsByRectangle().ToList();
            foreach (Element selectedElement in selectedElements)
            {
                try
                {
                    Debug.WriteLine($"element is {selectedElement.Name}");
                    Category selectedElementCategory = selectedElement.Category;
                    //Debug.WriteLine($"element category is {selectedElementCategory.Name} ");
                    Options options = new Options()
                    {
                        IncludeNonVisibleObjects = true
                    };



                    var solids = selectedElement.get_Geometry(options)
                        .OfType<GeometryInstance>()
                        .SelectMany(g => g.GetInstanceGeometry().OfType<Solid>()
                        .Where(s => s.Volume > 0))
                        .ToList();


                    Debug.WriteLine($"inside {selectedElement.Name} = {solids.Count} ");

                    foreach (var solid in solids)
                    {
                        ElementId eid = solid.GraphicsStyleId;
                        GraphicsStyle gs = doc.GetElement(eid) as GraphicsStyle;
                        if (gs != null)
                        {
                            Category category = gs.GraphicsStyleCategory;
                            Category parentCategory = gs.GraphicsStyleCategory;
                            string categoryNameCombined = $"{selectedElementCategory.Name} : {gs.Name}";
                            //Debug.WriteLine($"{categoryNameCombined}");

                            IElement objectStyle = objectStyleList.FirstOrDefault(os => os.Name == categoryNameCombined);
                            if (objectStyle != null)
                            {
                                objectStyle.IsUsed = true;
                            }
                            Debug.WriteLine($"+++ {objectStyle.Name} - {objectStyle.Id}");
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }

            Debug.WriteLine("USED STYLES");
            foreach (var os in objectStyleList.Where(os => os.IsUsed).ToList())
            {
                Debug.WriteLine(os.Name);
            }
            return Result.Succeeded;
        }

        private ISet<ObjectStyleModel> GetObjectStyles(Document doc)
        {
            ISet<ObjectStyleModel> objectStyleList = new HashSet<ObjectStyleModel>();

            Categories categories = doc.Settings.Categories;
            foreach (Category category in categories)
            {
                CategoryNameMap subcategories = category.SubCategories;
                foreach (Category subcategory in subcategories)
                {
                    //if (IsSubcategoryBuiltIn(subcategory)) { continue; }
                    string name = $"{category.Name} : {subcategory.Name}";
                    ElementId id = subcategory.Id;
                    ObjectStyleModel objectStyleModel = new ObjectStyleModel(name, id);
                    objectStyleList.Add(objectStyleModel);

                }
            }

            return objectStyleList;
        }
    }
}
