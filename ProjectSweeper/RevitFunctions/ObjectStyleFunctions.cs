using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    objectStyleList.Add(objectStyleModel);
                }
            }

            SetUsedObjectStyles(doc, objectStyleList);

            return objectStyleList;
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
                        List<Family> families = new FilteredElementCollector(doc)
                        .OfClass(typeof(Family))
                        .Cast<Family>()
                        .Where(q => q.FamilyCategoryId == category.Id && q.Name != string.Empty)
                        .ToList();
                        foreach (Family family in families)
                        {
                            Debug.WriteLine($"{category.Name} = {family.Name}");
                            var document = family.Document;
                        }
                    }

                    //ElementCategoryFilter(builtInCategory);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"error occured for {builtInCategory}");
                }

            }


            //private void MergeFamilies(Document doc, ElementId categoryId, List<string> namesOfSubcategoriesToMerge, string newCategoryName)
            //{
            //    List<Family> families = new FilteredElementCollector(doc)
            //        .OfClass(typeof(Family))
            //        .Cast<Family>()
            //        .Where(q =>
            //        q.FamilyCategoryId == categoryId &&
            //        q.Name != string.Empty)
            //        .ToList();
            //    foreach (var family in families)
            //    {
            //        MergeFamilies(family, categoryId, namesOfSubcategoriesToMerge, newCategoryName);
            //    }
            //}

            //private void MergeFamilies(Family f, ElementId categoryId, List<string> namesOfSubcategoriesToMerge, string newCategoryName)
            //{
            //    var document = f.Document;
            //    var famDoc = document.EditFamily(f);
            //    // update any subfamilies before this family
            //    MergeFamilies(famDoc, categoryId, namesOfSubcategoriesToMerge, newCategoryName);
            //https://boostyourbim.wordpress.com/2023/11/21/recursively-merge-subcategories-approximately-750-useful/


            Debug.WriteLine($"end function");
        }


        private static bool IsSubcategoryBuiltIn(Category subcategory)
        {
            BuiltInCategory builtInCategory = (BuiltInCategory)subcategory.Id.IntegerValue;
            return Enum.IsDefined(typeof(BuiltInCategory), builtInCategory);
        }
    }
}
