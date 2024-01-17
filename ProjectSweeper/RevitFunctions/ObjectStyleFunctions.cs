using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
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

            HashSet<string> usedObjectStyles = GetUsedObjects(doc);

            foreach (string objectStyle in usedObjectStyles)
            {
                ObjectStyleModel objectStyleModel = objectStyleList.FirstOrDefault(x => x.Name == objectStyle);
                if (objectStyleModel != null)
                {
                    objectStyleModel.IsUsed = true;
                }
            }

            return objectStyleList;
        }
        private static HashSet<string> GetUsedObjects(Document doc)
        {
            HashSet<string> analyzedFamilies = new HashSet<string>();
            HashSet<string> usedObjectStyles = new HashSet<string>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<Element> selectedElements = collector.WhereElementIsNotElementType().Where(elem => elem is FamilyInstance || elem is ImportInstance).ToList();


            Options options = new Options()
            {
                DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = true,
                ComputeReferences = true
            };

            foreach (Element selectedElement in selectedElements)
            {
                GetUsedObjectStylesFromLoadableFamilies(doc, usedObjectStyles, selectedElement, analyzedFamilies, options);
            }

            return usedObjectStyles;

        }

        private static void GetUsedObjectStylesFromLoadableFamilies(Document doc, HashSet<string> usedObjectStyles, Element selectedElement, HashSet<string> analyzedFamilies, Options options)
        {
            try
            {
                Category mainCategory = selectedElement.Category;
                if (selectedElement is FamilyInstance familyInstance)
                {

                    Family family = familyInstance.Symbol.Family;
                    string familyName = family.Name;

                    if (analyzedFamilies.Contains(familyName))
                    {
                        return;
                    }

                    Document familyDoc = doc.EditFamily(family);
                    if (null == familyDoc && familyDoc.IsFamilyDocument == false)
                    {
                        return;
                    }

                    List<Element> nestedFams = new FilteredElementCollector(familyDoc).WhereElementIsNotElementType().Where(elem => elem is FamilyInstance || elem is ImportInstance).ToList();
                    foreach (Element nestedElement in nestedFams)
                    {
                        GetUsedObjectStylesFromLoadableFamilies(doc, usedObjectStyles, nestedElement, analyzedFamilies, options);
                    }

                    //CURVE ELEMENT STYLES
                    FilteredElementCollector curveElementCollector = new FilteredElementCollector(familyDoc).OfClass(typeof(CurveElement)).WhereElementIsNotElementType();
                    HashSet<string> curveStyeNames = curveElementCollector.Cast<CurveElement>().Select(l => l.LineStyle.Name).ToHashSet();
                    foreach (string styleName in curveStyeNames)
                    {
                        usedObjectStyles.Add($"{mainCategory.Name} : {styleName}");
                    }

                    //SOLID ELEMENT STYLES
                    List<Solid> solids = familyInstance.get_Geometry(options)
                        .OfType<GeometryInstance>()
                        .SelectMany(g => g.GetInstanceGeometry().OfType<Solid>()
                        .Where(s => s.Volume > 0))
                        .ToList();


                    foreach (Solid solid in solids)
                    {
                        ElementId eid = solid.GraphicsStyleId;
                        GraphicsStyle gs = doc.GetElement(eid) as GraphicsStyle;
                        if (gs != null)
                        {
                            string categoryNameCombined = $"{mainCategory.Name} : {gs.Name}";
                            usedObjectStyles.Add(categoryNameCombined);
                        }
                    }
                }

                //IMPORT INSTANCE
                if (selectedElement is ImportInstance importInstance)
                {
                    List<Curve> importCurves = selectedElement.get_Geometry(options)
                    .OfType<GeometryInstance>()
                    .SelectMany(g => g.GetInstanceGeometry().OfType<Curve>())
                    .ToList();

                    foreach (Curve importCurve in importCurves)
                    {
                        ElementId eid = importCurve.GraphicsStyleId;
                        GraphicsStyle gs = doc.GetElement(eid) as GraphicsStyle;
                        if (gs != null)
                        {
                            string importCurveStyle = $"{mainCategory.Name} : {gs.GraphicsStyleCategory.Name}";
                            usedObjectStyles.Add(importCurveStyle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static bool IsSubcategoryBuiltIn(Category subcategory)
        {
            BuiltInCategory builtInCategory = (BuiltInCategory)subcategory.Id.IntegerValue;
            return Enum.IsDefined(typeof(BuiltInCategory), builtInCategory);
        }
    }
}
