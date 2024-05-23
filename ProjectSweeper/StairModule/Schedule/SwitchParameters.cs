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

namespace ProjectSweeper.StairModule.Schedule
{
    [Transaction(TransactionMode.Manual)]
    public class SwitchParameters : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FloorSelection floorSelection = new FloorSelection(uidoc, doc);
            Reference selectedAlignment;
            View view = doc.ActiveView;

            try
            {
                //alignment
                selectedAlignment = uidoc.Selection.PickObject(ObjectType.Element, "Select alignment");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

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

            //COLUMN
            List<Element> columns = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("supportcolumn"))
                .ToList();

            using (Transaction transaction = new Transaction(doc, "Number switch structure"))
            {
                transaction.Start();
                int j = 0;
                foreach (Floor landing in landings)
                {

                        string mark = landing.LookupParameter("Walkway_mark").AsString();
                        string number = landing.LookupParameter("Walkway_number").AsString();

                        Debug.WriteLine(mark + " " + number);
                        //A
                        Parameter A1_param = landing.LookupParameter("Walkway_A1");
                        double A1_len = floorSelection.GetClosesToAlignmentCurveBySelection(landing, selectedAlignment)[0].Length;
                        A1_len = Math.Round(Utils.FeetToMMConverter(A1_len), 0);
                        A1_param.Set(A1_len.ToString());

                        Parameter A2_param = landing.LookupParameter("Walkway_A2");
                        double A2_len = floorSelection.GetFarToAlignmentCurveBySelection(landing, selectedAlignment)[0].Length;
                        A2_len = Math.Round(Utils.FeetToMMConverter(A2_len), 0);
                        A2_param.Set(A2_len.ToString());

                        //B
                        Parameter B_param = landing.LookupParameter("Walkway_B");
                        double stepsCount = landing.LookupParameter("Walkway_stairCount").AsInteger() * 296;
                        string B_param_val = stepsCount.ToString();
                        B_param.Set(B_param_val);

                        //N
                        Parameter N1_param = landing.LookupParameter("Walkway_N1");
                        double N1_len = floorSelection.GetLandingStartEndLines(landing)[0].Length;
                        N1_len = Math.Round(Utils.FeetToMMConverter(N1_len), 0);
                        N1_param.Set(N1_len.ToString());

                        Parameter N2_param = landing.LookupParameter("Walkway_N2");
                        double N2_len = floorSelection.GetLandingStartEndLines(landing)[1].Length;
                        N2_len = Math.Round(Utils.FeetToMMConverter(N2_len), 0);
                        N2_param.Set(N2_len.ToString());

                    //E
                    //Element edgeBeamRun = GetBelongingElements(edgeBeams, mark, number)
                    //    .Where(e => e.LookupParameter("Walkway_EdgeBeam_use")
                    //    .AsString() == "Run")
                    //    .First();
                    //landing.LookupParameter("Walkway_E").Set(ConvertParamererValue(edgeBeamRun.LookupParameter("C")));

                    // columns
                    List<Element> belongingSupportColumns = GetBelongingElements(columns, mark, number).OrderByDescending(c => ScheduleUtil.GetColumnXYZ(c).X).ToList();
                        //P
                        List<string> F_parameters = new List<string>
                          {
                              "Walkway_P1",
                              "Walkway_P2",
                              "Walkway_P3",
                              "Walkway_P4",
                              "Walkway_P5"

                          };
                        for (int i = 0; i < belongingSupportColumns.Count; i++)
                        {
                            Parameter paramF = landing.LookupParameter(F_parameters[i]);

                            string paramF_value = ConvertParamererValue(belongingSupportColumns[i].LookupParameter("F"));
                            paramF.Set(paramF_value);
                        }


                        //// beams
                        List<Element> belongingBeams = GetBelongingElements(hBeams, mark, number).OrderByDescending(c => ScheduleUtil.GetFramingXYZ(c).X).ToList();
                        //H
                        List<string> H_parameters = new List<string>
                          {
                              "Walkway_H1",
                          };
                        for (int i = 0; i < belongingBeams.Count - 1; i++)
                        {
                            XYZ p1 = ScheduleUtil.GetFramingXYZ(belongingBeams[i]);
                            XYZ p2 = ScheduleUtil.GetFramingXYZ(belongingBeams[i + 1]);
                            double beamSpan = Line.CreateBound(p1, p2).Length;

                            Parameter paramH = landing.LookupParameter(H_parameters[0]);
                            string paramH_value = Math.Round(Utils.FeetToMMConverter(beamSpan), 0).ToString();
                            paramH.Set(paramH_value);
                        }

                    //I
                    //Parameter I_param = landing.LookupParameter("Walkway_I");
                    //double paramI_value = Utils.FeetToMMConverter(edgeBeamRun.LookupParameter("C").AsDouble() + Utils.MMToFeetConverter(37));
                    //I_param.Set(Math.Round(paramI_value, 0).ToString());

                    //J
                    Parameter J_param = landing.LookupParameter("Walkway_J");
                    string paramJ_value = "129";
                    J_param.Set(paramJ_value);
                    //K
                    List<string> K_parameters = new List<string>
                          {
                              "Walkway_K1",
                              "Walkway_K2",
                              "Walkway_K3",
                              "Walkway_K4",
                              "Walkway_K5"
                          };
                        //L
                        List<string> L_parameters = new List<string>
                          {
                              "Walkway_L1",
                              "Walkway_L2",
                              "Walkway_L3",
                              "Walkway_L4",
                              "Walkway_L5"
                          };

                        //M
                        List<string> M_parameters = new List<string>
                          {
                              "Walkway_M1",
                              "Walkway_M2",
                              "Walkway_M3",
                              "Walkway_M4",
                              "Walkway_M5"
                          };

                        for (int i = 0; i < belongingBeams.Count; i++)
                        {

                            Parameter paramK = landing.LookupParameter(K_parameters[i]);
                            Parameter paramL = landing.LookupParameter(L_parameters[i]);
                            Parameter paramM = landing.LookupParameter(M_parameters[i]);

                            string paramK_value = ConvertParamererValue(belongingBeams[i].LookupParameter("K"));
                            string paramL_value = ConvertParamererValue(belongingBeams[i].LookupParameter("L"));


                            string paramM_value = Math.Round(Utils.FeetToMMConverter(belongingBeams[i].LookupParameter("K").AsDouble()) - Utils.FeetToMMConverter(belongingBeams[i].LookupParameter("L").AsDouble()) - 10, 0).ToString();


                            paramK.Set(paramK_value);
                            paramL.Set(paramL_value);
                            paramM.Set(paramM_value);
                        }

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
            return Math.Round(Utils.FeetToMMConverter(val), 0).ToString();
        }
    }
}
