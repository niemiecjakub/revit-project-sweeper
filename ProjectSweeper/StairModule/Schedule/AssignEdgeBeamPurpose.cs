using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjectSweeper.StairModule.Schedule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.Schedule
{
    [Transaction(TransactionMode.Manual)]
    public class AssignEdgeBeamPurpose : IExternalCommand
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
                .Where(f => f.Name.ToLower().Contains("landing"))
                .OrderBy(f => floorSelection.GetFloorAproxLocation(f).X)
                .ToList();

            //EDGE BEAM
            List<Element> edgeBeams = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructConnections)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("edgebeam"))
                .ToList();



            using (Transaction transaction = new Transaction(doc, "Number structure"))
            {
                transaction.Start();

                foreach (Floor landing in landings)
                {
                    XYZ startXYZ = floorSelection.GetLandingStartEndLines(landing)[0].GetEndPoint(0);
                    XYZ endXYZ = floorSelection.GetLandingStartEndLines(landing)[1].GetEndPoint(0);

                    string walkwayNumber = landing.LookupParameter("Walkway_number").AsString();
                    //List<Element> belongingEdgeBeams = edgeBeams.Where(e => e.LookupParameter("Walkway_number").AsString().Equals(walkwayNumber)).ToList();

                    List<Element> belongingEdgeBeams = new List<Element>();
                    foreach (Element e in edgeBeams)
                    {
                        string number = e.LookupParameter("Walkway_number").AsString();
                        if (number == walkwayNumber)
                        {
                            belongingEdgeBeams.Add(e);
                        }
                    }


                    foreach (Element edgeBeam in belongingEdgeBeams)
                    {
                        Parameter beamUse = edgeBeam.LookupParameter("Walkway_EdgeBeam_use");
                        double locX = ScheduleUtil.GetAdaptiveFamilyXYZ(edgeBeam, doc).X;
                        if (locX > startXYZ.X && locX < endXYZ.X)
                        {
                            beamUse.Set("Landing");
                        }
                        else
                        {
                            beamUse.Set("Run");
                        }
                    }
                }

                transaction.Commit();
            }

            Debug.WriteLine("COMMITED");

            return Result.Succeeded;
        }
    }
    }
