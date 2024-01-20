using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectSweeper.RevitFunctions
{
    public static class FilledRegionFunctions
    {
        public static ISet<FilledRegionModel> GetAllFilledRegions(Document doc)
        {
            ISet<FilledRegionModel> filledRegionsList = new HashSet<FilledRegionModel>();

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(FilledRegionType));
            IEnumerable<FilledRegionType> filledRegions = collector.Cast<FilledRegionType>().ToHashSet();

            foreach (FilledRegionType filledRegion in filledRegions)
            {
                FilledRegionModel filledRegionModel = new FilledRegionModel(filledRegion);
                filledRegionModel.CanBeRemoved = DocumentValidation.CanDeleteElement(doc, filledRegion.Id);
                filledRegionsList.Add(filledRegionModel);
            }

            SetUsedFilledRegions(doc, filledRegionsList);

            return filledRegionsList;
        }

        private static void SetUsedFilledRegions(Document doc, ISet<FilledRegionModel> filledRegions)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> allElements = collector.WhereElementIsNotElementType().ToElements();

            foreach (Element element in allElements)
            {
                if (element is FilledRegion)
                {
                    FilledRegion filledRegionElement = element as FilledRegion;
                    ElementId filledRegionTypeId = filledRegionElement.GetTypeId();
                    FilledRegionType filledRegionType = doc.GetElement(filledRegionTypeId) as FilledRegionType;
                    if (filledRegionType == null) { continue; }
                    FilledRegionModel filledRegionModel = filledRegions.Where(fr => fr.Id == filledRegionType.Id).FirstOrDefault();

                    if (filledRegionModel == null) { continue; }
                    filledRegionModel.IsUsed = true;

                }
            }
        }
    }
}
