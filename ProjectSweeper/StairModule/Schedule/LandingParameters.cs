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

namespace ProjectSweeper.StairModule.LandingParameters
{
    [Transaction(TransactionMode.Manual)]
    public class LandingParameters : IExternalCommand
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
                .ToList();

            //EDGE BEAM
            List<Element> edgeBeams = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructConnections)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("edgebeam"))
                .ToList();

            //HBEAM
            List<Element> hBeams = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("hbeam"))
                .ToList();

            //LBEAM
            List<Element> lBeams = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("lbeam"))
                .ToList();

            //BRACE
            List<Element> braces = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("brace"))
                .ToList();

            //COLUMN
            List<Element> columns = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("supportcolumn"))
                .ToList();

            using (Transaction transaction = new Transaction(doc, "Number structure"))
            {
                transaction.Start();

                int j = 0;
                foreach (Floor landing in landings)
                {

                    //try
                    //{
                        string mark = landing.LookupParameter("Walkway_mark").AsString();
                        string number = landing.LookupParameter("Walkway_number").AsString();

                        //Parameter A_param = landing.LookupParameter("Walkway_A");
                        //double landingLength = floorSelection.GetLandingStartSideCurves(landing)[0].Length;
                        //landingLength = Math.Round(Utils.FeetToMMConverter(landingLength), 0);
                        //A_param.Set(landingLength.ToString());


                        Parameter B_param = landing.LookupParameter("Walkway_B");
                        double stepsCount = landing.LookupParameter("Walkway_stairCount").AsInteger() * 296;
                        string B_param_val = stepsCount.ToString();
                        B_param.Set(B_param_val);

                    //      Element edgeBeamLanding = GetBelongingElements(edgeBeams, mark, number).Where(e => e.LookupParameter("Walkway_EdgeBeam_use").AsString() == "Landing").First();
                    //      landing.LookupParameter("Walkway_C").Set(ConvertParamererValue(edgeBeamLanding.LookupParameter("C")));
                    //      landing.LookupParameter("Walkway_D").Set(ConvertParamererValue(edgeBeamLanding.LookupParameter("D")));


                    //      Element edgeBeamRun = GetBelongingElements(edgeBeams, mark, number).Where(e => e.LookupParameter("Walkway_EdgeBeam_use").AsString() =="Run").First();
                    //      landing.LookupParameter("Walkway_E").Set(ConvertParamererValue(edgeBeamRun.LookupParameter("C")));

                    //      //// columns
                    //      List<Element> belongingSupportColumns = GetBelongingElements(columns, mark, number).OrderByDescending(c => ScheduleUtil.GetColumnXYZ(c).X).ToList();
                    //      //F
                    //      List<string> F_parameters = new List<string>
                    //      {
                    //          "Walkway_F1",
                    //          "Walkway_F2",
                    //          "Walkway_F3"

                    //      };
                    //      for (int i = 0; i < belongingSupportColumns.Count; i++)
                    //      {
                    //          Parameter paramF = landing.LookupParameter(F_parameters[i]);

                    //          string paramF_value = ConvertParamererValue(belongingSupportColumns[i].LookupParameter("F"));
                    //          paramF.Set(paramF_value);
                    //      }

                    //      //// braces
                    //      List<Element> belongingBraces = GetBelongingElements(braces, mark, number).OrderByDescending(c => ScheduleUtil.GetFramingXYZ(c).X).ToList();
                    //      //G
                    //      List<string> G_parameters = new List<string>
                    //      {
                    //          "Walkway_G1",
                    //          "Walkway_G2",

                    //      };
                    //      for (int i = 0; i < belongingBraces.Count; i++)
                    //      {
                    //          Parameter paramG = landing.LookupParameter(G_parameters[i]);

                    //          string paramG_value = ConvertParamererValue(belongingBraces[i].LookupParameter("G"));
                    //          paramG.Set(paramG_value);
                    //      }


                    //      //// beams
                    //      List<Element> belongingBeams = GetBelongingElements(hBeams, mark, number).OrderByDescending(c => ScheduleUtil.GetFramingXYZ(c).X).ToList();
                    //      //H
                    //      List<string> H_parameters = new List<string>
                    //      {
                    //          "Walkway_H1",
                    //          "Walkway_H2",
                    //      };
                    //      for (int i = 0; i < belongingBeams.Count - 1; i++)
                    //      {
                    //          XYZ p1 = ScheduleUtil.GetFramingXYZ(belongingBeams[i]);
                    //          XYZ p2 = ScheduleUtil.GetFramingXYZ(belongingBeams[i + 1]);
                    //          double beamSpan = Line.CreateBound(p1, p2).Length;

                    //          Parameter paramH = landing.LookupParameter(H_parameters[i]);
                    //          string paramH_value = Math.Round(Utils.FeetToMMConverter(beamSpan), 0).ToString();
                    //          paramH.Set(paramH_value);
                    //      }
                    //      //I
                    //      Parameter I_param = landing.LookupParameter("Walkway_I");
                    //      double paramI_value = Utils.FeetToMMConverter(edgeBeamRun.LookupParameter("C").AsDouble() + Utils.MMToFeetConverter(37));
                    //      I_param.Set(Math.Round(paramI_value, 0).ToString());
                    //      //J
                    //      Parameter J_param = landing.LookupParameter("Walkway_J");
                    //      string paramJ_value = "129";
                    //      J_param.Set(paramJ_value);
                    //      //K
                    //      List<string> K_parameters = new List<string>
                    //      {
                    //          "Walkway_K1",
                    //          "Walkway_K2",
                    //          "Walkway_K3"

                    //      };
                    //      //L
                    //      List<string> L_parameters = new List<string>
                    //      {
                    //          "Walkway_L1",
                    //          "Walkway_L2",
                    //          "Walkway_L3"

                    //      };
                    //      //M
                    //      List<string> M_parameters = new List<string>
                    //      {
                    //          "Walkway_M1",
                    //          "Walkway_M2",
                    //          "Walkway_M3"

                    //      };

                    //      for (int i = 0; i < belongingBeams.Count; i++)
                    //      {
                    //          Parameter paramK = landing.LookupParameter(K_parameters[i]);
                    //          Parameter paramL = landing.LookupParameter(L_parameters[i]);
                    //          Parameter paramM = landing.LookupParameter(M_parameters[i]);

                    //          string paramK_value = ConvertParamererValue(belongingBeams[i].LookupParameter("K"));
                    //          string paramL_value = ConvertParamererValue(belongingBeams[i].LookupParameter("L"));
                    //          string paramM_value = ConvertParamererValue(belongingBeams[i].LookupParameter("M"));

                    //          paramK.Set(paramK_value);
                    //          paramL.Set(paramL_value);
                    //          paramM.Set(paramM_value);
                    //      }

                    //      Parameter N_param = landing.LookupParameter("Walkway_N");
                    //      double landingWidth = floorSelection.GetLandingStartEndLines(landing)[0].Length;
                    //      landingWidth = Math.Round(Utils.FeetToMMConverter(landingWidth), 0) - 1;
                    //      N_param.Set(landingWidth.ToString());
                    //  } catch (Exception ex)
                    //  {

                    //  }

                }

                transaction.Commit();
                }
                Debug.WriteLine("COMMITED");

            return Result.Succeeded;
        }

        public List<Element> GetBelongingElements(List<Element> elements, string mark, string number)
        {
            return elements.Where(e => e.LookupParameter("Walkway_number").AsString() == number && e.LookupParameter("Walkway_mark").AsString() == mark).ToList();
        }

        public string ConvertParamererValue(Parameter parameter)
        {
            double val = parameter.AsDouble();
            return Math.Round(Utils.FeetToMMConverter(val),0).ToString();
        }

    }
}
