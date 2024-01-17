using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
            //solids in family instances
            //SetUsedObjectStylesFamilyInstanceSolids(doc, objectStyleList);

            //import instances
            SetUsedObjectStylesImportInstance(uidoc, doc, objectStyleList);

            return Result.Succeeded;
        }

        private void SetUsedObjectStylesImportInstance(UIDocument uidoc, Document doc, ISet<ObjectStyleModel> objectStyleList)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<ImportInstance> selectedElements = collector.WhereElementIsNotElementType().OfType<ImportInstance>().ToList();

            //List<Element> selectedElements = uidoc.Selection.PickElementsByRectangle().ToList();
            foreach (ImportInstance selectedElement in selectedElements)
            {
                try
                {
                    Category selectedElementCategory = selectedElement.Category;
                    //Debug.WriteLine($"element category is {selectedElementCategory.Name} ");
                    Options options = new Options()
                    {
                        DetailLevel = ViewDetailLevel.Fine,
                        IncludeNonVisibleObjects = true,
                        ComputeReferences = true
                    };

                    var importCurves = selectedElement.get_Geometry(options)
                        .OfType<GeometryInstance>()
                        .SelectMany(g => g.GetInstanceGeometry().OfType<Curve>())
                        .ToList();

                    var importCurveStyles = new HashSet<string>(
                        importCurves
                            .Select(curve =>
                            {
                                ElementId eid = curve.GraphicsStyleId;
                                GraphicsStyle gs = doc.GetElement(eid) as GraphicsStyle;
                                return $"{selectedElementCategory.Name} : {gs?.GraphicsStyleCategory?.Name}";
                            })
                    );

                    foreach(string curveStyle in importCurveStyles)
                    {
                        IElement objectStyle = objectStyleList.FirstOrDefault(os => os.Name == curveStyle);
                        if (objectStyle != null)
                        {
                            objectStyle.IsUsed = true;
                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }

            Debug.WriteLine("USED STYLES");
            foreach (var os in objectStyleList.Where(os => os.IsUsed).ToList())
            {
                Debug.WriteLine(os.Name);
            }
        }

        private void SetUsedObjectStylesFamilyInstanceSolids(Document doc, ISet<ObjectStyleModel> objectStyleList)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<Element> selectedElements = collector.WhereElementIsNotElementType().OfType<FamilyInstance>().Cast<Element>().ToList();

            //List<Element> selectedElements = uidoc.Selection.PickElementsByRectangle().ToList();
            foreach (Element selectedElement in selectedElements)
            {
                try
                {
                    Category selectedElementCategory = selectedElement.Category;
                    //Debug.WriteLine($"element category is {selectedElementCategory.Name} ");
                    Options options = new Options()
                    {
                        DetailLevel = ViewDetailLevel.Fine,
                        IncludeNonVisibleObjects = true,
                        ComputeReferences = true
                    };


                    var solids = selectedElement.get_Geometry(options)
                        .OfType<GeometryInstance>()
                        .SelectMany(g => g.GetInstanceGeometry().OfType<Solid>()
                        .Where(s => s.Volume > 0))
                        .ToList();


                    foreach (var solid in solids)
                    {
                        ElementId eid = solid.GraphicsStyleId;
                        GraphicsStyle gs = doc.GetElement(eid) as GraphicsStyle;
                        if (gs != null)
                        {
                            Category category = gs.GraphicsStyleCategory;
                            Category parentCategory = gs.GraphicsStyleCategory;
                            string categoryNameCombined = $"{selectedElementCategory.Name} : {gs.Name}";

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
