using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace ProjectSweeper.StairModule.Structure
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction transaction = new Transaction(doc, "Create stair elements"))
            {
                transaction.Start();

                IList<Element> floorList = null;
                Reference pickedTunnel = null;
                Reference pickedSlab = null;
                Reference selectedAlignment;
                try
                {
                    //floors
                    ISelectionFilter floorFilter = new FloorFilter();
                    floorList = uidoc.Selection.PickElementsByRectangle(floorFilter, "Select floors");
                    //alignment
                    selectedAlignment = uidoc.Selection.PickObject(ObjectType.Element, "Select alignment");
                    //tunnel
                    pickedTunnel = uidoc.Selection.PickObject(ObjectType.Element, "Select an tunnel lining");
                    //salab
                    pickedSlab = uidoc.Selection.PickObject(ObjectType.Element, "Select slab");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Cancelled;
                }


                FloorSelection floorSelection = new FloorSelection(uidoc, doc);
                StairModuleUI stairUI = new StairModuleUI(commandData);
                stairUI.ShowDialog();

                string SIDE = stairUI.Side.ToString().ToUpper();
                double BEAM_HEIGHT = Utils.MMToFeetConverter(254);
                double BEAM_WIDTH = Utils.MMToFeetConverter(69.6);
                double STEP_WIDTH = Utils.MMToFeetConverter(296);
                //double OFFSET_VALUE = SIDE == "R" ? BEAM_WIDTH / 2 : BEAM_WIDTH / 2;
                double OFFSET_VALUE_TO_COLUMN_AX = Utils.MMToFeetConverter(77);

                SolidCurveIntersectionOptions intersectOptions = new SolidCurveIntersectionOptions();

                List<Solid> tunnelSolids = Utils.GetSolidFromElement(pickedTunnel, doc);
                List<Solid> slabSolids = Utils.GetSolidFromElement(pickedSlab, doc);
                List<Curve> floorCurves = floorSelection.GetClosesToAlignmentCurveBySelection(floorList, selectedAlignment);

                double maxSpacing = Utils.MMToFeetConverter(3750);

                //string IPE_FAMILY_FAME = "TNL-SFM-LININGBEAM-CH2";
                string IPE_FAMILY_FAME = "TNL-SFM-W6X8.5A36AISC15.0-CH2";
                FamilySymbol horizontalBeamFamily = Utils.GetFamilySymbolByName(IPE_FAMILY_FAME, BuiltInCategory.OST_StructuralFraming, doc);
                HorizontalBeamFactory horizontalBeamFactory = new HorizontalBeamFactory(doc, maxSpacing, horizontalBeamFamily, tunnelSolids);

                //string BRACE_FAMILY_NAME = "TNL-SFM-BRACING-CH2";
                string BRACE_FAMILY_NAME = "TNL-SFM-HSSQ2.5X2.5X0.188A36AISC15.0-CH2";
                FamilySymbol braceFamily = Utils.GetFamilySymbolByName(BRACE_FAMILY_NAME, BuiltInCategory.OST_StructuralFraming, doc);


                //string SUPPORT_FAMILY_NAME = "TNL-SCM-SUPPORTCOLUMN-CH2";
                string SUPPORT_FAMILY_NAME = "TNL-SCM-HSSQ2.5X2.5X0.188A36AISC15.0-CH2";
                FamilySymbol supportFamily = Utils.GetFamilySymbolByName(SUPPORT_FAMILY_NAME, BuiltInCategory.OST_StructuralColumns, doc);
                SupportColumnFactory supportColumnFactory = new SupportColumnFactory(doc, maxSpacing, supportFamily, slabSolids);


                string FAMILY_SYMBOL_LANDING_SUPPORT_BEAM = "TNL-SFM-L2-1/2X2X3/16AISC15.0-CH2";
                FamilySymbol landingSupportBeamSymbol = Utils.GetFamilySymbolByName(FAMILY_SYMBOL_LANDING_SUPPORT_BEAM, BuiltInCategory.OST_StructuralFraming, doc);

                foreach (Element floor in floorList)
                {
                    string orientation = floor.LookupParameter("Walkway_orientation").AsValueString();
                    string purpose = floor.LookupParameter("Walkway_purpose").AsValueString();
                    string mark = floor.LookupParameter("Walkway_mark").AsValueString();
                    string number = floor.LookupParameter("Walkway_number").AsValueString();

                    List<FamilyInstance> builtElements = new List<FamilyInstance>();
                    List<Curve> crves = floorSelection.GetClosesToAlignmentCurveBySelection(floor, selectedAlignment);
                    foreach (Curve edgeCurve in crves)
                    {
                        //double trimStartValue = Utils.MMToFeetConverter(50);
                        double trimStartValue = SIDE == "R" ? Utils.MMToFeetConverter(296 + 50 + 60) : Utils.MMToFeetConverter(60);
                        double trimEndValue = SIDE == "R" ? Utils.MMToFeetConverter(60) : Utils.MMToFeetConverter(296 + 50 + 60);
                        Curve edgeCurveTrimmed = Utils.TrimStartEndByValue(edgeCurve as Line, trimStartValue, trimEndValue);
                        Line edgeLineOffset = Utils.LineOffset(edgeCurveTrimmed as Line, OFFSET_VALUE_TO_COLUMN_AX);

                        //SUPPORT BUILD
                        List<FamilyInstance> supportColumns = supportColumnFactory.BuildAll(SIDE, BEAM_HEIGHT, edgeLineOffset);

                        //BRACE BUILD
                        BraceFactory braceFactory = new BraceFactory(doc, supportColumnFactory.ColumnPlacementlines, braceFamily);
                        List<FamilyInstance> braces = braceFactory.BuildAll();

                        //HBEAM BUILDER 
                        List<FamilyInstance> horizontalBeams = horizontalBeamFactory.BuildAll(SIDE, edgeCurveTrimmed);

                        supportColumnFactory.ColumnPlacementlines.Clear();

                        builtElements.AddRange(supportColumns);
                        builtElements.AddRange(braces);
                        builtElements.AddRange(horizontalBeams);
                    }

                    List<Curve> sideCurves = floorSelection.GetFloorSideLines(floor, selectedAlignment);
                    foreach (Curve landingCurve in sideCurves)
                    {
                        Line profileLine = Utils.LineOffsetVerically(landingCurve as Line, Utils.MMToFeetConverter(-30));
                        Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;
                        FamilyInstance landingSupportBeam = doc.Create.NewFamilyInstance(profileLine, landingSupportBeamSymbol, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                        builtElements.Add(landingSupportBeam);
                    }

                    foreach (FamilyInstance elem in builtElements)
                    {
                        elem.LookupParameter("Walkway_orientation").Set(orientation);
                        elem.LookupParameter("Walkway_purpose").Set(purpose);
                        elem.LookupParameter("Walkway_mark").Set(mark);
                        elem.LookupParameter("Walkway_number").Set(number);
                    }
                }


                //OLD
                //foreach (Curve edgeCurve in floorCurves)
                //{
                //    //double trimStartValue = Utils.MMToFeetConverter(50);
                //    double trimStartValue = SIDE == "R" ? Utils.MMToFeetConverter(296 + 50 + 60) : Utils.MMToFeetConverter(60);
                //    double trimEndValue = SIDE == "R" ? Utils.MMToFeetConverter(60) : Utils.MMToFeetConverter(296 + 50 + 60);
                //    Curve edgeCurveTrimmed = Utils.TrimStartEndByValue(edgeCurve as Line, trimStartValue, trimEndValue);
                //    Line edgeLineOffset = Utils.LineOffset(edgeCurveTrimmed as Line, OFFSET_VALUE_TO_COLUMN_AX);

                //    //SUPPORT BUILD
                //    List<FamilyInstance> supportColumns = supportColumnFactory.BuildAll(SIDE, BEAM_HEIGHT, edgeLineOffset);

                //    //BRACE BUILD
                //    BraceFactory braceFactory = new BraceFactory(doc, supportColumnFactory.ColumnPlacementlines, braceFamily);
                //    List<FamilyInstance> braces = braceFactory.BuildAll();

                //    //HBEAM BUILDER 
                //    List<FamilyInstance> horizontalBeams = horizontalBeamFactory.BuildAll(SIDE, edgeCurveTrimmed);

                //    supportColumnFactory.ColumnPlacementlines.Clear();
                //}

                //// L BEAMS BUILDER
                //List<Curve> floorLandingSideCurves = floorSelection.GetFloorSideLines(floorList, selectedAlignment);

                //string FAMILY_SYMBOL_LANDING_SUPPORT_BEAM = "TNL-GRATINGSUPPORTBEAM-CH2";
                //string FAMILY_SYMBOL_LANDING_SUPPORT_BEAM = "TNL-SFM-L2-1/2X2X3/16AISC15.0-CH2";
                //FamilySymbol landingSupportBeamSymbol = Utils.GetFamilySymbolByName(FAMILY_SYMBOL_LANDING_SUPPORT_BEAM, BuiltInCategory.OST_StructuralFraming, doc);
                //foreach (Curve landingCurve in floorLandingSideCurves)
                //{
                //    Line profileLine = Utils.LineOffsetVerically(landingCurve as Line, Utils.MMToFeetConverter(-30));
                //    Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;
                //    FamilyInstance landingSupportBeam = doc.Create.NewFamilyInstance(profileLine, landingSupportBeamSymbol, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                //}    

                transaction.Commit();
            }
            return Result.Succeeded;
        }
    }
}
