using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.Schedule
{
    [Transaction(TransactionMode.Manual)]
    public class LandingCoordinatesSwitch : IExternalCommand
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
                .ToList();

            using (Transaction transaction = new Transaction(doc, "Number structure"))
            {
                transaction.Start();
                int i = 0;
                foreach (Floor landing in landings)
                {
                    string mark = landing.LookupParameter("Walkway_mark").AsString();
                    string number = landing.LookupParameter("Walkway_number").AsString();

                    Debug.WriteLine($"{mark}{number}");
                    List<Curve> lines = floorSelection.GetLandingLines(landing);
                    HashSet<XYZ> points = new HashSet<XYZ>();
                    foreach (Curve curve in lines)
                    {
                        XYZ p1 = curve.GetEndPoint(0);
                        points.Add(p1);
                    }

                    ProjectLocation currentLocation = doc.ActiveProjectLocation;

                    List<string> paramList_northing = new List<string>()
                    {
                        "p1_Northing",
                        "p2_Northing",
                        "p3_Northing",
                        "p4_Northing",
                    };
                    List<string> paramList_easting = new List<string>()
                    {
                        "p1_Easting",
                        "p2_Easting",
                        "p3_Easting",
                        "p4_Easting",
                    };
                    int j = 0;
                    try
                    {
                        foreach (XYZ sopPoint in points)
                        {
                            double N = Math.Round(Utils.FeetToMMConverter(currentLocation.GetProjectPosition(sopPoint).NorthSouth) / 1000, 3);
                            landing.LookupParameter(paramList_northing[j]).Set(N.ToString());
                            j++;
                        }

                        int v = 0;
                        foreach (XYZ sopPoint in points)
                        {
                            double E = Math.Round(Utils.FeetToMMConverter(currentLocation.GetProjectPosition(sopPoint).EastWest) / 1000, 3);
                            landing.LookupParameter(paramList_easting[v]).Set(E.ToString());
                            v++;
                        }
                    }
                    catch { 
                    }
                }

                transaction.Commit();
            }

            Debug.WriteLine("COMMITED");

            return Result.Succeeded;
        }
    }
}
