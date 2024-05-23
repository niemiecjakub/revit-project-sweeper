using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ProjectSweeper.StairModule.Schedule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.StructureNumbering
{
    [Transaction(TransactionMode.Manual)]
    public class StructureNumbering : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FloorSelection floorSelection = new FloorSelection(uidoc, doc);
            View view = doc.ActiveView;


            //LANDINGS
            List<Floor> landings = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_Floors)
                .WhereElementIsNotElementType()
                .Cast<Floor>()
                .Where(f => f.Name.ToLower().Contains("stair"))
                .OrderBy(f => floorSelection.GetFloorAproxLocation(f).X)
                .ToList();

            //STEP
            List<Element> steps = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructConnections)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("296"))
                .OrderBy(s => ScheduleUtil.GetAdaptiveFamilyXYZ(s, doc).X)
                .ToList();

            //EDGE BEAM
            List<Element> edgeBeams = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructConnections)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("edgebeam"))
                .OrderBy(s => ScheduleUtil.GetAdaptiveFamilyXYZ(s, doc).X)
                .ToList();

            //HBEAM
            List<Element> hBeams = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("hbeam"))
                .OrderBy(e => ScheduleUtil.GetHbeamXYZ(e).X)
                .ToList();

            //LBEAM
            List<Element> lBeams = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("lbeam"))
                .OrderBy(e => ScheduleUtil.GetFramingXYZ(e).X)
                .ToList();

            //COLUMN
            List<Element> columns = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("supportcolumn"))
                .OrderBy(e => ScheduleUtil.GetColumnXYZ(e).X)
                .ToList();


            Debug.WriteLine($"{landings.Count} floors");
            Debug.WriteLine($"{steps.Count} steps");
            Debug.WriteLine($"{edgeBeams.Count} edge beams");
            Debug.WriteLine($"{hBeams.Count} hBeams");
            Debug.WriteLine($"{lBeams.Count} lBeams");
            Debug.WriteLine($"{columns.Count} columns");


            using (Transaction transaction = new Transaction(doc, "Number structure"))
            {
                transaction.Start();
                
                for (int i = 1; i < landings.Count; i++)
                {
                    Floor currentLanding = landings[i];
                    Floor previousLanding = landings[i-1];

                    XYZ startXYZ = floorSelection.GetLandingStartEndLines(previousLanding)[1].GetEndPoint(0);
                    XYZ endXYZ = floorSelection.GetLandingStartEndLines(currentLanding)[0].GetEndPoint(1);

                    string mark = currentLanding.LookupParameter("Walkway_mark").AsString();
                    string number = currentLanding.LookupParameter("Walkway_number").AsString();

                    List<Element> hbeams_str = hBeams.Where(e => ScheduleUtil.GetHbeamXYZ(e).X > startXYZ.X && ScheduleUtil.GetHbeamXYZ(e).X < endXYZ.X).ToList();
                    List<Element> lbeams_str = lBeams.Where(e => ScheduleUtil.GetFramingXYZ(e).X > startXYZ.X && ScheduleUtil.GetFramingXYZ(e).X < endXYZ.X).ToList();
                    List<Element> columns_str = columns.Where(e => ScheduleUtil.GetColumnXYZ(e).X > startXYZ.X && ScheduleUtil.GetColumnXYZ(e).X < endXYZ.X).ToList();

                    List<Element> steps_str = steps.Where(e => ScheduleUtil.GetAdaptiveFamilyXYZ(e, doc).X > startXYZ.X && ScheduleUtil.GetAdaptiveFamilyXYZ(e, doc).X < endXYZ.X).ToList();
                    List<Element> edgeBeams_str = edgeBeams.Where(e => ScheduleUtil.GetAdaptiveFamilyXYZ(e, doc).X > startXYZ.X && ScheduleUtil.GetAdaptiveFamilyXYZ(e, doc).X < endXYZ.X).ToList();

                    Debug.WriteLine($"{mark}{number}:\n" +
                        $" hebams:{hbeams_str.Count}\n" +
                        $" lbeams: {lbeams_str.Count} \n" +
                        $" columns: {columns_str.Count} \n" +
                        $" edge beams: {edgeBeams_str.Count}   \n" +
                        $"steps: {steps_str.Count}\n" +
                        $"__________________________________________________");

                    List<Element> structureElements = new List<Element>();
                    structureElements.AddRange(hbeams_str);
                    structureElements.AddRange(lbeams_str);
                    structureElements.AddRange(columns_str);
                    structureElements.AddRange(steps_str);
                    structureElements.AddRange(edgeBeams_str);

                    AssignParameters(structureElements, mark, number);

                }

                transaction.Commit();
            }

            Debug.WriteLine("COMMITED");

            return Result.Succeeded;
        }
        public void AssignParameters(List<Element> elements, string mark, string number)
        {
            foreach(Element element in elements)
            {
                element.LookupParameter("Walkway_mark").Set(mark);
                element.LookupParameter("Walkway_number").Set(number);
            }
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
                double stepXvalue = ScheduleUtil.GetAdaptiveFamilyXYZ(step).X;
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


        public List<double> GetLandingRanges(List<Floor> sortedFloors, FloorSelection floorSelection)
        {
            List<double> ranges = new List<double>()
            {
                -double.MaxValue
            };

            foreach (Floor floor in sortedFloors)
            {
                double aproxLocationX = floorSelection.GetFloorAproxLocation(floor).X;
                ranges.Add(aproxLocationX);
                //i++;
            }
            return ranges;
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
