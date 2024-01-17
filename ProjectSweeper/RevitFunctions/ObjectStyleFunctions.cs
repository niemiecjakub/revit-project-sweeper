using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProjectSweeper.RevitFunctions
{
    public static class ObjectStyleFunctions
    {
        public static ISet<ObjectStyleModel> GetAllObjectStyles(Document doc)
        {
            ISet<ObjectStyleModel> objectStyleList = new HashSet<ObjectStyleModel>();

            Categories categories = doc.Settings.Categories;
            foreach (Category category in categories)
            {
                CategoryNameMap subcategories = category.SubCategories;
                foreach (Category subcategory in subcategories)
                {
                    if (IsSubcategoryBuiltIn(subcategory)) { continue; }
                    string name = $"{category.Name} : {subcategory.Name}";
                    ElementId id = subcategory.Id;
                    ObjectStyleModel objectStyleModel = new ObjectStyleModel(name, id);
                    objectStyleModel.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, subcategory.Id);
                    objectStyleList.Add(objectStyleModel);
                }
            }

            SetUsedObjectStylesFamilyInstanceSolids(doc, objectStyleList);
            SetUsedObjectStylesImportInstance(doc, objectStyleList);

            return objectStyleList;
        }

        private static void SetUsedObjectStylesFamilyInstanceSolids(Document doc, ISet<ObjectStyleModel> objectStyleList)
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
                            //Category category = gs.GraphicsStyleCategory;
                            //Category parentCategory = gs.GraphicsStyleCategory;
                            string categoryNameCombined = $"{selectedElementCategory.Name} : {gs.Name}";

                            IElement objectStyle = objectStyleList.FirstOrDefault(os => os.Name == categoryNameCombined);
                            if (objectStyle != null)
                            {
                                objectStyle.IsUsed = true;
                            }
                            //Debug.WriteLine($"+++ {objectStyle.Name} - {objectStyle.Id}");
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private static void SetUsedObjectStylesImportInstance(Document doc, ISet<ObjectStyleModel> objectStyleList)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<ImportInstance> selectedElements = collector.WhereElementIsNotElementType().OfType<ImportInstance>().ToList();

            foreach (ImportInstance selectedElement in selectedElements)
            {
                try
                {
                    Category selectedElementCategory = selectedElement.Category;
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

                    foreach (string curveStyle in importCurveStyles)
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


        //LEGACY
        private static void SetUsedObjectStyles(Document doc, ISet<ObjectStyleModel> objectStyleList)
        {

            Debug.WriteLine($"stayrt function");
            IEnumerable<BuiltInCategory> allBuiltInCategories = Enum.GetValues(typeof(BuiltInCategory)).Cast<BuiltInCategory>();

            foreach (BuiltInCategory builtInCategory in allBuiltInCategories)
            {
                try
                {
                    Category category = Category.GetCategory(doc, builtInCategory);
                    if (category != null && category.IsVisibleInUI)
                    {
                        ElementId categoryId = category.Id;
                        List<Family> families = new FilteredElementCollector(doc)
                        .OfClass(typeof(Family))
                        .Cast<Family>()
                        .Where(q => q.FamilyCategoryId == category.Id && q.Name != string.Empty)
                        .ToList();
                        foreach (Family family in families)
                        {
                            var document = family.Document;
                            var famDoc = document.EditFamily(family);

                            Debug.WriteLine($"{category.Name} = {family.Name}");
                            RecursiveSearch(famDoc, categoryId, objectStyleList);
                        }
                    }

                    //ElementCategoryFilter(builtInCategory);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"error occured for {builtInCategory}");
                }
            }
            Debug.WriteLine($"end function");
        }


        private static void RecursiveSearch(Document doc, ElementId categoryId, ISet<ObjectStyleModel> objectStyleList)
        {
            Debug.WriteLine("OPENING");
            List<Family> subfamilies = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .ToList();
            Debug.WriteLine($"There are {subfamilies.Count} subfamilies");

            foreach (Family subfamily in subfamilies)
            {
                RecursiveSearch(subfamily, categoryId, objectStyleList);
            }
        }

        private static void RecursiveSearch(Family family, ElementId categoryId, ISet<ObjectStyleModel> objectStyleList)
        {
            Debug.WriteLine($"INSIDE {family.Name}");
            var document = family.Document;
            var famDoc = document.EditFamily(family);

            var parentCategory = famDoc.Settings.Categories.Cast<Category>().First(q => q.Id == categoryId);
            var subcategories = parentCategory.SubCategories.Cast<Category>();


            ISet<string> usedSubcategories = new HashSet<string>();

            var elements = new FilteredElementCollector(famDoc).ToList();
            var curves = new FilteredElementCollector(famDoc).OfClass(typeof(CurveElement)).Cast<CurveElement>().ToList();
            var lineStyles = curves.Select(q => q.LineStyle.Id.IntegerValue).ToList();
            foreach (var e in elements)
            {
                if (e is GenericForm gf)
                {
                    usedSubcategories.Add(gf.Subcategory.Name);
                }
                else if (e is ModelText mt)
                {
                    usedSubcategories.Add(mt.Subcategory.Name);
                }
            }
            foreach (var e in curves)
            {
                if (e is ModelCurve mc)
                {
                    usedSubcategories.Add(mc.Subcategory.Name);
                }
                else if (e is SymbolicCurve sc)
                {
                    usedSubcategories.Add(sc.Subcategory.Name);
                }
                else if (e is CurveByPoints cbp)
                {
                    usedSubcategories.Add(cbp.Subcategory.Name);
                }
            }
            foreach (var e in curves)
            {
                if (e is ModelCurve mc)
                {
                    usedSubcategories.Add(mc.Subcategory.Name);
                }
                else if (e is SymbolicCurve sc)
                {
                    usedSubcategories.Add(sc.Subcategory.Name);
                }
                else if (e is CurveByPoints cbp)
                {
                    usedSubcategories.Add(cbp.Subcategory.Name);
                }
            }

            foreach (string cat in usedSubcategories)
            {
                Debug.WriteLine(cat);
            }

            famDoc.Close(false);

            RecursiveSearch(famDoc, categoryId, objectStyleList);
        }

        private static bool IsSubcategoryBuiltIn(Category subcategory)
        {
            BuiltInCategory builtInCategory = (BuiltInCategory)subcategory.Id.IntegerValue;
            return Enum.IsDefined(typeof(BuiltInCategory), builtInCategory);
        }
    }
}
