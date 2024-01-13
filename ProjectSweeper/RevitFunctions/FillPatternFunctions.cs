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
    public class FillPatternFunctions
    {

        public static ISet<FillPatternModel> GetAllFillPatterns(Document doc)
        {
            ISet<FillPatternModel> patternsList = new HashSet<FillPatternModel>();

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement));
            IEnumerable<FillPatternElement> patterns = collector.Cast<FillPatternElement>();

            foreach (FillPatternElement pattern in patterns)
            {
                FillPatternModel fillPatternModel = new FillPatternModel(pattern);
                fillPatternModel.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, pattern.Id);
                patternsList.Add(fillPatternModel);
            }

            SetUsedFillPatterns(doc, patternsList);

            foreach (var pattern in patternsList.OrderBy(P => P.Name))
            {
                //Debug.WriteLine($"{pattern.Name} == isUsed -> {pattern.IsUsed} == canBeRemoved -> {pattern.CanBeRemoved}");
            }
            return patternsList;
        }

        public static void SetUsedFillPatterns(Document doc, ISet<FillPatternModel> fillPatterns)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Material> allMaterials = collector.OfClass(typeof(Material)).Cast<Material>().ToList();

            foreach (Material material in allMaterials)
            {
                IList<ElementId> patternIds = new List<ElementId>()
                {
                    material.CutBackgroundPatternId,
                    material.CutForegroundPatternId,
                    material.SurfaceForegroundPatternId,
                    material.SurfaceBackgroundPatternId
                };

                foreach (ElementId patternId in patternIds)
                {
                    FillPatternModel fpm = fillPatterns.FirstOrDefault(fp => fp.Id == patternId);
                    if (fpm == null) { continue; }
                    fpm.IsUsed = true;
                }
            }


            FilteredElementCollector collector2 = new FilteredElementCollector(doc);
            IList<FilledRegionType> filledRegions = collector2.OfClass(typeof(FilledRegionType)).Cast<FilledRegionType>().ToList();

            foreach (FilledRegionType filledRegion in filledRegions)
            {
                IList<ElementId> patternIds = new List<ElementId>()
                {
                    filledRegion.BackgroundPatternId,
                    filledRegion.ForegroundPatternId,
                };

                foreach (ElementId patternId in patternIds)
                {
                    FillPatternModel fpm = fillPatterns.FirstOrDefault(fp => fp.Id == patternId);
                    if (fpm == null) { continue; }

                    fpm.IsUsed = true;
                }
            }

            FilteredElementCollector collector3 = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance));
            IList<FamilyInstance> familyInstances = collector3.Cast<FamilyInstance>().ToList();
            foreach (FamilyInstance familyInstance in familyInstances)
            {
                Debug.WriteLine(familyInstance.Name);
            }


            //// Create a filtered element collector for family instances
            //FilteredElementCollector collector = new FilteredElementCollector(doc);

            //// Set the category to family instances
            //collector.OfCategory(BuiltInCategory.OST_GenericModel); // Change the category as needed

            //// Get all family instances in the document
            //ICollection<Element> familyInstances = collector.OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).ToElements();

            //// Iterate through each family instance
            //foreach (Element familyInstance in familyInstances)
            ////{
            //    // Do something with the family instance, e.g., get its type, location, etc.
            //    FamilyInstance fi = familyInstance as FamilyInstance;
            //    if (fi != null)
            //    {
            //        TaskDialog.Show("Family Instance Information",
            //            $"Family Instance Id: {fi.Id}\n" +
            //            $"Family Instance Name: {fi.Name}");
            //    }
            //}


            //// List to store FillPatternElement for each category
            //List<FillPatternElement> fillPatternElements = new List<FillPatternElement>();

            //// Categories that may have a direct association with FillPatternElement
            //BuiltInCategory[] fillPatternCategories = new BuiltInCategory[]
            //{
            //BuiltInCategory.OST_Walls,
            //BuiltInCategory.OST_Floors,
            //BuiltInCategory.OST_Ceilings,
            //BuiltInCategory.OST_Doors,
            //BuiltInCategory.OST_Windows,
            //BuiltInCategory.OST_Furniture,
            //BuiltInCategory.OST_GenericModel, // Generic Model Categories
            //BuiltInCategory.OST_DetailComponents,
            //BuiltInCategory.OST_Rooms,
            //BuiltInCategory.OST_Areas,
            //BuiltInCategory.OST_Materials
            //};

            //// Iterate through each category
            //foreach (BuiltInCategory category in fillPatternCategories)
            //{
            //    // Create a filtered element collector for the category
            //    FilteredElementCollector collector4 = new FilteredElementCollector(doc).OfCategory(category);

            //    // Get the first element of the category (if any)
            //    Element element = collector4.FirstOrDefault();

            //    // If the category has elements in the project
            //    if (element != null)
            //    {
            //        ElementId graphicsStyleId = element.GetGraphicsStyleId();

            //        // Retrieve the GraphicsStyle using the GraphicsStyleId
            //        GraphicsStyle graphicsStyle = doc.GetElement(graphicsStyleId) as GraphicsStyle;
            //        // Get the GraphicsStyle of the element (which may contain the FillPatternId)

            //        // If GraphicsStyle is not null and has a FillPatternId
            //        if (graphicsStyle != null && graphicsStyle.FillPatternId != ElementId.InvalidElementId)
            //        {
            //            // Get the FillPatternElement from the FillPatternId
            //            FillPatternElement fillPatternElement = doc.GetElement(graphicsStyle.FillPatternId) as FillPatternElement;

            //            // Add to the list if not null
            //            if (fillPatternElement != null)
            //            {
            //                fillPatternElements.Add(fillPatternElement);
            //            }
            //        }
            //    }
            //}

            //// Print or process the obtained FillPatternElements as needed
            //foreach (FillPatternElement fillPatternElement in fillPatternElements)
            //{
            //    TaskDialog.Show("Fill Pattern Information",
            //        $"Fill Pattern Id: {fillPatternElement.Id}\n" +
            //        $"Fill Pattern Name: {fillPatternElement.Name}");
            //}


        }
    }
}
