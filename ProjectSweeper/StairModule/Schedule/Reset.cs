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
    public class Reset : IExternalCommand
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

            using (Transaction transaction = new Transaction(doc, "Number structure"))
            {
                transaction.Start();

                foreach (Floor landing in landings)
                {



                        List<string> F_parameters = new List<string>
                    {
                        "Walkway_A",
                        "Walkway_A1",
                        "Walkway_A2",
                        "Walkway_B",
                        "Walkway_C",
                        "Walkway_D",
                        "Walkway_E",
                        "Walkway_F1",
                        "Walkway_F2",
                        "Walkway_F3",
                        "Walkway_F4",
                        "Walkway_F5",
                        "Walkway_G1",
                        "Walkway_G2",
                        "Walkway_H1",
                        "Walkway_H2",
                        "Walkway_I",
                        "Walkway_J",
                        "Walkway_K1",
                        "Walkway_K2",
                        "Walkway_K3",
                        "Walkway_K4",
                        "Walkway_K5",
                        "Walkway_L1",
                        "Walkway_L2",
                        "Walkway_L3",
                        "Walkway_L4",
                        "Walkway_L5",
                        "Walkway_M1",
                        "Walkway_M2",
                        "Walkway_M3",
                        "Walkway_M4",
                        "Walkway_M5",
                        "Walkway_N",
                        "Walkway_N1",
                        "Walkway_N2",
                        "Walkway_P1",
                        "Walkway_P1.1",
                        "Walkway_P2",
                        "Walkway_P2.1",
                        "Walkway_P3",
                        "Walkway_P3.1",
                        "Walkway_P4",
                        "Walkway_P4.1",
                        "Walkway_P5",
                        "Walkway_P5.1",
                    };
                        for (int i = 0; i < F_parameters.Count; i++)
                        {
                            Parameter param = landing.LookupParameter(F_parameters[i]);
                            param.Set("");

                        }
                    

                }
                transaction.Commit();

                Debug.WriteLine("COMMITED");

                return Result.Succeeded;
            }
        }
    }
}
