using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                patternsList.Add(fillPatternModel);
            }

            SetUsedFillPatterns(doc, patternsList);
            return patternsList;
        }

        public static void SetUsedFillPatterns(Document doc, ISet<FillPatternModel> fillPatterns)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Material> allMaterials = collector.OfClass(typeof(Material)).Cast<Material>().ToList();

            foreach (Material material in allMaterials)
            {
                IList<ElementId> patternIds = new List<ElementId>();
                patternIds.Add(material.CutBackgroundPatternId);
                patternIds.Add(material.CutForegroundPatternId);
                patternIds.Add(material.SurfaceForegroundPatternId);
                patternIds.Add(material.SurfaceBackgroundPatternId);

                foreach (ElementId patternId in patternIds)
                {
                    if (patternId.IntegerValue > 0)
                    {
                        FillPatternModel fpm = fillPatterns.Where(fp => fp.Id == patternId).First();
                        if (fpm == null) { continue; }
                        fpm.IsUsed = true;
                    }
                }
            }


            FilteredElementCollector collector2 = new FilteredElementCollector(doc);
            IList<FilledRegionType> filledRegions = collector2.OfClass(typeof(FilledRegionType)).Cast<FilledRegionType>().ToList();

            foreach (FilledRegionType filledRegion in filledRegions)
            {
                IList<ElementId> patternIds = new List<ElementId>();
                patternIds.Add(filledRegion.BackgroundPatternId);
                patternIds.Add(filledRegion.ForegroundPatternId);

                foreach (ElementId patternId in patternIds)
                {
                    if (patternId.IntegerValue > 0)
                    {
                        FillPatternModel fpm = fillPatterns.Where(fp => fp.Id == patternId).First();
                        if (fpm == null) { continue; }

                        fpm.IsUsed = true;

                    }
                }
            }
        }
    }
}
