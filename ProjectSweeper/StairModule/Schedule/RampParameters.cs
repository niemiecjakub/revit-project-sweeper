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
    public class RampParameters : IExternalCommand
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
                .Where(f => f.Name.ToLower().Contains("ramplanding"))
                .OrderBy(f => floorSelection.GetFloorAproxLocation(f).X)
                .ToList();

            //STEP
            List<Element> steps = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .WhereElementIsNotElementType()
                .Where(e => e.Name.ToLower().Contains("296"))
                .ToList();

            //EDGE BEAM
            List<Element> edgeBeams = new FilteredElementCollector(doc, view.Id)
                .OfCategory(BuiltInCategory.OST_GenericModel)
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

                    string mark = landing.LookupParameter("Walkway_mark").AsString();
                    string number = landing.LookupParameter("Walkway_number").AsString();

                    Parameter A_param = landing.LookupParameter("Walkway_A");
                    double landingLength = floorSelection.GetLandingStartSideCurves(landing)[0].Length;
                    landingLength = Math.Round(Utils.FeetToMMConverter(landingLength), 0);
                    A_param.Set(landingLength.ToString());


                    Element edgeBeamLanding = GetBelongingElements(edgeBeams, mark, number).Where(e => e.LookupParameter("Walkway_EdgeBeam_use").AsString().Equals("Landing")).First();
                    landing.LookupParameter("Walkway_C").Set(ConvertParamererValue(edgeBeamLanding.LookupParameter("C")));
                    landing.LookupParameter("Walkway_D").Set(ConvertParamererValue(edgeBeamLanding.LookupParameter("D")));


                    Element edgeBeamRun = GetBelongingElements(edgeBeams, mark, number).Where(e => e.LookupParameter("Walkway_EdgeBeam_use").AsString().Equals("Run")).First();
                    landing.LookupParameter("Walkway_E").Set(ConvertParamererValue(edgeBeamRun.LookupParameter("C")));

                    //// columns
                    List<Element> belongingSupportColumns = GetBelongingElements(columns, mark, number).OrderByDescending(c => ScheduleUtil.GetColumnXYZ(c).X).Take(2).ToList();
                    //F
                    List<string> F_parameters = new List<string>
                    {
                        "Walkway_F1",
                        "Walkway_F2",

                    };
                    for (int i = 0; i < belongingSupportColumns.Count; i++)
                    {
                        Parameter paramF = landing.LookupParameter(F_parameters[i]);

                        string paramF_value = ConvertParamererValue(belongingSupportColumns[i].LookupParameter("F"));
                        paramF.Set(paramF_value);
                    }

                    //// braces
                    List<Element> belongingBraces = GetBelongingElements(braces, mark, number).OrderByDescending(c => ScheduleUtil.GetFramingXYZ(c).X).Take(2).ToList();
                    //G
                    List<string> G_parameters = new List<string>
                    {
                        "Walkway_G1",

                    };
                    for (int i = 0; i < belongingBraces.Count; i++)
                    {
                        Parameter paramG = landing.LookupParameter(G_parameters[i]);

                        string paramG_value = ConvertParamererValue(belongingBraces[i].LookupParameter("G"));
                        paramG.Set(paramG_value);
                    }


                    //// beams
                    List<Element> belongingBeams = GetBelongingElements(hBeams, mark, number).OrderByDescending(c => ScheduleUtil.GetFramingXYZ(c).X).Take(2).ToList();
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

                        Parameter paramH = landing.LookupParameter(H_parameters[i]);
                        string paramH_value = Math.Round(Utils.FeetToMMConverter(beamSpan), 0).ToString();
                        paramH.Set(paramH_value);
                    }
                    //I
                    Parameter I_param = landing.LookupParameter("Walkway_I");
                    double paramI_value = Utils.FeetToMMConverter(edgeBeamRun.LookupParameter("C").AsDouble() + Utils.MMToFeetConverter(22));
                    I_param.Set(Math.Round(paramI_value, 0).ToString());
                    //J
                    Parameter J_param = landing.LookupParameter("Walkway_J");
                    string paramJ_value = "61";
                    J_param.Set(paramJ_value);
                    //K
                    List<string> K_parameters = new List<string>
                    {
                        "Walkway_K1",
                        "Walkway_K2"
                    };
                    //L
                    List<string> L_parameters = new List<string>
                    {
                        "Walkway_L1",
                        "Walkway_L2"

                    };
                    //M
                    List<string> M_parameters = new List<string>
                    {
                        "Walkway_M1",
                        "Walkway_M2"

                    };

                    for (int i = 0; i < belongingBeams.Count; i++)
                    {
                        Parameter paramK = landing.LookupParameter(K_parameters[i]);
                        Parameter paramL = landing.LookupParameter(L_parameters[i]);
                        Parameter paramM = landing.LookupParameter(M_parameters[i]);

                        string paramK_value = ConvertParamererValue(belongingBeams[i].LookupParameter("K"));
                        string paramL_value = ConvertParamererValue(belongingBeams[i].LookupParameter("L"));
                        string paramM_value = ConvertParamererValue(belongingBeams[i].LookupParameter("M"));

                        paramK.Set(paramK_value);
                        paramL.Set(paramL_value);
                        paramM.Set(paramM_value);
                    }


                    //// beams
                    List<Element> belongingBeamsRamp = GetBelongingElements(hBeams, mark, number).OrderByDescending(c => ScheduleUtil.GetFramingXYZ(c).X).Skip(2).ToList();
                    //K
                    List<string> K_parameters_ramp = new List<string>
                    {
                        "Walkway_K4",
                        "Walkway_K5"
                    };
                    //L
                    List<string> L_parameters_ramp = new List<string>
                    {
                        "Walkway_L4",
                        "Walkway_L5"

                    };
                    //M
                    List<string> M_parameters_ramp = new List<string>
                    {
                        "Walkway_M4",
                        "Walkway_M5"

                    };

                    for (int i = 0; i < belongingBeamsRamp.Count; i++)
                    {
                        Parameter paramK = landing.LookupParameter(K_parameters_ramp[i]);
                        Parameter paramL = landing.LookupParameter(L_parameters_ramp[i]);
                        Parameter paramM = landing.LookupParameter(M_parameters_ramp[i]);

                        string paramK_value = ConvertParamererValue(belongingBeamsRamp[i].LookupParameter("K"));
                        string paramL_value = ConvertParamererValue(belongingBeamsRamp[i].LookupParameter("L"));
                        string paramM_value = ConvertParamererValue(belongingBeamsRamp[i].LookupParameter("M"));

                        paramK.Set(paramK_value);
                        paramL.Set(paramL_value);
                        paramM.Set(paramM_value);
                    }


                    //// columns
                    List<Element> belongingSupportColumnsRamp = GetBelongingElements(columns, mark, number).OrderByDescending(c => ScheduleUtil.GetColumnXYZ(c).X).Skip(2).ToList();
                    List<string> F_parameters_ramp = new List<string>
                    {
                        "Walkway_F4",
                        "Walkway_F5",

                    };
                    for (int i = 0; i < belongingSupportColumnsRamp.Count; i++)
                    {
                        Parameter paramF = landing.LookupParameter(F_parameters_ramp[i]);

                        string paramF_value = ConvertParamererValue(belongingSupportColumnsRamp[i].LookupParameter("F"));
                        paramF.Set(paramF_value);
                    }


                }

                transaction.Commit();
            }

            Debug.WriteLine("COMMITED");

            return Result.Succeeded;
        }

        public List<Element> GetBelongingElements(List<Element> elements, string mark, string number)
        {
            return elements.Where(e => e.LookupParameter("Walkway_number").AsString().Equals(number) && e.LookupParameter("Walkway_mark").AsString().Equals(mark)).ToList();
        }

        public string ConvertParamererValue(Parameter parameter)
        {
            double val = parameter.AsDouble();
            return Math.Round(Utils.FeetToMMConverter(val), 0).ToString();
        }

    }
}
