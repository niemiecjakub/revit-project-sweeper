using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.DependencyInjection;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                    //if (IsSubcategoryBuiltIn(subcategory)) { continue; }
                    string name = $"{category.Name} : {subcategory.Name}";
                    ElementId id = subcategory.Id;
                    ObjectStyleModel objectStyleModel = new ObjectStyleModel(name, id);
                    objectStyleList.Add(objectStyleModel);

                    if (subcategory.Id.IntegerValue == 535003)
                    {
                        Debug.WriteLine(subcategory.Name);
                    }

                }
            }

            //SetUsedObjectStylesFamilyInstance(doc, objectStyleList);

            return objectStyleList;
        }

        private static void SetUsedObjectStylesFamilyInstance(Document doc, ISet<ObjectStyleModel> objectStyleList)
        {
            //List<Family> families = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().ToList();
            var families = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToList();
            foreach (Element family in families)
            {
                try
                {
                    Debug.WriteLine($"element is {family.Name}");
                    Category familyCategory = family.Category;
                    Options options = new Options();
                    //{
                    //    IncludeNonVisibleObjects = true
                    //};

                    var solids = family.get_Geometry(options)
                        .OfType<GeometryInstance>()
                        .SelectMany(g => g.GetInstanceGeometry())
                        .ToList();

                    Debug.WriteLine($"element category is {familyCategory.Name} with {solids.Count} solids");
                    foreach (var solid in solids)
                    {
                        ElementId eid = solid.GraphicsStyleId;
                        GraphicsStyle gs = doc.GetElement(eid) as GraphicsStyle;
                        if (gs != null)
                        {
                            //Category category = gs.GraphicsStyleCategory;
                            //Category parentCategory = gs.GraphicsStyleCategory;
                            string categoryNameCombined = $"{family.Name} : {gs.Name}";
                            Debug.WriteLine($"{family.Id} : {gs.Id}");

                            IElement objectStyle = objectStyleList.FirstOrDefault(os => os.Name == categoryNameCombined);
                            if (objectStyle != null)
                            {
                                Debug.WriteLine($"Selected category {objectStyle.Name} - {objectStyle.Id}");
                                objectStyle.IsUsed = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }



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
