using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace ProjectSweeper.StairModule.Numbering
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FloorSelection floorSelection = new FloorSelection(uidoc, doc);

            View view = doc.ActiveView;

            List<Floor> sortedFloors = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsNotElementType()
                .Cast<Floor>()
                //.Where(f => f.Name.ToLower().Contains("LANDING"))
                .OrderBy(f => floorSelection.GetFloorAproxLocation(f).X)
                .ToList();

            Debug.WriteLine(sortedFloors.Count);
            List<Element> sortedSteps = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("296"))
                .OrderBy(s => GetStepLocation(s).X)
                .ToList();

            Debug.WriteLine($"{sortedFloors.Count} floors");
            Debug.WriteLine($"{sortedSteps.Count} steps");

            using (Transaction transaction = new Transaction(doc, "Number landings"))
            {
                transaction.Start();
                int startNumber = 1;
                SetFloorNumbering(sortedFloors, floorSelection, startNumber);
                List<double> landingRanges = GetLandingRanges(sortedFloors, floorSelection, startNumber);
                Dictionary<string, int> stairRunMap = MatchStairToLanding(0, sortedSteps, landingRanges);
                WriteStepsToLandings(sortedFloors, stairRunMap);

                foreach (var kv in stairRunMap)
                {
                    //Debug.WriteLine($"{kv.Key} has {kv.Value} steps");
                }

                transaction.Commit();
            }
            Debug.WriteLine("COMMITED");

            return Result.Succeeded;
        }

        public void WriteStepsToLandings(List<Floor> sortedFloors, Dictionary<string, int> stairRunMap)
        {
            int i = 0;
            foreach (Floor floor in sortedFloors)
            {
                Parameter walkwayStairCount = floor.LookupParameter("Walkway_stairCount");
                int stairCount = stairRunMap.ElementAt(i).Value;
                walkwayStairCount.Set(stairCount);
                i++;
            }
        }

        public Dictionary<string, int> MatchStairToLanding(int startValue, List<Element> sortedSteps, List<double> landingRanges)
        {
            Dictionary<string, int> stairRunMap = new Dictionary<string, int>();
            double currentRange = landingRanges[startValue];
            //Debug.WriteLine($"current range {currentRange}");
            foreach (Element step in sortedSteps)
            {
                double stepXvalue = GetStepLocation(step).X;
                try
                {
                    if (stepXvalue > currentRange)
                    {
                        startValue++;
                        currentRange = landingRanges[startValue];
                    }

                    string mapKey = $"landing {startValue}";
                    if (stairRunMap.ContainsKey(mapKey))
                    {
                        stairRunMap[mapKey]++;
                    }
                    else
                    {
                        stairRunMap[mapKey] = 1;
                    }
                }
                catch (Exception e)
                {

                }
            }
            return stairRunMap;
        }


        public List<double> GetLandingRanges(List<Floor> sortedFloors, FloorSelection floorSelection, int startNumber)
        {
            int i = startNumber;
            List<double> ranges = new List<double>()
            {
                -double.MaxValue
            };
            foreach (Floor floor in sortedFloors)
            {
                double aproxLocationX = floorSelection.GetFloorAproxLocation(floor).X;
                ranges.Add(aproxLocationX);
                i++;
            }
            return ranges;
        }

        public XYZ GetStepLocation(Element element)
        {
            LocationPoint location = element.Location as LocationPoint;
            return location.Point;
        }

        public void SetFloorNumbering(List<Floor> sortedFloors, FloorSelection floorSelection, int startNumber)
        {
            int i = startNumber;
            foreach (Floor floor in sortedFloors)
            {
                Parameter walkwayOrientation = floor.LookupParameter("Walkway_orientation");
                Parameter walkwayPurpose = floor.LookupParameter("Walkway_purpose");
                Parameter walkwayMark = floor.LookupParameter("Walkway_mark");
                Parameter walkwayNumber = floor.LookupParameter("Walkway_number");
                Parameter walkwayLength = floor.LookupParameter("Walkway_length");

                string stairCode = Utils.GetStairCode(walkwayOrientation, walkwayPurpose);
                walkwayMark.Set(stairCode);

                string number = Utils.ToNumberingFormat(i);
                walkwayNumber.Set(number);

                Line landingLengthCurve = floorSelection.GetFloorLongestCurve(floor) as Line;
                double landingLength = landingLengthCurve.Length;
                walkwayLength.Set(landingLength);
                i++;
            }
        }
    }
}
